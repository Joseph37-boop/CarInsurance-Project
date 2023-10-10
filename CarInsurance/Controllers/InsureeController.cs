using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using CarInsurance.Models;

namespace CarInsurance.Controllers
{
    public class InsureeController : Controller
    {
        private InsuranceEntities db = new InsuranceEntities();

        // GET: Insuree
        public ActionResult Index()
        {
            return View(db.Tables.ToList());
        }

        // GET: Insuree/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Table table = db.Tables.Find(id);
            if (table == null)
            {
                return HttpNotFound();
            }
            return View(table);
        }

        // GET: Insuree/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: Insuree/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Table table)
        {
            if (ModelState.IsValid)
            {
                db.Tables.Add(table);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            return View(table);
        }

        // GET: Insuree/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Table table = db.Tables.Find(id);
            if (table == null)
            {
                return HttpNotFound();
            }
            return View(table);
        }

        // POST: Insuree/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id,FirstName,LastName,EmailAddress,DateOfBirth,CarYear,CarMake,CarModel,DUI,SpeedingTickets,CoverageType,Quote")] Table table)
        {
            if (ModelState.IsValid)
            {
                db.Entry(table).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            return View(table);
        }

        // GET: Insuree/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Table table = db.Tables.Find(id);
            if (table == null)
            {
                return HttpNotFound();
            }
            return View(table);
        }

        // POST: Insuree/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Table table = db.Tables.Find(id);
            db.Tables.Remove(table);
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

        public ActionResult CalculateQuote(InsureeViewModel model)
        {
            decimal baseQuote = 50.0m;

            // Age logic
            if (model.Age <= 18)
            {
                baseQuote += 100.0m;
            }
            else if (model.Age >= 19 && model.Age <= 25)
            {
                baseQuote += 50.0m;
            }
            else
            {
                baseQuote += 25.0m;
            }

            // Car year logic
            if (model.CarYear < 2000)
            {
                baseQuote += 25.0m;
            }
            else if (model.CarYear > 2015)
            {
                baseQuote += 25.0m;
            }

            // Car make and model logic
            if (model.CarMake == "Porsche")
            {
                baseQuote += 25.0m;

                if (model.CarModel == "911 Carrera")
                {
                    baseQuote += 25.0m;
                }
            }

            // Speeding ticket and DUI logic
            baseQuote += model.SpeedingTickets * 10.0m;

            if (model.HasDUI)
            {
                baseQuote *= 1.25m; // Add 25% for DUI
            }

            // Full coverage logic
            if (model.IsFullCoverage)
            {
                baseQuote *= 1.5m; // Add 50% for full coverage
            }

            

            // Create a new Insuree instance and save it to the database
            Insuree insuree = new Insuree
            {
                FirstName = model.FirstName,
                LastName = model.LastName,
                Email = model.Email,
                Quote = baseQuote
            };

            using (InsuranceEntities db = new InsuranceEntities())
            {
                db.Insuree.Add(insuree);
                db.SaveChanges();
            }

            // Redirect to a success page or display a confirmation message
            return RedirectToAction("QuoteConfirmation");
        }
    }

   

}
