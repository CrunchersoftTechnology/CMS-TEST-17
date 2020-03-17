using AutoMapper;
using CMS.Web.Automapper;

namespace CMS.Web.App_Start
{
    public class AutoMapperWebConfig
    {
        public static void Configure()
        {
            Mapper.Initialize(cfg =>
            {
                cfg.AddProfile(new WebProfile());
            });

            Mapper.AssertConfigurationIsValid();
        }
    }
}