using log4net;
using PrivacyConsentDB.Commons;
using PrivacyConsentDB.Models;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Reflection;
using System.Web.Mvc;
using System.Web.Security;

namespace PrivacyConsentDB.Controllers
{
    [Log]
    public class UserController : Controller
    {
        private PCMSDBContext db = new PCMSDBContext();
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult Signin()
        {
            if (User.Identity.IsAuthenticated) {
              return this.RedirectToAction("Dashboard", "Dashboard");
            }
            return View();
        }

        public ActionResult NotUser()
        {
            return View();
        }

        private void initFirstLogin(string username) {
            IEnumerable<UserRole> roles = db.UserRoles.Where(r => r.username == username).ToList();
            if (roles.Count() < 1)
            {
                db.UserRoles.Add(new UserRole { username = username, roletype = MyRoleManager.RoleType.SALES });
                db.SaveChanges();
            }
        }

        private bool IsRoledUser(string username) {

            AppSettingsReader appSettingsReader = new AppSettingsReader();
            bool IsRoledUser = (bool)appSettingsReader.GetValue("IsRoledUser", typeof(bool));
            if (IsRoledUser)
            {
                IEnumerable<UserRole> roles = db.UserRoles.Where(r => r.username == username).ToList();
                if (roles.Count() < 1) return false;
                
            }
            return true;
        }

        [HttpPost]
        public ActionResult Signin(LoginModel model, string returnUrl)
        {            
            try
            {   
                if (Membership.ValidateUser(model.UserName, model.Password))
                {

                    if (!ADService.IsAuthGroup(model.UserName))
                    {
                        this.ModelState.AddModelError(string.Empty, "이용 가능한 사용자 그룹에 속해 있지 않습니다.");
                        return this.View(model);
                    }
                    if (!IsRoledUser(model.UserName))
                    {
                        this.ModelState.AddModelError(string.Empty, "등록 되지 않은 사용자 입니다.");
                        return this.View(model);
                    }
                    
                    initFirstLogin(model.UserName);
                    // View
                    FormsAuthentication.SetAuthCookie(model.UserName, true);
                    if (this.Url.IsLocalUrl(returnUrl) && returnUrl.Length > 1 && returnUrl.StartsWith("/")
                        && !returnUrl.StartsWith("//") && !returnUrl.StartsWith("/\\"))
                    {
                        return this.Redirect(returnUrl);
                    }

                    return this.RedirectToAction("Dashboard", "Dashboard");
                }
            }
            catch (Exception e) {
                log.Error(e);
                Console.WriteLine(e);
                this.ModelState.AddModelError(string.Empty, e.Message);
                return this.View(model);
            }            

            this.ModelState.AddModelError(string.Empty, "사용자 계정 정보가 정확하지 않습니다.");

            return this.View(model);
        }


        public ActionResult SignOut()
        {
            FormsAuthentication.SetAuthCookie(User.Identity.Name.ToUpper(), false);
            FormsAuthentication.SignOut();

            return RedirectToAction("Signin", "User");
        }
        
    }
}