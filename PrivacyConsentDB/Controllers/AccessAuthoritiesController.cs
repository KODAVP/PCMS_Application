using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using PrivacyConsentDB;
using PrivacyConsentDB.Models;
using PrivacyConsentDB.Dto;
using PrivacyConsentDB.Commons;

namespace PrivacyConsentDB.Controllers
{
    [Log]
    [Role]
    [Auth]
    public class AccessAuthoritiesController : Controller
    {
        private PCMSDBContext db = new PCMSDBContext();

        // GET: AccessAuthorities
        public ActionResult Index()
        {
            AccessAuthorities auth = new AccessAuthorities();

            auth.pathID = 1;

            return View(db.AccessAuthorities.ToList());
        }

        // GET: AccessAuthorities/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccessAuthorities accessAuthorities = db.AccessAuthorities.Find(id);
            if (accessAuthorities == null)
            {
                return HttpNotFound();
            }
            return View(accessAuthorities);
        }

        // GET: AccessAuthorities/Create
        public ActionResult Create()
        {
            AccessAuthoritiesDto dto = new AccessAuthoritiesDto();

            dto.AccessPaths = db.AccessPaths.ToList();
            dto.AccessRoles = db.AccessRoles.ToList();

            return View(dto);
        }

        // POST: AccessAuthorities/Create
        // 초과 게시 공격으로부터 보호하려면 바인딩하려는 특정 속성을 사용하도록 설정하십시오. 
        // 자세한 내용은 http://go.microsoft.com/fwlink/?LinkId=317598을(를) 참조하십시오.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "pathID,roleID")] AccessAuthorities accessAuthorities)
        {
            if (accessAuthorities.pathID > 0 && accessAuthorities.roleID > 0)
            {
                string currentuser = !string.IsNullOrEmpty(User?.Identity?.Name) ? User.Identity.Name.ToUpper() : "Anonymous";
                currentuser = currentuser.Substring(currentuser.IndexOf('\\') + 1).Replace("\\", "").ToUpper();

                accessAuthorities.creator = currentuser;
                accessAuthorities.createdate = DateTime.UtcNow;

                db.AccessAuthorities.Add(accessAuthorities);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(accessAuthorities);
        }

        // GET: AccessAuthorities/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccessAuthorities accessAuthorities = db.AccessAuthorities.Find(id);
            if (accessAuthorities == null)
            {
                return HttpNotFound();
            }
            return View(accessAuthorities);
        }

        // POST: AccessAuthorities/Edit/5
        // 초과 게시 공격으로부터 보호하려면 바인딩하려는 특정 속성을 사용하도록 설정하십시오. 
        // 자세한 내용은 http://go.microsoft.com/fwlink/?LinkId=317598을(를) 참조하십시오.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,pathID,roleID")] AccessAuthorities accessAuthorities)
        {
            if (accessAuthorities.pathID > 0 && accessAuthorities.roleID > 0 && accessAuthorities.ID > 0)
            {
                string currentuser = !string.IsNullOrEmpty(User?.Identity?.Name) ? User.Identity.Name.ToUpper() : "Anonymous";
                currentuser = currentuser.Substring(currentuser.IndexOf('\\') + 1).Replace("\\", "").ToUpper();

                accessAuthorities.modifier = currentuser;
                accessAuthorities.modifieddate = DateTime.UtcNow;

                db.Entry(accessAuthorities).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(accessAuthorities);
        }

        // GET: AccessAuthorities/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccessAuthorities accessAuthorities = db.AccessAuthorities.Find(id);
            if (accessAuthorities == null)
            {
                return HttpNotFound();
            }
            return View(accessAuthorities);
        }

        // POST: AccessAuthorities/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AccessAuthorities accessAuthorities = db.AccessAuthorities.Find(id);
            db.AccessAuthorities.Remove(accessAuthorities);
            db.SaveChanges();
            return RedirectToAction("Index");
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
