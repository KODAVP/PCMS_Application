using PrivacyConsentDB.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrivacyConsentDB.Commons
{
    public class AuthAttribute : ActionFilterAttribute
    {
        private PCMSDBContext dbContext = new PCMSDBContext();
        public override void OnActionExecuting(ActionExecutingContext filterContext)
        {
            AppSettingsReader appSettingsReader = new AppSettingsReader();
            bool IsRoledUser = (bool)appSettingsReader.GetValue("IsRoledUser", typeof(bool));
            if (IsRoledUser)
            {
                var currentUsername = !string.IsNullOrEmpty(HttpContext.Current?.User?.Identity?.Name) ? HttpContext.Current.User.Identity.Name : "Anonymous";
                currentUsername = currentUsername.Substring(currentUsername.IndexOf('\\') + 1).Replace("\\", "").ToUpper();
                IEnumerable<UserRole> roles = dbContext.UserRoles.Where(r => r.username == currentUsername).ToList();
                if (roles.Count() < 1)
                {
                    filterContext.RequestContext.HttpContext.Response.Redirect("/User/NotUser", true);
                }
                else
                {
                    base.OnActionExecuting(filterContext);
                }
            }
            else
            {
                base.OnActionExecuting(filterContext);
            }
            
        }
    }
}