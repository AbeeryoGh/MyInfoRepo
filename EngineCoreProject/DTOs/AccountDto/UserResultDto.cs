using EngineCoreProject.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace EngineCoreProject.DTOs.AccountDto
{
    public class UserResultDto
    {
       // public string StatusCode { get; set; }
        public string Message { get; set; }
        public User user { get; set; }
    }

    public class CreateUserOldResultDto
    {
        public int UserId { get; set; }
        public string Message { get; set; }
        public OldUserPostDto User { get; set; }
    }

    public class OnLineEmployee
    {
        public int UserId { get; set; }
        public string UserName { get; set; }
        public DateTime LastStartWork { get; set; }
    }
}
