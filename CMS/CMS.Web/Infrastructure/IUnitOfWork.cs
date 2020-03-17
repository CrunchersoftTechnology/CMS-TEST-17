using CMS.Domain.Infrastructure;
using System;

namespace CMS.Web.Infrastructure
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository Repository { get; }
    }
}
