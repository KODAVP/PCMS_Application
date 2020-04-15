using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using PrivacyConsentDB.Models;
using PrivacyConsentDB.Commons;

namespace PrivacyConsentDB.Controllers
{
    [Log]
    [Role]
    [Auth]
    public class ChannelsController : Controller
    {
        private PCMSDBContext db = new PCMSDBContext();

        // GET: Channels
        public ActionResult Index()
        {
            return View(db.Channels.ToList());
        }

        // GET: Channels/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Channel channel = db.Channels.Find(id);
            if (channel == null)
            {
                return HttpNotFound();
            }
            return View(channel);
        }

        // GET: Channels/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Channels/Create
        // 초과 게시 공격으로부터 보호하려면 바인딩하려는 특정 속성을 사용하도록 설정하십시오. 
        // 자세한 내용은 http://go.microsoft.com/fwlink/?LinkId=317598을(를) 참조하십시오.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "ID,name,type,athour,usage,modifieddate,host,account,pwd,path")] Channel channel)
        {
            if (ModelState.IsValid)
            {
                db.Channels.Add(channel);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(channel);
        }

        // GET: Channels/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Channel channel = db.Channels.Find(id);
            if (channel == null)
            {
                return HttpNotFound();
            }
            return View(channel);
        }
        
        public ActionResult Run([Bind(Include = "ID")] Channel channel)
        {
            Channel c = db.Channels.Find(channel.ID);
            c.Instantrun = !c.Instantrun;
            db.Entry(c).State = EntityState.Modified;
            db.SaveChanges2noDate();

            return RedirectToAction("Index");
        }

        // POST: Channels/Edit/5
        // 초과 게시 공격으로부터 보호하려면 바인딩하려는 특정 속성을 사용하도록 설정하십시오. 
        // 자세한 내용은 http://go.microsoft.com/fwlink/?LinkId=317598을(를) 참조하십시오.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "ID,athour,usage,path,exportpath,backuppath")] Channel channel)
        {
            if (ModelState.IsValid)
            {
                Channel c = db.Channels.Find(channel.ID);
                c.athour = channel.athour;
                c.usage = channel.usage;
                c.path = channel.path;
                c.exportpath = channel.exportpath;
                c.backuppath = channel.backuppath;
                db.Entry(c).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(channel);
        }

        // GET: Channels/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Channel channel = db.Channels.Find(id);
            if (channel == null)
            {
                return HttpNotFound();
            }
            return View(channel);
        }

        // POST: Channels/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Channel channel = db.Channels.Find(id);
            db.Channels.Remove(channel);
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
