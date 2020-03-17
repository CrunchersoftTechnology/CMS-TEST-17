using CMS.Common;
using CMS.Domain.Infrastructure;
using CMS.Domain.Models;
using CMS.Domain.Storage.Helpers;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Linq;

namespace CMS.Domain.Storage.Services
{
    public class ApplicationUserService : IApplicationUserService
    {
        readonly IRepository _repository;

        public ApplicationUserService(IRepository repository)
        {
            _repository = repository;
        }

        public CMSResult Save(ApplicationUser user, string password)
        {
            bool isContactExists = false;
            var cmsresult = new CMSResult();
            CMSDbContext context = new CMSDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var isEmailExists = _repository.Project<Student, bool>(students => (from s in students where s.User.Email == user.Email select s).Any());
            if (user.PhoneNumber != "")
            {
                isContactExists = _repository.Project<Student, bool>(students => (from s in students where s.StudentContact == user.PhoneNumber select s).Any());
            }
            var isPunchExists = _repository.Project<Student, bool>(students => (from s in students where s.PunchId == user.Student.PunchId && s.BranchId == user.Student.BranchId && s.ClientId==user.Student.ClientId select s).Any());

            if (isEmailExists)
            {
                cmsresult.Results.Add(new Result
                {
                    IsSuccessful = false,
                    Message = "Email already exists!"
                });
            }
            if (isContactExists)
            {
                cmsresult.Results.Add(new Result
                {
                    IsSuccessful = false,
                    Message = "Student Contact Number already exists!"
                });
            }
            if (isPunchExists)
            {
                cmsresult.Results.Add(new Result
                {
                    IsSuccessful = false,
                    Message = "The Punch Id already exists!"
                });
            }
            if (!isContactExists && !isEmailExists && !isPunchExists)
            {
                var chkUser = UserManager.Create(user, password);
                if (chkUser.Succeeded)
                {
                    var resultRole = UserManager.AddToRole(user.Id, Common.Constants.StudentRole);
                    if (resultRole.Succeeded)
                    {
                        var commaseperatedList = user.Student.SelectedSubject;
                        var subjectIds = commaseperatedList.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
                        var b = _repository.LoadList<Subject>(x => subjectIds.Contains(x.SubjectId));
                        var student = _repository.Load<ApplicationUser>(x => x.Id == user.Id, x => x.Student);
                        student.Student.Subjects.AddRange(b);

                        cmsresult.Results.Add(new Result
                        {
                            IsSuccessful = true,
                            Message = "Student successfully added!"
                        });
                    }
                }
                else
                {
                    cmsresult.Results.Add(new Result
                    {
                        IsSuccessful = false,
                        Message = chkUser.Errors.FirstOrDefault()
                    });
                }
            }
            return cmsresult;
        }

        public CMSResult SaveTeacher(ApplicationUser user, string password)
        {
            var cmsresult = new CMSResult();
            CMSDbContext context = new CMSDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var isEmailExists = _repository.Project<Teacher, bool>(teachers => (from t in teachers where t.User.Email == user.Email select t).Any());
            var isContactExists = _repository.Project<Teacher, bool>(teachers => (from t in teachers where t.ContactNo == user.PhoneNumber select t).Any());
            if (isEmailExists)
            {
                cmsresult.Results.Add(new Result
                {
                    IsSuccessful = false,
                    Message = "Email already exists!"
                });
            }
            if (isContactExists)
            {
                cmsresult.Results.Add(new Result
                {
                    IsSuccessful = false,
                    Message = "Contact Number already exists!"
                });
            }
            if (!isContactExists && !isEmailExists)
            {
                var chkUser = UserManager.Create(user, password);
                if (chkUser.Succeeded)
                {
                    var resultRole = UserManager.AddToRole(user.Id, Common.Constants.TeacherRole);
                    if (resultRole.Succeeded)
                    {
                        cmsresult.Results.Add(new Result
                        {
                            IsSuccessful = true,
                            Message = "Teacher added successfully!"
                        });
                    }
                }
                else
                {
                    cmsresult.Results.Add(new Result
                    {
                        IsSuccessful = false,
                        Message = chkUser.Errors.FirstOrDefault()
                    });
                }
            }

            return cmsresult;
        }

