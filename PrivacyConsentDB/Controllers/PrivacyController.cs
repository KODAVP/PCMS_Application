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
using Common.Logging;
using System.Reflection;
using PrivacyConsentDB.Commons;
using System.IO;
using System.Web.Services;
using ClosedXML.Excel;
using static PrivacyConsentDB.Commons.Status;
using System.ComponentModel;
using System.Text;

namespace PrivacyConsentDB.Controllers
{
    [Log]
    [Auth]
    public class PrivacyController : Controller
    {
        private PCMSDBContext db = new PCMSDBContext();
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);
        private static string bulkpath = @"\\SDCUNS600VFS02\kr_pcms\Bulk";
        //private static string bulkpath = @"D:\\temp\\temp";
        // GET: Privacy

        private IEnumerable<Privacy> PrivacySearchResult(PrivacySearch search, bool alldata)
        {
            string currentuser = !string.IsNullOrEmpty(User?.Identity?.Name) ? User.Identity.Name.ToUpper() : "Anonymous";
            currentuser = currentuser.Substring(currentuser.IndexOf('\\') + 1).Replace("\\", "").ToUpper();

            IQueryable<Privacy> privacys;

            // 만료여부
            Setting setting = null;
            try
            {
                
                setting = db.Settings.Where(s => s.type == SettingType.ConsentTerm).First();

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                setting = null;
            }

            // 동의 사항에서 검색
            if (search.consentbegindt != null || search.consentenddt != null || (search.EXPIRED != null && !search.EXPIRED.Equals("total")))
            {
                var consents = db.Consents.Where(cst => 1 == 1);

                if (search.consentbegindt != null)
                    consents = consents.Where(cst => cst.CONSENT_DATE >= search.consentbegindt);

                if (search.consentenddt != null)
                    consents = consents.Where(cst => cst.CONSENT_DATE <= search.consentenddt);

                if (!search.CONSENT_TOTAL)
                {
                    consents = consents.Where(cst => cst.CONSENT_USE == search.CONSENT_USE);
                    consents = consents.Where(cst => cst.CONSENT_ABROAD == search.CONSENT_ABROAD);
                    consents = consents.Where(cst => cst.CONSENT_TRUST == search.CONSENT_TRUST);
                }

                if (setting != null)
                {
                    DateTime expired = DateTime.UtcNow.AddYears(-1 * Int32.Parse(setting.value));
                    if (search.EXPIRED != null)
                    {
                        if (search.EXPIRED.Equals("notexpired"))
                        {
                            consents = consents.Where(cst => cst.CONSENT_DATE > expired);
                        }
                        else if (search.EXPIRED.Equals("expired"))
                        {
                            consents = consents.Where(cst => cst.CONSENT_DATE <= expired);
                        }
                    }

                }
                privacys = consents.Select(cst => cst.privacy).Distinct();
            }
            else
            {
                privacys = db.Privacies;
            }

            // added BUMCM as part of APC29622417i by Nagaraju Madishetti-29/Oct/2018
            if (MyRoleManager.hasRole(MyRoleManager.RoleType.SYSTEMADMIN) || MyRoleManager.hasRole(MyRoleManager.RoleType.MARKETING) || MyRoleManager.hasRole(MyRoleManager.RoleType.DCEADMIN) || MyRoleManager.hasRole(MyRoleManager.RoleType.BUMCM))
            {
                privacys = privacys.Include(p => p.approval).Where(p => p.approval.status == ApprovalStatus.Approved || p.approval == null);

                if (!String.IsNullOrEmpty(search.owner)) privacys = privacys.Where(p => p.OWNER == search.owner);
            }
            // Commneted as part of APC29622417i by Nagaraju Madishetti-29/Oct/2018
            //else if (MyRoleManager.hasRole(MyRoleManager.RoleType.BUMCM))
            //{
            //    privacys = privacys.Include(p => p.approval).Where(p =>
            //        (p.OWNER == currentuser && p.status != PrivacyStatus.DELETED && p.status != PrivacyStatus.ERASED) || 
            //        ((p.approval.status == ApprovalStatus.Approved || p.approval == null) && p.status != PrivacyStatus.DELETED && p.status != PrivacyStatus.ERASED && p.LINK_PHONE != "") 
            //    );

            //    if (!String.IsNullOrEmpty(search.owner)) privacys = privacys.Where(p => p.OWNER == search.owner);
            //}
            else
            {
                privacys = privacys.Include(p => p.approval).Where(p => p.OWNER == currentuser);
                if (!string.IsNullOrEmpty(search.ApprovalStatus))
                {
                    ApprovalStatus astatus = (ApprovalStatus)Int32.Parse(search.ApprovalStatus);
                    privacys = privacys.Where(p => p.approval.status == astatus);
                }
            }

            //privacys = privacys.Where(p=> p.status != PrivacyStatus.DELETED || p.status != PrivacyStatus.ERASED);

            if (!String.IsNullOrEmpty(search.name)) privacys = privacys.Where(p => p.IND_FULL_NAME.Contains(search.name));
            if (!String.IsNullOrEmpty(search.depart)) privacys = privacys.Where(p => p.IND_SP.Equals(search.depart));
            if (!String.IsNullOrEmpty(search.wkpname)) privacys = privacys.Where(p => p.WKP_NAME.Contains(search.wkpname));
            if (!String.IsNullOrEmpty(search.wkpname)) privacys = privacys.Where(p => p.WKP_NAME.Contains(search.wkpname));
            if (!String.IsNullOrEmpty(search.PCMS_ID)) privacys = privacys.Where(p => p.PCMSID.Contains(search.PCMS_ID));
            if (search.active.Equals("active"))
            {
                privacys = privacys.Where(cst => cst.status != PrivacyStatus.INACTIVED);
            }
            else if (search.active.Equals("inactive"))
            {
                privacys = privacys.Where(cst => cst.status == PrivacyStatus.INACTIVED);
            }

            if (search.collectbegindt != null) privacys = privacys.Where(p => p.modifieddate >= search.collectbegindt);
            if (search.collectenddt != null) privacys = privacys.Where(p => p.modifieddate <= search.collectenddt);

            if (search.subscribe.Equals("total"))
            {
            }
            else if (search.subscribe.Equals("unsubscribe"))
            {
                privacys = privacys.Where(p => p.Unsubscribe == true);
            }
            else if (search.subscribe.Equals("subscribe"))
            {
                privacys = privacys.Where(p => p.Unsubscribe == false);
            }


            // 전체 갯수
            search.totalCount = privacys.Count();
            // DB에서 가져오기
            if (alldata)
                return privacys.ToList();
            else
                return privacys.OrderBy(p => p.status).OrderByDescending(pv => pv.PCMSID).Skip(search.startIndex * search.pageSize).Take(search.pageSize).ToList();
        }

        public ActionResult Index(PrivacySearch search)
        {
            PrivacyIndexDto dto = new PrivacyIndexDto();
            dto.Channels = db.Channels.Where(c => c.usage == true).ToList();
            dto.Privacies = PrivacySearchResult(search, false);
            PrivacySearch ps = new PrivacySearch(search);
            dto.Search = ps;
            dto.Search.totalCount = search.totalCount;

            ViewBag.CompaniesList = GetCompanyList.GetCompany();

            return View(dto);
        }


