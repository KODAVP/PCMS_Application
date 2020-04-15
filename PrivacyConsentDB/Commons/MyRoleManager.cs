using PrivacyConsentDB.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.DirectoryServices;
using System.Linq;
using System.Linq.Expressions;
using System.Web;

namespace PrivacyConsentDB.Commons
{
    public class MyRoleManager
    {

        static List<string> accessablePath = new List<string>();
        private static int roleId = 0;

        public enum RoleType
        {
            [Display(Name = "Sales")]
            SALES = 1,
            [Display(Name = "Marketing")]
            MARKETING = 2,
            [Display(Name = "DCE 관리자")]
            DCEADMIN = 4,
            [Display(Name = "시스템 관리자")]
            SYSTEMADMIN = 5,
            [Display(Name = "BU MCM")]
            BUMCM = 3
        }

        public enum RoleTypeNotSA
        {
            [Display(Name = "Sales")]
            SALES = 1,
            [Display(Name = "Marketing")]
            MARKETING = 2,
            [Display(Name = "DCE 관리자")]
            DCEADMIN = 4,            
            [Display(Name = "BU MCM")]
            BUMCM = 3
        }
        public static bool hasRole(RoleType roletype)
        {
            PCMSDBContext db = new PCMSDBContext();
            string username = getCurrentName();
            IEnumerable<UserRole> roles = db.UserRoles.Where(r => r.username == username && r.roletype == roletype).ToList();
            return roles.Count() > 0;
        }

        //public static bool canAccessPage(string path)
        //{
        //    PCMSDBContext db = new PCMSDBContext();
        //    string username = getCurrentName();
        //    if (accessablePath.Contains(path)) return true;
        //    UserRole roles = db.UserRoles.Where(r => r.username == username).FirstOrDefault();
        //    if (roles == null) return false;


        //    bool ret = db.AccessAuthorities.Where(p => p.AccessPaths.path.Equals(path) && p.roleID == (int)roles.roletype).Count() > 0;
        //    if (ret) accessablePath.Add(path);
        //    return ret;
        //}
        public static bool canAccessPage(string path)
        {
            PCMSDBContext pcmsdbContext = new PCMSDBContext();
            string username = MyRoleManager.getCurrentName();
            UserRole roles = ((IQueryable<UserRole>)pcmsdbContext.UserRoles).Where<UserRole>((Expression<Func<UserRole, bool>>)(r => r.username == username)).FirstOrDefault<UserRole>();
            if (roles == null)
                return false;
            if (MyRoleManager.accessablePath.Contains(path) && roles.roletype != (MyRoleManager.RoleType)MyRoleManager.roleId)
                MyRoleManager.accessablePath.Clear();
            else if (MyRoleManager.accessablePath.Contains(path))
                return true;
            bool flag = ((IQueryable<AccessAuthorities>)pcmsdbContext.AccessAuthorities).Where<AccessAuthorities>((Expression<Func<AccessAuthorities, bool>>)(p => p.AccessPaths.path.Equals(path) && p.roleID == (int)roles.roletype)).Count<AccessAuthorities>() > 0;
            if (flag)
            {
                MyRoleManager.accessablePath.Add(path);
                MyRoleManager.roleId = (int)roles.roletype;
            }
            return flag;
        }
        public static string getWinName() {
            string winusername = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToUpper();
            if (winusername.IndexOf('\\') > -1) winusername = winusername.Substring(winusername.IndexOf('\\') + 1).Replace("\\", "");
            return winusername;
        }
        public static string getCurrentName()
        {
            string username = HttpContext.Current.User.Identity.Name.ToUpper();
            if (username.IndexOf('\\') > -1) username = username.Substring(username.IndexOf('\\') + 1).Replace("\\", "");
            return username;
        }
        public static string getRoleName()
        {
            PCMSDBContext db = new PCMSDBContext();
            string username = getCurrentName();
            string rolename = "";
            IEnumerable<UserRole> roles = db.UserRoles.Where(r => r.username == username).OrderByDescending(r => r.roletype).ToList();
            if (roles.Count() > 0)
            {
                UserRole ur = roles.First();
                rolename = EnumHelper<RoleType>.GetDisplayValue(ur.roletype);
            }
            return username + " [" + rolename + "]";
        }
    }
}