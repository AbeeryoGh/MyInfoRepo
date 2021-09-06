
using System.Collections.Generic;

namespace EngineCoreProject.DTOs
{
    public class APIResult
    {
        public int Id { get; set; } = -1;
        public dynamic Result { get; set; } = null;
        public int Code { get; set; } = 500;
        public List<string> Message { get; set; } = new List<string>();



        public APIResult(int IdV, dynamic ResultV, int CodeV, List<string> MessageV)
        {
            Id = IdV;
            Result = ResultV;
            Code = CodeV;
            Message = MessageV;
        }

        public APIResult()
        {
        }
    }
}
