using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

namespace EngineCoreProject
{
    public class ValidatorException : Exception
    {
        public List<string> AttributeMessages { get; set; }

        public ValidatorException()
        {
            AttributeMessages = new List<string>();
        }
        public string GetMessages()
        {
            return JsonSerializer.Serialize(AttributeMessages);
        }

    }
}
