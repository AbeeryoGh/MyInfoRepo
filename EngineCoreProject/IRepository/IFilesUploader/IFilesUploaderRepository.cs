using EngineCoreProject.DTOs.FileDto;
using Microsoft.AspNetCore.Http;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace EngineCoreProject.IRepository.IFilesUploader
{
    public interface IFilesUploaderRepositiory
    {
        Task<UploadedFileMessage> UploadFile(IFormFile file, string targetFolder, string type = "");
        Task<UploadedFileMessage> UploadFileToTemp(IFormFile file, string targetFolder, string type = "");
        CreateFolderMessage CreateFolder(string folderPath);
        bool MoveFile(string source, string target);
        bool RemoveFile(string source);
        bool CopyFile(string source, string target);

        string GetFilePath(string folder, string file);
        string GetFilePath(string file);
        string GetRootPath();
        string GetRootFolder();
        string GetMimeType(string extension);
        public string FromBase64ToImage(string base64image, string target);
        public bool FileExist(string folder, string file);
        public bool FileExist(string file);
        public bool FolderExist(string folder);


        /// <summary>
        ///    Deletes the specified directory and, if indicated, any subdirectories and files
        ///     in the directory.
        /// </summary>
        /// <param name="folder"></param>
        public void DeleteFolder(string folder);
        public List<string> GetFolderFilesNames(string folder);

        public List<string> GetLogsFilesNames(string search);

        public int DeleteTemporaryFiles(DateTime dateToDel);
    }
}
