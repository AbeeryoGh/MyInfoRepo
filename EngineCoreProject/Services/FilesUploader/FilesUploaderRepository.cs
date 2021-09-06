﻿using EngineCoreProject.DTOs.FileDto;
using EngineCoreProject.IRepository.IFilesUploader;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;



namespace EngineCoreProject.Services.FilesUploader
{
    public class FilesUploaderRepository : IFilesUploaderRepositiory
    {

        // private readonly IGeneralRepository _iGeneralRepository;
        public static IWebHostEnvironment _webHostEnvironment;
        private readonly IFileConfigurationRepository _IFileConfigurationRepository;
        private readonly IConfiguration _IConfiguration;
        private static readonly IDictionary<string, string> _mappings = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase) {

        {".doc", "application/msword"},
        {".docx", "application/vnd.openxmlformats-officedocument.wordprocessingml.document"},
        {".gif", "image/gif"},
        {".jpe", "image/jpeg"},
        {".jpeg", "image/jpeg"},
        {".jpg", "image/jpeg"},
        {".pdf", "application/pdf"},
        {".png", "image/png"},
        {".pntg", "image/x-macpaint"},
        {".pptx", "application/vnd.openxmlformats-officedocument.presentationml.presentation"},
        {".zip", "application/x-zip-compressed"}


        };

        public FilesUploaderRepository(IWebHostEnvironment webHostEnvironment
               , IFileConfigurationRepository iFileConfigurationRepository, IConfiguration iConfiguration)
        {


            _webHostEnvironment = webHostEnvironment;
            _IFileConfigurationRepository = iFileConfigurationRepository;
            _IConfiguration = iConfiguration;
        }



        public async Task<UploadedFileMessage> UploadFile(IFormFile File, string targetFolder, string type)
        {
            string fileExtension, fileExtensionWithOutDot, newFileName, filepath, fileServer, folder;
            long size;

            folder = _IConfiguration[targetFolder];
            fileServer = _IConfiguration["DocumentStorage"];
            string path = Path.Combine(GetRootPath(), folder);
            UploadedFileMessage message,
             failurMessage = new UploadedFileMessage
             {
                 Id = 0,
                 FileName = null,
                 SuccessUpload = false,
                 Message = "No File Provided!"
             };

            if (File == null || File.Length == 0)
                return failurMessage;
            fileExtension = Path.GetExtension(Path.GetFileName(File.FileName));
            fileExtensionWithOutDot = fileExtension.Substring(1);
            // fileExtension = Path.GetExtension(Path.GetFileName(File.FileName)).Substring(1).ToLower();

            var ext = await _IFileConfigurationRepository.GetFileConfigurationByExt(fileExtensionWithOutDot);
            if (ext == null)
            {
                failurMessage.FileName = File.FileName;
                failurMessage.Message = "Unallowed File Extention";
                return failurMessage;
            }

            if (type != null && type.Length > 2)
                if (ext.Type != type)
                {
                    failurMessage.FileName = File.FileName;
                    failurMessage.Message = "Unallowed File Type";
                    return failurMessage;
                }

            size = File.Length;
            if (size < ext.MinSize || size > ext.MaxSize)
            {
                failurMessage.FileName = File.FileName;
                failurMessage.Message = "Unallowed File Size";
                return failurMessage;
            }

            newFileName = String.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);//Path.GetRandomFileName())
            filepath = Path.Combine(path, newFileName);

