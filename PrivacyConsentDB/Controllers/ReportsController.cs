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
    [Role]
    [Auth]
    public class ReportsController : Controller
    {
        private PCMSDBContext db = new PCMSDBContext();
        SelectList IND_SP_LIST = new SelectList(
            new List<SelectListItem>
            {
            new SelectListItem { Text ="가정의학과", Value = "가정의학과"},
            new SelectListItem { Text ="감염내과", Value = "감염내과"},
            new SelectListItem { Text ="건강관리과", Value = "건강관리과"},
            new SelectListItem { Text ="검사의학과", Value = "검사의학과"},
            new SelectListItem { Text ="결핵과", Value = "결핵과"},
            new SelectListItem { Text ="기타", Value = "기타"},
            new SelectListItem { Text ="내과", Value = "내과"},
            new SelectListItem { Text ="내분비내과", Value = "내분비내과"},
            new SelectListItem { Text ="내분비외과", Value = "내분비외과"},
            new SelectListItem { Text ="노년내과", Value = "노년내과"},
            new SelectListItem { Text ="레지던트", Value = "레지던트"},
            new SelectListItem { Text ="류마티스내과", Value = "류마티스내과"},
            new SelectListItem { Text ="마취과", Value = "마취과"},
            new SelectListItem { Text ="마취통증의학과", Value = "마취통증의학과"},
            new SelectListItem { Text ="면역학", Value = "면역학"},
            new SelectListItem { Text ="방사선과", Value = "방사선과"},
            new SelectListItem { Text ="방사선종양학과", Value = "방사선종양학과"},
            new SelectListItem { Text ="병리과", Value = "병리과"},
            new SelectListItem { Text ="보건소", Value = "보건소"},
            new SelectListItem { Text ="부인종양과", Value = "부인종양과"},
            new SelectListItem { Text ="비뇨기과", Value = "비뇨기과"},
            new SelectListItem { Text ="비뇨종양과", Value = "비뇨종양과"},
            new SelectListItem { Text ="산부인과", Value = "산부인과"},
            new SelectListItem { Text ="산업의학과", Value = "산업의학과"},
            new SelectListItem { Text ="성형외과", Value = "성형외과"},
            new SelectListItem { Text ="소아감염내과", Value = "소아감염내과"},
            new SelectListItem { Text ="소아과", Value = "소아과"},
            new SelectListItem { Text ="소아내분비", Value = "소아내분비"},
            new SelectListItem { Text ="소아소화기", Value = "소아소화기"},
            new SelectListItem { Text ="소아신경", Value = "소아신경"},
            new SelectListItem { Text ="소아심장", Value = "소아심장"},
            new SelectListItem { Text ="소아알레르기", Value = "소아알레르기"},
            new SelectListItem { Text ="소아외과", Value = "소아외과"},
            new SelectListItem { Text ="소아정신과", Value = "소아정신과"},
            new SelectListItem { Text ="소아정형외과", Value = "소아정형외과"},
            new SelectListItem { Text ="소아청소년과", Value = "소아청소년과"},
            new SelectListItem { Text ="소아치과", Value = "소아치과"},
            new SelectListItem { Text ="소아혈액종양클리닉", Value = "소아혈액종양클리닉"},
            new SelectListItem { Text ="소아호흡기", Value = "소아호흡기"},
            new SelectListItem { Text ="소화기내과", Value = "소화기내과"},
            new SelectListItem { Text ="순환기내과", Value = "순환기내과"},
            new SelectListItem { Text ="신경과", Value = "신경과"},
            new SelectListItem { Text ="신경내과", Value = "신경내과"},
            new SelectListItem { Text ="신경방사선과", Value = "신경방사선과"},
            new SelectListItem { Text ="신경외과", Value = "신경외과"},
            new SelectListItem { Text ="신경정신과", Value = "신경정신과"},
            new SelectListItem { Text ="신장내과", Value = "신장내과"},
            new SelectListItem { Text ="심장내과", Value = "심장내과"},
            new SelectListItem { Text ="안과", Value = "안과"},
            new SelectListItem { Text ="알레르기내과", Value = "알레르기내과"},
            new SelectListItem { Text ="암센타", Value = "암센타"},
            new SelectListItem { Text ="약사", Value = "약사"},
            new SelectListItem { Text ="약제과", Value = "약제과"},
            new SelectListItem { Text ="영상의학과", Value = "영상의학과"},
            new SelectListItem { Text ="예방의학과", Value = "예방의학과"},
            new SelectListItem { Text ="완화의료과", Value = "완화의료과"},
            new SelectListItem { Text ="유전학", Value = "유전학"},
            new SelectListItem { Text ="응급의학과", Value = "응급의학과"},
            new SelectListItem { Text ="의공학과", Value = "의공학과"},
            new SelectListItem { Text ="이비인후과", Value = "이비인후과"},
            new SelectListItem { Text ="이식외과", Value = "이식외과"},
            new SelectListItem { Text ="인턴", Value = "인턴"},
            new SelectListItem { Text ="일반외과", Value = "일반외과"},
            new SelectListItem { Text ="일반의", Value = "일반의"},
            new SelectListItem { Text ="임상병리과", Value = "임상병리과"},
            new SelectListItem { Text ="재활의학과", Value = "재활의학과"},
            new SelectListItem { Text ="정신과", Value = "정신과"},
            new SelectListItem { Text ="정형외과", Value = "정형외과"},
            new SelectListItem { Text ="종양내과", Value = "종양내과"},
            new SelectListItem { Text ="진단검사과", Value = "진단검사과"},
            new SelectListItem { Text ="진단방사선과", Value = "진단방사선과"},
            new SelectListItem { Text ="진단병리과", Value = "진단병리과"},
            new SelectListItem { Text ="척추외과", Value = "척추외과"},
            new SelectListItem { Text ="통증의학과", Value = "통증의학과"},
            new SelectListItem { Text ="피부과", Value = "피부과"},
            new SelectListItem { Text ="피부비뇨기과", Value = "피부비뇨기과"},
            new SelectListItem { Text ="한방과", Value = "한방과"},
            new SelectListItem { Text ="해부병리과", Value = "해부병리과"},
            new SelectListItem { Text ="핵의학과", Value = "핵의학과"},
            new SelectListItem { Text ="혈관외과", Value = "혈관외과"},
            new SelectListItem { Text ="혈액종양내과", Value = "혈액종양내과"},
            new SelectListItem { Text ="호흡기내과", Value = "호흡기내과"},
            new SelectListItem { Text ="흉부외과", Value = "흉부외과"}
            }, "Value", "Text");
        private ReportDto ReportSearch(ReportDto search, bool alldata = false)
        {
            var query = from i in (search.distinct ? (from x in db.Privacies group x by x.NucleusKey into o select o.FirstOrDefault()) : db.Privacies) select i;
            if (search.collectbegindt != null)
            {
                search.collectbegindt = (DateTime)search.collectbegindt;
                query = query.Where(i => i.createdate >= search.collectbegindt);
            }
            if (search.collectenddt != null)
            {
                search.collectenddt = (DateTime)search.collectenddt;
                query = query.Where(i => i.createdate <= search.collectenddt);
            }

            if (search.consentbegindt != null)
            {
                search.consentbegindt = (DateTime)search.consentbegindt;
                query = query.Where(i => i.Consents.FirstOrDefault().CONSENT_DATE >= search.consentbegindt);
            }
            if (search.consentenddt != null)
            {
                search.consentenddt = (DateTime)search.consentenddt;
                query = query.Where(i => i.Consents.FirstOrDefault().CONSENT_DATE <= search.consentenddt);
            }

            var queryTotal = query.Where(p => p.status != PrivacyStatus.INACTIVED);
            search.resultTotal = queryTotal.GroupBy(n => n.IND_SP).Select(grp => new ReportSP { SP = grp.Key, count = grp.Count() }).OrderBy(g => g.SP).ToList();

            var queryEmail = query.Where(p => p.status != PrivacyStatus.INACTIVED && p.EMAIL != null);
            search.resultEmail = queryEmail.GroupBy(n => n.IND_SP).Select(grp => new ReportSP { SP = grp.Key, count = grp.Count() }).OrderBy(g => g.SP).ToList();

            var queryMobile = query.Where(p => p.status != PrivacyStatus.INACTIVED && p.EMAIL != null);
            search.resultMobile = queryMobile.GroupBy(n => n.IND_SP).Select(grp => new ReportSP { SP = grp.Key, count = grp.Count() }).OrderBy(g => g.SP).ToList();

            var qTotal = query.Where(p => p.status != PrivacyStatus.INACTIVED);
            search.rsTotal = qTotal.GroupBy(n => new { n.createdate.Month, n.createdate.Year }).Select(grp => new ReportSP { SP = grp.Key.Year.ToString() + "년 " + grp.Key.Month.ToString() + "월", count = grp.Count() }).OrderBy(g => g.SP).ToList();

            var qGRV = query.Where(p => p.status != PrivacyStatus.INACTIVED && p.CONSENT_SOURCE.Equals("GRV"));
            search.rsGRV = qGRV.GroupBy(n => new { n.createdate.Month, n.createdate.Year }).Select(grp => new ReportSP { SP = grp.Key.Year.ToString() + "년 " + grp.Key.Month.ToString() + "월", count = grp.Count() }).OrderBy(g => g.SP).ToList();

            var qMMS= query.Where(p => p.status != PrivacyStatus.INACTIVED && p.CONSENT_SOURCE.Equals("MMS"));
            search.rsMMS = qMMS.GroupBy(n => new { n.createdate.Month, n.createdate.Year }).Select(grp => new ReportSP { SP = grp.Key.Year.ToString() + "년 " + grp.Key.Month.ToString() + "월", count = grp.Count() }).OrderBy(g => g.SP).ToList();


            var qDOC = query.Where(p => p.status != PrivacyStatus.INACTIVED && !p.CONSENT_SOURCE.Equals("MMS") && !p.CONSENT_SOURCE.Equals("GRV") && !p.CONSENT_SOURCE.Equals("PFORCERX"));
            search.rsDOC = qDOC.GroupBy(n => new { n.createdate.Month, n.createdate.Year }).Select(grp => new ReportSP { SP = grp.Key.Year.ToString() + "년 " + grp.Key.Month.ToString() + "월", count = grp.Count() }).OrderBy(g => g.SP).ToList();


            var qPFRX = query.Where(p => p.status != PrivacyStatus.INACTIVED && p.CONSENT_SOURCE.Equals("PFORCERX"));
            search.rsPFRX = qPFRX.GroupBy(n => new { n.createdate.Month, n.createdate.Year }).Select(grp => new ReportSP { SP = grp.Key.Year.ToString() + "년 " + grp.Key.Month.ToString() + "월", count = grp.Count() }).OrderBy(g => g.SP).ToList();

            return search;
        }

        private ReportDto AdminSearch(ReportDto search, bool allData = false)
        {

            /*
             * 1. 항상 활성화된 데이터만 나와야 함. - clear
             * 2. 등록일 기준 검색. from to - clear
             * 3. 승인일 기준 검색. from to - clear
             * 4. 중복여부 체크 - clear
             */
            var query = from i in (search.distinct ? (from x in db.Privacies group x by x.NucleusKey into o select o.FirstOrDefault()) : db.Privacies) select i;
            if (search.collectbegindt != null)
            {
                search.collectbegindt = ((DateTime)search.collectbegindt);
                query = query.Where(i => i.createdate >= search.collectbegindt);
            }
            if (search.collectenddt != null)
            {
                search.collectenddt = ((DateTime)search.collectenddt);
                query = query.Where(i => i.createdate <= search.collectenddt);
            }

            if (search.consentbegindt != null)
            {
                search.consentbegindt = ((DateTime)search.consentbegindt);
                query = query.Where(i => i.Consents.FirstOrDefault().CONSENT_DATE >= search.consentbegindt);
            }
            if (search.consentenddt != null)
            {
                search.consentenddt = ((DateTime)search.consentenddt);
                query = query.Where(i => i.Consents.FirstOrDefault().CONSENT_DATE <= search.consentenddt);
            }

            var queryRejected = query.Where(i => i.approval.status == Status.ApprovalStatus.Rejected && i.NucleusKey != null);
            search.rsRejected = queryRejected.GroupBy(n => new { n.createdate.Month, n.createdate.Year }).Select(grp => new ReportSP { SP = grp.Key.Year.ToString() + "년 " + grp.Key.Month.ToString() + "월", count = grp.Count() }).OrderBy(g => g.SP).ToList();

            // 승인
            var queryApproved = query.Where(i => i.approval.status == Status.ApprovalStatus.Approved && i.NucleusKey != null);
             search.rsApproved = queryApproved.GroupBy(n => new { n.createdate.Month, n.createdate.Year }).Select(grp => new ReportSP { SP = grp.Key.Year.ToString() + "년 " + grp.Key.Month.ToString() + "월", count = grp.Count() }).OrderBy(g => g.SP).ToList();

            // Nuc 코드 매핑 카운트,
            var queryN360 = query.Where(i => i.status != PrivacyStatus.INACTIVED && i.NucleusKey != null);
            search.rsN360 = queryN360.GroupBy(n => new { n.createdate.Month, n.createdate.Year }).Select(grp => new ReportSP { SP = grp.Key.Year.ToString() + "년 " + grp.Key.Month.ToString() + "월", count = grp.Count() }).OrderBy(g => g.SP).ToList();

            // Onekey Code Mapping 카운트
            var queryOneKey = query.Where(i => i.status != PrivacyStatus.INACTIVED && i.OneKey != null);
            search.rsOnekey = queryOneKey.GroupBy(n => new { n.createdate.Month, n.createdate.Year }).Select(grp => new ReportSP { SP = grp.Key.Year.ToString() + "년 " + grp.Key.Month.ToString() + "월", count = grp.Count() }).OrderBy(g => g.SP).ToList();

            // PCMS 레코드 수
            search.pcms_count = db.Privacies.Count();

            // Nuc 부여 레코드 수 (중복 포함)
            search.n360_count = db.Privacies.Where(g => g.NucleusKey.Length > 0 && g.status != PrivacyStatus.INACTIVED).Select(g => g.NucleusKey).Count();

            // Nuc 부여 레코드 (중복 제외)
            search.n360_distinct_count = db.Privacies.Where(g => g.NucleusKey.Length > 0 && g.status != PrivacyStatus.INACTIVED).Select(g => g.NucleusKey).Distinct().Count();

            // Onekey 부여 레코드 수 (중복 포함)
            search.onekey_count = db.Privacies.Where(g => g.OneKey.Length > 0 && g.status != PrivacyStatus.INACTIVED).Select(g => g.OneKey).Count();

            // Onekey 부여 레코드 (중복 제외)
            search.onekey_distinct_count = db.Privacies.Where(g => g.OneKey.Length > 0 && g.status != PrivacyStatus.INACTIVED).Select(g => g.OneKey).Distinct().Count();

            // 활성화 레코드
            search.active_count = db.Privacies.Where(cst => cst.status != PrivacyStatus.INACTIVED).Count();
            // 비활성화 레코드
            search.inactive_count = db.Privacies.Where(cst => cst.status == PrivacyStatus.INACTIVED).Count();

            return search;
        }


        // GET: Reports
        public ActionResult Index(ReportDto search)
        {
            string currentuser = !string.IsNullOrEmpty(User?.Identity?.Name) ? User.Identity.Name.ToUpper() : "Anonymous";
            currentuser = currentuser.Substring(currentuser.IndexOf('\\') + 1).Replace("\\", "").ToUpper();

            search = ReportSearch(search);


            return View(search);
        }

        public ActionResult AdminReport(ReportDto search)
        {
            if (!MyRoleManager.hasRole(MyRoleManager.RoleType.SYSTEMADMIN) && !MyRoleManager.hasRole(MyRoleManager.RoleType.DCEADMIN))
            {
                return RedirectToAction("Index");
            }

            search = AdminSearch(search);

            ViewBag.CompaniesList = GetCompanyList.GetCompany();

            return View(search);
        }

        public ActionResult XlsDownload(ReportDto search)
        {
            var sheetname = DateTime.Now.ToString("yyyyMMddHHmmss");
            string currentuser = !string.IsNullOrEmpty(User?.Identity?.Name) ? User.Identity.Name.ToUpper() : "Anonymous";

            // Data
            var privacys = ReportSearch(search, true);
            // Xls export
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("진료과별고객수" + sheetname);

            ws.Cell("A1").Value = "진료과";
            ws.Cell("B1").Value = "고객수";
            ws.Cell("C1").Value = "이메일 보우";
            ws.Cell("D1").Value = "휴대폰번호 보유";

            int row = 2;
            int cus = 0;
            int email = 0;
            int cp = 0;
            int cusTot = 0;
            int emailTot = 0;
            int cpTot = 0;
            int nsCus = 0;
            int nsEmail = 0;
            int nsCP = 0;

            foreach (var item in search.resultTotal)
            {
                if (IND_SP_LIST.Where(p => p.Text.Equals(item.SP)).Count() > 0)
                {
                    ws.Cell(row, 1).Value = item.SP;
                    cus = item.count;
                    cusTot += cus;

                    ws.Cell(row, 2).Value = cus;

                    var sp = search.resultEmail.Where(p => p.SP.Equals(item.SP)).FirstOrDefault();
                    if (sp == null)
                    {
                        email = 0;
                    }
                    else
                    {
                        email = sp.count;
                    }
                    emailTot += email;
                    ws.Cell(row, 3).Value = email;

                    sp = search.resultMobile.Where(p => p.SP.Equals(item.SP)).FirstOrDefault();
                    if (sp == null)
                    {
                        cp = 0;
                    }
                    else
                    {
                        cp = sp.count;
                    }
                    cpTot += cp;
                    ws.Cell(row, 4).Value = cp;
                } else
                {

                }
                    

                row++;
            }
            ws.Cell(row, 1).Value = "미분류";
            ws.Cell(row, 2).Value = cusTot;
            ws.Cell(row, 3).Value = emailTot;
            ws.Cell(row, 4).Value = cpTot;
            row++;
            ws.Cell(row, 1).Value = "합계";
            ws.Cell(row, 2).Value = cusTot;
            ws.Cell(row, 3).Value = emailTot;
            ws.Cell(row, 4).Value = cpTot;

            var ws2 = wb.Worksheets.Add("채널별유입고객수" + sheetname);

            ws2.Cell("A1").Value = "월";
            ws2.Cell("B1").Value = "GRV";
            ws2.Cell("C1").Value = "MMS";
            ws2.Cell("D1").Value = "서면동의서";
            ws2.Cell("E1").Value = "PForceRX";
            ws2.Cell("F1").Value = "총합계";

            row = 2;
            int mms, grv, doc, pfrx, mmsTot, grvTot, docTot, pfrxTot, totSum;
            mms = grv = doc = pfrx = mmsTot = grvTot = docTot = pfrxTot = totSum = 0;

            foreach (var item in search.resultTotal)
            {

                var it = search.rsMMS.Where(p => p.SP.Equals(item.SP)).FirstOrDefault();
                if (it == null)
                {
                    mms = 0;
                }
                else
                {
                    mms = it.count;
                }
                mmsTot += mms;

                it = search.rsGRV.Where(p => p.SP.Equals(item.SP)).FirstOrDefault();
                if (it == null)
                {
                    grv = 0;
                }
                else
                {
                    grv = it.count;
                }
                grvTot += grv;

                it = search.rsDOC.Where(p => p.SP.Equals(item.SP)).FirstOrDefault();
                if (it == null)
                {
                    doc = 0;
                }
                else
                {
                    doc = it.count;
                }
                docTot += doc;

                it = search.rsPFRX.Where(p => p.SP.Equals(item.SP)).FirstOrDefault();
                if (it == null)
                {
                    pfrx = 0;
                }
                else
                {
                    pfrx = it.count;
                }
                pfrxTot += pfrx;

                totSum += item.count;

                ws2.Cell(row, 1).Value = item.SP;
                ws2.Cell(row, 2).Value = mms;
                ws2.Cell(row, 3).Value = grv;
                ws2.Cell(row, 4).Value = doc;
                ws2.Cell(row, 5).Value = pfrx;
                ws2.Cell(row, 6).Value = item.count;

                row++;
            }

            ws2.Cell(row, 1).Value = "합계";
            ws2.Cell(row, 2).Value = mmsTot;
            ws2.Cell(row, 3).Value = grvTot;
            ws2.Cell(row, 4).Value = docTot;
            ws2.Cell(row, 5).Value = pfrxTot;
            ws2.Cell(row, 6).Value = totSum;

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

        public ActionResult AdminXlsDownload(ReportDto search)
        {
            var sheetname = DateTime.Now.ToString("yyyyMMddHHmmss");
            string currentuser = !string.IsNullOrEmpty(User?.Identity?.Name) ? User.Identity.Name.ToUpper() : "Anonymous";

            // Data
            search = AdminSearch(search, true);
            // Xls export
            var wb = new XLWorkbook();
            var ws = wb.Worksheets.Add("승인반려 " + sheetname);

            ws.Cell("A1").Value = "월";
            ws.Cell("B1").Value = "승인";
            ws.Cell("C1").Value = "반려";
            ws.Cell("D1").Value = "합계";

            int row = 2;
            int aprTot = 0, rejTot = 0, Tot = 0, cnt = 0;
            foreach (var item in search.rsApproved)
            {
                var objRejected = search.rsRejected.Where(p => p.SP.Equals(item.SP)).FirstOrDefault();
                if (objRejected != null) cnt = objRejected.count;
                ws.Cell(row, 1).Value = item.SP;
                ws.Cell(row, 2).Value = item.count;
                ws.Cell(row, 3).Value = cnt;
                ws.Cell(row, 4).Value = cnt + item.count;
                aprTot += item.count;
                rejTot += cnt;
                Tot += item.count + cnt;
                row++;
            }
            ws.Cell(row, 1).Value = "합계";
            ws.Cell(row, 2).Value = aprTot;
            ws.Cell(row, 3).Value = rejTot;
            ws.Cell(row, 4).Value = Tot;

            var ws2 = wb.Worksheets.Add("Code Mapping" + sheetname);

            ws2.Cell("A1").Value = "월";
            ws2.Cell("B1").Value = "Nucleus Code";
            ws2.Cell("C1").Value = "OneKey Code";

            row = 2;
            int nTot = 0, oTot = 0;
            cnt = 0;
            foreach (var item in search.rsN360)
            {
                var obj = search.rsOnekey.Where(p => p.SP.Equals(item.SP)).FirstOrDefault();
                if (obj != null) { cnt = obj.count; }
                ws2.Cell(row, 1).Value = item.SP;
                ws2.Cell(row, 2).Value = item.count;
                ws2.Cell(row, 3).Value = cnt;
                nTot+= item.count;
                oTot+= cnt;
                row++;
            }
            ws2.Cell(row, 1).Value = "합계";
            ws2.Cell(row, 2).Value = nTot;
            ws2.Cell(row, 3).Value = oTot;

            var ws3 = wb.Worksheets.Add("고객개인정보" + sheetname);

            ws3.Cell("A1").Value = "전체 PCMS 레코드";
            ws3.Cell("B1").Value = "Nucleus 부여 레코드(중복포함)";
            ws3.Cell("C1").Value = "Nucleus 부여 레코드(중복제외)";

            ws3.Cell("A2").Value = search.pcms_count;
            ws3.Cell("B2").Value = search.n360_count;
            ws3.Cell("C2").Value = search.n360_distinct_count;

            ws3.Cell("A3").Value = "전체 PCMS 레코드";
            ws3.Cell("B3").Value = "Onekey 부여 레코드(중복포함)";
            ws3.Cell("C3").Value = "Onekey 부여 레코드(중복제외)";

            ws3.Cell("A4").Value = search.pcms_count;
            ws3.Cell("B4").Value = search.onekey_count;
            ws3.Cell("C4").Value = search.onekey_distinct_count;

            ws3.Cell("A5").Value = "전체 PCMS 레코드";
            ws3.Cell("B5").Value = "활성화 레코드";
            ws3.Cell("C5").Value = "비활성화 레코드";

            ws3.Cell("A6").Value = search.pcms_count;
            ws3.Cell("B6").Value = search.active_count;
            ws3.Cell("C6").Value = search.inactive_count;

            Stream fs = new MemoryStream();
            wb.SaveAs(fs);
            fs.Position = 0;

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = "pcms_admin_report_" + currentuser + "_" + sheetname + ".xlsx",
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(fs, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }
    }
}