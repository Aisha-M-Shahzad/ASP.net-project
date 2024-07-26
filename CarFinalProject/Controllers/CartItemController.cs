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
    
    public class CartItemController : Controller
    {
        private CarDBEntities db = new CarDBEntities();

        // GET: CartItem
        [Authorize(Users = "admin@admin.com")]
        public ActionResult Index()
        {
            var cartItems = db.CartItems.Include(c => c.Cart).Include(c => c.Product);
            return View(cartItems.ToList());
        }

        // GET: CartItem/Details/5
        [Authorize(Users = "admin@admin.com")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CartItem cartItem = db.CartItems.Find(id);
            if (cartItem == null)
            {
                return HttpNotFound();
            }
            return View(cartItem);
        }

        // GET: CartItem/Create
        [Authorize(Users = "admin@admin.com")]
        public ActionResult Create()
        {
            ViewBag.CartId = new SelectList(db.Carts, "Id", "CustomerName");
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name");
            return View();
        }

        // POST: CartItem/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,ProductId,CartId,Quantity")] CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                db.CartItems.Add(cartItem);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CartId = new SelectList(db.Carts, "Id", "CustomerName", cartItem.CartId);
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", cartItem.ProductId);
            return View(cartItem);
        }

        // GET: CartItem/Edit/5
        [Authorize(Users = "admin@admin.com")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CartItem cartItem = db.CartItems.Find(id);
            if (cartItem == null)
            {
                return HttpNotFound();
            }
            ViewBag.CartId = new SelectList(db.Carts, "Id", "CustomerName", cartItem.CartId);
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", cartItem.ProductId);
            return View(cartItem);
        }

        // POST: CartItem/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,ProductId,CartId,Quantity")] CartItem cartItem)
        {
            if (ModelState.IsValid)
            {
                db.Entry(cartItem).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CartId = new SelectList(db.Carts, "Id", "CustomerName", cartItem.CartId);
            ViewBag.ProductId = new SelectList(db.Products, "Id", "Name", cartItem.ProductId);
            return View(cartItem);
        }

        // GET: CartItem/Delete/5
        [Authorize()]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CartItem cartItem = db.CartItems.Find(id);
            if (cartItem == null)
            {
                return HttpNotFound();
            }
            return View(cartItem);
        }

        public ActionResult DeleteItem(int? id)
        {
            if(id==null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }

            CartItem cartItem = db.CartItems.Find(id);
            if (cartItem.Cart.Status == "paid")
            {
                return RedirectToAction("Details", "Cart", new { id = cartItem.CartId });
            }
            else if (cartItem == null)
            {
                return HttpNotFound();
            }
            else
            {
                DeleteConfirmed((int)id);
                return RedirectToAction("Details", "Cart", new { id = cartItem.CartId });
            }
        }
        // POST: CartItem/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CartItem cartItem = db.CartItems.Find(id);
            db.CartItems.Remove(cartItem);
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