        public CMSResult Update(ApplicationUser user)
        {
            var result = new CMSResult();
            var batch = _repository.Project<ApplicationUser, bool>(users => (from u in users where u.Id == user.Id select u).Any());

            if (!batch)
            {
                result.Results.Add(new Result { Message = "Student not exists." });
            }
            else
            {
                var commaseperatedList = user.Student.SelectedSubject;
                var subjectIds = commaseperatedList.Split(',').Where(x => !string.IsNullOrEmpty(x)).Select(int.Parse);
                var student = _repository.Load<ApplicationUser>(x => x.Id == user.Id, x => x.Student);
                var subjects = _repository.LoadList<Subject>(x => subjectIds.Contains(x.SubjectId));

                student.Student.Subjects.ToList().RemoveAll(x => subjectIds.Contains(x.SubjectId));

                student.Student.Subjects.AddRange(subjects);


             //   var student = _repository.Load<ApplicationUser>(x => x.Id == user.Id);
                student.Email = user.Email;
                student.Student.BoardId = user.Student.BoardId;
                student.Student.ClassId = user.Student.ClassId;
                student.Student.FirstName = user.Student.FirstName;
                student.Student.MiddleName = user.Student.MiddleName;
                student.Student.LastName = user.Student.LastName;
                student.Student.Gender = user.Student.Gender;
                student.Student.Address = user.Student.Address;
                student.Student.Pin = user.Student.Pin;
                student.Student.DOB = user.Student.DOB;
                student.Student.BloodGroup = user.Student.BloodGroup;
                student.Student.StudentContact = user.Student.StudentContact;
                student.Student.ParentContact = user.Student.ParentContact;
             //   student.Student.PickAndDrop = user.Student.PickAndDrop;
                student.Student.DOJ = user.Student.DOJ;
                student.Student.RollNo = user.Student.RollNo;
                student.Student.SchoolId = user.Student.SchoolId;
                student.Student.Batches = user.Student.Batches;
                _repository.Update(user);
                result.Results.Add(new Result { IsSuccessful = true, Message = "Student successfully updated!" });
            }

            return result;
        }

        public CMSResult SaveBranchAdmin(ApplicationUser user, string password)
        {
            var cmsresult = new CMSResult();
            CMSDbContext context = new CMSDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var isEmailExists = _repository.Project<BranchAdmin, bool>(branchAdmins => (from a in branchAdmins where a.User.Email == user.Email select a).Any());
            var isContactExists = _repository.Project<BranchAdmin, bool>(branchAdmins => (from a in branchAdmins where a.ContactNo == user.PhoneNumber select a).Any());

            var isEmailExistsClient = _repository.Project<ClientAdmin, bool>(clientAdmins => (from a in clientAdmins where a.User.Email == user.Email select a).Any());
            var isContactExistsClient = _repository.Project<ClientAdmin, bool>(clientAdmins => (from a in clientAdmins where a.ContactNo == user.PhoneNumber select a).Any());

            if (isEmailExists || isEmailExistsClient )
            {
                cmsresult.Results.Add(new Result
                {
                    IsSuccessful = false,
                    Message = "Email already exists!"
                });
            }
            if (isContactExists || isContactExistsClient)
            {
                cmsresult.Results.Add(new Result
                {
                    IsSuccessful = false,
                    Message = "Contact Number already exists!"
                });
            }

            if (!isContactExists && !isEmailExists || !isEmailExistsClient && !isContactExistsClient)
            {
                var chkUser = UserManager.Create(user, password);
                if (chkUser.Succeeded)
                {
                    var resultRole = UserManager.AddToRole(user.Id, Common.Constants.BranchAdminRole);
                    if (resultRole.Succeeded)
                    {
                        cmsresult.Results.Add(new Result
                        {
                            IsSuccessful = true,
                            Message = "Branch Admin added successfully!"
                        });
                    }
                }
                else
                {
                    cmsresult.Results.Add(new Result
                    {
                        IsSuccessful = false,
                        Message = chkUser.Errors.FirstOrDefault()
                    });
                }
            }
            return cmsresult;
        }


        public CMSResult SaveClientAdmin(ApplicationUser user, string password)
        {
            var cmsresult = new CMSResult();
            CMSDbContext context = new CMSDbContext();
            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            var isEmailExists = _repository.Project<ClientAdmin, bool>(clientAdmins => (from a in clientAdmins where a.User.Email == user.Email select a).Any());
            var isContactExists = _repository.Project<ClientAdmin, bool>(clientAdmins => (from a in clientAdmins where a.ContactNo == user.PhoneNumber select a).Any());

            if (isEmailExists)
            {
                cmsresult.Results.Add(new Result
                {
                    IsSuccessful = false,
                    Message = "Email already exists!"
                });
            }
            if (isContactExists)
            {
                cmsresult.Results.Add(new Result
                {
                    IsSuccessful = false,
                    Message = "Contact Number already exists!"
                });
            }

            if (!isContactExists && !isEmailExists)
            {
                var chkUser = UserManager.Create(user, password);
                if (chkUser.Succeeded)
                {
                    var resultRole = UserManager.AddToRole(user.Id, Common.Constants.ClientAdminRole);
                    if (resultRole.Succeeded)
                    {
                        cmsresult.Results.Add(new Result
                        {
                            IsSuccessful = true,
                            Message = "Client Admin added successfully!"
                        });
                    }
                }
                else
                {
                    cmsresult.Results.Add(new Result
                    {
                        IsSuccessful = false,
                        Message = chkUser.Errors.FirstOrDefault()
                    });
                }
            }
            return cmsresult;
        }
    }
}
