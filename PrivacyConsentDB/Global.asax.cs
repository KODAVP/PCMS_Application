using PrivacyConsentDB.Models;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using System.Web.Security;

namespace PrivacyConsentDB
{
    public class MvcApplication : System.Web.HttpApplication
    {

        protected void Application_Start()
        {
            AreaRegistration.RegisterAllAreas();
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            
        }

        public void WindowsAuthentication_OnAuthenticate(object sender, WindowsAuthenticationEventArgs args)
        {
            
        }
    }
}
