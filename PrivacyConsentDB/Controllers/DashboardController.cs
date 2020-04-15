using ClosedXML.Excel;
using PrivacyConsentDB.Commons;
using PrivacyConsentDB.Dto;
using PrivacyConsentDB.Models;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using static PrivacyConsentDB.Commons.Status;

namespace PrivacyConsentDB.Controllers
{
    [Log]
    [Auth]
    public class DashboardController : Controller
    {
        private PCMSDBContext db = new PCMSDBContext();
        // GET: Dashboard
        public ActionResult Dashboard(string pcmsid = null)
        {
            string currentuser = !string.IsNullOrEmpty(User?.Identity?.Name) ? User.Identity.Name.ToUpper() : "Anonymous";
            currentuser = currentuser.Substring(currentuser.IndexOf('\\') + 1).Replace("\\", "").ToUpper();

            DashboardDto dto = new DashboardDto();

            if (!MyRoleManager.hasRole(MyRoleManager.RoleType.SYSTEMADMIN) && !MyRoleManager.hasRole(MyRoleManager.RoleType.MARKETING) && !MyRoleManager.hasRole(MyRoleManager.RoleType.DCEADMIN) && !MyRoleManager.hasRole(MyRoleManager.RoleType.BUMCM))
            {
                dto.approvalrequests = db.Approvals.Where(a => a.status == ApprovalStatus.Request && a.creater == currentuser).ToList().Count();
                dto.rejectedrequests = db.Approvals.Where(a => a.status == ApprovalStatus.Rejected && a.creater == currentuser).ToList().Count();

                if (pcmsid == null)
                    dto.logs = db.Privacylogs.Where(p => p.creater == currentuser).OrderByDescending(p => p.createdate).Take(300).ToList();
                else
                    dto.logs = db.Privacylogs.Where(p => p.creater == currentuser && p.privacy.PCMSID.Contains(pcmsid)).OrderByDescending(p => p.createdate).Take(300).ToList();
            }
            else if (MyRoleManager.hasRole(MyRoleManager.RoleType.DCEADMIN))
            {
                var query = from u in db.UserRoles
                            join c in db.Companies on u.COMP_CODE equals c.COMP_CODE
                            select c.DCE_TSA;

                foreach (var q in query)
                {
                    if (q == "Y")
                    {
                        dto.approvedrequests = db.Approvals.Where(a => a.status == ApprovalStatus.Approved).ToList().Count();
                        dto.rejectedrequests = db.Approvals.Where(a => a.status == ApprovalStatus.Rejected).ToList().Count();
                        dto.approvalrequests = db.Approvals.Where(a => a.status == ApprovalStatus.Request).ToList().Count();


                        dto.mmscount = db.Consents.Where(c => c.CONSENT_SOURCE == @"MMS").Count();
                        //dto.pforcerxcount = db.Privacylogs.Where(pl => pl.creater == @"PFORCERX").Count();
                        dto.pforcerxcount = db.Consents.Where(c => c.CONSENT_SOURCE == @"PFORCERX").Count();

                        dto.hardcount = db.Consents.Where(c => c.CONSENT_SOURCE != @"MMS" && c.CONSENT_SOURCE != @"GRV" && c.CONSENT_SOURCE != @"PFORCERX").Select(c => c.privacy).Distinct().Count();

                        dto.grvcount = db.Consents.Where(c => c.CONSENT_SOURCE == @"GRV").Count();

                        if (pcmsid == null)
                            dto.logs = db.Privacylogs.OrderByDescending(p => p.createdate).Take(300).ToList();
                        else
                            dto.logs = db.Privacylogs.Where(p => p.privacy.PCMSID.Contains(pcmsid)).OrderByDescending(p => p.createdate).Take(300).ToList();
                    }
                }
            }
            else
            {
                dto.approvedrequests = db.Approvals.Where(a => a.status == ApprovalStatus.Approved).ToList().Count();
                dto.rejectedrequests = db.Approvals.Where(a => a.status == ApprovalStatus.Rejected).ToList().Count();
                dto.approvalrequests = db.Approvals.Where(a => a.status == ApprovalStatus.Request).ToList().Count();

                // 만료 갯수
                /*
                Setting setting = db.Settings.Where(s => s.type == SettingType.ConsentTerm).First();
                DateTime expired = DateTime.UtcNow.AddYears(-1 * Int32.Parse(setting.value));
                IQueryable<Privacy> privacys = db.Consents.Where(cst => cst.CONSENT_DATE < expired).Select(cst => cst.privacy).Distinct();
                privacys = privacys.Where(p => p.status != PrivacyStatus.DELETED || p.status != PrivacyStatus.DELETED);
                var count = privacys.Count();

                // String sqlQuery = "select count(*) from Privacies where id in (select privacy_ID from Consents where CONSENT_DATE < DATEADD(year ," + -1 * Int32.Parse(setting.value) + ", CURRENT_TIMESTAMP));";
                // var count = db.Database.SqlQuery(sqlQuery);

                dto.expiredconsents = (int)count;
                */
                // dto.mmscount = db.Privacylogs.Where(pl => pl.creater == @"MMS").Count();
                dto.mmscount = db.Consents.Where(c => c.CONSENT_SOURCE == @"MMS").Count();
                //dto.pforcerxcount = db.Privacylogs.Where(pl => pl.creater == @"PFORCERX").Count();
                dto.pforcerxcount = db.Consents.Where(c => c.CONSENT_SOURCE == @"PFORCERX").Count();

                dto.hardcount = db.Consents.Where(c => c.CONSENT_SOURCE != @"MMS" && c.CONSENT_SOURCE != @"GRV" && c.CONSENT_SOURCE != @"PFORCERX").Select(c => c.privacy).Distinct().Count();

                //dto.codemappingcount = db.Privacylogs.Where(pl => pl.creater == @"ODSM").Count();
                //dto.codemappingcount += db.Privacylogs.Where(pl => pl.creater == @"TASK" && (pl.changes.Contains(@"NucleusKey") || pl.changes.Contains(@"OneKey"))).Count();

                dto.grvcount = db.Consents.Where(c => c.CONSENT_SOURCE == @"GRV").Count();

                if (pcmsid == null)
                    dto.logs = db.Privacylogs.OrderByDescending(p => p.createdate).Take(300).ToList();
                else
                    dto.logs = db.Privacylogs.Where(p => p.privacy.PCMSID.Contains(pcmsid)).OrderByDescending(p => p.createdate).Take(300).ToList();
            }
            ViewBag.pcmsid = pcmsid;
            return View(dto);

        }

