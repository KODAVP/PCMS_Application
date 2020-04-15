using System.Linq;
using System.Net;
using System.Web.Mvc;
using PrivacyConsentDB.Models;
using PrivacyConsentDB.Commons;

namespace PrivacyConsentDB.Controllers
{
    [Log]
    [Auth]
    public class AgreementsController : Controller
    {
        private PCMSDBContext db = new PCMSDBContext();

        // GET: Agreements
        public ActionResult Index()
        {
            return View(db.Agreements.ToList());
        }

        // GET: Agreements/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agreement agreement = db.Agreements.Find(id);
            if (agreement == null)
            {
                return HttpNotFound();
            }
            return View(agreement);
        }

        // GET: Agreements/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Agreements/Create
        // 초과 게시 공격으로부터 보호하려면 바인딩하려는 특정 속성을 사용하도록 설정하십시오. 
        // 자세한 내용은 http://go.microsoft.com/fwlink/?LinkId=317598을(를) 참조하십시오.
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Create([Bind(Include = "ID,title,contents,createdate,creater,modifieddate,modifier")] Agreement agreement)
        {
            if (ModelState.IsValid)
            {
                db.Agreements.Add(agreement);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(agreement);
        }

        // GET: Agreements/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agreement agreement = db.Agreements.Find(id);
            if (agreement == null)
            {
                return HttpNotFound();
            }
            return View(agreement);
        }

        // POST: Agreements/Edit/5
        // 초과 게시 공격으로부터 보호하려면 바인딩하려는 특정 속성을 사용하도록 설정하십시오. 
        // 자세한 내용은 http://go.microsoft.com/fwlink/?LinkId=317598을(를) 참조하십시오.
        [HttpPost]
        [ValidateInput(false)]
        public ActionResult Edit([Bind(Include = "ID,title,contents")] Agreement agreement)
        {
            if (ModelState.IsValid)
            {
                Agreement a = db.Agreements.Find(agreement.ID);
                a.title = agreement.title;
                a.contents = agreement.contents;
                db.Entry(a).State = System.Data.Entity.EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(agreement);
        }

        // GET: Agreements/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Agreement agreement = db.Agreements.Find(id);
            if (agreement == null)
            {
                return HttpNotFound();
            }
            return View(agreement);
        }

        // POST: Agreements/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Agreement agreement = db.Agreements.Find(id);
            db.Agreements.Remove(agreement);
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
