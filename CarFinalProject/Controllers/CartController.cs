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
    [Authorize]
    public class CartController : Controller
    {
        private CarDBEntities db = new CarDBEntities();

        // GET: Cart
        public ActionResult Index()
        {
            string username = User.Identity.Name;
            var carts = db.Carts.Where(c => c.CustomerName == username);
            return View(carts.ToList());
        }

        // GET: Cart/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            int? prevtotal = cart.CartItems.Sum(c => c.Quantity * c.Product.Price);
            int? total = 0;
            ViewBag.prevtotal = prevtotal;
            //the customer has offer no 1 added to account
            if(hasOffer(1) && countPaidCarts() == 0)
            {
                ViewBag.offer1 = "yes";
                total = prevtotal - (int)(prevtotal * 0.15);
                db.Entry(cart).State = EntityState.Modified;
                cart.Total = total;
                db.SaveChanges();
            }

            //no the customer doesnt have offer no1
            else
            {
                total = prevtotal;
                db.Entry(cart).State = EntityState.Modified;
                cart.Total = total;
                db.SaveChanges();
            }
            
            //is the cutomer a lucky draw winner, if yes then the order will be 50%off
            if(isWinner())
            {
                total = prevtotal - (int) (0.50 * prevtotal);
                db.Entry(cart).State = EntityState.Modified;
                cart.Total = total;
                db.SaveChanges();
            }

            //adjust the total even after the check out process
            if(Session["winner"] != null)
            {
                total = prevtotal - (int)(0.50 * prevtotal);
                db.Entry(cart).State = EntityState.Modified;
                cart.Total = total;
                db.SaveChanges();
            }
            if (cart == null)
            {
                return HttpNotFound();
            }

            //does the customer already has shipping information in the table
            //if has, then pass the id of the record
            if(db.Shippings.Where( c => c.CustomerName == User.Identity.Name).Count() > 0)
            {
                ViewBag.hasShipping = "Yes";
                ViewBag.shipid = db.Shippings.Where(s => s.CustomerName == User.Identity.Name).First().Id;
            }
            
            return View(cart);
        }

        // GET: Cart/Create
        public ActionResult Create()
        {
            ViewBag.CustomerName = new SelectList(db.Customers, "UserName", "Name");
            return View();
        }

        // POST: Cart/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CustomerName,Status")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                db.Carts.Add(cart);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerName = new SelectList(db.Customers, "UserName", "Name", cart.CustomerName);
            return View(cart);
        }

        // GET: Cart/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerName = new SelectList(db.Customers, "UserName", "Name", cart.CustomerName);
            return View(cart);
        }

        // POST: Cart/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CustomerName,Status")] Cart cart)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cart).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerName = new SelectList(db.Customers, "UserName", "Name", cart.CustomerName);
            return View(cart);
        }

        // GET: Cart/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // POST: Cart/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cart cart = db.Carts.Find(id);
            if (cart.Status == "unpaid")
            {
                foreach (CartItem items in cart.CartItems.ToList())
                {
                    db.CartItems.Remove(items);
                    db.SaveChanges();
                }
                db.Carts.Remove(cart);
                db.SaveChanges();
            }
            else
            {
                Session["Message"] = "The paid cart cannot be deleted.";
            }
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
        public Boolean hasOffer(int offerno)
        {
            if(db.CustomerOffers.Where(c => c.CustomerName == User.Identity.Name && c.Offer == offerno).Count() > 0)
            {
                return true;
            }
            return false;
        }

        public int countPaidCarts()
        {
            return db.Carts.Where(c => c.CustomerName == User.Identity.Name && c.Status == "paid").Count();
        }

        //is the customer eligible for the lucky draw offer
        public Boolean isWinner()
        {

            if (db.Winners.Where(w => w.CustomerName == User.Identity.Name && w.Status != "done").Count() != 0)
            {
                return true;
            }
            else
                return false;
        }
    }
}



