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
    public class ShippingsController : Controller
    {
        private CarDBEntities db = new CarDBEntities();


        // GET: Shippings
        [Authorize(Users ="admin@admin.com")]
        public ActionResult Index()
        {
            var shippings = db.Shippings.Include(s => s.Customer);
            return View(shippings.ToList());
        }

        // GET: Shippings/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipping shipping = db.Shippings.Find(id);
            if (shipping == null)
            {
                return HttpNotFound();
            }
            return View(shipping);
        }

        // GET: Shippings/Create
        public ActionResult Create()
        {
            ViewBag.CustomerName = new SelectList(db.Customers, "UserName", "Name");
            return View();
        }

        // POST: Shippings/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,City,Country,Street,Phone,Zip,CustomerName")] Shipping shipping)
        {
            if (ModelState.IsValid)
            {
                db.Shippings.Add(shipping);
                db.SaveChanges();
                var cart = db.Carts.Where(c => c.Status == "unpaid" && c.CustomerName == shipping.CustomerName).First();
                int cartid = cart.Id;
                ViewBag.cartid = cartid;
                ViewBag.CustomerName = new SelectList(db.Customers, "UserName", "Name", shipping.CustomerName);
                return RedirectToAction("CheckOut", "Transaction", new { id = cartid });
            }
            else
            {
                return View(shipping);
            }
        }

        // GET: Shippings/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipping shipping = db.Shippings.Find(id);
            var cart = db.Carts.Where(c => c.Status == "unpaid" && c.CustomerName == User.Identity.Name).First();
            int cartid = cart.Id;
            ViewBag.cartid = cartid;
            if (shipping == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerName = new SelectList(db.Customers, "UserName", "Name", shipping.CustomerName);
            return View(shipping);
        }

        // POST: Shippings/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,City,Country,Street,Phone,Zip,CustomerName")] Shipping shipping)
        {
            
            if (ModelState.IsValid)
            {
                var cart = db.Carts.Where(c => c.Status == "unpaid" && c.CustomerName == User.Identity.Name).First();
                int cartid = cart.Id;
                ViewBag.cartid = cartid;
                db.Entry(shipping).State = EntityState.Modified;
                db.SaveChanges();
                ViewBag.CustomerName = new SelectList(db.Customers, "UserName", "Name", shipping.CustomerName);
                return RedirectToAction("CheckOut", "Transaction", new { id = cartid });
            }
            else
            {
                return View(shipping);
            }
            
        }

        // GET: Shippings/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Shipping shipping = db.Shippings.Find(id);
            if (shipping == null)
            {
                return HttpNotFound();
            }
            return View(shipping);
        }

        // POST: Shippings/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Shipping shipping = db.Shippings.Find(id);
            db.Shippings.Remove(shipping);
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
