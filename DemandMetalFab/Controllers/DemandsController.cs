using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DemandMetalFab.Models;
using Newtonsoft.Json;

namespace DemandMetalFab.Controllers
{
    public class DemandsController : Controller
    {
        
        private DemandDBEntities db = new DemandDBEntities();

        // GET: Demands
        public ActionResult Index()
        {
            var demand = db.MF_Demand.Include(d => d.MF_Customer);
            return View(demand.ToList());
        }

        // GET: Demands/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MF_Demand demand = db.MF_Demand.Find(id);
            if (demand == null)
            {
                return HttpNotFound();
            }
            return View(demand);
        }

        // GET: Demands/Create
        public ActionResult Create()
        {
            ViewBag.Id_Customer = new SelectList(db.MF_Customer, "Id_Customer", "Customer");
            return View();
        }

        // POST: Demands/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_Demand,Demand,Id_Customer")] MF_Demand demand)
        {
            if (ModelState.IsValid)
            {
                db.MF_Demand.Add(demand);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id_Customer = new SelectList(db.MF_Customer, "Id_Customer", "Customer", demand.Id_Customer);
            return View(demand);
        }

        // GET: Demands/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MF_Demand demand = db.MF_Demand.Find(id);
            if (demand == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_Customer = new SelectList(db.MF_Customer, "Id_Customer", "Customer", demand.Id_Customer);
            return View(demand);
        }

        // POST: Demands/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_Demand,Demand,Id_Customer")] MF_Demand demand)
        {
            if (ModelState.IsValid)
            {
                db.Entry(demand).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_Customer = new SelectList(db.MF_Customer, "Id_Customer", "Customer", demand.Id_Customer);
            return View(demand);
        }

        // GET: Demands/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MF_Demand demand = db.MF_Demand.Find(id);
            if (demand == null)
            {
                return HttpNotFound();
            }
            return View(demand);
        }

        // POST: Demands/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MF_Demand demand = db.MF_Demand.Find(id);
            db.MF_Demand.Remove(demand);
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
        public PartialViewResult addemandPartial()
        {
            ViewBag.Id_Customer = new SelectList(db.MF_Customer, "Id_Customer", "Customer");
            ViewBag.Id_Sector = new SelectList(db.MF_Sector, "Id_Sector", "Sector");
            return PartialView();
        }
        [HttpPost]
        public JsonResult agregardemanda(string demand, int customer, int sector)
        {
            int item;
            try
            {
                item = (int)db.MF_Demand.OrderByDescending(x => x.Id_Demand).First().Item + 1;
                MF_Demand dem = new MF_Demand()
                {
                    Item=item,
                    Demand = demand,
                    Id_Customer = customer,
                    Id_Sector = sector
                };
                db.MF_Demand.Add(dem);
                db.SaveChanges();
                return Json(new { Success = true, Message = "The demand was successfully added" }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception e)
            {
                return Json(new { Success = false, Message = "The demand could not be added" }, JsonRequestBehavior.DenyGet);
            }
        }
        public PartialViewResult editdemandPartial(int id)
        {
            MF_Demand dem = db.MF_Demand.Find(id);
            ViewBag.Id_Customer = new SelectList(db.MF_Customer, "Id_Customer", "Customer",dem.Id_Customer);
            ViewBag.Id_Sector = new SelectList(db.MF_Sector, "Id_Sector", "Sector",dem.Id_Sector);
            return PartialView(dem);
        }
        [HttpPost]
        public JsonResult editardemanda(int id, string demand, int customer, int sector)
        {
            try
            {
                MF_Demand dem = db.MF_Demand.Find(id);
                dem.Demand = demand;
                dem.Id_Customer = customer;
                dem.Id_Sector = sector;
                
                db.SaveChanges();
                return Json(new { Success = true, Message = "Demand successfully modified" }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception e)
            {
                return Json(new { Success = false, Message = "Error modifying the demand" }, JsonRequestBehavior.DenyGet);
            }
        }
        
        public PartialViewResult deletedemandPartial(int id)
        {
            MF_Demand dem = db.MF_Demand.Find(id);
            return PartialView(dem);
        }
        [HttpPost]
        public JsonResult eliminardemand(int id)
        {
            try
            {
                MF_Demand dem = db.MF_Demand.Find(id);
                db.MF_Demand.Remove(dem);
                db.SaveChanges();
                return Json(new { Success = true, Message = "The demand was successfully removed" }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(new { Success = true, Message = "Error deleting demand" }, JsonRequestBehavior.DenyGet);
            }
        }

    }
}
