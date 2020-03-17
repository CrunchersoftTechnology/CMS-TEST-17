using CMS.Common;
using CMS.Common.GridModels;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using System.Collections.Generic;

namespace CMS.Domain.Storage.Services
{
    public interface IMachineService
    {
        IEnumerable<MachineProjection> GetMachines();
        CMSResult Save(Machine newMachine);
        CMSResult Update(Machine oldMachine);
        CMSResult Delete(int id);
        MachineProjection GetMachineById(int machineId);
        IEnumerable<MachineProjection> GetMachineByBranchId(int branchId);
        int IsMachineExists(string machineSerial);
        IEnumerable<MachineGridModel> GetMachineData(out int totalRecords, int userId,
        int? limitOffset, int? limitRowCount, string orderBy, bool desc);
        IEnumerable<AttendanceSerialMachine> GetNotSetMachinesForAttendance();
        CMSResult UpdateMachineStatus(AttendanceSerialMachine machine);
    }
}
