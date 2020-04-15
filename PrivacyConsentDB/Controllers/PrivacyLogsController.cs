using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PrivacyConsentDB.Models;
using PrivacyConsentDB.Dto;
using System.Data.Entity.Core.Objects;
using ClosedXML.Excel;
using System.IO;

namespace PrivacyConsentDB.Controllers
{
    public class PrivacyLogsController : Controller
    {
        private PCMSDBContext db = new PCMSDBContext();


        private List<PrivacyLog> PrivacyLogSearchResult(PrivacyLogSearch search)
        {
            IQueryable<PrivacyLog> logs;

            logs = db.Privacylogs.Include( p => p.privacy);
            if (!string.IsNullOrEmpty(search.modifier))
            {
                logs = logs.Where(l => l.creater.Contains(search.modifier));
            }

            if (!string.IsNullOrEmpty(search.pcmsid))
            {
                logs = logs.Where(l => l.privacy.PCMSID.Contains(search.pcmsid));
            }

            if (search.chngbegindt != null)
            {
                logs = logs.Where(l => DbFunctions.TruncateTime(l.createdate) >= DbFunctions.TruncateTime(search.chngbegindt));
            }

            if (search.chngenddt != null)
            {
                logs = logs.Where(l => DbFunctions.TruncateTime(l.createdate) <= DbFunctions.TruncateTime(search.chngenddt));
            }
            return logs.ToList();
        }
        // GET: PrivacyLogs
        public ActionResult Index(PrivacyLogIndexDto search)
        {
            IQueryable<PrivacyLog> logs;

            logs = db.Privacylogs.Include(p => p.privacy);
            if (!string.IsNullOrEmpty(search.modifier))
            {
                logs = logs.Where(l => l.creater.Contains(search.modifier));
            }

            if (!string.IsNullOrEmpty(search.pcmsid))
            {
                logs = logs.Where(l => l.privacy.PCMSID.Contains(search.pcmsid));
            }

            if (search.chngbegindt != null)
            {
                logs = logs.Where(l => DbFunctions.TruncateTime(l.createdate) >= DbFunctions.TruncateTime(search.chngbegindt));
            }

            if (search.chngenddt != null)
            {
                logs = logs.Where(l => DbFunctions.TruncateTime(l.createdate) <= DbFunctions.TruncateTime(search.chngenddt));
            }
            logs = logs.OrderByDescending(l => l.ID);

            search.totalCount = logs.Count();
            search.Logs = logs.Skip(search.startIndex * search.pageSize).Take(search.pageSize).ToList();
            
            return View(search);
        }

        public ActionResult XlsDownload(PrivacyLogSearch search)
        {
            var sheetname = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            string currentuser = !string.IsNullOrEmpty(User?.Identity?.Name) ? User.Identity.Name.ToUpper() : "Anonymous";

            // Data
            List<PrivacyLog> list = PrivacyLogSearchResult(search);
            // Xls export
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("PCMS" + sheetname);
            ws.Cell("A1").Value = "변경사항";
            ws.Cell("B1").Value = "변경일자";
            ws.Cell("C1").Value = "변경자";
            ws.Cell("D1").Value = "링크";
            int row = 2;
            foreach (PrivacyLog p in list)
            {
                ws.Cell(row, 1).Value = p.changes;
                ws.Cell(row, 2).Value = p.createdate;
                ws.Cell(row, 3).Value = p.creater;
                ws.Cell(row, 4).Value = Request.Url.Authority + "/Privacy/Details/" + p.privacy.ID;
                row++;
            }
            Stream fs = new MemoryStream();
            wb.SaveAs(fs);
            fs.Position = 0;

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = "pcms_changes_" + sheetname + ".xlsx",
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(fs, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }


        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
