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
    public class TestDrivesController : Controller
    {
        private CarDBEntities db = new CarDBEntities();

        // GET: TestDrives

        public ActionResult Index()
        {
            var testDrives = from t in db.TestDrives
                             select t;
            if (User.Identity.Name != "admin@admin.com")
            {
                testDrives = db.TestDrives.Where(t => t.CustomerName == User.Identity.Name);
            }

            return View(testDrives.ToList());
        }

        // GET: TestDrives/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestDrive testDrive = db.TestDrives.Find(id);
            if (testDrive == null)
            {
                return HttpNotFound();
            }
            return View(testDrive);
        }

        // GET: TestDrives/Create
        public ActionResult Create()
        {
            ViewBag.CustomerName = new SelectList(db.Customers, "UserName", "Name");
            ViewBag.CarId = new SelectList(db.Products, "Id", "Name");
            return View();
        }

        // POST: TestDrives/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CustomerName,CarId,BookDate,TestDate")] TestDrive testDrive)
        {

            ViewBag.CustomerName = new SelectList(db.Customers, "UserName", "Name", testDrive.CustomerName);
            ViewBag.CarId = new SelectList(db.Products, "Id", "Name", testDrive.CarId);
            var totaldrives = db.TestDrives.Where(t => t.TestDate == testDrive.TestDate);
            //the customer must select a future date to book a test drive
            if (Convert.ToDateTime(testDrive.TestDate) <= Convert.ToDateTime(testDrive.BookDate))
            {
                Session["test-error"] = "The date is invalid. Please select a future date";
                return RedirectToAction("Create");
            }
            else
            {
                //one customer can only book one test drive per day
                if (db.TestDrives.Where(t => t.CustomerName == User.Identity.Name && t.TestDate == testDrive.TestDate).Count() > 0)
                {
                    Session["test-error"] = "You can only book one test drive per day.";
                    return RedirectToAction("Create");
                }
                //allows 10 test booking drives per day
                else if (totaldrives.Count() > 10)
                {
                    Session["test-error"] = "Please choose another day. This day is fully booked.";
                    return RedirectToAction("Create");
                }
                else
                {
                    db.TestDrives.Add(testDrive);
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
        }

        // GET: TestDrives/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestDrive testDrive = db.TestDrives.Find(id);
            if (testDrive == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerName = new SelectList(db.Customers, "UserName", "Name", testDrive.CustomerName);
            ViewBag.CarId = new SelectList(db.Products, "Id", "Name", testDrive.CarId);
            return View(testDrive);
        }

        // POST: TestDrives/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CustomerName,CarId,BookDate,TestDate")] TestDrive testDrive)
        {
            ViewBag.CustomerName = new SelectList(db.Customers, "UserName", "Name", testDrive.CustomerName);
            ViewBag.CarId = new SelectList(db.Products, "Id", "Name", testDrive.CarId);
            var prevTestDrive = db.TestDrives.Where(t => t.CustomerName == testDrive.CustomerName && t.TestDate == testDrive.TestDate);
            var totaldrives = db.TestDrives.Where(t => t.TestDate == testDrive.TestDate);
            //the customer has to select future date to book a test drive
            if (Convert.ToDateTime(testDrive.TestDate) <= Convert.ToDateTime(testDrive.BookDate))
            {
                Session["test-error"] = "The date is invalid. Please select a future date";
                return RedirectToAction("Edit");
            }
            else
            {
                //one customer can book one test drive per day
                if (prevTestDrive.Count() >= 1)
                {
                    Session["test-error"] = "You can only book one test drive per day.";
                    return RedirectToAction("Edit");
                }
                //allows 10 test booking drives per day
                else if (totaldrives.Count() > 10)
                {
                    Session["test-error"] = "Please choose another day. This day is fully booked.";
                    return RedirectToAction("Edit");
                }
                else
                {
                    db.Entry(testDrive).State = EntityState.Modified;
                    db.SaveChanges();
                    return RedirectToAction("Index");
                }
            }
        }

        // GET: TestDrives/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            TestDrive testDrive = db.TestDrives.Find(id);
            if (testDrive == null)
            {
                return HttpNotFound();
            }
            return View(testDrive);
        }

        // POST: TestDrives/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            TestDrive testDrive = db.TestDrives.Find(id);
            db.TestDrives.Remove(testDrive);
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
