using CMS.Web.Logger;
using log4net;
using Ninject.Activation;
using Ninject.Modules;
using System.Reflection;

namespace CMS.Web.Infrastructure
{
    public class LoggingModule : NinjectModule
    {
        public override void Load()
        {
            //Bind<ILog>().ToMethod(x => LogManager.GetLogger(GetParentTypeName(x)))
            //    .InSingletonScope();

            Bind<ILog>()
            .ToMethod(c => LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType))
            .InSingletonScope();

            Bind<ILogger>().To<Logger.Logger>()
                .InSingletonScope();
        }

        private string GetParentTypeName(IContext context)
        {
            return context.Request.ParentContext.Request.ParentContext.Request.Service.FullName;
        }
    }
}