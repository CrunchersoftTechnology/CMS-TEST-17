using System;
using System.Collections.Generic;
using CMS.Common;
using CMS.Domain.Models;
using CMS.Domain.Storage.Projections;
using CMS.Domain.Infrastructure;
using System.Linq;
using CMS.Common.GridModels;

namespace CMS.Domain.Storage.Services
{
    public class MachineService : IMachineService
    {
        readonly IRepository _repository;

        public MachineService(IRepository repository)
        {
            _repository = repository;
        }

        public CMSResult Delete(int machineId)
        {
            CMSResult result = new CMSResult();
            var model = _repository.Load<Machine>(m => m.MachineId == machineId);
            if (model == null)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Machine '{0}' already exists!", model.Name) });
            }
            else
            {
                _repository.Delete(model);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Machine '{0}' deleted successfully!", model.Name) });
            }
            return result;
        }

        public IEnumerable<MachineProjection> GetMachines()
        {
            return _repository.Project<Machine, MachineProjection[]>(
                machines => (from m in machines
                             select new MachineProjection
                             {
                                 SerialNumber = m.SerialNumber,
                                 Name = m.Name,
                                 MachineId = m.MachineId,
                                 BranchId = m.BranchId,
                                 BranchName = m.Branch.Name
                             }).ToArray());
        }

        public CMSResult Save(Machine newMachine)
        {
            var result = new CMSResult();
            var isExistsName = _repository.Project<Machine, bool>(machines => (
                                            from m in machines
                                            where m.Name == newMachine.Name && m.BranchId == newMachine.BranchId
                                            select m)
                                            .Any());

            var isExistsSerialNumber = _repository.Project<Machine, bool>(machines => (
                                            from m in machines
                                            where m.SerialNumber == newMachine.SerialNumber
                                            select m)
                                            .Any());

            if (isExistsName)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Machine '{0}' already exists!", newMachine.Name) });
            }
            if (isExistsSerialNumber)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Machine '{0}' already exists!", newMachine.SerialNumber) });
            }
            if (!isExistsName && !isExistsSerialNumber)
            {
                _repository.Add(newMachine);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Machine '{0}' added successfully!", newMachine.Name) });
            }
            return result;
        }

        public CMSResult Update(Machine oldMachine)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<Machine, bool>(machines => (from m in machines where m.MachineId != oldMachine.MachineId && m.SerialNumber == oldMachine.SerialNumber select m).Any());

            if (isExists)
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Machine '{0}' already exists!", oldMachine.SerialNumber) });
            }
            else
            {
                var machine = _repository.Load<Machine>(x => x.MachineId == oldMachine.MachineId);
                machine.Name = oldMachine.Name;
                machine.SerialNumber = oldMachine.SerialNumber;
                machine.BranchId = oldMachine.BranchId;
                _repository.Update(machine);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Machine '{0}' successfully updated!", oldMachine.Name) });
            }
            return result;
        }

        public MachineProjection GetMachineById(int machineId)
        {
            return _repository.Project<Machine, MachineProjection>(
                machines => (from m in machines
                             where m.MachineId == machineId
                             select new MachineProjection
                             {
                                 MachineId = m.MachineId,
                                 SerialNumber = m.SerialNumber,
                                 Name = m.Name,
                                 BranchId = m.BranchId,
                                 BranchName = m.Branch.Name
                             }).FirstOrDefault());
        }

        public IEnumerable<MachineProjection> GetMachineByBranchId(int branchId)
        {
            return _repository.Project<Machine, MachineProjection[]>(
                machines => (from m in machines
                             where m.BranchId == branchId
                             select new MachineProjection
                             {
                                 SerialNumber = m.SerialNumber,
                                 Name = m.Name,
                                 MachineId = m.MachineId,
                                 BranchId = m.BranchId,
                                 BranchName = m.Branch.Name
                             }).ToArray());
        }

        public int IsMachineExists(string machineSerial)
        {
            return _repository.Project<Machine, int>(
                machines => (from m in machines
                             where m.SerialNumber == machineSerial
                             select m.BranchId).FirstOrDefault());
        }

        public IEnumerable<MachineGridModel> GetMachineData(out int totalRecords, int userId,
            int? limitOffset, int? limitRowCount, string orderBy, bool desc)
        {
            int BranchId = userId;
            var query = _repository.Project<Machine, IQueryable<MachineGridModel>>(machines => (
                 from m in machines
                 select new MachineGridModel
                 {
                     MachineId = m.MachineId,
                     BranchId = m.BranchId,
                     BranchName = m.Branch.Name,
                     Name = m.Name,
                     SerialNumber = m.SerialNumber,
                     CreatedOn = m.CreatedOn,

                 })).AsQueryable();

            if (BranchId != 0)
            {
                query = query.Where(p => p.BranchId == BranchId);
            }
            totalRecords = query.Count();

            if (!string.IsNullOrWhiteSpace(orderBy))
            {
                switch (orderBy)
                {
                    case nameof(MachineGridModel.Name):
                        if (!desc)
                            query = query.OrderBy(p => p.Name);
                        else
                            query = query.OrderByDescending(p => p.Name);
                        break;
                    case nameof(MachineGridModel.BranchName):
                        if (!desc)
                            query = query.OrderBy(p => p.BranchName);
                        else
                            query = query.OrderByDescending(p => p.BranchName);
                        break;
                    case nameof(MachineGridModel.SerialNumber):
                        if (!desc)
                            query = query.OrderBy(p => p.SerialNumber);
                        else
                            query = query.OrderByDescending(p => p.SerialNumber);
                        break;

                    default:
                        if (!desc)
                            query = query.OrderBy(p => p.CreatedOn);
                        else
                            query = query.OrderByDescending(p => p.CreatedOn);
                        break;
                }
            }


            if (limitOffset.HasValue)
            {
                query = query.Skip(limitOffset.Value).Take(limitRowCount.Value);
            }


            return query.ToList();
        }

        public IEnumerable<AttendanceSerialMachine> GetNotSetMachinesForAttendance()
        {
            return _repository.Project<Machine, AttendanceSerialMachine[]>(
                machines => (from m in machines
                             where m.Status == false
                             select new AttendanceSerialMachine
                             {
                                 Status = m.Status,
                                 SerialNumber = m.SerialNumber
                             }).ToArray());
        }

        public CMSResult UpdateMachineStatus(AttendanceSerialMachine machine)
        {
            CMSResult result = new CMSResult();
            var isExists = _repository.Project<Machine, bool>(machines => (from m in machines where m.SerialNumber == machine.SerialNumber select m).Any());

            if (isExists)
            {
                var machineDetails = _repository.Load<Machine>(x => x.SerialNumber == machine.SerialNumber);
                machineDetails.Status = machine.Status;
                _repository.Update(machineDetails);
                result.Results.Add(new Result { IsSuccessful = true, Message = string.Format("Machine updated successfully!") });
            }
            else
            {
                result.Results.Add(new Result { IsSuccessful = false, Message = string.Format("Machine not exists!") });
            }
            return result;
        }
    }
}
