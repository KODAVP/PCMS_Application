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
    public class AccessPathsController : Controller
    {
        private PCMSDBContext db = new PCMSDBContext();

        // GET: AccessPaths
        public ActionResult Index()
        {
            return View(db.AccessPaths.ToList());
        }

        // GET: AccessPaths/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccessPaths accessPaths = db.AccessPaths.Find(id);
            if (accessPaths == null)
            {
                return HttpNotFound();
            }
            return View(accessPaths);
        }

        // GET: AccessPaths/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: AccessPaths/Create
        // 초과 게시 공격으로부터 보호하려면 바인딩하려는 특정 속성을 사용하도록 설정하십시오. 
        // 자세한 내용은 http://go.microsoft.com/fwlink/?LinkId=317598을(를) 참조하십시오.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,path,name")] AccessPaths accessPaths)
        {
            if (accessPaths != null && accessPaths.name != null && accessPaths.path != null)
            {

                string currentuser = !string.IsNullOrEmpty(User?.Identity?.Name) ? User.Identity.Name.ToUpper() : "Anonymous";
                currentuser = currentuser.Substring(currentuser.IndexOf('\\') + 1).Replace("\\", "").ToUpper();

                accessPaths.creator = currentuser;
                accessPaths.createdate = DateTime.UtcNow;

                db.AccessPaths.Add(accessPaths);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(accessPaths);
        }

        // GET: AccessPaths/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccessPaths accessPaths = db.AccessPaths.Find(id);
            if (accessPaths == null)
            {
                return HttpNotFound();
            }
            return View(accessPaths);
        }

        // POST: AccessPaths/Edit/5
        // 초과 게시 공격으로부터 보호하려면 바인딩하려는 특정 속성을 사용하도록 설정하십시오. 
        // 자세한 내용은 http://go.microsoft.com/fwlink/?LinkId=317598을(를) 참조하십시오.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,path,name")] AccessPaths accessPaths)
        {
            if (ModelState.IsValid)
            {
                string currentuser = !string.IsNullOrEmpty(User?.Identity?.Name) ? User.Identity.Name.ToUpper() : "Anonymous";
                currentuser = currentuser.Substring(currentuser.IndexOf('\\') + 1).Replace("\\", "").ToUpper();

                accessPaths.modifier = currentuser;
                accessPaths.modifieddate= DateTime.UtcNow;

                db.Entry(accessPaths).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(accessPaths);
        }

        // GET: AccessPaths/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            AccessPaths accessPaths = db.AccessPaths.Find(id);
            if (accessPaths == null)
            {
                return HttpNotFound();
            }
            return View(accessPaths);
        }

        // POST: AccessPaths/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            AccessPaths accessPaths = db.AccessPaths.Find(id);
            db.AccessPaths.Remove(accessPaths);
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
