using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarFinalProject.Models;

namespace CarFinalProject.Controllers
{
    public class SpecialOfferController : Controller
    {
        private CarDBEntities db = new CarDBEntities();

        // GET: SpecialOffer
        public ActionResult Index()
        {
            return View(db.SpecialOffers.ToList());
        }

        // GET: SpecialOffer/Details/5
        [Authorize(Users = "admin@admin.com")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpecialOffer specialOffer = db.SpecialOffers.Find(id);
            if (specialOffer == null)
            {
                return HttpNotFound();
            }
            return View(specialOffer);
        }

        // GET: SpecialOffer/Create
        [Authorize(Users = "admin@admin.com")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: SpecialOffer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,Name,Code")] SpecialOffer specialOffer)
        {
            if (ModelState.IsValid)
            {
                db.SpecialOffers.Add(specialOffer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(specialOffer);
        }

        // GET: SpecialOffer/Edit/5
        [Authorize(Users = "admin@admin.com")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpecialOffer specialOffer = db.SpecialOffers.Find(id);
            if (specialOffer == null)
            {
                return HttpNotFound();
            }
            return View(specialOffer);
        }

        // POST: SpecialOffer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Code")] SpecialOffer specialOffer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(specialOffer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(specialOffer);
        }

        // GET: SpecialOffer/Delete/5
        [Authorize(Users = "admin@admin.com")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SpecialOffer specialOffer = db.SpecialOffers.Find(id);
            if (specialOffer == null)
            {
                return HttpNotFound();
            }
            return View(specialOffer);
        }

        // POST: SpecialOffer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SpecialOffer specialOffer = db.SpecialOffers.Find(id);
            db.SpecialOffers.Remove(specialOffer);
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
