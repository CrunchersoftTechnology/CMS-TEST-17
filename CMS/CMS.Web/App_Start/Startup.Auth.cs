using CMS.Domain.Models;
using CMS.Domain.Storage;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.AspNet.Identity.Owin;
using Microsoft.Owin;
using Microsoft.Owin.Security.Cookies;
using Owin;
using System;

namespace CMS.Web
{
    public partial class Startup
    {
        // For more information on configuring authentication, please visit http://go.microsoft.com/fwlink/?LinkId=301864
        public void ConfigureAuth(IAppBuilder app)
        {
            // Configure the db context, user manager and signin manager to use a single instance per request
            app.CreatePerOwinContext(CMSDbContext.Create);
            app.CreatePerOwinContext<ApplicationUserManager>(ApplicationUserManager.Create);
            app.CreatePerOwinContext<ApplicationSignInManager>(ApplicationSignInManager.Create);

            // Enable the application to use a cookie to store information for the signed in user
            // and to use a cookie to temporarily store information about a user logging in with a third party login provider
            // Configure the sign in cookie
            app.UseCookieAuthentication(new CookieAuthenticationOptions
            {
                AuthenticationType = DefaultAuthenticationTypes.ApplicationCookie,
                LoginPath = new PathString("/Account/Login"),
                Provider = new CookieAuthenticationProvider
                {
                    // Enables the application to validate the security stamp when the user logs in.
                    // This is a security feature which is used when you change a password or add an external login to your account.  
                    OnValidateIdentity = SecurityStampValidator.OnValidateIdentity<ApplicationUserManager, ApplicationUser>(
                        validateInterval: TimeSpan.FromMinutes(30),
                        regenerateIdentity: (manager, user) => user.GenerateUserIdentityAsync(manager))
                }
            });
            app.UseExternalSignInCookie(DefaultAuthenticationTypes.ExternalCookie);

            // Enables the application to temporarily store user information when they are verifying the second factor in the two-factor authentication process.
            app.UseTwoFactorSignInCookie(DefaultAuthenticationTypes.TwoFactorCookie, TimeSpan.FromMinutes(5));

            // Enables the application to remember the second login verification factor such as phone or email.
            // Once you check this option, your second step of verification during the login process will be remembered on the device where you logged in from.
            // This is similar to the RememberMe option when you log in.
            app.UseTwoFactorRememberBrowserCookie(DefaultAuthenticationTypes.TwoFactorRememberBrowserCookie);

            // Uncomment the following lines to enable logging in with third party login providers
            //app.UseMicrosoftAccountAuthentication(
            //    clientId: "",
            //    clientSecret: "");

            //app.UseTwitterAuthentication(
            //   consumerKey: "",
            //   consumerSecret: "");

            //app.UseFacebookAuthentication(
            //   appId: "",
            //   appSecret: "");

            //app.UseGoogleAuthentication(new GoogleOAuth2AuthenticationOptions()
            //{
            //    ClientId = "",
            //    ClientSecret = ""
            //});
        }

        private void CreateRolesAndUsers()
        {
            CMSDbContext context = new CMSDbContext();

            var roleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(context));
            var UserManager = new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(context));

            if (!roleManager.RoleExists(Common.Constants.AdminRole.ToString()))
            {
                var role = new IdentityRole();
                role.Name = CMS.Common.Constants.AdminRole.ToString();
                roleManager.Create(role);

                var user = new ApplicationUser();
                user.UserName = "admin@gmail.com";
                user.Email = "admin@gmail.com";

                user.CreatedBy = "admin@gmail.com";
                user.CreatedOn = DateTime.UtcNow;
                user.PhoneNumber = "123456789";

                string userPWD = "Admin@123";

                var chkUser = UserManager.Create(user, userPWD);

                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, CMS.Common.Constants.AdminRole.ToString());
                }
            }

            // creating user role
            if (!roleManager.RoleExists(CMS.Common.Constants.UserRole))
            {
                var role = new IdentityRole();
                role.Name = CMS.Common.Constants.UserRole;
                roleManager.Create(role);

                var user = new ApplicationUser();
                user.UserName = "demo@gmail.com";
                user.Email = "demo@gmail.com";

                user.CreatedBy = "demo@gmail.com";
                user.CreatedOn = DateTime.UtcNow;
                user.PhoneNumber = "123456789";

                string userPWD = "Demo@123";

                var chkUser = UserManager.Create(user, userPWD);

                if (chkUser.Succeeded)
                {
                    var result1 = UserManager.AddToRole(user.Id, CMS.Common.Constants.UserRole.ToString());
                }
            }
            // Teach Role
            if (!roleManager.RoleExists(Common.Constants.TeacherRole))
            {
                var role = new IdentityRole();
                role.Name = CMS.Common.Constants.TeacherRole;
                roleManager.Create(role);
            }

            // creating Student role
            if (!roleManager.RoleExists(Common.Constants.StudentRole))
            {
                var role = new IdentityRole();
                role.Name = CMS.Common.Constants.StudentRole;
                roleManager.Create(role);
                
              
            }
            // Branch Admin Role
            if (!roleManager.RoleExists(Common.Constants.BranchAdminRole))
            {
                var role = new IdentityRole();
                role.Name = CMS.Common.Constants.BranchAdminRole;
                roleManager.Create(role);
            }

            // Client Role
            if (!roleManager.RoleExists(Common.Constants.ClientAdminRole))
            {
                var role = new IdentityRole();
                role.Name = CMS.Common.Constants.ClientAdminRole;
                roleManager.Create(role);
            }
        }
    }
}