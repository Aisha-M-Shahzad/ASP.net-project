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
    public class ProductController : Controller
    {
        private CarDBEntities db = new CarDBEntities();

        // GET: Product
        public ActionResult Index(String productName, String brandName, String amount)
        {
            var cars = from c in db.Products
                       select c;
            if (!String.IsNullOrEmpty(productName))
            {
                cars = cars.Where(e => e.Name.Contains(productName));
            }
            var list = new List<String> { "All", "Ford", "BMW", "Porsche", "Mercedes", "Nissan", "Ferrari" };
            ViewBag.brandName = new SelectList(list);
            if(!String.IsNullOrEmpty(brandName) && brandName != "All")
            {
                cars = cars.Where(c => c.Category.Name == brandName);
            }
            if (!String.IsNullOrEmpty(amount))
            {
                if (amount == "200000")
                {
                    int amnt = int.Parse(amount);
                    cars = cars.Where(c => c.Price <= amnt);
                }
                if (amount == "200001")
                {
                    int amnt = int.Parse(amount);
                    cars = cars.Where(c => c.Price >= amnt);
                }
            }
            return View(cars.ToList());
        }

        // GET: Product/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // GET: Product/Create
        [Authorize(Users = "admin@admin.com")]
        public ActionResult Create()
        {
            return View();
        }

        // POST: Product/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Users = "admin@admin.com")]

        public ActionResult Create([Bind(Include = "Id,Name,Price")] Product product)
        {
            var products = from p in db.Products
                           select p;
            products = products.Where(p => p.Name.Equals(product.Name));
            
            if(products.Count() != 0)
            {
                ViewBag.error = "Cannot Create Duplicate Records.";
            }
            else
            {
                db.Products.Add(product);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(product);
        }

        // GET: Product/Edit/5
        [Authorize(Users = "admin@admin.com")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Product/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,Name,Price")] Product product)
        {
            if (ModelState.IsValid)
            {

                if (db.Products.Where(p => p.Name == product.Name).Count() > 0)
                {
                    Session["saved-error"] = "This car name is already saved.";
                    var pr = db.Products.Find(product.Id);
                    pr.Price = product.Price;
                    db.Entry(pr).Property("Price").IsModified = true;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
                else
                {
                    db.Entry(product).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
            return RedirectToAction("Index");
        }

        // GET: Product/Delete/5
        [Authorize(Users = "admin@admin.com")]

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            if (product == null)
            {
                return HttpNotFound();
            }
            return View(product);
        }

        // POST: Product/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Product product = db.Products.Find(id);
            if (product.CartItems.Count() > 0)
            {
                Session["message"] = "You cannot delete this item.";
                return RedirectToAction("Delete");
            }
            else
            {
                db.Products.Remove(product);
                db.SaveChanges();
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

        //gets the popular product based on how many times the product was purchased
        public ActionResult PopularProduct()
        {
            int maxQuantity = 0;
            Product maxProduct = null;
            foreach(Product pr in db.Products)
            {
                if(db.CartItems.Where(c => c.ProductId == pr.Id).Count() > maxQuantity)
                {
                    maxQuantity = db.CartItems.Where(c => c.ProductId == pr.Id).Count();
                    maxProduct = db.Products.Find(pr.Id);
                }
            }
            return View(maxProduct);
        }
    }
}
