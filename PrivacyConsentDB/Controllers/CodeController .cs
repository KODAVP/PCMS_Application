
using System.Web.Mvc;
using PrivacyConsentDB.Models;
using PrivacyConsentDB.Dto;
using Common.Logging;
using System.Reflection;
using PrivacyConsentDB.Commons;
using System.Linq;
using ClosedXML.Excel;
using System;
using System.Collections.Generic;
using System.IO;
using static PrivacyConsentDB.Commons.Status;

namespace PrivacyConsentDB.Controllers
{
    [Log]
    [Auth]
    public class CodeController : Controller
    {
        private PCMSDBContext db = new PCMSDBContext();
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        public ActionResult InvalidList(InvalidDto search) {
            search.list =  db.Privacies.Where(p => p.SENDCHANEL.HasFlag(SendChannel.PFORCERX_INVALID) || p.SENDCHANEL.HasFlag(SendChannel.PFORCERX_ONEKEY_DUP)).ToList();
            
            return View(search);
        }

        public ActionResult Index(CodeDto search)
        {
            IQueryable<PrivacyGroupDto> results;
            // privacys.OrderByDescending(pv => pv.PCMSID).Skip(search.startIndex * search.pageSize).Take(search.pageSize).ToList();
            if (string.IsNullOrEmpty(search.searchname))
            {
                if(search.onekey == null || search.onekey.Length == 0)
                results = from p in db.Privacies
                              where p.OneKey != null && p.status == Status.PrivacyStatus.GRANTED
                              group p by p.OneKey into g                              
                              select new PrivacyGroupDto { OneKey = g.Key, Count = g.Count(), Name = g.OrderBy(p => p.IND_FULL_NAME).FirstOrDefault().IND_FULL_NAME, Privacies = g.OrderBy(p => p.IND_FULL_NAME) }
                              
                          ;
                else
                    results = from p in db.Privacies
                              where p.OneKey.Contains(search.onekey) && p.status == Status.PrivacyStatus.GRANTED
                              group p by p.OneKey into g
                              select new PrivacyGroupDto { OneKey = g.Key, Count = g.Count(), Name = g.OrderBy(p => p.IND_FULL_NAME).FirstOrDefault().IND_FULL_NAME, Privacies = g.OrderBy(p => p.IND_FULL_NAME) }

                          ;
            }
            else {
                if (search.onekey == null || search.onekey.Length == 0)
                    results = from p in db.Privacies
                              where p.OneKey != null && p.status == Status.PrivacyStatus.GRANTED && p.IND_FULL_NAME.Contains(search.searchname)
                              group p by p.OneKey into g
                              select new PrivacyGroupDto { OneKey = g.Key, Count = g.Count(), Name = g.OrderBy(p => p.IND_FULL_NAME).FirstOrDefault().IND_FULL_NAME, Privacies = g.OrderBy(p => p.IND_FULL_NAME) }
                          ;
                else
                    results = from p in db.Privacies
                              where p.OneKey.Contains(search.onekey) && p.status == Status.PrivacyStatus.GRANTED && p.IND_FULL_NAME.Contains(search.searchname)
                              group p by p.OneKey into g
                              select new PrivacyGroupDto { OneKey = g.Key, Count = g.Count(), Name = g.OrderBy(p => p.IND_FULL_NAME).FirstOrDefault().IND_FULL_NAME, Privacies = g.OrderBy(p => p.IND_FULL_NAME) }
                          ;
            }
            search.totalCount = results.Count();
            search.list = results.OrderBy(r=>r.OneKey).Skip(search.startIndex * search.pageSize).Take(search.pageSize).ToList();

            ViewBag.CompaniesList = GetCompanyList.GetCompany();

            return View(search);
        }

        public ActionResult XlsDownload(CodeDto search)
        {
            // Data
            var privacys = db.Privacies.Where(p=> p.OneKey != null && p.status == Status.PrivacyStatus.GRANTED );
            if (!string.IsNullOrEmpty(search.onekey))
            {
                privacys = privacys.Where(p => p.OneKey.Contains(search.onekey));
            }

            if (!string.IsNullOrEmpty(search.searchname))
            {
                privacys = privacys.Where(p=>p.IND_FULL_NAME.Contains(search.searchname));
            }
            privacys = privacys.OrderBy(p => p.OneKey).OrderBy(p => p.NucleusKey);
            // Xls export
            var wb = new XLWorkbook();
            var sheetname = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var ws = wb.Worksheets.Add("CodeMatch" + sheetname);

            ws.Cell("A1").Value = "NucleusKey";
            ws.Cell("B1").Value = "OneKey";
            ws.Cell("C1").Value = "PCMSID";
            ws.Cell("D1").Value = "근무처(병원명)";
            ws.Cell("E1").Value = "근무처연락처";
            ws.Cell("F1").Value = "우편번호";
            ws.Cell("G1").Value = "주소";
            ws.Cell("H1").Value = "진료과";
            ws.Cell("I1").Value = "직위";
            ws.Cell("J1").Value = "고객명";
            ws.Cell("K1").Value = "이메일";
            ws.Cell("L1").Value = "핸드폰";
            ws.Cell("M1").Value = "수신거부여부";
            ws.Cell("N1").Value = "수집/이용동의";
            ws.Cell("O1").Value = "위탁동의";
            ws.Cell("P1").Value = "국외이전동의";
            ws.Cell("Q1").Value = "서명여부";
            ws.Cell("R1").Value = "동의서 버전";
            ws.Cell("S1").Value = "동의일자";
            ws.Cell("T1").Value = "동의채널";
            List<Privacy> list = privacys.ToList();
            int row = 2;
            foreach (Privacy p in list)
            {
                ws.Cell(row, 1).Value = p.NucleusKey;
                ws.Cell(row, 2).Value = p.OneKey;
                ws.Cell(row, 3).Value = p.PCMSID;
                ws.Cell(row, 4).Value = p.WKP_NAME;
                ws.Cell(row, 5).Value = p.WKP_TEL;
                ws.Cell(row, 6).Value = p.ZIP;
                ws.Cell(row, 7).Value = p.FULL_ADDR;
                ws.Cell(row, 8).Value = p.IND_SP;
                ws.Cell(row, 9).Value = p.TITLE;
                ws.Cell(row, 10).Value = p.IND_FULL_NAME;
                ws.Cell(row, 11).Value = p.EMAIL;
                ws.Cell(row, 12).Value = p.MOBILE;
                ws.Cell(row, 13).Value = p.Unsubscribe;
                ws.Cell(row, 14).Value = p.CONSENT_USE;
                ws.Cell(row, 15).Value = p.CONSENT_TRUST;
                ws.Cell(row, 16).Value = p.CONSENT_ABROAD;
                ws.Cell(row, 17).Value = p.CONSENT_SIGN;
                ws.Cell(row, 18).Value = p.CONSENT.CONSENT_VERSION;
                ws.Cell(row, 19).Value = p.CONSENT.CONSENT_DATE_KOREA;
                ws.Cell(row, 20).Value = p.CONSENT.CONSENT_SOURCE;
                row++;
            }
            Stream fs = new MemoryStream();
            wb.SaveAs(fs);
            fs.Position = 0;

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = "pcms_code_" + sheetname + ".xlsx",
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(fs, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}
