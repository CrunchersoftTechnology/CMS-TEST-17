using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace CMS.Web.Models
{
    public class MachineAttendence
    {
        public MachineAttendence()
        {
            PunchDataList = new List<PunchData>();
        }
        public string MachineSerial { get; set; }
        public List<PunchData> PunchDataList { get; set; }
    }

    public class PunchData
    {
        public int PunchId { get; set; }
        public DateTime PunchDateTime { get; set; }
    }
}