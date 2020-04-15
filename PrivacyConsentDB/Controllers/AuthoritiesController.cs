using PrivacyConsentDB.Commons;
using PrivacyConsentDB.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace PrivacyConsentDB.Controllers
{
    [Log]
    [Auth]
    public class AuthoritiesController : Controller
    {
        private PCMSDBContext db = new PCMSDBContext();

        // GET: Authorities
        public ActionResult Index()
        {
            return View();
        }

        // POST: Agreements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            /*
            Agreement agreement = db.Agreements.Find(id);
            db.Agreements.Remove(agreement);
            db.SaveChanges();
            */
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