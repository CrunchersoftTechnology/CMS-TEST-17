using CMS.Domain.Infrastructure;

namespace CMS.Web.Infrastructure
{
    public class UnitOfWork : IUnitOfWork
    {
        readonly IRepository _repository;

        public UnitOfWork(IRepository repository)
        {
            _repository = repository;
        }

        public IRepository Repository
        {
            get { return _repository; }
        }

        public void Dispose()
        {
            _repository.Dispose();
        }
    }
}