        // GET: Privacy/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Privacy privacy = db.Privacies.Find(id);
            if (privacy == null)
            {
                return HttpNotFound();
            }

            ViewBag.CompaniesList = GetCompanyList.GetCompany();

            return View(privacy);
        }

        // GET: Privacy/Create
        public ActionResult Create()
        {
            ViewBag.CompaniesList = GetCompanyList.GetCompany();
            return View();
        }

        private bool chkConsent(string consent)
        {
            if (string.IsNullOrEmpty(consent)) return false;
            if (consent == "false") return false;
            return true;
        }
        // POST: Privacy/Create
        // 초과 게시 공격으로부터 보호하려면 바인딩하려는 특정 속성을 사용하도록 설정하십시오. 
        // 자세한 내용은 http://go.microsoft.com/fwlink/?LinkId=317598을(를) 참조하십시오.
        //CONSENT_MARKETING_AGREEMENT Added by Nagaraju as part of APC30683265i
        [HttpPost]
        public ActionResult Create(string ddlCompanies, string WKP_NAME, string WKP_TEL, string ZIP, string PROVINCE, string CITY, string DONG, string STREET, string FULL_ADDR, string IND_SP
            , string TITLE, string IND_FULL_NAME, string EMAIL, string MOBILE, string CONSENT_SOURCE, string CONSENT_SUB_SOURCE, string LINK_RESERVATION, string LINK_PHONE
            , DateTime CONSENT_DATE, string CONSENTVERSION, string CONSENT_USE_YN, string CONSENT_TRUST_YN, string CONSENT_ABROAD_YN, string CONSENT_SIGN_YN, string CONSENT_MARKETING_AGREEMENT, string Unsubscribe)
        {
            DateTime convertedTime = DateTime.SpecifyKind(CONSENT_DATE, DateTimeKind.Utc);
            Privacy privacy = new Privacy(WKP_NAME, WKP_TEL, ZIP, PROVINCE, CITY, DONG, STREET, FULL_ADDR, IND_SP, TITLE, IND_FULL_NAME, EMAIL, MOBILE, CONSENT_SOURCE
                , CONSENT_SUB_SOURCE, LINK_RESERVATION, LINK_PHONE, convertedTime, CONSENTVERSION
                , chkConsent(CONSENT_USE_YN), chkConsent(CONSENT_TRUST_YN), chkConsent(CONSENT_ABROAD_YN), chkConsent(CONSENT_SIGN_YN),chkConsent(CONSENT_MARKETING_AGREEMENT), chkConsent(Unsubscribe));
            if (ModelState.IsValid)
            {

                // PFORCERX VALIDATION
                if (!CommonUtil.checkEmail(privacy.EMAIL) && !CommonUtil.checkMobile(privacy.MOBILE))
                {
                    privacy.SENDCHANEL |= SendChannel.PFORCERX_INVALID;
                }
                if (string.IsNullOrEmpty(CONSENT_SOURCE))
                {
                    privacy.SENDCHANEL |= SendChannel.PFORCERX_INVALID;
                }

                db.Privacies.Add(privacy);
                db.SaveChanges();

                if (!MyRoleManager.hasRole(MyRoleManager.RoleType.SYSTEMADMIN) && !MyRoleManager.hasRole(MyRoleManager.RoleType.DCEADMIN))
                {
                    Approval a = new Approval { status = ApprovalStatus.Request };
                    a.privacy = privacy;
                    privacy.approval = a;
                    db.Approvals.Add(a);

                    // Send Notification Mail
                    SMTPHelper.SendApprovalRequest();
                }
                db.SaveChanges();
            }
            return RedirectToAction("Index");
        }

        // GET: Privacy/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Privacy privacy = db.Privacies.Find(id);
            if (privacy == null)
            {
                return HttpNotFound();
            }
            if (string.IsNullOrEmpty(privacy.CONSENT_SOURCE))
            {
                privacy.CONSENT_SOURCE = privacy.CONSENT.CONSENT_SOURCE;
            }

            ViewBag.CompaniesList = GetCompanyList.GetCompany();
            return View(privacy);
        }
        public ActionResult EditWithReApprove(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Privacy privacy = db.Privacies.Find(id);
            if (privacy == null)
            {
                return HttpNotFound();
            }
            if (privacy.approval.status == ApprovalStatus.Rejected)
            {
                privacy.approval.status = ApprovalStatus.Request;
                privacy.approval.message = "재상신";
                db.Entry(privacy).State = EntityState.Modified;
                db.SaveChanges();
            }

            if (string.IsNullOrEmpty(privacy.CONSENT_SOURCE))
            {
                privacy.CONSENT_SOURCE = privacy.CONSENT.CONSENT_SOURCE;
            }
            return RedirectToAction("Edit", new { id = id });
        }


        private string fnDisplayname(string propertyname)
        {
            string displayname = string.Empty;
            MemberInfo property = typeof(Privacy).GetProperty(propertyname);
            var attribute = property.GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().Single();
            if (attribute != null) displayname = attribute.DisplayName;
            return displayname;
        }
        private string fnDisplaynameCS(string propertyname)
        {
            string displayname = string.Empty;
            MemberInfo property = typeof(Consent).GetProperty(propertyname);
            var attribute = property.GetCustomAttributes(typeof(DisplayNameAttribute), true).Cast<DisplayNameAttribute>().Single();
            if (attribute != null) displayname = attribute.DisplayName;
            return displayname;
        }
        private bool IsEqual(string a, string b)
        {
            if (string.IsNullOrEmpty(a) && string.IsNullOrEmpty(b)) return true;
            if (a == b) return true;
            return false;
        }

        // POST: Privacy/Edit/5
        //CONSENT_MARKETING_AGREEMENT Added by Nagaraju as part of APC30683265i
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(
            int ID, string WKP_NAME, string WKP_TEL, string ZIP, string FULL_ADDR
            , string IND_SP, string TITLE, string IND_FULL_NAME, string EMAIL, string MOBILE
            , string LINK_RESERVATION, string LINK_PHONE, string Unsubscribe
            , Consent CONSENT)
        {
            DateTime convertedTime = DateTime.SpecifyKind(CONSENT.CONSENT_DATE, DateTimeKind.Utc);

            Privacy privacy = db.Privacies.Find(ID);
            // 변경사항 추적
            List<string> tracelist = new List<string>();
            if (!IsEqual(privacy.WKP_NAME, WKP_NAME)) tracelist.Add(fnDisplayname("WKP_NAME") + " : " + privacy.WKP_NAME + " => " + WKP_NAME);
            if (!IsEqual(privacy.WKP_TEL, WKP_TEL)) tracelist.Add(fnDisplayname("WKP_TEL") + " : " + privacy.WKP_TEL + " => " + WKP_TEL);
            if (!IsEqual(privacy.ZIP, ZIP)) tracelist.Add(fnDisplayname("ZIP") + " : " + privacy.ZIP + " => " + ZIP);
            if (!IsEqual(privacy.FULL_ADDR, FULL_ADDR)) tracelist.Add(fnDisplayname("FULL_ADDR") + " : " + privacy.FULL_ADDR + " => " + FULL_ADDR);
            if (!IsEqual(privacy.IND_SP, IND_SP)) tracelist.Add(fnDisplayname("IND_SP") + " : " + privacy.IND_SP + " => " + IND_SP);
            if (!IsEqual(privacy.TITLE, TITLE)) tracelist.Add(fnDisplayname("TITLE") + " : " + privacy.TITLE + " => " + TITLE);
            if (!IsEqual(privacy.IND_FULL_NAME, IND_FULL_NAME)) tracelist.Add(fnDisplayname("IND_FULL_NAME") + " : " + privacy.IND_FULL_NAME + " => " + IND_FULL_NAME);
            if (!IsEqual(privacy.EMAIL, EMAIL)) tracelist.Add(fnDisplayname("EMAIL") + " : " + privacy.EMAIL + " => " + EMAIL);
            if (!IsEqual(privacy.MOBILE, MOBILE)) tracelist.Add(fnDisplayname("MOBILE") + " : " + privacy.MOBILE + " => " + MOBILE);
            if (!IsEqual(privacy.LINK_PHONE, LINK_PHONE)) tracelist.Add(fnDisplayname("LINK_PHONE") + " : " + privacy.LINK_PHONE + " => " + LINK_PHONE);
            if (!IsEqual(privacy.LINK_RESERVATION, LINK_RESERVATION)) tracelist.Add(fnDisplayname("LINK_RESERVATION") + " : " + privacy.LINK_RESERVATION + " => " + LINK_RESERVATION);

            if (privacy.Unsubscribe != chkConsent(Unsubscribe)) tracelist.Add(fnDisplayname("Unsubscribe") + " : " + privacy.Unsubscribe + " => " + Unsubscribe);

            if (privacy.CONSENT.CONSENT_DATE != convertedTime) tracelist.Add(fnDisplaynameCS("CONSENT_DATE") + " : " + privacy.CONSENT.CONSENT_DATE + " => " + convertedTime);
            if (!IsEqual(privacy.CONSENT.CONSENT_VERSION, CONSENT.CONSENT_VERSION)) tracelist.Add(fnDisplaynameCS("CONSENT_VERSION") + " : " + privacy.CONSENT.CONSENT_VERSION + " => " + CONSENT.CONSENT_VERSION);
            if (privacy.CONSENT.CONSENT_USE != CONSENT.CONSENT_USE) tracelist.Add(fnDisplaynameCS("CONSENT_USE") + " : " + privacy.CONSENT.CONSENT_USE + " => " + CONSENT.CONSENT_USE);
            if (privacy.CONSENT.CONSENT_MARKETING_AGREEMENT != CONSENT.CONSENT_MARKETING_AGREEMENT) tracelist.Add(fnDisplaynameCS("CONSENT_MARKETING_AGREEMENT") + " : " + privacy.CONSENT.CONSENT_MARKETING_AGREEMENT + " => " + CONSENT.CONSENT_MARKETING_AGREEMENT);
            if (privacy.CONSENT.CONSENT_TRUST != CONSENT.CONSENT_TRUST) tracelist.Add(fnDisplaynameCS("CONSENT_TRUST") + " : " + privacy.CONSENT.CONSENT_TRUST + " => " + CONSENT.CONSENT_TRUST);
            if (privacy.CONSENT.CONSENT_ABROAD != CONSENT.CONSENT_ABROAD) tracelist.Add(fnDisplaynameCS("CONSENT_ABROAD") + " : " + privacy.CONSENT.CONSENT_ABROAD + " => " + CONSENT.CONSENT_ABROAD);
            if (privacy.CONSENT.CONSENT_SIGN != CONSENT.CONSENT_SIGN) tracelist.Add(fnDisplaynameCS("CONSENT_SIGN") + " : " + privacy.CONSENT.CONSENT_SIGN + " => " + CONSENT.CONSENT_SIGN);
            if (!IsEqual(privacy.CONSENT.CONSENT_SOURCE, CONSENT.CONSENT_SOURCE))
            {
                tracelist.Add(fnDisplaynameCS("CONSENT_SOURCE") + " : " + privacy.CONSENT.CONSENT_SOURCE + " => " + CONSENT.CONSENT_SOURCE);
            }
            if (tracelist.Count() > 0)
            {
                string changes = tracelist.Aggregate((a, b) => a + ", " + b);
                db.Privacylogs.Add(new PrivacyLog { changes = changes, privacy = privacy });
            }

            // 변경사항 적용
            privacy.WKP_NAME = WKP_NAME;
            privacy.WKP_TEL = WKP_TEL;
            privacy.ZIP = ZIP;
            privacy.FULL_ADDR = FULL_ADDR;
            privacy.IND_SP = IND_SP;
            privacy.TITLE = TITLE;
            privacy.IND_FULL_NAME = IND_FULL_NAME;
            privacy.EMAIL = EMAIL;
            privacy.MOBILE = MOBILE;
            privacy.Unsubscribe = chkConsent(Unsubscribe);
            privacy.LINK_RESERVATION = LINK_RESERVATION;
            privacy.LINK_PHONE = LINK_PHONE;

            privacy.CONSENT.CONSENT_DATE = convertedTime;
            privacy.CONSENT.CONSENT_VERSION = CONSENT.CONSENT_VERSION;
            privacy.CONSENT.CONSENT_USE = CONSENT.CONSENT_USE;
            privacy.CONSENT.CONSENT_MARKETING_AGREEMENT = CONSENT.CONSENT_MARKETING_AGREEMENT;
            privacy.CONSENT.CONSENT_TRUST = CONSENT.CONSENT_TRUST;
            privacy.CONSENT.CONSENT_ABROAD = CONSENT.CONSENT_ABROAD;
            privacy.CONSENT.CONSENT_SIGN = CONSENT.CONSENT_SIGN;
            privacy.CONSENT.CONSENT_SOURCE = CONSENT.CONSENT_SOURCE;
            privacy.CONSENT_SOURCE = privacy.CONSENT.CONSENT_SOURCE;

            privacy.status = PrivacyStatus.REGISTED;

            // PFORCERX VALIDATION
            if (string.IsNullOrEmpty(CONSENT.CONSENT_SOURCE) || string.IsNullOrEmpty(CONSENT.CONSENT_VERSION))
            {
                privacy.SENDCHANEL |= SendChannel.PFORCERX_INVALID;
            }
            else
            {
                privacy.SENDCHANEL &= ~SendChannel.PFORCERX_INVALID;
            }
            if (CommonUtil.checkEmail(privacy.EMAIL) && CommonUtil.checkMobile(privacy.MOBILE))
            {
                privacy.SENDCHANEL &= ~SendChannel.PFORCERX_INVALID;
                // resend
                if (privacy.SENDCHANEL.HasFlag(SendChannel.PFORCERX))
                {
                    privacy.SENDCHANEL &= ~SendChannel.PFORCERX;
                }
            }
            else
            {
                privacy.SENDCHANEL |= SendChannel.PFORCERX_INVALID;
            }


            db.Entry(privacy).State = EntityState.Modified;
            db.SaveChanges();

            if (privacy.SENDCHANEL.HasFlag(SendChannel.PFORCERX_INVALID))
            {
                return RedirectToAction("Edit", new { ID = privacy.ID }); ;
            }
            return RedirectToAction("Details", new { ID = privacy.ID }); ;
        }

        // GET: Privacy/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Privacy privacy = db.Privacies.Find(id);
            if (privacy == null)
            {
                return HttpNotFound();
            }
            return View(privacy);
        }

        // POST: Privacy/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Privacy privacy = db.Privacies.Find(id);
            db.Privacies.Remove(privacy);
            db.SaveChanges();
            return RedirectToAction("Index");
        }

        public ActionResult TemplateDownload()
        {
            string filepath = Server.MapPath("~/Content/pcms_template.xlsx");
            byte[] filedata = System.IO.File.ReadAllBytes(filepath);
            string contentType = MimeMapping.GetMimeMapping(filepath);

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = "pcms_template.xlsx",
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());

            return File(filedata, contentType);
        }

        public ActionResult TestNAS()
        {
            try
            {
                SMTPHelper.SendApprovalRequest();
            }
            catch (Exception e)
            {
                log.Error(e);
                return RedirectToAction("Index");
            }
            return RedirectToAction("Create");
        }

        public ActionResult XlsDownload(PrivacySearch search)
        {
            var sheetname = DateTime.Now.ToString("yyyyMMddHHmmss");
            string currentuser = !string.IsNullOrEmpty(User?.Identity?.Name) ? User.Identity.Name.ToUpper() : "Anonymous";

            // Data
            var privacys = PrivacySearchResult(search, true);
            // Xls export
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("PCMS" + sheetname);

            ws.Cell("A1").Value = "PCMSID";
            ws.Cell("B1").Value = "NucleusKey";
            ws.Cell("C1").Value = "OneKey";
            ws.Cell("D1").Value = "고객명";
            ws.Cell("E1").Value = "진료과";
            ws.Cell("F1").Value = "직위";
            ws.Cell("G1").Value = "근무처(병원명)";
            ws.Cell("H1").Value = "우편번호";
            ws.Cell("I1").Value = "주소";
            ws.Cell("J1").Value = "근무처연락처";
            ws.Cell("K1").Value = "이메일";
            ws.Cell("L1").Value = "핸드폰";
            ws.Cell("M1").Value = "수신거부여부";
            ws.Cell("N1").Value = "수집/이용동의";

            ws.Cell("O1").Value = "마케팅 활용 동의";  //Added by Nagaraju as part of APC30683265i

            ws.Cell("P1").Value = "위탁동의";
            ws.Cell("Q1").Value = "국외이전동의";
            ws.Cell("R1").Value = "서명여부";
            ws.Cell("S1").Value = "동의서 버전";
            ws.Cell("T1").Value = "동의일자";
            ws.Cell("U1").Value = "동의채널";
            ws.Cell("V1").Value = "승인";
            ws.Cell("W1").Value = "승인메세지";
            ws.Cell("X1").Value = "승인요청일";
            ws.Cell("Y1").Value = "승인일";
           

            List<Privacy> list = privacys.ToList();
            int row = 2;
            foreach (Privacy p in list)
            {
                ws.Cell(row, 1).Value = p.PCMSID;
                ws.Cell(row, 2).Value = p.NucleusKey;
                ws.Cell(row, 3).Value = p.OneKey;
                ws.Cell(row, 4).Value = p.IND_FULL_NAME;
                ws.Cell(row, 5).Value = p.IND_SP;
                ws.Cell(row, 6).Value = p.TITLE;
                ws.Cell(row, 7).Value = p.WKP_NAME;
                ws.Cell(row, 8).Value = p.ZIP;
                ws.Cell(row, 9).Value = p.FULL_ADDR;
                ws.Cell(row, 10).Value = p.WKP_TEL;
                ws.Cell(row, 11).Value = p.EMAIL;
                ws.Cell(row, 12).Value = p.MOBILE;
                ws.Cell(row, 13).Value = p.Unsubscribe;
                ws.Cell(row, 14).Value = p.CONSENT_USE;
                ws.Cell(row, 15).Value = p.CONSENT.CONSENT_MARKETING_AGREEMENT;    //CONSENT_MARKETING_AGREEMENT Added by Nagaraju as part of APC30683265i
                ws.Cell(row, 16).Value = p.CONSENT_TRUST;
                ws.Cell(row, 17).Value = p.CONSENT_ABROAD;
                // MMS 는 항목이 없으므로 항상 true
                if (p.CONSENT.CONSENT_SOURCE == @"MMS")
                {
                    ws.Cell(row, 18).Value = @"TRUE";
                }
                else
                {
                    ws.Cell(row, 18).Value = p.CONSENT_SIGN;
                }
                ws.Cell(row, 19).Value = p.CONSENT.CONSENT_VERSION;
                ws.Cell(row, 20).Value = p.CONSENT.CONSENT_DATE_KOREA;

                //p.CONSENT.CONSENT_SOURCE != @"OneApp" added by Nagaraju as part of APC29593498i-23/Oct/18

                if (p.CONSENT.CONSENT_SOURCE != @"MMS" && p.CONSENT.CONSENT_SOURCE != @"GRV" && p.CONSENT.CONSENT_SOURCE != @"PforceRX" && p.CONSENT.CONSENT_SOURCE != @"PFORCERX"
                    && p.CONSENT.CONSENT_SOURCE != @"MyPfizer" && p.CONSENT.CONSENT_SOURCE != @"신약관(유한수집)" && p.CONSENT.CONSENT_SOURCE != @"OneApp")
                {
                    ws.Cell(row, 21).Value = @"서면동의서";
                }
                else if (p.CONSENT.CONSENT_SOURCE == @"PforceRX" || p.CONSENT.CONSENT_SOURCE == @"PFORCERX")
                {
                    ws.Cell(row, 21).Value = @"PforceRX";
                }
                else
                {
                    ws.Cell(row, 21).Value = p.CONSENT.CONSENT_SOURCE;
                }

                // 승인/반려 데이터가 있는 경우
                if (p.approval != null)
                {
                    // 승인반려 21
                    if (p.approval.status == ApprovalStatus.Approved)
                    {
                        ws.Cell(row, 22).Value = @"승인";
                    }
                    else if (p.approval.status == ApprovalStatus.Rejected)
                    {
                        ws.Cell(row, 22).Value = @"반려";
                    }
                    else
                    {
                        ws.Cell(row, 22).Value = @"요청";
                    }
                    // 메세지
                    ws.Cell(row, 23).Value = p.approval.message;
                    // 요청일자
                    if (p.approval.createdate != null) ws.Cell(row, 24).Value = p.approval.createdate.ToShortDateString();
                    // 수정일자
                    if (p.approval.modifieddate != null) ws.Cell(row, 25).Value = p.approval.modifieddate.ToShortDateString();
                   
                }

              

                row++;
            }
            Stream fs = new MemoryStream();
            wb.SaveAs(fs);
            fs.Position = 0;

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = "pcms_" + currentuser + "_" + sheetname + ".xlsx",
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(fs, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        [WebMethod]
        public JsonResult TemplateUpload()
        {
            // 네트워크드라이브 연결
            NetworkDriveHelper.Connect();

            string filename = "";
            string path = "";
            StringBuilder sb = new StringBuilder();
            if (Request.Files.Count > 0)
            {
                // file upload
                var file = Request.Files[0];
                var fileExt = Path.GetExtension(file.FileName);
                using (ImpersonateUser u = new ImpersonateUser())
                {
                    // directory check

                    bool exists = Directory.Exists(bulkpath);
                    if (!exists) Directory.CreateDirectory(bulkpath);
                    filename = Path.GetRandomFileName() + "." + fileExt;
                    path = Path.Combine(bulkpath, filename);

                    if (file != null && file.ContentLength > 0)
                    {
                        file.SaveAs(path);
                    }
                    else
                    {
                        return Json(new { result = false, msg = @"잘못된 파일입니다." });
                    }

                    // file parsing
                    var workbook = new XLWorkbook(path);
                    var worksheet = workbook.Worksheet(1);
                    int rowcnt = worksheet.RowCount() > 30000 ? 30000 : worksheet.RowCount();

                    // 데이타 검증
                    bool validcheck = true;
#pragma warning disable CS0219 // The variable 'validcheck2' is assigned but its value is never used
                    bool validcheck2 = true;
#pragma warning restore CS0219 // The variable 'validcheck2' is assigned but its value is never used
                    List<string> errlist = new List<string>();
                    for (int i = 2; i <= rowcnt; i++)
                    {
                        DateTime consentdate = DateTime.MinValue;
                        worksheet.Cell(i, 13).TryGetValue<DateTime>(out consentdate);

                        if (consentdate == null || consentdate == DateTime.MinValue)
                        {
                            continue;
                        }
                        if (consentdate != null && consentdate != DateTime.MinValue)
                        {
                            string temail = (string)worksheet.Cell(i, 7).Value.ToString();
                            string tmobile = (string)worksheet.Cell(i, 6).Value.ToString();
                            if (CommonUtil.checkEmail(temail) == false && CommonUtil.checkMobile(tmobile) == false)
                            {
                                errlist.Add(i.ToString() + @"행 ");
                                validcheck = false;
                            }
                        }
                        string consentsource = (string)worksheet.Cell(i, 11).Value;
                        if (string.IsNullOrEmpty(consentsource))
                        {
                            errlist.Add(i.ToString() + @"행 ");
                            validcheck = false;
                        }
                    }
                    if (validcheck == false)
                    {
                        string changes = errlist.Aggregate((a, b) => a + ", " + b) + @" 형식에 맞지 않는 필드가 있습니다.(이메일 또는 핸드폰 , 동의날짜는 필수 항목입니다.";
                        return Json(new { result = false, msg = changes });
                    }
                    // 데이타 입력
                    rowcnt = worksheet.RowCount() > 30000 ? 30000 : worksheet.RowCount();
                    for (int i = 2; i <= rowcnt; i++)
                    {
                        DateTime consentdate = DateTime.MinValue;
                        worksheet.Cell(i, 13).TryGetValue<DateTime>(out consentdate);

                        if (consentdate != null && consentdate != DateTime.MinValue)
                        {
                            Privacy p = null;
                            try
                            {
                                string fullname = (string)worksheet.Cell(i, 1).Value.ToString(); // 고객명
                                string wkp_name = (string)worksheet.Cell(i, 2).Value.ToString(); // 병원명
                                string indsp = (string)worksheet.Cell(i, 3).Value.ToString();  // 진료과
                                string fulladdr = (string)worksheet.Cell(i, 4).Value.ToString(); // 주소
                                object tmp = worksheet.Cell(i, 5).Value; // 우편번호
                                string zip = typeof(Double) == tmp.GetType() ? String.Format("{0:0.##}", tmp) : (string)worksheet.Cell(i, 5).Value;
                                string mobile = (string)worksheet.Cell(i, 6).Value.ToString(); // 핸드폰                                
                                string email = (string)worksheet.Cell(i, 7).Value.ToString(); // 이메일
                                string title = (string)worksheet.Cell(i, 8).Value.ToString(); // 직위
                                string tel = (string)worksheet.Cell(i, 9).Value.ToString(); // 근무처 연락처


                                string consentsource = (string)worksheet.Cell(i, 11).Value;
                                string consentvertion = (string)worksheet.Cell(i, 12).Value;
                                string COMP_CODE = (string)worksheet.Cell(i, 13).Value.ToString();
                                p = new Privacy(
                                 //COMP_CODE,
                                 wkp_name     // WKP_NAME
                                , tel
                                , zip
                                , string.Empty, string.Empty, string.Empty, string.Empty
                                , fulladdr
                                , indsp    // IND_SP
                                , title    // TITLE
                                , fullname   // FULL_NAME
                                , email    // EMAIL
                                , mobile    // MOBILE
                                , consentsource
                                , filename, string.Empty, string.Empty
                                , CommonUtil.toUtcDT(consentdate)
                                , consentvertion
                                , chkOpt((string)worksheet.Cell(i, 14).Value)
                                , chkOpt((string)worksheet.Cell(i, 16).Value)
                                , chkOpt((string)worksheet.Cell(i, 17).Value)
                                , chkOpt((string)worksheet.Cell(i, 18).Value)
                                , chkOpt((string)worksheet.Cell(i, 15).Value)
                                , chkOpt2((string)worksheet.Cell(i, 10).Value)
                                );
                            }
                            catch (Exception e)
                            {
                                sb.Append("Line " + i + " :" + e.Message);
                            }
                            if (p != null)
                            {
                                db.Privacies.Add(p);
                                db.SaveChanges();
                            }
                            if (!MyRoleManager.hasRole(MyRoleManager.RoleType.SYSTEMADMIN) && !MyRoleManager.hasRole(MyRoleManager.RoleType.DCEADMIN))
                            {
                                Approval a = new Approval { status = ApprovalStatus.Request };
                                a.privacy = p;
                                p.approval = a;
                                db.Approvals.Add(a);
                                db.SaveChanges();
                            }
                            else
                            {
                                Approval a = new Approval { status = ApprovalStatus.Approved };
                                a.privacy = p;
                                p.approval = a;
                                db.Approvals.Add(a);
                                db.SaveChanges();
                            }
                        }
                    }
                }
                db.SaveChanges();

                if (!MyRoleManager.hasRole(MyRoleManager.RoleType.SYSTEMADMIN) && !MyRoleManager.hasRole(MyRoleManager.RoleType.DCEADMIN))
                {
                    // Send Notification Mail
                    SMTPHelper.SendApprovalRequest();
                }
            }
            if (!string.IsNullOrEmpty(sb.ToString()))
            {
                return Json(new { result = false, message = sb.ToString() });
            }

            NetworkDriveHelper.Disconnect();
            return Json(new { result = true });
        }

        private bool chkOpt(string val)
        {
            if (string.IsNullOrEmpty(val)) return false;
            if (val == @"동의") return true;
            return false;
        }
        private bool chkOpt2(string val)
        {
            if (string.IsNullOrEmpty(val)) return false;
            if (val == @"수신거부") return true;
            return false;
        }
        private bool chkOptIn(string val)
        {
            if (string.IsNullOrEmpty(val)) return false;
            if (val == @"Opt-In") return true;
            return false;
        }
        private bool chkOptIn2(string val)
        {
            if (string.IsNullOrEmpty(val)) return false;
            if (val == @"Y") return true;
            return false;
        }
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                db.Dispose();
            }
            base.Dispose(disposing);
        }

        [WebMethod]
        public JsonResult ForceUpload()
        {
            string filename = "";
            if (Request.Files.Count > 0)
            {
                // file upload
                var file = Request.Files[0];

                // file parsing
                var workbook = new XLWorkbook(file.InputStream);
                var worksheet = workbook.Worksheet(1);
                int rowcnt = worksheet.RowCount() > 30000 ? 30000 : worksheet.RowCount();
                for (int i = 2; i <= rowcnt; i++)
                {
                    DateTime consentdate = DateTime.MinValue;
                    worksheet.Cell(i, 25).TryGetValue<DateTime>(out consentdate);

                    if (consentdate != null && consentdate != DateTime.MinValue && !string.IsNullOrEmpty((string)worksheet.Cell(i, 5).Value))
                    {
                        Privacy p = new Privacy(
                             //(string)worksheet.Cell(i, 26).Value //COMP_CODE
                            (string)worksheet.Cell(i, 7).Value      // WKP_NAME
                            , (string)worksheet.Cell(i, 8).Value
                            , (string)worksheet.Cell(i, 10).Value
                            , (string)worksheet.Cell(i, 11).Value
                            , (string)worksheet.Cell(i, 12).Value
                            , (string)worksheet.Cell(i, 13).Value
                            , (string)worksheet.Cell(i, 14).Value
                            , (string)worksheet.Cell(i, 15).Value

                            , (string)worksheet.Cell(i, 16).Value    // IND_SP
                            , (string)worksheet.Cell(i, 17).Value    // TITLE
                            , (string)worksheet.Cell(i, 20).Value    // FULL_NAME
                            , (string)worksheet.Cell(i, 22).Value    // EMAIL
                            , (string)worksheet.Cell(i, 23).Value    // MOBILE
                            , (string)worksheet.Cell(i, 1).Value    // CONSENT_SOURCE
                            , filename                              // CONSENT_SUB_SOURCE
                            , string.Empty, string.Empty            // LINK Re / LINK PHONE
                            , consentdate
                            , string.Empty                          // CONSENT_VERSION
                            , chkOptIn((string)worksheet.Cell(i, 24).Value)
                            , chkOptIn((string)worksheet.Cell(i, 24).Value)
                            , chkOptIn((string)worksheet.Cell(i, 24).Value)
                            , chkOptIn((string)worksheet.Cell(i, 24).Value)
                            , chkOptIn((string)worksheet.Cell(i, 24).Value)
                            , false
                        );
                        p.SOURCE = (string)worksheet.Cell(i, 1).Value;

                        p.WKP_ID = (string)worksheet.Cell(i, 3).Value;
                        p.WKP_EXT_ID = (string)worksheet.Cell(i, 4).Value;
                        p.IND_ID = (string)worksheet.Cell(i, 5).Value;
                        p.PCMSID = (string)worksheet.Cell(i, 5).Value;
                        p.ACT_STATUS = (string)worksheet.Cell(i, 6).Value;                       

                        db.Privacies.Add(p);
                        db.SaveChanges();
                    }
                }
            }
            db.SaveChanges();


            return Json(new { result = true });
        }

        [WebMethod]
        public JsonResult MMSUpload()
        {
            if (Request.Files.Count > 0)
            {
                // file upload
                var file = Request.Files[0];
                int cc = 0;
                // file parsing
                var workbook = new XLWorkbook(file.InputStream);
                var worksheet = workbook.Worksheet(1);
                int rowcnt = worksheet.RowCount() > 30000 ? 30000 : worksheet.RowCount();
                for (int i = 2; i <= rowcnt; i++)
                {
                    DateTime consentdate = DateTime.MinValue;
                    worksheet.Cell(i, 12).TryGetValue<DateTime>(out consentdate);

                    if (consentdate != null && consentdate != DateTime.MinValue && !string.IsNullOrEmpty((string)worksheet.Cell(i, 8).Value))
                    {
                        string str1 = (string)worksheet.Cell(i, 1).Value.ToString();
                        string str2 = (string)worksheet.Cell(i, 2).Value.ToString();
                        string str3 = (string)worksheet.Cell(i, 3).Value.ToString();
                        string str4 = (string)worksheet.Cell(i, 4).Value.ToString();
                        string str5 = (string)worksheet.Cell(i, 5).Value.ToString();
                        string str6 = (string)worksheet.Cell(i, 6).Value.ToString();
                        string str7 = (string)worksheet.Cell(i, 7).Value.ToString();
                        string str8 = (string)worksheet.Cell(i, 8).Value.ToString();
                        string str9 = (string)worksheet.Cell(i, 9).Value.ToString();
                        string str11 = (string)worksheet.Cell(i, 11).Value.ToString();
                        string str13 = (string)worksheet.Cell(i, 13).Value.ToString();
                        string str14 = (string)worksheet.Cell(i, 14).Value.ToString();

                        Privacy p = new Privacy(
                            //str14 //COMP_CODE
                            str1     // WKP_NAME
                            , str2    // WKP_TEL
                            , str3   // ZIP
                            , string.Empty   // PROVINCE
                            , string.Empty   // CITY
                            , str4   // DONG
                            , str5   // STREET
                            , (string)worksheet.Cell(i, 4).Value + " " + (string)worksheet.Cell(i, 5).Value   // FUKKADDR
                            , str6   // IND_SP
                            , string.Empty    // TITLE
                            , str7   // FULL_NAME
                            , str8    // EMAIL
                            , str9   // MOBILE
                            , "MMS"    // CONSENT_SOURCE
                            , str13 // CONSENT_SUB_SOURCE
                            , string.Empty, string.Empty            // LINK Re / LINK PHONE
                            , CommonUtil.toUtcDT(consentdate)
                            , str11                     // CONSENT_VERSION
                            , chkOptIn2((string)worksheet.Cell(i, 14).Value)
                            , chkOptIn2((string)worksheet.Cell(i, 15).Value)
                            , chkOptIn2((string)worksheet.Cell(i, 16).Value)
                            , false
                            , !chkOptIn2((string)worksheet.Cell(i, 10).Value)
                            , true
                        );
                        p.creater = " ";
                        db.Privacies.Add(p);
                        cc++;
                        if (cc > 1000)
                        {
                            db.SaveChanges();
                            cc = 0;
                        }
                    }
                }
            }
            db.SaveChanges();


            return Json(new { result = true });
        }

        [WebMethod]
        public JsonResult MMSUploadCSV()
        {
            PCMSDBContext upsertContext = new PCMSDBContext();
            if (Request.Files.Count > 0)
            {
                // file upload
                var file = Request.Files[0];
                int cc = 0;
                // file parsing
                string line;
                StreamReader filetarget = new StreamReader(file.InputStream);
                line = filetarget.ReadLine();
                try
                {
                    List<MMSFile> mms = new List<MMSFile>();

                    while ((line = filetarget.ReadLine()) != null)
                    {
                        string[] arr = line.Split('|');
                        mms.Add(new MMSFile(arr));
                    }
                    filetarget.Close();

                    // privacy 추가
                    foreach (MMSFile m in mms)
                    {
                        Privacy p = new Privacy(
                            //m.COMP_CODE
                            m.COMPANY     // WKP_NAME
                            , m.TEL    // WKP_TEL
                            , m.ZIP  // ZIP
                            , string.Empty   // PROVINCE
                            , string.Empty   // CITY
                            , m.ADDRESS1   // DONG
                            , m.ADDRESS2   // STREET
                            , m.ADDRESS1 + " " + m.ADDRESS2   // FUKKADDR
                            , m.CATEGORY // IND_SP
                            , string.Empty    // TITLE
                            , m.NAME   // FULL_NAME
                            , m.EMAIL    // EMAIL
                            , m.HP   // MOBILE
                            , "MMS"    // CONSENT_SOURCE
                            , m.SUB_CHANNEL // CONSENT_SUB_SOURCE
                            , string.Empty, string.Empty            // LINK Re / LINK PHONE
                            , CommonUtil.toUtcDT(m.AGR_DATE)
                            , m.AGR_VER                     // CONSENT_VERSION
                            , chkOptIn2(m.AGREE1)
                            , chkOptIn2(m.AGREE2)
                            , chkOptIn2(m.AGREE3)
                            , true
                            , !chkOptIn2(m.RCV_MAIL)
                            , true
                        );
                        // db.Consents.Where(cst => cst.CONSENT_DATE < expired).Select(cst => cst.privacy).Distinct().Count();
                        IEnumerable<Privacy> tmp = db.Privacies.Where(pp => pp.EMAIL == m.EMAIL);
                        if (tmp.Count() > 0)
                        {
                            foreach (Privacy pitem in tmp.ToList())
                            {
                                if (pitem.CONSENT_SOURCE == "MMS")
                                {
                                    Privacy priv = pitem;
                                    List<string> tracelist = new List<string>();

                                    if (!IsEqual(m.COMPANY, priv.WKP_NAME))
                                    {
                                        tracelist.Add("근무처(병원명) : " + priv.WKP_NAME + " => " + m.COMPANY);
                                        priv.WKP_NAME = m.COMPANY;
                                    }
                                    if (!IsEqual(m.TEL, priv.WKP_TEL))
                                    {
                                        tracelist.Add("근무지연락처 : " + priv.WKP_TEL + " => " + m.TEL);
                                        priv.WKP_TEL = m.TEL;
                                    }
                                    if (!IsEqual(m.ZIP, priv.ZIP))
                                    {
                                        tracelist.Add("우편번호 : " + priv.ZIP + " => " + m.ZIP);
                                        priv.ZIP = m.ZIP;
                                    }
                                    if (!IsEqual(m.ADDRESS1, priv.DONG))
                                    {
                                        tracelist.Add("주소(군/구/동) : " + priv.DONG + " => " + m.ADDRESS1);
                                        priv.DONG = m.ADDRESS1;
                                    }
                                    if (!IsEqual(m.ADDRESS2, priv.STREET))
                                    {
                                        tracelist.Add("상세주소 : " + priv.STREET + " => " + m.ADDRESS2);
                                        priv.STREET = m.ADDRESS2;
                                    }
                                    string tmpaddr = m.ADDRESS1 + " " + m.ADDRESS2;
                                    if (!IsEqual(tmpaddr, priv.FULL_ADDR))
                                    {
                                        tracelist.Add("주소 : " + priv.FULL_ADDR + " => " + tmpaddr);
                                        priv.FULL_ADDR = tmpaddr;
                                    }
                                    if (!IsEqual(m.CATEGORY, priv.IND_SP))
                                    {
                                        tracelist.Add("진료과 : " + priv.IND_SP + " => " + m.CATEGORY);
                                        priv.IND_SP = m.CATEGORY;
                                    }
                                    if (!IsEqual(m.NAME, priv.IND_FULL_NAME))
                                    {
                                        tracelist.Add("고객명 : " + priv.IND_FULL_NAME + " => " + m.NAME);
                                        priv.IND_FULL_NAME = m.NAME;
                                    }
                                    if (!IsEqual(m.HP, priv.MOBILE))
                                    {
                                        tracelist.Add("핸드폰 : " + priv.MOBILE + " => " + m.HP);
                                        priv.MOBILE = m.HP;
                                    }
                                    if (!IsEqual(m.SUB_CHANNEL, priv.CONSENT_SUB_SOURCE))
                                    {
                                        tracelist.Add("채널(부) : " + priv.CONSENT_SUB_SOURCE + " => " + m.SUB_CHANNEL);
                                        priv.CONSENT_SUB_SOURCE = m.SUB_CHANNEL;
                                    }
                                    if (!chkOptIn2(m.RCV_MAIL) != priv.Unsubscribe)
                                    {
                                        tracelist.Add("수신거부 : " + priv.Unsubscribe + " => " + !chkOptIn2(m.RCV_MAIL));
                                        priv.Unsubscribe = !chkOptIn2(m.RCV_MAIL);
                                    }
                                    // 동의서 변경시
                                    if (CommonUtil.toUtcDT(m.AGR_DATE) != priv.CONSENTDATE)
                                    {
                                        priv.Consents.Add(new Consent
                                        {
                                            CONSENT_DATE = CommonUtil.toUtcDT(m.AGR_DATE),
                                            CONSENT_VERSION = m.AGR_VER,
                                            CONSENT_USE = chkOptIn2(m.AGREE1),
                                            CONSENT_TRUST = chkOptIn2(m.AGREE2),
                                            CONSENT_ABROAD = chkOptIn2(m.AGREE3),
                                            CONSENT_SIGN = true,
                                            CONSENT_SOURCE = "MMS"
                                        });
                                    }
                                    if (tracelist.Count() > 0)
                                    {
                                        string changes = tracelist.Aggregate((a, b) => a + ", " + b);
                                        db.Privacylogs.Add(new PrivacyLog { changes = changes, privacy = priv });
                                    }

                                    db.Entry(priv).State = System.Data.Entity.EntityState.Modified;
                                    db.SaveChanges();
                                }
                            }
                        }
                        else
                        {
                            upsertContext.Privacies.Add(p);
                        }
                        cc++;
                        if (cc > 1000)
                        {
                            upsertContext.SaveChanges();
                            cc = 0;
                        }
                    }
                    upsertContext.SaveChanges();

                }
#pragma warning disable CS0168 // The variable 'e' is declared but never used
                catch (Exception e)
#pragma warning restore CS0168 // The variable 'e' is declared but never used
                {
                }
                finally
                {
                    if (filetarget != null) filetarget.Dispose();
                }
            }
            return Json(new { result = true });
        }


        [WebMethod]
        public JsonResult UnMaskData()
        {
            if (Request.Files.Count > 0)
            {
                // file upload
                var file = Request.Files[0];
                // file parsing
                var workbook = new XLWorkbook(file.InputStream);
                var worksheet = workbook.Worksheet(1);
                int rowcnt = worksheet.RowCount();
                for (int i = 2; i <= rowcnt; i++)
                {
                    DateTime consentdate = DateTime.MinValue;
                    worksheet.Cell(i, 19).TryGetValue<DateTime>(out consentdate);
                    string pcmsid = worksheet.Cell(i, 3).Value.ToString();
                    string name = worksheet.Cell(i, 10).Value.ToString();
                    string email = worksheet.Cell(i, 11).Value.ToString();
                    string phone = worksheet.Cell(i, 12).Value.ToString();

                    if (string.IsNullOrEmpty(pcmsid)) continue;
                    IQueryable<Privacy> iqr = db.Privacies.Where(p => p.PCMSID == pcmsid);
                    if (iqr.Count() < 1) continue;

                    Privacy prv = iqr.First();
                    if (prv != null && prv.IND_FULL_NAME.Equals("*****"))
                    {
                        prv.IND_FULL_NAME = name;
                        prv.EMAIL = email;
                        prv.MOBILE = phone;
                        prv.IND_FIRSTNAME = "";
                        prv.IND_LASTNAME = "";

                        if (string.IsNullOrEmpty(prv.NucleusKey))
                        {
                            prv.status = PrivacyStatus.IMPORTED;
                        }
                        else
                        {
                            prv.status = PrivacyStatus.GRANTED;
                        }


                        db.Entry(prv).State = EntityState.Modified;
                        db.SaveChanges();
                    }
                }
            }
            return Json(new { result = true });
        }
        [WebMethod]
        public JsonResult ChangeUnsubscribe(UnsubscribeDto dto)
        {
            try
            {
                foreach (var str in dto.contacts)
                {
                    var targets = db.Privacies.Where(p => p.EMAIL == str || p.MOBILE == str || p.LINK_PHONE == str).ToList();
                    foreach (Privacy t in targets)
                    {
                        // 변경사항 추적
                        db.Privacylogs.Add(new PrivacyLog { changes = @"수신거부:" + t.Unsubscribe + " => " + dto.unsubscribe, privacy = t });
                        t.Unsubscribe = dto.unsubscribe;
                        db.Entry(t).State = EntityState.Modified;
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                log.Error(e);
                return Json(new { result = false });
            }
            return Json(new { result = true });
        }

        [WebMethod]
        public JsonResult SearchConsent(SearchDto dto)
        {
#pragma warning disable CS0168 // The variable 'jr' is declared but never used
            JsonResult jr;
#pragma warning restore CS0168 // The variable 'jr' is declared but never used
            if (string.IsNullOrEmpty(dto.searchkey))
            {
                return Json(new { result = false });
            }
            try
            {
                var r = db.Privacies.Where(p => p.status != PrivacyStatus.DELETED && p.status != PrivacyStatus.ERASED).Where(p => p.EMAIL.Contains(dto.searchkey) || p.MOBILE.Contains(dto.searchkey)
                || p.IND_FULL_NAME.Contains(dto.searchkey) || p.NucleusKey == dto.searchkey).ToList();

                dto.lists = new List<PrivacyDto>();
                foreach (var p in r)
                {
                    dto.lists.Add(new PrivacyDto { ID = p.ID, PCMSID = p.PCMSID, NucleusKey = p.NucleusKey, EMAIL = p.EMAIL, MOBILE = p.MOBILE, IND_FULL_NAME = p.IND_FULL_NAME, status = p.status });
                }
                dto.count = dto.lists.Count();
                dto.result = true;
            }
            catch (Exception e)
            {
                log.Error(e);
                return Json(new { result = false });
            }
            return Json(dto, JsonRequestBehavior.AllowGet);
        }

        [WebMethod]
        public JsonResult OptoutConsent(SearchDto dto)
        {
            try
            {
                if (dto.id < 1) return Json(new { result = false });
                var target = db.Privacies.Find(dto.id);
                if (target == null) return Json(new { result = false });

                string nkey = target.NucleusKey;
                if (string.IsNullOrEmpty(nkey))
                {
                    target.IND_FIRSTNAME = "*****";
                    target.IND_LASTNAME = "*****";
                    target.IND_FULL_NAME = "*****";
                    target.MOBILE = "*****";
                    target.EMAIL = "*****";
                    target.CONSENT.CONSENT_ABROAD = false;
                    target.CONSENT.CONSENT_SIGN = false;
                    target.CONSENT.CONSENT_TRUST = false;
                    target.CONSENT.CONSENT_USE = false;
                    target.CONSENT.CONSENT_MARKETING_AGREEMENT = false;
                    target.Unsubscribe = true;
                    target.status = PrivacyStatus.ERASED;
                    target.SENDCHANEL &= ~SendChannel.N360;
                    target.SENDCHANEL &= ~SendChannel.PFORCERX;

                    db.Entry(target).State = System.Data.Entity.EntityState.Modified;
                    db.Privacylogs.Add(new PrivacyLog { privacy = target, changes = @"OptOut NucleusKey :  => " + target.NucleusKey });
                    db.SaveChanges();
                }
                else
                {
                    var targets = db.Privacies.Where(p => p.NucleusKey == nkey).ToList();
                    foreach (Privacy p in targets)
                    {
                        p.IND_FIRSTNAME = "*****";
                        p.IND_LASTNAME = "*****";
                        p.IND_FULL_NAME = "*****";
                        p.MOBILE = "*****";
                        p.EMAIL = "*****";
                        p.CONSENT.CONSENT_ABROAD = false;
                        p.CONSENT.CONSENT_SIGN = false;
                        p.CONSENT.CONSENT_TRUST = false;
                        p.CONSENT.CONSENT_USE = false;
                        p.CONSENT.CONSENT_MARKETING_AGREEMENT = false;
                        p.Unsubscribe = true;
                        p.status = PrivacyStatus.ERASED;
                        p.SENDCHANEL &= ~SendChannel.N360;
                        p.SENDCHANEL &= ~SendChannel.PFORCERX;

                        db.Entry(p).State = System.Data.Entity.EntityState.Modified;
                        db.Privacylogs.Add(new PrivacyLog { privacy = p, changes = @"OptOut NucleusKey :  => " + p.NucleusKey });
                    }
                    db.SaveChanges();
                }
            }
            catch (Exception e)
            {
                log.Error(e);
                return Json(new { result = false });
            }
            return Json(new { result = true });
        }

        [WebMethod]
        public JsonResult SetActive(int id, string status)
        {
            try
            {
                if (id < 1) return Json(new { result = false });
                var target = db.Privacies.Find(id);
                if (target == null) return Json(new { result = false });

                if (status != null && status.Equals("inactive"))
                    target.status = PrivacyStatus.INACTIVED;
                else
                {
                    if (target.IND_FIRSTNAME.Equals("*****"))
                        target.status = PrivacyStatus.ERASED;
                    else if (target.NucleusKey != null)
                        target.status = PrivacyStatus.GRANTED;
                    else
                        target.status = PrivacyStatus.REGISTED;
                }
                db.Entry(target).State = System.Data.Entity.EntityState.Modified;
                db.Privacylogs.Add(new PrivacyLog { privacy = target, changes = @"Inactive PCMSID:  => " + target.PCMSID });
                db.SaveChanges();
            }
            catch (Exception e)
            {
                log.Error(e);
                return Json(new { result = false });
            }
            return Json(new { result = true });
        }
    }
}