        public ActionResult XlsDownload(string pcmsid = null)
        {
            List<PrivacyLog> logs;

            string currentuser = !string.IsNullOrEmpty(User?.Identity?.Name) ? User.Identity.Name.ToUpper() : "Anonymous";
            currentuser = currentuser.Substring(currentuser.IndexOf('\\') + 1).Replace("\\", "").ToUpper();

            // Data
            if (pcmsid == null)
                logs = db.Privacylogs.Where(p => p.creater == currentuser).OrderByDescending(p => p.createdate).ToList();
            else
                logs = db.Privacylogs.Where(p => p.creater == currentuser && p.privacy.PCMSID.Contains(pcmsid)).OrderByDescending(p => p.createdate).ToList();

            // Xls export
            var wb = new XLWorkbook();
            var sheetname = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var ws = wb.Worksheets.Add("RecentActivity" + sheetname);

            ws.Cell("A1").Value = "활동시간";
            ws.Cell("B1").Value = "작업자";
            ws.Cell("C1").Value = "PCMSID";
            int row = 2;
            foreach (PrivacyLog item in logs)
            {
                var dtcreate = TimeZoneInfo.ConvertTime(item.createdate, TimeZoneInfo.Utc, TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time")).ToString();
                ws.Cell(row, 1).Value = dtcreate;
                ws.Cell(row, 2).Value = item.creater;
                ws.Cell(row, 3).Value = item.privacy.PCMSID;
                row++;
            }
            Stream fs = new MemoryStream();
            wb.SaveAs(fs);
            fs.Position = 0;

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = "RecentActivity_" + sheetname + ".xlsx",
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(fs, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}