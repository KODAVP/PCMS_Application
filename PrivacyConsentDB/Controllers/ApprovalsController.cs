using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PrivacyConsentDB.Models;
using PrivacyConsentDB.Commons;
using static PrivacyConsentDB.Commons.Status;
using PrivacyConsentDB.Dto;
using System.Web.Services;
using ClosedXML.Excel;
using System.IO;

namespace PrivacyConsentDB.Controllers
{
    [Log]
    [Auth]
    public class ApprovalsController : Controller
    {
        private PCMSDBContext db = new PCMSDBContext();

        // GET: Approvals
        public ActionResult Index(ApprovalSearch search)
        {
            ApprovalIndexDto dto = new ApprovalIndexDto();
            IEnumerable<Approval> list = db.Approvals.Where(a => a.status == ApprovalStatus.Request);
            if (search.requestbegindt != null) list = list.Where(a => a.createdate >= search.requestbegindt);
            if (search.requestenddt != null) list = list.Where(a => a.createdate <= search.requestenddt);
            if (!string.IsNullOrEmpty(search.owner)) list = list.Where(a => a.creater.Contains(search.owner));
            if (!string.IsNullOrEmpty(search.name)) list = list.Where(a => a.privacy.IND_FULL_NAME.Contains(search.name));

            dto.Approvals = list.ToList();
            dto.Search = search;

            ViewBag.CompaniesList = GetCompanyList.GetCompany();

            return View(dto);
        }

        // GET: Approvals
        public ActionResult History(ApprovalSearch search)
        {
            string currentuser = !string.IsNullOrEmpty(User?.Identity?.Name) ? User.Identity.Name.ToUpper() : "Anonymous";
            currentuser = currentuser.Substring(currentuser.IndexOf('\\') + 1).Replace("\\", "").ToUpper();

            

            ApprovalIndexDto dto = new ApprovalIndexDto();
            IEnumerable<Approval> list = db.Approvals.Where(a => 1 == 1);
            if (search.sp != null) list = list.Where(p => p.privacy.IND_SP.Contains(search.sp));
            if (search.hospital != null) list = list.Where(p => p.privacy.WKP_NAME.Contains(search.hospital));

            if (!MyRoleManager.hasRole(MyRoleManager.RoleType.SYSTEMADMIN) && !MyRoleManager.hasRole(MyRoleManager.RoleType.DCEADMIN))
            {
                list = list.Where(p => p.creater.Equals(currentuser));
            } else if (search.owner != null)
            {
                list = list.Where(p => p.creater.Equals(search.owner));
            }

            if (search.status != null && !search.status.Equals("total")) {
                if (search.status.Equals("approved")) list = list.Where(p => p.status.Equals(ApprovalStatus.Approved));
                else if (search.status.Equals("rejected")) list = list.Where(p => p.status.Equals(ApprovalStatus.Rejected));
                else if (search.status.Equals("request")) list = list.Where(p => p.status.Equals(ApprovalStatus.Request));
            } else
            {
                list = list.Where(p => p.status != ApprovalStatus.Request);
            }
            if (search.requestbegindt != null) list = list.Where(a => a.createdate >= search.requestbegindt);
            if (search.requestenddt != null) list = list.Where(a => a.createdate <= search.requestenddt);

            if (search.approvalbegindt != null) list = list.Where(a => a.modifieddate >= search.approvalbegindt);
            if (search.approvalenddt != null) list = list.Where(a => a.modifieddate <= search.approvalenddt);

            if (!string.IsNullOrEmpty(search.owner)) list = list.Where(a => a.creater.Contains(search.owner));
            if (!string.IsNullOrEmpty(search.name)) list = list.Where(a => a.privacy.IND_FULL_NAME.Contains(search.name));

            dto.Approvals = list.ToList();
            dto.Search = search;

            ViewBag.CompaniesList = GetCompanyList.GetCompany();

            return View(dto);
        }


        // GET: Approvals/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Approval approval = db.Approvals.Find(id);
            if (approval == null)
            {
                return HttpNotFound();
            }
            return View(approval);
        }

