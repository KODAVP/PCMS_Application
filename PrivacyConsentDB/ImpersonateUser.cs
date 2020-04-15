using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Runtime.InteropServices;
using System.Security;
using System.Security.Permissions;
using System.Security.Principal;
using System.Web;

namespace PrivacyConsentDB
{
    public class ImpersonateUser : IDisposable

    {
        [DllImport("advapi32.dll", CharSet= CharSet.Unicode)]
        // This method will perform the logon.
        public static extern bool LogonUser(String lpszUserName,
        String lpszDomain,
        String lpszPassword,
        int dwLogonType,
        int dwLogonProvider,
        ref IntPtr phToken);

        public const int SECURITY_IMPERSONATION_LEVEL_SecurityAnonymous = 0;
        public const int SECURITY_IMPERSONATION_LEVEL_SecurityIdentification = 1;
        public const int SECURITY_IMPERSONATION_LEVEL_SecurityImpersonation = 2;
        public const int SECURITY_IMPERSONATION_LEVEL_SecurityDelegation = 3;

        public const int LOGON32_PROVIDER_DEFAULT = 0;
        public const int LOGON32_PROVIDER_WINNT35 = 1;
        public const int LOGON32_PROVIDER_WINNT40 = 2;
        public const int LOGON32_PROVIDER_WINNT50 = 3;

        public const int LOGON32_LOGON_INTERACTIVE = 2;
        public const int LOGON32_LOGON_NETWORK = 3;
        public const int LOGON32_LOGON_BATCH = 4;
        public const int LOGON32_LOGON_SERVICE = 5;
        public const int LOGON32_LOGON_UNLOCK = 7;
        public const int LOGON32_LOGON_NETWORK_CLEARTEXT = 8; // This is the constant needed for the LogonUser method.
        public const int LOGON32_LOGON_NEW_CREDENTIALS = 9;

        private System.Security.Principal.WindowsImpersonationContext impersonationContext;

        public ImpersonateUser() {
            AppSettingsReader appSettingsReader = new AppSettingsReader();
            string userName = @"SRVASP-PCMSMGT";
            string password = @"Pfe51627";
            string domain = null;
            SecurityPermission secPerm = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
            secPerm.Assert();

            IntPtr token = IntPtr.Zero;
            bool success = false;
            string error = "No specific information.";

            try
            {
                if (LogonUser(userName, domain, password, LOGON32_LOGON_NETWORK_CLEARTEXT, LOGON32_PROVIDER_DEFAULT, ref token))
                {
                    WindowsIdentity tempWindowsIdentity = new WindowsIdentity(token, "NTLM", WindowsAccountType.Normal, true);
                    impersonationContext = tempWindowsIdentity.Impersonate(); // actually impersonate the user
                    if (impersonationContext != null)
                        success = true;
                }
            }
            catch (Exception e)
            {
                success = false;
                error = "ERROR: " + e.Message;
            }
            
            if (!success)
            {
                this.Dispose();
                throw new Exception("The logon attempt as user " + domain + "\\" + userName + " with password " + password + " failed. " + error);
            }
        }

        public ImpersonateUser(int logintype)
        {
            AppSettingsReader appSettingsReader = new AppSettingsReader();
            string userName = (string)appSettingsReader.GetValue("ImpersonateUser", typeof(string));
            string password = (string)appSettingsReader.GetValue("ImpersonatePassword", typeof(string));
            string domain = null;
            SecurityPermission secPerm = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
            secPerm.Assert();

            IntPtr token = IntPtr.Zero;
            bool success = false;
            string error = "No specific information.";

            try
            {
                if (LogonUser(userName, domain, password, logintype, LOGON32_PROVIDER_DEFAULT, ref token))
                {
                    WindowsIdentity tempWindowsIdentity = new WindowsIdentity(token, "NTLM", WindowsAccountType.Normal, true);
                    impersonationContext = tempWindowsIdentity.Impersonate(); // actually impersonate the user
                    if (impersonationContext != null)
                        success = true;
                }
            }
            catch (Exception e)
            {
                error = "ERROR: " + e.Message;
            }

            CodeAccessPermission.RevertAssert();

            if (!success)
            {
                this.Dispose();
                throw new Exception("The logon attempt as user " + domain + "\\" + userName + " with password " + password + " failed. " + error);
            }
        }

        public ImpersonateUser(string userName, string password, string domain)
        {
            SecurityPermission secPerm = new SecurityPermission(SecurityPermissionFlag.UnmanagedCode);
            secPerm.Assert();

            IntPtr token = IntPtr.Zero;
            bool success = false;
            string error = "No specific information.";

            try
            {
                if (LogonUser(userName, domain, password, LOGON32_LOGON_NETWORK_CLEARTEXT, LOGON32_PROVIDER_DEFAULT, ref token))
                {
                    WindowsIdentity tempWindowsIdentity = new WindowsIdentity(token, "NTLM", WindowsAccountType.Normal, true);
                    impersonationContext = tempWindowsIdentity.Impersonate(); // actually impersonate the user
                    if (impersonationContext != null)
                        success = true;
                }
            }
            catch (Exception e)
            {
                error = "ERROR: " + e.Message;
            }

            CodeAccessPermission.RevertAssert();

            if (!success)
            {
                this.Dispose();
                throw new Exception("The logon attempt as user " + domain + "\\" + userName + " with password " + password + " failed. " + error);
            }
        }

        public void Dispose()
        {
            if (impersonationContext != null)
                impersonationContext.Undo();
        }
    }
}