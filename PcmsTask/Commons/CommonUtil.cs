using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;

namespace PcmsTask.Commons
{
    public class CommonUtil
    {
        public static DateTime toKoreaDT(DateTime utc) {
            return TimeZoneInfo.ConvertTimeFromUtc(utc, TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time"));
        }
        public static DateTime toUtcDT(DateTime korea) {
            TimeZoneInfo kr = TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time");
            return TimeZoneInfo.ConvertTimeToUtc(korea, kr);
        }

        public static void LogWrite(string str)
        {
            try {
                string uploadfilename = @"\\SDCUNS600VFS02\kr_pcms\Temp\pcmsTask_" + DateTime.UtcNow.ToString("yyyyMMdd") + @".txt";
                StreamWriter fileStream = new StreamWriter(uploadfilename, true);
                fileStream.WriteLine(DateTime.UtcNow.ToString("yyyy-MM-dd HH:mm:ss") + " [" +AppDomain.CurrentDomain.BaseDirectory+"] "+str);
                fileStream.Close();
                fileStream.Dispose();
            } catch (Exception e) {
                Console.WriteLine(e.Message);
            }
        }
    }
}