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
    [Authorize(Users = "admin@admin.com")]
    public class CartAdminController : Controller
    {
        private CarDBEntities db = new CarDBEntities();

        // GET: CartAdmin
        public ActionResult Index()
        {
            var carts = db.Carts.Include(c => c.Customer);
            return View(carts.ToList());
        }

        // GET: CartAdmin/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            int? total = cart.CartItems.Sum(s => s.Quantity * s.Product.Price);
            ViewBag.total = total;
            if (cart == null)
            {
                return HttpNotFound();
            }
            return View(cart);
        }

        // GET: CartAdmin/Create
        public ActionResult Create()
        {
            ViewBag.CustomerName = new SelectList(db.Customers, "UserName", "Name");
            return View();
        }

        // POST: CartAdmin/Create
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

        // GET: CartAdmin/Edit/5
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
            ViewBag.CustomerID = new SelectList(db.Customers, "UserName", "Name", cart.CustomerName);
            return View(cart);
        }

        // POST: CartAdmin/Edit/5
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

        // GET: CartAdmin/Delete/5
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

        // POST: CartAdmin/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Cart cart = db.Carts.Find(id);
            if (cart.CartItems.Count() > 0)
            {
                Session["adminCart"] = "This cart cannot be deleted.";
                return RedirectToAction("Index");
            }
            else
            {
                db.Carts.Remove(cart);
                db.SaveChanges();
                return RedirectToAction("Index");
            }


        }

        public ActionResult AdminCrud()
        {
            return View();
        }

       public Customer GetLuckyCustomer()
       {
            var query = from c in db.CustomerOffers
                        select c;
            query = query.Where(c => c.Offer == 3);
            int count = query.Count();
            if(query.Count() > 0)
            {
                Random r = new Random();
                Customer luckyCustomer = query.ToList()[r.Next(0, count)].Customer;
                //delete all the customer names for the lucky draw 
                //this will make the current customer offer list of the lucky draw to be empty
                var offers = db.CustomerOffers.Where(c => c.Offer == 3);
                db.CustomerOffers.RemoveRange(offers);
                db.SaveChanges();
                return luckyCustomer;
            }
            else
            {
                //if there are no customers in the database table or if the customer table is empty, then just return null
                return null;
            }
       } 

        public ActionResult DisplayLuckyDraw()
        {
            var allCustomers = from c in db.CustomerOffers
                               select c;
            allCustomers = allCustomers.Where(c => c.Offer == 3);
            if(allCustomers.Count() == 0)
            {
                Session["nocust"] = "No customers";
            }
            return View(allCustomers);
        }

        public ActionResult LuckyCustomer()
        {
            Customer lucky = GetLuckyCustomer();
            //insert the lukcy customer into the winner table
            Winner winner = new Winner();
            winner.CustomerName = lucky.UserName;
            db.Winners.Add(winner);
            db.SaveChanges();
            return View(lucky);
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
