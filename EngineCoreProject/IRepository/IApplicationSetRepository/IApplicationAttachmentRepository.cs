using EngineCoreProject.DTOs.ApplicationDtos;
using EngineCoreProject.DTOs.FileDto;
using EngineCoreProject.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.IApplicationSetRepository
{
    public interface IApplicationAttachmentRepository
    {
        Task<List<ApplicationAttachment>> GetAll(int? ApplicationId);
        Task<ApplicationAttachment> GetOne(int id);
        Task<List<int>> DeleteMany(int[] ids);
        Task<int> DeleteOne(int id);
        Task<int> AddWithAttachment(ApplicationAttachmentWithFileDto applicationAttachmentDto , string targetFolder);//IFormFile File
        Task<int> Add(ApplicationAttachmentDto applicationAttachmentDto,int userId);
        Task<int> Update(int id, int userId, ApplicationAttachmentDto applicationAttachmentDto);
        Task<UploadedFileMessage> UploadApplicationAttachment(IFormFile File);

    }
}
