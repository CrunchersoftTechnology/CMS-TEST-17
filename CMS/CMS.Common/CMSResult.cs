using System.Collections.Generic;
using System.Linq;

namespace CMS.Common
{
    public class CMSResult
    {
        public string Data { get; set; }
        public CMSResult()
        {
            Results = new List<Result>();
        }

        public List<Result> Results { get; set; }

        public bool Success
        {
            get
            {
                return (Results.Where(x => !x.IsSuccessful).Any() ? false : true);
            }
            set { Success = value; }
        }

    }

    public class Result
    {
        public Result()
        {

        }

        public Result(string message, bool isSuccessful)
        {
            Message = message;
            IsSuccessful = isSuccessful;
        }
        public string Message { get; set; }
        public bool IsSuccessful { get; set; }
    }
}
