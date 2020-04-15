using log4net;
using PcmsTask.Models;
using System;
using System.Configuration;
using System.Net.Mail;
using System.Reflection;
using System.Collections.Generic;
using System.IO;
using System.Linq;
namespace PcmsTask.Commons
{
    public class SMTPHelper
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static SmtpClient initClient()
        {
            SmtpClient client = new SmtpClient("sgpsmtp.apac.pfizer.com", 25);
            client.UseDefaultCredentials = false;
            client.EnableSsl = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            return client;
        }

        public static bool TestMail()
        {
            SmtpClient client = initClient();
            AppSettingsReader appSettingsReader = new AppSettingsReader();
            string smtpAccount = "PCMSKorea@pfizer.com";
            string remail = string.Empty;
            // 
            MailMessage message = new MailMessage();
            try
            {
                remail = "Do-Hun.Kim@pfizer.com";

                message.From = new MailAddress(smtpAccount);
                message.To.Add(remail);
                message.BodyEncoding = System.Text.Encoding.UTF8;
                message.SubjectEncoding = System.Text.Encoding.UTF8;
                message.IsBodyHtml = true;
                message.Subject = "[PCMS]test";
                message.Body = @"test";
                client.Send(message);
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return false;
            }
            finally
            {
                message.Dispose();
            }

            message.Dispose();
            return true;
        }
        private static MailMessage initMail(Status.SettingType stype)
        {
            PCMSDBContext db = new PCMSDBContext();
            AppSettingsReader appSettingsReader = new AppSettingsReader();
            string smtpAccount = "PCMSKorea@pfizer.com";

            // 승인 발생시 승인요청 알림.
            Setting conf = db.Settings.Where(s => s.type == stype).First();
            String[] toaddr = conf.value.Split(';');

            MailMessage message = new MailMessage();
            message.From = new MailAddress(smtpAccount);
            try
            {
                foreach (var a in toaddr)
                {
                    if (!string.IsNullOrEmpty(a))
                    {
                        string remail = a;
                        if (!string.IsNullOrEmpty(remail)) message.To.Add(remail);
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
            }
            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;

            return message;
        }
        private static MailMessage initMail(string receiver) // receiver = NTID
        {
            PCMSDBContext db = new PCMSDBContext();
            AppSettingsReader appSettingsReader = new AppSettingsReader();
            string smtpAccount = "PCMSKorea@pfizer.com";
            string remail = string.Empty;
            // 
            remail = receiver;

            MailMessage message = new MailMessage();
            message.From = new MailAddress(smtpAccount);
            message.To.Add(remail);

            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;

            return message;
        }

        public static void SendAlertInterface(string inf, string msg = null)
        {
            PCMSDBContext db = new PCMSDBContext();
            SmtpClient client = initClient();
            MailMessage message = initMail(Status.SettingType.InterfaceAlertMail);

            message.Subject = "[PCMS] 인터페이스 오류 알림";
            try
            {
                message.Body = "Interface error occured from " + inf +"\n" + msg;
                db.Userlogs.Add(new Userlog { useremail = message.To.ToString(), reqtype = @"Email", url = message.Subject, parameters = message.Body });
                client.Send(message);
            }
            catch (Exception e)
            {
                log.Error(e);
            }

            message.Dispose();
            db.SaveChanges();
        }
    }
}