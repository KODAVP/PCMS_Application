using PrivacyConsentDB.Models;
using System;
using System.Diagnostics;
using System.Web;

namespace PrivacyConsentDB.Commons
{
    public class XferHelper
    {
        public static void ImportN360() {
            PCMSDBContext db = new PCMSDBContext();
            //using (ImpersonateUser u = new ImpersonateUser())
            {
                try
                {
                    string n360importpath = HttpContext.Current.Server.MapPath("~/Content/xfer/xfer.exe");
                    string xferargs = @"-z https://secure-transfer.cegedim.com -user:okc_pfizer_kr -password:4azr39 -s:" + HttpContext.Current.Server.MapPath("~/Content/xfer/xfer_n360_import.txt");
                    var processInfo = new ProcessStartInfo(n360importpath, xferargs);
                    processInfo.CreateNoWindow = true;
                    processInfo.UseShellExecute = true;
                    processInfo.WorkingDirectory = HttpContext.Current.Server.MapPath("~/Content/xfer/");
                    
                    var process = Process.Start(processInfo);
                    process.WaitForExit();
                    process.Close();
                    // System.Diagnostics.Process.Start(n360importpath, xferargs);
                }
                catch (Exception e)
                {
                    Batch batch = new Batch { name = @"XferHelper.ImportN360", message = e.Message, status = BatchStatus.Error };
                    db.Batchs.Add(batch);
                    db.SaveChanges();
                }
            }
        }

        public static void ExportN360() {
            PCMSDBContext db = new PCMSDBContext();
            string n360importpath = HttpContext.Current.Server.MapPath("~/Content/xfer/xfer.exe");
            string xferargs = @"-z https://secure-transfer.cegedim.com -user:okc_pfizer_kr -password:4azr39 -s:" + HttpContext.Current.Server.MapPath("~/Content/xfer/xfer_n360_export.txt");
            try
            {
                using (ImpersonateUser u = new ImpersonateUser())
                {
                    var processInfo = new ProcessStartInfo(n360importpath, xferargs);
                    processInfo.CreateNoWindow = false;
                    processInfo.UseShellExecute = true;
                    processInfo.WorkingDirectory = HttpContext.Current.Server.MapPath("~/Content/xfer/");

                    var process = Process.Start(processInfo);
                    process.WaitForExit();
                    process.Close();
                }
                // System.Diagnostics.Process.Start(n360importpath, xferargs);
            }
            catch (Exception e)
            {
                Batch batch = new Batch { name = @"XferHelper.ExportN360", message = e.Message, status = BatchStatus.Error };
                db.Batchs.Add(batch);
                db.SaveChanges();
            }
        }
    }
}