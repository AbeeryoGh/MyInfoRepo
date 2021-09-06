using EngineCoreProject.DTOs;
using EngineCoreProject.Models;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository
{
    public interface IDocumentStorageRepository
    {
     //   Task<LoadFileMessage> UploadFile(IFormFile file, int userId);
      //  Task<byte[]> GetOneFile(int idFile);
        Task<int> Delete(int idFile);

        /*  Task<object> Add(DocumentStorageDto registerDto);
          Task<List<DocumentStorage>> GetDocuments();
          Task<int> Delete(int id);
          
          */
    }
}
