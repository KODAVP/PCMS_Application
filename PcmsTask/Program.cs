using PcmsTask.Commons;
using PcmsTask.Jobs;
using PcmsTask.Models;

namespace PcmsTask
{
    class Program
    {
        static void Main(string[] args)
        {
            /*
            string basedir = AppDomain.CurrentDomain.BaseDirectory;
            string b2 = System.IO.Path.GetDirectoryName(System.Reflection.Assembly.GetExecutingAssembly().GetName().CodeBase);
            string n2 = basedir + @"xfer\xfer.exe";
            string xferargs = @"-z https://secure-transfer.cegedim.com -user:okc_pfizer_kr -password:4azr39 -s:" + basedir + @"xfer\xfer_n360_import.txt";
            var processInfo = new ProcessStartInfo(n2, xferargs);
            processInfo.CreateNoWindow = true;
            processInfo.UseShellExecute = true;
            processInfo.WorkingDirectory = basedir + @"xfer\";

            var process = Process.Start(processInfo);
            process.WaitForExit();
            process.Close();
            */
//#if DEBUG
//            PCMSDBContext db = new PCMSDBContext();
//            Channel c = db.Channels.Find(2);
//            N360Outbound bound = new N360Outbound(c);
//            bound.Execute();
//            //SMTPHelper.SendAlertInterface("Do-Hun.Kim@pfizer.com", "MMS");
//#else
//            TaskRunner tr = new TaskRunner();
//            tr.TaskApi();
//#endif

            TaskRunner tr = new TaskRunner();
            tr.TaskApi();

        }


    }
}