        // GET: Approvals/Create
        public ActionResult Create()
        {
            ViewBag.CompaniesList = GetCompanyList.GetCompany();
            return View();
        }

        // POST: Approvals/Create
        // 초과 게시 공격으로부터 보호하려면 바인딩하려는 특정 속성을 사용하도록 설정하십시오. 
        // 자세한 내용은 http://go.microsoft.com/fwlink/?LinkId=317598을(를) 참조하십시오.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ApprovalId,approvalstatus,createdate,creater,modifieddate,Companies")] Approval approval)
        {
            if (ModelState.IsValid)
            {
                db.Approvals.Add(approval);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(approval);
        }

        // GET: Approvals/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Approval approval = db.Approvals.Find(id);
            if (approval == null)
            {
                return HttpNotFound();
            }
            return View(approval);
        }
        [WebMethod]
        public JsonResult DecideRequest(int id, int action, string message)
        {
            Approval approval = db.Approvals.Find(id);
            if (approval == null)
            {
                return Json(new { result = false });
            }
            Privacy p = db.Privacies.Find(approval.privacyId);
            
            if (action == 1)
            {
                if (!CommonUtil.checkEmail(p.EMAIL) && !CommonUtil.checkMobile(p.MOBILE)) {
                    return Json(new { result = false, msg = @"일부 데이터가 적합하지 않습니다. 승인할 수 없습니다.[이메일,핸드폰]" });
                }
                if (string.IsNullOrEmpty(p.CONSENT.CONSENT_SOURCE))
                {
                    return Json(new { result = false, msg = @"일부 데이터가 적합하지 않습니다. 승인할 수 없습니다.[동의서채널]" });
                }
                approval.status = ApprovalStatus.Approved;
            } else if(action == 2) { 
                approval.status = ApprovalStatus.Rejected;
            }
            if (!string.IsNullOrEmpty(message))
                approval.message = message;

            db.Entry(approval).State = EntityState.Modified;
            db.SaveChanges();
            if (!string.IsNullOrEmpty(p.LINK_RESERVATION) && approval.status == ApprovalStatus.Approved) SMTPHelper.SendAlertPfizerLink(p);
            if (approval.status == ApprovalStatus.Approved) SMTPHelper.SendAlertApproved(p);
            else if (approval.status == ApprovalStatus.Rejected) SMTPHelper.SendAlertRejected(p);
            return Json(new { result = true });
        }

        // Put: Approvals/Approve/5
        public ActionResult Approve(int id)
        {
            Approval approval = db.Approvals.Find(id);
            if (approval == null)
            {
                return HttpNotFound();
            }
            approval.status = ApprovalStatus.Approved;
            db.Entry(approval).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", "Approvals");
        }

        // Put: Approvals/Deny/5
        public ActionResult Deny(int id)
        {
            Approval approval = db.Approvals.Find(id);
            if (approval == null)
            {
                return HttpNotFound();
            }
            approval.status = ApprovalStatus.Rejected;
            db.Entry(approval).State = EntityState.Modified;
            db.SaveChanges();
            return RedirectToAction("Index", "Approvals");
        }

        // POST: Approvals/Edit/5
        // 초과 게시 공격으로부터 보호하려면 바인딩하려는 특정 속성을 사용하도록 설정하십시오. 
        // 자세한 내용은 http://go.microsoft.com/fwlink/?LinkId=317598을(를) 참조하십시오.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ApprovalId,approvalstatus,createdate,creater,modifieddate")] Approval approval)
        {
            if (ModelState.IsValid)
            {
                db.Entry(approval).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(approval);
        }

        // GET: Approvals/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Approval approval = db.Approvals.Find(id);
            if (approval == null)
            {
                return HttpNotFound();
            }
            return View(approval);
        }

