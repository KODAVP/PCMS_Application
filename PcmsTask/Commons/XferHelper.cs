using PcmsTask.Models;
using System;
using System.Diagnostics;
using System.IO;
using System.Configuration;
using System.Text;

namespace PcmsTask.Commons
{
    public class XferHelper
    {
        public enum XferActionEnum {
            Export , Import
        }

        public static void Call(XferActionEnum action, string scriptfile)
        {
            StringBuilder sb = new StringBuilder();
            PCMSDBContext db = new PCMSDBContext();
            
            string basefolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"xfer\");
            string exepath = basefolder + @"xfer.exe";
            try
            {
                string id = ConfigurationManager.AppSettings["user"];
                string pwd = ConfigurationManager.AppSettings["password"];
               
                //string xferargs = @" -z https://secure-transfer.cegedim.com -user:" + id + " -password:" + pwd + " -s:" + scriptfile;
                string xferargs = @" -z https://SECURE-TRANSFER.SOLUTIONS.IQVIA.COM -user:" + id + " -password:" + pwd + " -s:" + scriptfile;

                Process p = new Process();
                var processInfo = p.StartInfo;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = false;
                p.StartInfo.WorkingDirectory = basefolder;
                p.StartInfo.FileName = exepath;
                p.StartInfo.Arguments = xferargs;

                p.OutputDataReceived += new DataReceivedEventHandler((s, e) => { sb.Append(e.Data); sb.Append("\r\n"); });
                p.ErrorDataReceived += new DataReceivedEventHandler((s, e) => { sb.Append(e.Data); sb.Append("\r\n"); });

                p.Start();
                p.BeginOutputReadLine();
                p.WaitForExit();
                p.Close();

                Batch batch = new Batch { name = @"XPER_" + action.ToString(), message = sb.ToString(), status = BatchStatus.Running };
                db.Batches.Add(batch);
                db.SaveChanges();
            }
            catch (Exception e)
            {
                sb.Append(e.Message);
                Batch batch = new Batch { name = @"XPER_" + action.ToString(), message = sb.ToString(), status = BatchStatus.Error };
                db.Batches.Add(batch);
                db.SaveChanges();
            }
        }

        public static void Call(XferActionEnum action, string localpath, string remotepath)
        {
            StringBuilder sb = new StringBuilder();
            PCMSDBContext db = new PCMSDBContext();
            
            string basefolder = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, @"xfer\");
            string exepath = basefolder + @"xfer.exe";
            string scriptfile = basefolder + @"xfer_n360_" + action.ToString() + "_" + DateTime.UtcNow.ToString("yyyyMMddTHHmmss") + @".txt";
            try
            {
                StreamWriter fileStream = new StreamWriter(scriptfile);
                fileStream.WriteLine(@"lcd " + localpath);
                fileStream.WriteLine(@"cd " + remotepath);
                if(action == XferActionEnum.Import)
                {
                    fileStream.WriteLine(@"mget *.txt");
                }
                else if(action == XferActionEnum.Export)
                {
                    fileStream.WriteLine(@"mput *.txt");
                }                    
                fileStream.WriteLine(@"quit");
                fileStream.Close();
                fileStream.Dispose();

                string id = ConfigurationManager.AppSettings["user"];
                string pwd = ConfigurationManager.AppSettings["password"];

                //string xferargs = @" -z https://secure-transfer.cegedim.com -user:"+id+" -password:" + pwd + " -s:" + scriptfile;
                 string xferargs = @" -z https://SECURE-TRANSFER.SOLUTIONS.IQVIA.COM -user:" + id + " -password:" + pwd + " -s:" + scriptfile;

                Process p = new Process();
                var processInfo = p.StartInfo;
                p.StartInfo.RedirectStandardError = true;
                p.StartInfo.RedirectStandardOutput = true;
                p.StartInfo.UseShellExecute = false;
                p.StartInfo.CreateNoWindow = false;
                p.StartInfo.WorkingDirectory = basefolder;
                p.StartInfo.FileName = exepath;
                p.StartInfo.Arguments = xferargs;
                
                p.OutputDataReceived += new DataReceivedEventHandler((s, e) =>{ sb.Append(e.Data); sb.Append("\r\n"); });
                p.ErrorDataReceived += new DataReceivedEventHandler((s, e) => { sb.Append(e.Data); sb.Append("\r\n"); });

                p.Start();
                p.BeginOutputReadLine();
                p.WaitForExit();
                p.Close();
                
                Batch batch = new Batch { name = @"XPER_" + action.ToString(), message = sb.ToString(), status = BatchStatus.Running };
                db.Batches.Add(batch);
                db.SaveChanges();
                File.Delete(scriptfile);
            }
            catch (Exception e)
            {
                sb.Append(e.Message);
                Batch batch = new Batch { name = @"XPER_" + action.ToString(), message = sb.ToString(), status = BatchStatus.Error };
                db.Batches.Add(batch);
                db.SaveChanges();
            }
        }        
    }
}