using PrivacyConsentDB.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrivacyConsentDB.Commons
{
    public class RoleAttribute : ActionFilterAttribute
    {
        
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            if (!MyRoleManager.hasRole(MyRoleManager.RoleType.SYSTEMADMIN) && !MyRoleManager.hasRole(MyRoleManager.RoleType.MARKETING) && !MyRoleManager.hasRole(MyRoleManager.RoleType.DCEADMIN))
            {
                filterContext.RequestContext.HttpContext.Response.Redirect("/Dashboard/Dashboard");
            }                
        }
    }
}