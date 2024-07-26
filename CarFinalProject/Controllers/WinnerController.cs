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
    public class WinnerController : Controller
    {
        private CarDBEntities db = new CarDBEntities();

        // GET: Winner
        public ActionResult Index()
        {
            //get the winner of the most recent lucky draw
            var winners = from w in db.Winners
                          orderby w.Id descending
                          select w;
            Winner winner = winners.First();
            //if the lucky draw has been started and the results are not out yet,
            //display results are not yet announced
            if((db.CustomerOffers.Where(c => c.Offer == 3)).Count() > 0)
            {
                Session["winner"] = "Results are not yet announced.";
                winner = new Winner(); 
                return View(winner);
            }
            else
            {
                Session["winner"] = null;
                if (User.Identity.Name == "admin@admin.com")
                {
                    return View(winner);
                }
                else
                {
                    if (winner.CustomerName == User.Identity.Name && winner.Status != "done")
                    {
                        Session["winner"] = "Congratulations! You are the lucky draw winner";
                        return View(winner);
                    }
                    //incase if the customer didn't win the lucky draw.
                    else
                    {
                        if (User.Identity.Name != null)
                        {
                            Session["winner"] = "We encourage you to take part in the next lucky draw.";
                            winner = new Winner();
                            return View(winner);
                        }
                        else
                        {
                            Session["winner"] = "Please login to take part in our lucky.";
                            winner = new Winner();
                            return View(winner);
                        }
                    }
                }
            }
        }

        // GET: Winner/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Winner winner = db.Winners.Find(id);
            if (winner == null)
            {
                return HttpNotFound();
            }
            return View(winner);
        }

        // GET: Winner/Create
        [Authorize(Users ="nobody")]
        public ActionResult Create()
        {
            ViewBag.CustomerName = new SelectList(db.Customers, "UserName", "Name");
            return View();
        }

        // POST: Winner/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,CustomerName")] Winner winner)
        {
            if (ModelState.IsValid)
            {
                db.Winners.Add(winner);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.CustomerName = new SelectList(db.Customers, "UserName", "Name", winner.CustomerName);
            return View(winner);
        }

        // GET: Winner/Edit/5
        [Authorize(Users ="nobody")]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Winner winner = db.Winners.Find(id);
            if (winner == null)
            {
                return HttpNotFound();
            }
            ViewBag.CustomerName = new SelectList(db.Customers, "UserName", "Name", winner.CustomerName);
            return View(winner);
        }

        // POST: Winner/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,CustomerName")] Winner winner)
        {
            if (ModelState.IsValid)
            {
                db.Entry(winner).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.CustomerName = new SelectList(db.Customers, "UserName", "Name", winner.CustomerName);
            return View(winner);
        }

        // GET: Winner/Delete/5
        [Authorize(Users ="nobody")]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Winner winner = db.Winners.Find(id);
            if (winner == null)
            {
                return HttpNotFound();
            }
            return View(winner);
        }

        // POST: Winner/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Winner winner = db.Winners.Find(id);
            db.Winners.Remove(winner);
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

        public ActionResult Results()
        {
            var winner = from w in db.Winners
                         select w;
            //get the winner of the most recent lucky draw
            Winner win = winner.Last();
            return View(win);
        }
    }
}