            try
            {

                /*using (var memoryStream = new MemoryStream())
                {
                 await File.CopyToAsync(memoryStream);
                  var  result = FileTypeVerifier.CheckStream(memoryStream);*/

                //  using (var stream = new FileStream(filepath, FileMode.Create))
                //  {
                //var result = FileTypeVerifier.FromStream(stream);
                // await File.CopyToAsync(stream);
                //-------------------------------------------------
                //}
                //if (!Directory.Exists()) Directory.CreateDirectory();
                using (FileStream fs = System.IO.File.Create(filepath))
                {
                    await File.CopyToAsync(fs);
                    fs.Flush();
                    return message = new UploadedFileMessage
                    {
                        Id = 0,
                        FileName = newFileName,
                        SuccessUpload = true,
                        Message = "File uploaded", //+ result.Description+'|'+result.IsVerified+'|'+result.Name,
                        FileUrl = Path.Combine(folder, newFileName),
                        Size = size,
                        MimeType = File.ContentType
                    };
                }
                //  }

            }
            catch (Exception e)
            {
                failurMessage.Message = "Error . File not uploaded!";
                return failurMessage;

            }
        }

        public async Task<UploadedFileMessage> UploadFileToTemp(IFormFile File, string targetFolder, string type)
        {
            string fileExtension, fileExtensionWithOutDot, newFileName, filepath, folder, fileServer;
            long size;

            folder = _IConfiguration[targetFolder];
            fileServer = _IConfiguration["DocumentStorage"];
            string path = Path.Combine(_webHostEnvironment.WebRootPath, folder);
            UploadedFileMessage message,
             failurMessage = new UploadedFileMessage
             {
                 Id = 0,
                 FileName = null,
                 SuccessUpload = false,
                 Message = "No File Provided!"

             };

            if (File == null || File.Length == 0)
                return failurMessage;
            fileExtension = Path.GetExtension(Path.GetFileName(File.FileName));
            fileExtensionWithOutDot = fileExtension.Substring(1);

            var ext = await _IFileConfigurationRepository.GetFileConfigurationByExt(fileExtensionWithOutDot);
            if (ext == null)
            {
                failurMessage.FileName = File.FileName;
                failurMessage.Message = "Unallowed File Extention";
                return failurMessage;
            }

            if (type != null && type.Length > 2)
                if (ext.Type != type)
                {
                    failurMessage.FileName = File.FileName;
                    failurMessage.Message = "Unallowed File Type";
                    return failurMessage;
                }

            size = File.Length;
            if (size < ext.MinSize || size > ext.MaxSize)
            {
                failurMessage.FileName = File.FileName;
                failurMessage.Message = "Unallowed File Size";
                return failurMessage;
            }

            newFileName = String.Concat(Convert.ToString(Guid.NewGuid()), fileExtension);//Path.GetRandomFileName())
            filepath = Path.Combine(path, newFileName);

            try
            {

                /*using (var memoryStream = new MemoryStream())
                {
                 await File.CopyToAsync(memoryStream);
                  var  result = FileTypeVerifier.CheckStream(memoryStream);*/


                //  using (var stream = new FileStream(filepath, FileMode.Create))
                //  {
                //var result = FileTypeVerifier.FromStream(stream);
                // await File.CopyToAsync(stream);
                //-------------------------------------------------
                //}
                //if (!Directory.Exists()) Directory.CreateDirectory();
                using (FileStream fs = System.IO.File.Create(filepath))
                {
                    await File.CopyToAsync(fs);
                    fs.Flush();
                    return message = new UploadedFileMessage
                    {
                        Id = 0,
                        FileName = newFileName,
                        SuccessUpload = true,
                        Message = "File uploaded", //+ result.Description+'|'+result.IsVerified+'|'+result.Name,
                        FileUrl = Path.Combine(folder, newFileName),
                        Size = size,
                        MimeType = File.ContentType
                    };
                }
                //  }

            }
            catch
            {
                failurMessage.Message = "Error . File not uploaded!";
                return failurMessage;

            }

        }



        public string GetFilePath(string folder, string file)
        {
            return Path.Combine(GetRootPath(), folder, file);
        }

        public bool FileExist(string folder, string file)
        {
            
            return File.Exists(Path.Combine(GetRootPath(), folder, file));
        }
        public bool FileExist(string file)
        {
            if (string.IsNullOrEmpty(file))
            {
                return false;
            }
            return File.Exists(Path.Combine(GetRootPath(), file));
        }
        public string GetFilePath(string file)
        {
            return Path.Combine(GetRootPath(), file);
        }

        public string GetRootPath()
        {
            string root = _IConfiguration["DocumentStorage"] == "LOCAL" ? _webHostEnvironment.WebRootPath : _IConfiguration["DocumentStorage"];
            return root;
        }
        public string GetRootFolder()
        {
            return _IConfiguration["DocumentStorage"] == "LOCAL" ? "wwwroot" : "Enotary";
        }

        public string GetMimeType(string filename)
        {
            string extension = Path.GetExtension(filename);
            if (extension != null)
                if (!extension.StartsWith("."))
                {
                    extension = "." + extension;
                }
            string mime;
            return _mappings.TryGetValue(extension, out mime) ? mime : "application/octet-stream";
        }
        public string FromBase64ToImage(string base64image, string target)
        {
            try
            {
                string path = Path.Combine(GetRootPath(), target);
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }
                string fileName = String.Concat(Convert.ToString(Guid.NewGuid()), ".png");
                string filepath = Path.Combine(path, fileName);
                byte[] imageBytes = Convert.FromBase64String(base64image);
                File.WriteAllBytes(filepath, imageBytes);
                return filepath = Path.Combine(target, fileName);
            }
            catch
            {
                return "";
            }
        }

        public bool FolderExist(string folder)
        {
            return Directory.Exists(Path.Combine(GetRootPath(), folder));
        }


        public void DeleteFolder(string folder)
        {
            try
            {
                if (FolderExist(folder))
                {
                    Directory.Delete(Path.Combine(GetRootPath(), folder), true);
                }
            }
            catch (Exception ex)
            {
                var x = ex.Message;
            }
        }

        public CreateFolderMessage CreateFolder(string folderPath)
        {

            string fullPath = Path.Combine(GetRootPath(), folderPath);
            CreateFolderMessage CFM = new CreateFolderMessage();
            try
            {
                if (Directory.Exists(fullPath))
                {
                    CFM.Message = "المجلد موجود مسبقا";
                    return CFM;
                }

                DirectoryInfo di = Directory.CreateDirectory(fullPath);
                CFM.Message = "تم إنشاء المجلد";
                CFM.SuccessCreation = true;
                CFM.FolderPath = folderPath;

            }
            catch (Exception)
            {
                CFM.Message = "خطأ في انشاء المجلد";
                CFM.SuccessCreation = false;
            }
            return CFM;
        }


        public bool MoveFile(string source, string destination)
        {
            string sourceFullPath = Path.Combine(GetRootPath(), source);
            string destFullPath = Path.Combine(GetRootPath(), destination);
            try
            {
                File.Move(sourceFullPath, destFullPath);
                return true;
            }
            catch (Exception e)
            {
                var m = e.Message;
                return false;
            }
        }

        public bool RemoveFile(string source)
        {
            string sourceFullPath = Path.Combine(GetRootPath(), source);
            try
            {
                File.Delete(sourceFullPath);
                return true;
            }
            catch
            {
                return false;
            }
        }

        public bool CopyFile(string source, string destination)
        {
            string sourceFullPath = Path.Combine(GetRootPath(), source);
            string destFullPath = Path.Combine(GetRootPath(), destination);
            try
            {
                bool exists = System.IO.Directory.Exists(Path.GetDirectoryName(destFullPath));
                if (!exists)
                {
                    System.IO.Directory.CreateDirectory(Path.GetDirectoryName(destFullPath));
                }

                File.Copy(sourceFullPath, destFullPath);
                return true;
            }
            catch (Exception e)
            {
                var m = e.Message;
                return false;
            }
        }

        public List<string> GetFolderFilesNames(string folder)
        {
            List<string> filesNames=new List<string>();
            string path = Path.Combine(GetRootPath(), folder);
            string[] filePaths = Directory.GetFiles(path);
            foreach(string s in filePaths)
            {
               filesNames.Add(Path.GetFileName(s));
            }
            return filesNames;

        }
        
        public List<string> GetLogsFilesNames(string search)
        {
            if (search == null)
            {
                search = "";
            }

            return Directory.GetFiles("Logs").Where(s => s.Contains(search)).Select(x => Path.GetFileName(x)).ToList();
        }


        public int DeleteTemporaryFiles(DateTime dateToDel)
        {
            DirectoryInfo info = new DirectoryInfo(Path.Combine(GetRootPath(), _IConfiguration["TransactionFolder"]));
            FileInfo[] files = info.GetFiles().Where(p => p.CreationTime < dateToDel && p.CreationTime < DateTime.Now.AddDays(-2)).ToArray();
            int count = 0; 
            foreach (FileInfo file in files)
            {
                if (RemoveFile(Path.Combine(_IConfiguration["TransactionFolder"], file.Name)))
                {
                    count++;
                }
            }
            return count;

        }
    }
 }
