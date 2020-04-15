using log4net;
using PrivacyConsentDB.Models;
using RazorEngine;
using RazorEngine.Templating;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Reflection;
using System.Web;
using System.Web.Security;

namespace PrivacyConsentDB.Commons
{
    public class SMTPHelper
    {
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        private static SmtpClient initClient() {
            SmtpClient client = new SmtpClient("sgpsmtp.apac.pfizer.com", 25);
            client.UseDefaultCredentials = false;
            client.EnableSsl = false;
            client.DeliveryMethod = SmtpDeliveryMethod.Network;

            return client;
        }

        public static bool TestMail() {
            SmtpClient client = initClient();
            AppSettingsReader appSettingsReader = new AppSettingsReader();
            string smtpAccount = (string)appSettingsReader.GetValue("SMTPAccount", typeof(string));
            string remail = string.Empty;
            // 
            MailMessage message = new MailMessage();
            try
            {
                MembershipUser member = Membership.GetUser(@"Choiy28");
                remail = member.Email;

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
            finally {
                message.Dispose();
            }

            message.Dispose();
            return true;
        }
        private static MailMessage initMail(Status.SettingType stype) {
            PCMSDBContext db = new PCMSDBContext();
            AppSettingsReader appSettingsReader = new AppSettingsReader();
            string smtpAccount = (string)appSettingsReader.GetValue("SMTPAccount", typeof(string));

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
                        MembershipUser member = Membership.GetUser(a);
                        string remail = member.Email;
                        if (!string.IsNullOrEmpty(remail)) message.To.Add(remail);
                    }
                }
            }
            catch (Exception e) {
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
            string smtpAccount = (string)appSettingsReader.GetValue("SMTPAccount", typeof(string));
            string remail = string.Empty;
            // 
            MembershipUser member = Membership.GetUser(receiver);
            remail = member.Email;            

            MailMessage message = new MailMessage();
            message.From = new MailAddress(smtpAccount);
            message.To.Add(remail);

            message.BodyEncoding = System.Text.Encoding.UTF8;
            message.SubjectEncoding = System.Text.Encoding.UTF8;
            message.IsBodyHtml = true;

            return message;
        }
        public static void SendApprovalRequest() {
            PCMSDBContext db = new PCMSDBContext();
            SmtpClient client = initClient();
            MailMessage message = initMail(Status.SettingType.NotificationEmail);
            message.Subject = "[PCMS:알림] 승인요청이 등록되었습니다.";
            string template = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/Template/AprovalRequestEmail.html"));
            try
            {
                message.Body = Engine.Razor.RunCompile(template, Guid.NewGuid().ToString());
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
        public static void SendAlertPfizerLink(Privacy privacy)
        {
            PCMSDBContext db = new PCMSDBContext();
            SmtpClient client = initClient();
            MailMessage message = initMail(Status.SettingType.PfizerLinkAlert);
            message.Subject = "[PCMS:알림] 화이자링크 신청이 등록되었습니다.";
            string template = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/Template/PfizerLinkEmail.html"));
            try
            {
                message.Body = Engine.Razor.RunCompile(template, Guid.NewGuid().ToString(), typeof(Privacy), privacy);
                db.Userlogs.Add(new Userlog { useremail = message.To.ToString(), reqtype = @"Email", url = message.Subject, parameters = message.Body });
                client.Send(message);
                privacy.LINK_ALERTED = true;
                db.Entry(privacy).State = System.Data.Entity.EntityState.Modified;
            }
            catch (Exception e)
            {
                log.Error(e);
            }
            
            message.Dispose();
            db.SaveChanges();
        }
        public static void SendAlertPfizerLink()
        {
            PCMSDBContext db = new PCMSDBContext();
            SmtpClient client = initClient();
            MailMessage message = initMail(Status.SettingType.PfizerLinkAlert);
            message.Subject = "[PCMS] 화이자링크 신청이 등록되었습니다.";
            string template = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/Template/PfizerLinkEmail.html"));
            IEnumerable<Privacy> pLinks = db.Privacies.Where(p=> !string.IsNullOrEmpty(p.LINK_RESERVATION) && !p.LINK_ALERTED);
            foreach (Privacy p in pLinks) {
                try
                {
                    message.Body = Engine.Razor.RunCompile(template, Guid.NewGuid().ToString(), typeof(Privacy), p);
                    db.Userlogs.Add(new Userlog { useremail = message.To.ToString(), reqtype = @"Email", url = message.Subject, parameters = message.Body });
                    client.Send(message);
                    p.LINK_ALERTED = true;
                    db.Entry(p).State = System.Data.Entity.EntityState.Modified;
                }
                catch (Exception e)
                {
                    log.Error(e);
                }
            }
            message.Dispose();
            db.SaveChanges();
        }

        public static void SendAlertRejected(Privacy privacy)
        {
            PCMSDBContext db = new PCMSDBContext();
            SmtpClient client = initClient();
            MailMessage message = initMail(privacy.OWNER);

            message.Subject = "[PCMS] 고객개인정보 승인 반려";
            string template = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/Template/RejectedEmail.html"));
            try
            {
                message.Body = Engine.Razor.RunCompile(template, Guid.NewGuid().ToString(), typeof(Privacy), privacy);
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
        public static void SendAlertApproved(Privacy privacy)
        {
            PCMSDBContext db = new PCMSDBContext();
            SmtpClient client = initClient();
            MailMessage message = initMail(privacy.OWNER);

            message.Subject = "[PCMS] 고객개인정보 승인 완료";
            string template = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/Template/ApprovedEmail.html"));
            try
            {
                message.Body = Engine.Razor.RunCompile(template, Guid.NewGuid().ToString(), typeof(Privacy), privacy);
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

        public static void SendAlertInterface(string inf)
        {
            PCMSDBContext db = new PCMSDBContext();
            SmtpClient client = initClient();
            MailMessage message = initMail(inf);

            message.Subject = "[PCMS] 인터페이스 오류 알림";
            string template = File.ReadAllText(System.Web.Hosting.HostingEnvironment.MapPath("~/Template/InterfaceAlert.html"));
            try
            {
                message.Body = Engine.Razor.RunCompile(template, Guid.NewGuid().ToString(), typeof(string), inf);
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