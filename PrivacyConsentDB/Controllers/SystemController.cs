using ClosedXML.Excel;
using log4net;
using PrivacyConsentDB.Commons;
using PrivacyConsentDB.Dto;
using PrivacyConsentDB.Models;
using System;
using System.Collections;
using System.Collections.Generic;
using System.DirectoryServices.AccountManagement;
using System.IO;
using System.Linq;
using System.Net;
using System.Reflection;
using System.Text.RegularExpressions;
using System.Web.Mvc;
using System.Web.Services;
using static PrivacyConsentDB.Commons.Status;

namespace PrivacyConsentDB.Controllers
{
    [Log]
    [Role]
    [Auth]
    public class SystemController : Controller
    {
        private PCMSDBContext db = new PCMSDBContext();
        protected static readonly ILog log = LogManager.GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        // GET: System
        public ActionResult Role()
        {
            AccessAuthoritiesDto dto = new AccessAuthoritiesDto();

            dto.AccessPaths = db.AccessPaths.ToList();
            dto.AccessRoles = db.AccessRoles.ToList();
            if (MyRoleManager.hasRole(MyRoleManager.RoleType.DCEADMIN))
            {
                dto.UserRole = db.UserRoles.OrderBy(ur => ur.username).Where(ur => ur.roletype != MyRoleManager.RoleType.SYSTEMADMIN).ToList();
            }
            else
            {
                dto.UserRole = db.UserRoles.OrderBy(ur => ur.username).ThenBy(ur => ur.roletype).ToList();
            }

            ViewBag.CompaniesList = GetCompanyList.GetCompany();

            return View(dto);
        }

        //Filter rows based on company selection - Prakash
        [HttpPost]
        public ActionResult Role(string ddlCompanies)
        {
            AccessAuthoritiesDto dto = new AccessAuthoritiesDto();

            dto.AccessPaths = db.AccessPaths.ToList();
            dto.AccessRoles = db.AccessRoles.ToList();
            
            dto.UserRole = db.UserRoles.OrderBy(ur => ur.username).Where(ur => ur.COMP_CODE == ddlCompanies).ToList();

            ViewBag.CompaniesList = GetCompanyList.GetCompany();

            return View(dto);
        }

        [WebMethod]
        public JsonResult AddUserRole(UserRoleAddDto dto)
        {
            try
            {
                foreach (var u in dto.users)
                {
                    int cnt = db.UserRoles.Where(ur => ur.username == u).Count();
                    if (cnt < 1)
                    {
                        db.UserRoles.Add(new UserRole { roletype = dto.role, username = u, COMP_CODE = dto.company });

                        //RoleLogs code added by Nagaraju Madishetti as part of IND29615692i-5/Nov/2018
                        string myIP = Server.HtmlEncode(Request.UserHostAddress);
                        string currentuser = !string.IsNullOrEmpty(User?.Identity?.Name) ? User.Identity.Name.ToUpper() : "Anonymous";
                        currentuser = currentuser.Substring(currentuser.IndexOf('\\') + 1).Replace("\\", "").ToUpper();
                        string Activity = "Assigned to " + dto.role.GetDisplayName() + " role";
                        db.Rolelogs.Add(new Rolelog { UserID = currentuser, IP = myIP, Target_User_ID = u, Activity = Activity, COMP_CODE = dto.company });


                    }
                }
                db.SaveChanges();
            }
            catch (Exception e)
            {
                log.Error(e);
                return Json(new { result = false });
            }
            return Json(new { result = true });
        }

