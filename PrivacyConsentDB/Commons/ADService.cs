using System;
using System.Collections.Generic;
using System.Configuration;
using System.DirectoryServices;
using System.DirectoryServices.AccountManagement;
using System.Linq;
using System.Web;
using System.Web.Hosting;

namespace PrivacyConsentDB.Commons
{
    public class ADService
    {
        public static bool IsAuthGroup(string ntid)
        {
            AppSettingsReader appSettingsReader = new AppSettingsReader();
            string adgroupname = (string)appSettingsReader.GetValue("ADGroupName", typeof(string));

            if (string.IsNullOrEmpty(adgroupname)) return true;

            using (HostingEnvironment.Impersonate())
            {
                PrincipalContext yourDomain = new PrincipalContext(ContextType.Domain);
                // find your user
                UserPrincipal user = UserPrincipal.FindByIdentity(yourDomain, ntid);

                // if found - grab its groups
                if (user != null)
                {
                    if (user.IsMemberOf(yourDomain, IdentityType.Name, adgroupname)) // MTESWEB-PCMS-DEV-RW  DL-AP5-BT
                    {
                        return true;
                    }
                }
            }
            return false;
        }
    }
}