using System;
using System.ComponentModel.DataAnnotations;

namespace CMS.Common
{
    public class DateValidation : RangeAttribute
    {
        public DateValidation()
    : base(typeof(DateTime),
            DateTime.Now.AddYears(-10).ToString("dd/MM/yyyy"),
            DateTime.Now.AddYears(10).ToShortDateString())
        {
          
        }
    }
}
