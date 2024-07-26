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
    [Authorize()]
    public class CustomerOfferController : Controller
    {
        private CarDBEntities db = new CarDBEntities();

        // GET: CustomerOffer
        [Authorize(Users = "admin@admin.com")]
        public ActionResult Index()
        {
            var customerOffers = db.CustomerOffers.Include(c => c.Customer).Include(c => c.SpecialOffer);
            return View(customerOffers.ToList());
        }

        // GET: CustomerOffer/Details/5
        [Authorize(Users = "admin@admin.com")]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerOffer customerOffer = db.CustomerOffers.Find(id);
            if (customerOffer == null)
            {
                return HttpNotFound();
            }
            return View(customerOffer);
        }

        // GET: CustomerOffer/Create
        [Authorize(Users = "admin@admin.com")]
        public ActionResult Create()
        {
            ViewBag.CustomerName = new SelectList(db.Customers, "UserName", "Name");
            ViewBag.Offer = new SelectList(db.SpecialOffers, "Id", "Name");
            return View();
        }

        // POST: CustomerOffer/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CustomerName,Offer")] CustomerOffer customerOffer)
        {
            if (ModelState.IsValid)
            {
                db.CustomerOffers.Add(customerOffer);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerName = new SelectList(db.Customers, "UserName", "Name", customerOffer.CustomerName);
            ViewBag.Offer = new SelectList(db.SpecialOffers, "Id", "Name", customerOffer.Offer);
            return View(customerOffer);
        }

        // GET: CustomerOffer/Edit/5
        [Authorize(Users = "admin@admin.com")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerOffer customerOffer = db.CustomerOffers.Find(id);
            if (customerOffer == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerName = new SelectList(db.Customers, "UserName", "Name", customerOffer.CustomerName);
            ViewBag.Offer = new SelectList(db.SpecialOffers, "Id", "Name", customerOffer.Offer);
            return View(customerOffer);
        }

        // POST: CustomerOffer/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Users = "admin@admin.com")]
        public ActionResult Edit([Bind(Include = "Id,CustomerName,Offer")] CustomerOffer customerOffer)
        {
            if (ModelState.IsValid)
            {
                db.Entry(customerOffer).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerName = new SelectList(db.Customers, "UserName", "Name", customerOffer.CustomerName);
            ViewBag.Offer = new SelectList(db.SpecialOffers, "Id", "Name", customerOffer.Offer);
            return View(customerOffer);
        }

        // GET: CustomerOffer/Delete/5
        [Authorize(Users = "admin@admin.com")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CustomerOffer customerOffer = db.CustomerOffers.Find(id);
            if (customerOffer == null)
            {
                return HttpNotFound();
            }
            return View(customerOffer);
        }

        // POST: CustomerOffer/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CustomerOffer customerOffer = db.CustomerOffers.Find(id);
            db.CustomerOffers.Remove(customerOffer);
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
        [Authorize()]
        public ActionResult AddOffer(int id)
        {
            Customer customer = db.Customers.Where(c => c.UserName == User.Identity.Name).First();
            SpecialOffer offer = db.SpecialOffers.Find(id);
            switch (id)
            {
                case 1:
                    if (countPaidCarts() == 0)
                    {
                        if (hasOffer(1))
                        {
                            Session["offer"] = "You have already added this to your account.";
                        }
                        else
                        {
                            Session["offer"] = "Offer Added to Your Account.";
                            CustomerOffer customerOffer = new CustomerOffer();
                            customerOffer.Offer = offer.Id;
                            customerOffer.CustomerName = customer.UserName;
                            db.CustomerOffers.Add(customerOffer);
                            db.SaveChanges();
                        }
                    }
                    else
                    {
                        Session["offer"] = "Sorry. This offer is valid only for first order.";
                    }
                    break;
                case 3:
                    //checks if the customer already has offer 3
                    if(hasOffer(3))
                    {
                        Session["offer"] = "You have already taken part in the lucky draw.";
                    }
                    else
                    {
                        Session["offer"] = "Thank you for taking part in the lucky draw.";
                        CustomerOffer customerOffer = new CustomerOffer();
                        customerOffer.Offer = offer.Id;
                        customerOffer.CustomerName = customer.UserName;
                        db.CustomerOffers.Add(customerOffer);
                        db.SaveChanges();
                    }
                    break;
            }
            return RedirectToAction("Index", "SpecialOffer");
        }

        //checks whether the customer is eligibile for offer no. 1
        public Boolean isOffer1()
        {
            if (countPaidCarts() == 0)
            {
                return true;
            }
            return false;
        }

        //counts the paid carts of a user
        public int countPaidCarts()
        {
            return db.Carts.Where(c => c.CustomerName == User.Identity.Name && c.Status == "paid").Count();
        }

        //checks if the customer already has a certain offer in the customeroffer table
        public Boolean hasOffer(int offerno)
        {
            if (db.CustomerOffers.Where(c => c.CustomerName == User.Identity.Name && c.Offer == offerno).Count() > 0)
            {
                return true;
            }
            return false;
        }

    }
}