        [WebMethod]
        public JsonResult UpdateRole(UserRoleUpdateDto dto)
        {
            try
            {
                if (dto.type == "Add")
                {
                    foreach (var u in dto.users)
                    {
                        int cnt = db.UserRoles.Where(ur => ur.username == u).Count();
                        if(cnt > 0)
                        {
                            db.UserRoles.Add(new UserRole { roletype = dto.role, username = u, COMP_CODE = dto.company });
                            UserRole role = db.UserRoles.Where(ur => ur.username == u).FirstOrDefault();
                            if(role != null)
                            {
                                //RoleLogs code added by Nagaraju Madishetti as part of IND29615692i-5/Nov/2018

                                string myIP = Server.HtmlEncode(Request.UserHostAddress);
                                string currentuser = !string.IsNullOrEmpty(User?.Identity?.Name) ? User.Identity.Name.ToUpper() : "Anonymous";
                                currentuser = currentuser.Substring(currentuser.IndexOf('\\') + 1).Replace("\\", "").ToUpper();
                                string Activity = role.roletype.GetDisplayName() +" To " + dto.role.GetDisplayName() + " role";
                                db.Rolelogs.Add(new Rolelog { UserID = currentuser, IP = myIP, Target_User_ID = u, Activity = Activity,COMP_CODE = dto.company });

                                role.roletype = dto.role;
                            }
                        }
                        
                    }
                    db.SaveChanges();
                }
                else
                {
                    foreach (var u in dto.users)
                    {
                        IEnumerable<UserRole> list = db.UserRoles.Where(ur => ur.username == u && ur.roletype == dto.role).ToList();
                        db.UserRoles.RemoveRange(list);
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


        public ActionResult Channel()
        {
            return View();
        }

        public ActionResult ChannelHistory(PageDto dto)
        {
            if (dto == null) {
                dto = new PageDto();
            }
            var bs = db.Batchs.Where(b => 1 == 1);
            // 바운드
            if (dto.bound == "0") {
                bs = bs.Where(b => b.bound == BoundType.Inbound);
            } else if (dto.bound == "1")
            {
                bs = bs.Where(b => b.bound == BoundType.Outbound);
            }
            // 채널
            if (!string.IsNullOrEmpty(dto.channels))
            {
                bs = bs.Where(b => b.name.Contains(dto.channels));
            }
            // 상태
            if (dto.status == "0") {
                bs = bs.Where(b => b.status == BatchStatus.Begin);
            }
            else if (dto.status == "1")
            {
                bs = bs.Where(b => b.status == BatchStatus.Completed);
            }
            else if (dto.status == "2")
            {
                bs = bs.Where(b => b.status == BatchStatus.Error);
            }

            if (dto.executedt != null)
            {
                bs = bs.Where(b => b.createdate == dto.executedt);
            }

            dto.totalCount = bs.Count();
            dto.list = bs.OrderByDescending(c => c.createdate).Skip(dto.startIndex * dto.pageSize).Take(dto.pageSize).ToList();
            return View(dto);
        }

        public ActionResult XlsDownload(PageDto dto)
        {
            if (dto == null)
            {
                dto = new PageDto();
            }
            var bs = db.Batchs.Where(b => 1 == 1);
            // 바운드
            if (dto.bound == "0")
            {
                bs = bs.Where(b => b.bound == BoundType.Inbound);
            }
            else if (dto.bound == "1")
            {
                bs = bs.Where(b => b.bound == BoundType.Outbound);
            }
            // 채널
            if (!string.IsNullOrEmpty(dto.channels))
            {
                bs = bs.Where(b => b.name.Contains(dto.channels));
            }
            // 상태
            if (dto.status == "0")
            {
                bs = bs.Where(b => b.status == BatchStatus.Begin);
            }
            else if (dto.status == "1")
            {
                bs = bs.Where(b => b.status == BatchStatus.Completed);
            }
            else if (dto.status == "2")
            {
                bs = bs.Where(b => b.status == BatchStatus.Error);
            }
            if (dto.executedt != null)
            {
                bs = bs.Where(b => b.createdate == dto.executedt);
            }

            dto.totalCount = bs.Count();
            dto.list = bs.OrderByDescending(c => c.createdate).ToList();            // Xls export
            var wb = new XLWorkbook();
            var sheetname = DateTime.UtcNow.ToString("yyyyMMddHHmmss");
            var ws = wb.Worksheets.Add("InterfaceLog" + sheetname);

            ws.Cell("A1").Value = "수행일시";
            ws.Cell("B1").Value = "인터페이스명";
            ws.Cell("C1").Value = "방향";
            ws.Cell("D1").Value = "상태";
            ws.Cell("E1").Value = "메세지";

            int row = 2;
            foreach (Batch item in dto.list)
            {
                ws.Cell(row, 1).Value = TimeZoneInfo.ConvertTime(item.createdate, TimeZoneInfo.Utc, TimeZoneInfo.FindSystemTimeZoneById("Korea Standard Time")).ToString(); ;
                ws.Cell(row, 2).Value = item.name;
                ws.Cell(row, 3).Value = item.bound == 0 ? "Inbound" : "Outbound";

                var stat = @"";
                if (item.status == PrivacyConsentDB.Commons.BatchStatus.Begin)
                {
                    stat = @"Started";
                }
                else if (item.status == PrivacyConsentDB.Commons.BatchStatus.Completed)
                {
                    stat = @"Completed";
                }
                else if (item.status == PrivacyConsentDB.Commons.BatchStatus.Error)
                {
                    stat = @"Error";
                }

                ws.Cell(row, 4).Value = stat;
                ws.Cell(row, 5).Value = item.message;
                row++;
            }
            Stream fs = new MemoryStream();
            wb.SaveAs(fs);
            fs.Position = 0;

            var cd = new System.Net.Mime.ContentDisposition
            {
                FileName = "pcms_interfacelog_" + sheetname + ".xlsx",
                Inline = true,
            };

            Response.AppendHeader("Content-Disposition", cd.ToString());
            return File(fs, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet");
        }

        public ActionResult PrivacyHistory()
        {
            return View();
        }

        public JsonResult MailApi()
        {
            bool result = SMTPHelper.TestMail();
            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GroupApi() {
            ArrayList result = new ArrayList();

            PrincipalContext yourDomain = new PrincipalContext(ContextType.Domain);
            // find your user
            UserPrincipal user = UserPrincipal.FindByIdentity(yourDomain, @"Choiy28");

            // if found - grab its groups
            if (user != null)
            {
                if (user.IsMemberOf(yourDomain, IdentityType.Name, @"MTESWEB-PCMS-DEV-RW")) // MTESWEB-PCMS-DEV-RW  DL-AP5-BT
                { 
                    result.Add(true);
                }
            }

            return Json(new { result = result }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReSend()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ReSend(SendChannel channel, string nkeys) {
            string temp = Regex.Replace(nkeys, @"\D", " ");
            string[] nums = temp.Split(' ');
            foreach (string k in nums) {
                if (string.IsNullOrEmpty(k)) continue;
                List<Privacy> plist = db.Privacies.Where(p => p.NucleusKey.Equals(k)).ToList();
                foreach (Privacy pp in plist)
                {
                    pp.SENDCHANEL &= ~channel;
                    if (channel == SendChannel.PFORCERX) {
                        pp.SENDCHANEL &= ~SendChannel.PFORCERX_INVALID;
                        pp.SENDCHANEL &= ~SendChannel.PFORCERX_ONEKEY_DUP;
                    }
                    db.Entry(pp).State = System.Data.Entity.EntityState.Modified;
                }
                db.SaveChanges();                
            }

            return RedirectToAction("ReSend"); ;
        }
    }
}