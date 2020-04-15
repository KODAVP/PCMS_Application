using PrivacyConsentDB.Models;
using System;
using System.Configuration;


namespace PrivacyConsentDB.Commons
{
    public class NetworkDriveHelper
    {
        public static void Connect()
        {
            Disconnect();
            PCMSDBContext db = new PCMSDBContext();

            // 네트워크 드라이브 연결
            AppSettingsReader appSettingsReader = new AppSettingsReader();
            string userName = @"SRVASP-PCMSMGT";
            string password = @"Pfe51627";
            try
            {
                IWshRuntimeLibrary.IWshNetwork_Class network = new IWshRuntimeLibrary.IWshNetwork_Class();
                network.MapNetworkDrive("i:", @"\\SDCUNS600VFS02\kr_pcms", Type.Missing, userName, password);
            }
#pragma warning disable CS0168 // The variable 'e' is declared but never used
            catch (Exception e)
#pragma warning restore CS0168 // The variable 'e' is declared but never used
            {
            }

        }

        public static void Disconnect()
        {
            try
            {
                IWshRuntimeLibrary.IWshNetwork_Class network = new IWshRuntimeLibrary.IWshNetwork_Class();
                network.RemoveNetworkDrive("i:");
            }
#pragma warning disable CS0168 // The variable 'e' is declared but never used
            catch (Exception e)
#pragma warning restore CS0168 // The variable 'e' is declared but never used
            {
            }
        }

    }
}