        // POST: Approvals/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Approval approval = db.Approvals.Find(id);
            db.Approvals.Remove(approval);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult XlsDownload(ApprovalSearch search)
        {
            IEnumerable<Approval> list = db.Approvals.Where(a => a.status != ApprovalStatus.Request);
            if (search.sp != null) list = list.Where(p => p.privacy.IND_SP.Contains(search.sp));
            if (search.hospital != null) list = list.Where(p => p.privacy.WKP_NAME.Contains(search.hospital));

            if (search.status != null && !search.status.Equals("total"))
            {
                if (search.status.Equals("approved")) list = list.Where(p => p.status.Equals(ApprovalStatus.Approved));
                else if (search.status.Equals("rejected")) list = list.Where(p => p.status.Equals(ApprovalStatus.Rejected));
            }
            if (search.requestbegindt != null) list = list.Where(a => a.createdate >= search.requestbegindt);
            if (search.requestenddt != null) list = list.Where(a => a.createdate <= search.requestenddt);

            if (search.approvalbegindt != null) list = list.Where(a => a.modifieddate >= search.approvalbegindt);
            if (search.approvalenddt != null) list = list.Where(a => a.modifieddate <= search.approvalenddt);

            if (!string.IsNullOrEmpty(search.owner)) list = list.Where(a => a.creater.Contains(search.owner));
            if (!string.IsNullOrEmpty(search.name)) list = list.Where(a => a.privacy.IND_FULL_NAME.Contains(search.name));

            // Xls export
            var wb = new XLWorkbook();
            var sheetname = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var ws = wb.Worksheets.Add("ApprovalHistory" + sheetname);

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
            ws.Cell("U1").Value = "담당자";
            ws.Cell("V1").Value = "승인상태";
            ws.Cell("W1").Value = "요청일자(KST)";
            ws.Cell("X1").Value = "승인자";
            ws.Cell("Y1").Value = "승인일자(KST)";

            int row = 2;
            foreach (Approval p in list)
            {
                ws.Cell(row, 1).Value = p.privacy.NucleusKey;
                ws.Cell(row, 2).Value = p.privacy.OneKey;
                ws.Cell(row, 3).Value = p.privacy.PCMSID;
                ws.Cell(row, 4).Value = p.privacy.WKP_NAME;
                ws.Cell(row, 5).Value = p.privacy.WKP_TEL;
                ws.Cell(row, 6).Value = p.privacy.ZIP;
                ws.Cell(row, 7).Value = p.privacy.FULL_ADDR;
                ws.Cell(row, 8).Value = p.privacy.IND_SP;
                ws.Cell(row, 9).Value = p.privacy.TITLE;
                ws.Cell(row, 10).Value = p.privacy.IND_FULL_NAME;
                ws.Cell(row, 11).Value = p.privacy.EMAIL;
                ws.Cell(row, 12).Value = p.privacy.MOBILE;
                ws.Cell(row, 13).Value = p.privacy.Unsubscribe;
                ws.Cell(row, 14).Value = p.privacy.CONSENT_USE;
                ws.Cell(row, 15).Value = p.privacy.CONSENT_TRUST;
                ws.Cell(row, 16).Value = p.privacy.CONSENT_ABROAD;
                ws.Cell(row, 17).Value = p.privacy.CONSENT_SIGN;
                ws.Cell(row, 18).Value = p.privacy.CONSENT.CONSENT_VERSION;
                ws.Cell(row, 19).Value = p.privacy.CONSENT.CONSENT_DATE_KOREA;
                ws.Cell(row, 20).Value = p.privacy.CONSENT.CONSENT_SOURCE;
                ws.Cell(row, 21).Value = p.privacy.creater;
                ws.Cell(row, 22).Value = p.status == ApprovalStatus.Request ? "요청" : p.status == ApprovalStatus.Rejected ? "반려" : "승인";
                ws.Cell(row, 23).Value = TimeZoneInfo.ConvertTime(p.createdate, TimeZoneInfo.Utc, TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time")).ToString();
                ws.Cell(row, 24).Value = p.modifier;
                ws.Cell(row, 25).Value = TimeZoneInfo.ConvertTime(p.modifieddate, TimeZoneInfo.Utc, TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time")).ToString();
                row++;
            }
            Stream fs = new MemoryStream();
            wb.SaveAs(fs);
            fs.Position = 0;

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = "approval_history" + sheetname + ".xlsx",
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
