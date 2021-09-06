using EngineCoreProject.DTOs.ApplicationDtos;
using EngineCoreProject.DTOs.FileDto;
using EngineCoreProject.IRepository.IApplicationSetRepository;
using EngineCoreProject.IRepository.IFilesUploader;
using EngineCoreProject.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.Services.ApplicationSet
{
    public class ApplicationAttachmentRepositiory : IApplicationAttachmentRepository
    {
        private readonly EngineCoreDBContext _EngineCoreDBContext;
        private readonly IGeneralRepository _IGeneralRepository;
        private readonly IFilesUploaderRepositiory _IFilesUploaderRepository;

        public ApplicationAttachmentRepositiory(EngineCoreDBContext EngineCoreDBContext, IGeneralRepository iGeneralRepository,
            IFilesUploaderRepositiory iFilesUploaderRepository)
        {
            _EngineCoreDBContext = EngineCoreDBContext;
            _IGeneralRepository = iGeneralRepository;
            _IFilesUploaderRepository = iFilesUploaderRepository;
        }
        public async Task<List<ApplicationAttachment>> GetAll(int? ApplicationId)
        {
            Task<List<ApplicationAttachment>> query = null;
            if (ApplicationId == null)
                query = _EngineCoreDBContext.ApplicationAttachment.ToListAsync();
            else
                query = _EngineCoreDBContext.ApplicationAttachment.Where(s => s.ApplicationId == ApplicationId).ToListAsync();
            return await query;
        }
        public async Task<ApplicationAttachment> GetOne(int id)
        {
            id = Convert.ToInt32(id);
            var query = _EngineCoreDBContext.ApplicationAttachment.Where(x => x.Id == id);
            return await query.FirstOrDefaultAsync();
        }
        public async Task<int> AddWithAttachment(ApplicationAttachmentWithFileDto applicationAttachmentDto, string target)//IFormFile File
        {
            try
            {
                ApplicationAttachment applicationAttachment;
                UploadedFileMessage f = await _IFilesUploaderRepository.UploadFile(applicationAttachmentDto.File, target);
                if (f.SuccessUpload)
                {
                    applicationAttachment = new ApplicationAttachment
                    {
                        ApplicationId = applicationAttachmentDto.ApplicationId,
                        AttachmentId = applicationAttachmentDto.AttachmentId,
                        Size = f.Size,
                        FileName = f.FileName,
                        MimeType = f.MimeType
                    };

                    _IGeneralRepository.Add(applicationAttachment);
                    if (await _IGeneralRepository.Save())
                    {
                        return applicationAttachment.Id;
                    }
                }
                else
                    return Constants.ERROR;
            }

            catch (Exception)
            {
                return Constants.ERROR;
            }

            return Constants.ERROR;
        }

        //-------------------------------------------Add------------------------------------------------------
        public async Task<int> Add(ApplicationAttachmentDto appAttachment, int userId)
        {
            try
            {
                ApplicationAttachment applicationAttachment;

                applicationAttachment = new ApplicationAttachment
                {
                    ApplicationId = appAttachment.ApplicationId,
                    AttachmentId = appAttachment.AttachmentId,
                    Note = appAttachment.Note,
                    Size = appAttachment.Size,
                    FileName = appAttachment.FileName,
                    MimeType = appAttachment.MimeType,
                    CreatedBy= userId,
                    LastUpdatedBy= appAttachment.LastUpdatedBy,
                    CreatedDate=DateTime.Now,
                    LastUpdatedDate= DateTime.Now,

                };

                _IGeneralRepository.Add(applicationAttachment);
                if (await _IGeneralRepository.Save())
                {
                    return applicationAttachment.Id;
                }

            }

            catch (Exception e)
            {
                return Constants.ERROR;
            }

            return Constants.ERROR;
        }
        //-----------------------------------------------------------------------------------
        public Task<List<int>> DeleteMany(int[] ids)
        {
            throw new NotImplementedException();
        }
        //---------------------------------------------------------------------
        public async Task<int> DeleteOne(int id)
        {
            ApplicationAttachment applicationAttachment = await GetOne(id);
            if (applicationAttachment == null)
                return Constants.NOT_FOUND;
            try
            {
                _IGeneralRepository.Delete(applicationAttachment);
                if (await _IGeneralRepository.Save())
                    try
                    {
                        //var path = _IFilesUploaderRepository.GetFilePath("attachments/", applicationAttachment.FileName);
                        // if (File.Exists(path))   
                        //  File.Delete(path); 
                        return Constants.OK;
                    }

                    catch (Exception)
                    {
                        return Constants.ERROR;
                    }
            }
            catch (Exception)
            {
                return Constants.ERROR;
            }
            return Constants.ERROR;
        }
        public async Task<int> Update(int id, int userId, ApplicationAttachmentDto applicationAttachmentDto)
        {
            ApplicationAttachment applicationAttachment = await GetOne(id);
            if (applicationAttachment == null)
                return Constants.NOT_FOUND;
            try
            {
                applicationAttachment.ApplicationId = applicationAttachmentDto.ApplicationId;
                applicationAttachment.AttachmentId = applicationAttachmentDto.AttachmentId;
                applicationAttachment.FileName = applicationAttachmentDto.FileName;
                applicationAttachment.Note = applicationAttachmentDto.Note;
                applicationAttachment.LastUpdatedBy = userId;
                applicationAttachment.LastUpdatedDate = DateTime.Now;

                _IGeneralRepository.Update(applicationAttachment);
                if (await _IGeneralRepository.Save())
                {
                    return Constants.OK;
                }
            }
            catch (Exception)
            {
                return Constants.ERROR;
            }
            return Constants.ERROR;
        }
        public async Task<UploadedFileMessage> UploadApplicationAttachment(IFormFile File)
        {
            UploadedFileMessage f;
            string target = "ApplicationFileFolder";
            f = await _IFilesUploaderRepository.UploadFile(File, target);
            return f;
        }
    }
}
