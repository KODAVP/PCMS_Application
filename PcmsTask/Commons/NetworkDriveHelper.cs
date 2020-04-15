using PcmsTask.Models;
using System;
using System.Configuration;

namespace PcmsTask.Commons
{
    public class NetworkDriveHelper
    {
        public static void Connect() {
            Disconnect();
            PCMSDBContext db = new PCMSDBContext();

            // 네트워크 드라이브 연결
            AppSettingsReader appSettingsReader = new AppSettingsReader();
            string userName = (string)appSettingsReader.GetValue("ImpersonateUser", typeof(string));
            string password = (string)appSettingsReader.GetValue("ImpersonatePassword", typeof(string));
            try
            {
                IWshRuntimeLibrary.IWshNetwork_Class network = new IWshRuntimeLibrary.IWshNetwork_Class();
                network.MapNetworkDrive("i:", @"\\SDCUNS600VFS02\kr_pcms", Type.Missing, userName, password);
            }
            catch (Exception e)
            {                
                CommonUtil.LogWrite(e.Message);
            }

        }

        public static void Disconnect() {
            try
            {
                IWshRuntimeLibrary.IWshNetwork_Class network = new IWshRuntimeLibrary.IWshNetwork_Class();
                network.RemoveNetworkDrive("i:");
            }
            catch (Exception e)
            {                
                CommonUtil.LogWrite(e.Message);
            }
        }

    }
}