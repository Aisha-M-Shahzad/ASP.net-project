using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarFinalProject.Models;
namespace CarF.Controllers
{
    [Authorize]
    public class TransactionController : Controller
    {
        private CarDBEntities db = new CarDBEntities();
        // GET: Transaction
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Buy(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Product product = db.Products.Find(id);
            // did we find a product with that id?
            if (product == null)
            {
                return HttpNotFound();
            }// get unpaid Cart belonging to Customer with that username
            Cart cart = GetUsersCart();
            // search CartItems for existing CartItem forthat cart and product
            try
            {
                // if found, increment quantity
                CartItem cartitem = db.CartItems.First(c => (c.CartId == cart.Id && c.ProductId == product.Id));
                cartitem.Quantity++;
            }
            catch (Exception)
            {// if not found, create it and add it
                CartItem cartitem = new CartItem { ProductId = product.Id, CartId = cart.Id, Quantity = 1 };
                db.CartItems.Add(cartitem);
            }
            db.SaveChanges();
            // send user to Cart detail view
            return RedirectToAction("Details", "Cart", new { id = cart.Id });
        }

        private Cart GetUsersCart()
        {// 1 --get logged-in user'susername
            string username = User.Identity.Name;
            try
            {
                // 2 --do we have a customer with that username?//If not found throws System.InvalidOperationException
                db.Customers.First(c => c.UserName.Equals(username));
            }
            catch (Exception)
            {// no we don't --create one (need it due to foreign key constraint)
                db.Customers.Add(new Customer { UserName = username });
                db.SaveChanges();
            }
            Cart cart = null;
            try
            {
                // 3 --do we have an unpaid Cart belonging to Customer with that username?
                cart = db.Carts.First(c => c.CustomerName.Equals(username) && c.Status.Equals("unpaid"));
            }
            catch (Exception)
            {
                // no we don't --create one
                cart = new Cart { CustomerName = username, Status = "unpaid" };
                Session["orderdone"] = null;
                db.Carts.Add(cart);
                db.SaveChanges();
            }
            return cart;
        }

        public ActionResult MyCart()
        {// get user logged-in id + cart
            Cart cart = GetUsersCart();// go to that user's shopping cart page
            return RedirectToAction("Details", "Cart", new { id = cart.Id });
        }

        public ActionResult CheckOut(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Cart cart = db.Carts.Find(id);
            if (cart.CartItems.Count() == 0)
            {
                Session["Message"] = "Your cart is empty";
                return RedirectToAction("Details", "Cart", new { id = id });
            }
            if (User.Identity.Name == cart.Customer.UserName)
            {
                cart.Status = "paid";
                db.SaveChanges();

                //an order has been placed by the customer
                Session["orderdone"] = "yes";
                //update the winner table if the customer is a lucky draw winner
                var winner = db.Winners.Where(w => w.CustomerName == User.Identity.Name && w.Status != "done").First();
                if (winner != null)
                {
                    db.Winners.Find(winner.Id).Status = "done";
                    Session["winner"] = "yes";
                    db.SaveChanges();
                }
            }

            return RedirectToAction("Details", "Cart", new { id = cart.Id });
        }
    }
}