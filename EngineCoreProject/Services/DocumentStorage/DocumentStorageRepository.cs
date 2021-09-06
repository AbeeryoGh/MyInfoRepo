using EngineCoreProject.DTOs;
using EngineCoreProject.IRepository;
using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Configuration;
using System.IO;


namespace EngineCoreProject.Services
{
    public class DocumentStorageRepository : IDocumentStorageRepository
    {

        private readonly EngineCoreDBContext      _EngineCoreDBContext;
        private readonly IFileConfigRepository _FileConfigRepository;
        private readonly IConfiguration        _Configuration;
        private readonly IGeneralRepository    _IGeneralRepository;

        public DocumentStorageRepository(EngineCoreDBContext EngineCoreDBContext, IFileConfigRepository fileConfigRepository, IConfiguration configuration, IGeneralRepository iGeneralRepository)
        {
            _EngineCoreDBContext     = EngineCoreDBContext;
            _FileConfigRepository = fileConfigRepository;
            _Configuration        = configuration;
            _IGeneralRepository   = iGeneralRepository;
        }


        //public async Task<LoadFileMessage> UploadFile(IFormFile file, int userId)
        //{

        //    string path = _Configuration["DocumentStorage"];
        //    LoadFileMessage lfm = new LoadFileMessage(); //// NB! maybe need add param

        //    if (file == null)
        //    {
        //        lfm.result = false;
        //        lfm.message = "File does not exist";

        //        return lfm;
        //    }

        //    var fileCofig = await _FileConfigRepository.GetFileConfigurationByExtension(Path.GetExtension(file.FileName)[1..].ToLower());

        //    //if (fileCofig.Extension== jpg ...)

        //    if (file.Length < fileCofig.MinSize || file.Length > fileCofig.MaxSize)

        //    {
        //        lfm.result = false;
        //        lfm.message = "File is not the correct size";
        //        return lfm;
        //    }

        //    string newFileName = userId + "_" + Convert.ToString(Guid.NewGuid()) + "_" + file.FileName;
        //    string newFilePath = Path.Combine(path,userId.ToString());

        //    string resultPath = Path.Combine(newFilePath, newFileName);

        //    if (!Directory.Exists(newFilePath)) Directory.CreateDirectory(newFilePath);


        //    try
        //    {
        //        using (var memoryStream = new MemoryStream())
        //        {
        //            using (FileStream fs = System.IO.File.Create(resultPath))
        //            {
        //                await file.CopyToAsync(fs);
        //            }
        //        }

        //        DocumentStorage documentStorage = new DocumentStorage
        //        {
        //           // Id_User = userId,                                                //Get idUser!!!
        //           // File_Name = newFileName,
        //           // File_Path = newFilePath
        //        };

        //         _IGeneralRepository.Add(documentStorage);                        //Get idUser!!!
        //        await _IGeneralRepository.Save();

        //        lfm.message = "File Upload";
        //            lfm.result = true;

        //            return lfm;
        //    }
        //    catch
        //    {
        //        lfm.message = "File not Upload" ;
        //        lfm.result = false;
        //        return lfm;
        //    }
        //}


      /*  public async Task<byte[]> GetOneFile(int id)                     //Use the transfer as a byte array or set other suitable format           
        {

            id = Convert.ToInt32(id);
            var query = _EngineCoreDBContext.DocumentStorage.Where(x => x.Id == id);

            if (query.Count() == 1)
            {
                var docStorage = await query.FirstOrDefaultAsync();
                var path = Path.Combine(docStorage.File_Path, docStorage.File_Name);

               byte[] mas = System.IO.File.ReadAllBytes(path);

                return mas;
            }
            
            return null;
        }*/


        public async Task<int> Delete(int id)
        {
            id = Convert.ToInt32(id);
            var query = _EngineCoreDBContext.DocumentStorage.Where(x => x.Id == id);


            if (query.Count() == 1) 
            {
                try
                {
                    var docStorage = await query.FirstOrDefaultAsync();
                   // var path = Path.Combine(docStorage.File_Path, docStorage.File_Name);

                    _IGeneralRepository.Delete(docStorage);
                    if (await _IGeneralRepository.Save())
                    {
                       // File.Delete(path);
                        return Constants.OK;
                    }
                }
                catch (Exception) 
                { 
                    return Constants.ERROR; 
                }
            }            
            return Constants.ERROR;
        }
    }
}
