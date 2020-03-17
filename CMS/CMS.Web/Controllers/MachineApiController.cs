using CMS.Domain.Storage.Projections;
using CMS.Domain.Storage.Services;
using System.Net;
using System.Net.Http;
using System.Web.Http;

namespace CMS.Web.Controllers
{
    public class MachineApiController : ApiController
    {
        readonly IMachineService _machineService;

        public MachineApiController(IMachineService machineService)
        {
            _machineService = machineService;
        }

        [Route("Api/MachineApi")]
        public HttpResponseMessage Get()
        {
            var machines = _machineService.GetNotSetMachinesForAttendance();
            return Request.CreateResponse(HttpStatusCode.OK, machines);
        }

        [Route("Api/MachineApi/UpdateStatus")]
        public HttpResponseMessage Post(AttendanceSerialMachine machine)
        {
            var result = _machineService.UpdateMachineStatus(machine);
            return Request.CreateResponse(HttpStatusCode.OK, result.Results[0].Message);
        }
    }
}