using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CMS.Common.GridModels
{
    /// <summary>
    /// Exclude property from export functionality
    /// </summary>
    [AttributeUsage(AttributeTargets.Property)]
    public class ExcludeAttribute : Attribute
    {
    }
}
