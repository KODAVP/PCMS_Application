using Common.Logging;
using PrivacyConsentDB.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.Routing;

namespace PrivacyConsentDB.Commons
{
    public class LogAttribute : ActionFilterAttribute
    {
        private PCMSDBContext dbContext = new PCMSDBContext();

        
        public override void OnActionExecuted(ActionExecutedContext filterContext)
        {
            Log("", filterContext.RouteData, filterContext.HttpContext);            
        }
        private void Log(string stageName, RouteData routeData, HttpContextBase httpContext)
        {
            string userIP = httpContext.Request.UserHostAddress;
            var currentUsername = !string.IsNullOrEmpty(System.Web.HttpContext.Current?.User?.Identity?.Name) ? HttpContext.Current.User.Identity.Name : "Anonymous";
            currentUsername = currentUsername.Substring(currentUsername.IndexOf('\\') + 1).Replace("\\", "").ToUpper();
            
            string reqType = httpContext.Request.RequestType;
            string reqData = GetRequestData(httpContext);
            string path = httpContext.Request.CurrentExecutionFilePath;
            //string controller = routeData.Values["controller"].ToString();
            //string action = routeData.Values["action"].ToString();
            try
            {
                publishlog(currentUsername, userIP, path, reqType, reqData);
            }
            catch (Exception e) {
                Console.WriteLine(e.Message);
            }
            
            /*
            StringBuilder sb = new StringBuilder();
            sb.Append("[");
            sb.Append(currentUsername);
            sb.Append("][");
            sb.Append(userIP);
            sb.Append("][");
            sb.Append(path);
            sb.Append("][");
            sb.Append(reqType);
            sb.Append("][");
            sb.Append(reqData);
            sb.Append("]");
            */
        }

        private void publishlog(string user, string ip, string path, string rtype, string param) {
            using (System.Data.Entity.DbContextTransaction dbTran = dbContext.Database.BeginTransaction())
            {
                try
                {
                    Userlog batch = new Userlog { username = user, ip = ip, url = path, reqtype = rtype,  parameters = param };
                    dbContext.Userlogs.Add(batch);
                    dbContext.SaveChanges();
                    dbTran.Commit();
                }
                catch (Exception e)
                {
                    Console.WriteLine(e.Message);
                    dbTran.Rollback();
                }
                dbTran.Dispose();
            }
        }

        //Aux method to grab request data
        private string GetRequestData(HttpContextBase context)
        {
            StringBuilder sb = new StringBuilder();

            for (int i = 0; i < context.Request.QueryString.Count; i++)
            {
                sb.AppendFormat("Key={0}, Value={1}<br/>", context.Request.QueryString.Keys[i], context.Request.QueryString[i]);
            }

            for (int i = 0; i < context.Request.Form.Count; i++)
            {
                sb.AppendFormat("Key={0}, Value={1}<br/>", context.Request.Form.Keys[i], context.Request.Form[i]);
            }

            return sb.ToString();
        }

        public void publishRolelog(string UserID,string ip,string TargetUserID,string Activity)
        {


        }
    }
}