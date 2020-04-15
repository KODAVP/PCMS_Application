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
using PrivacyConsentDB.Commons;

namespace PrivacyConsentDB.Controllers
{
    [Log]
    [Role]
    [Auth]
    public class AccessRolesController : Controller
    {
        private PCMSDBContext db = new PCMSDBContext();

        // GET: AccessRoles
        public ActionResult Index()
        {
            return View(db.AccessRoles.ToList());
        }

        // GET: AccessRoles/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccessRoles accessRoles = db.AccessRoles.Find(id);
            if (accessRoles == null)
            {
                return HttpNotFound();
            }
            return View(accessRoles);
        }

        // GET: AccessRoles/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AccessRoles/Create
        // 초과 게시 공격으로부터 보호하려면 바인딩하려는 특정 속성을 사용하도록 설정하십시오. 
        // 자세한 내용은 http://go.microsoft.com/fwlink/?LinkId=317598을(를) 참조하십시오.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "name")] AccessRoles accessRoles)
        {
            if (accessRoles != null && accessRoles.name != null)
            {
                string currentuser = !string.IsNullOrEmpty(User?.Identity?.Name) ? User.Identity.Name.ToUpper() : "Anonymous";
                currentuser = currentuser.Substring(currentuser.IndexOf('\\') + 1).Replace("\\", "").ToUpper();

                accessRoles.creator = currentuser;
                accessRoles.createdate = DateTime.UtcNow;
                db.AccessRoles.Add(accessRoles);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(accessRoles);
        }

        // GET: AccessRoles/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccessRoles accessRoles = db.AccessRoles.Find(id);
            if (accessRoles == null)
            {
                return HttpNotFound();
            }
            return View(accessRoles);
        }

        // POST: AccessRoles/Edit/5
        // 초과 게시 공격으로부터 보호하려면 바인딩하려는 특정 속성을 사용하도록 설정하십시오. 
        // 자세한 내용은 http://go.microsoft.com/fwlink/?LinkId=317598을(를) 참조하십시오.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,name,createdate,creator,modifieddate,modifier")] AccessRoles accessRoles)
        {
            if (ModelState.IsValid)
            {
                string currentuser = !string.IsNullOrEmpty(User?.Identity?.Name) ? User.Identity.Name.ToUpper() : "Anonymous";
                currentuser = currentuser.Substring(currentuser.IndexOf('\\') + 1).Replace("\\", "").ToUpper();

                accessRoles.modifier = currentuser;
                accessRoles.modifieddate = DateTime.UtcNow;

                db.Entry(accessRoles).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(accessRoles);
        }

        // GET: AccessRoles/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccessRoles accessRoles = db.AccessRoles.Find(id);
            if (accessRoles == null)
            {
                return HttpNotFound();
            }
            return View(accessRoles);
        }

        // POST: AccessRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AccessRoles accessRoles = db.AccessRoles.Find(id);
            db.AccessRoles.Remove(accessRoles);
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
