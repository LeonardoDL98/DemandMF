using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DemandMetalFab.Models;

namespace DemandMetalFab.Controllers
{
    public class MachineController : Controller
    {
        private DemandDBEntities db = new DemandDBEntities();

        // GET: Machine
        public ActionResult Index()
        {
            var mF_Machine = db.MF_Machine.Include(m => m.MF_Sector).Include(m => m.MF_Proceso).Where(x => x.Id_Proceso == Datos.proceso).ToList();
            return View(mF_Machine);
        }

        public PartialViewResult AddMachinePartial()
        {
            ViewBag.Id_Sector = new SelectList(db.MF_Sector, "Id_Sector", "Sector");
            return PartialView();
        }
        [HttpPost]
        public JsonResult AgregarMachine(string machine, int idsector)
        {
            int item;
            try
            {
                item = (int)db.MF_Machine.Where(x => x.Id_Proceso == Datos.proceso).OrderByDescending(x => x.Id_Machine).First().Item;
                MF_Machine mac = new MF_Machine()
                {
                    Item = item,
                    Machine = machine,
                    Id_Sector = idsector,
                    Id_Proceso = Datos.proceso
                };
                db.MF_Machine.Add(mac);
                db.SaveChanges();
                return Json(new { Success = true, Message = "The machine was successfully added" }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception e)
            {
                return Json(new { Success = false, Message = "The machine could not be added" }, JsonRequestBehavior.DenyGet);
            }
        }

        public PartialViewResult EditmachinePartial(int id)
        {
            MF_Machine machine = db.MF_Machine.Find(id);
            ViewBag.Id_Sector = new SelectList(db.MF_Sector, "Id_Sector", "Sector", machine.Id_Sector);
            return PartialView(machine);
        }
        [HttpPost]
        public JsonResult EditarMachine(int id, string machine, int idsector)
        {
            try
            {
                MF_Machine mac = db.MF_Machine.Find(id);
                mac.Machine = machine;
                mac.Id_Sector = idsector;
                db.SaveChanges();
                return Json(new { Success = true, Message = "Machine successfully modified" }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception e)
            {
                return Json(new { Success = false, Message = "Error modifying the machine" }, JsonRequestBehavior.DenyGet);
            }

        }

        public PartialViewResult DeletemachinePartial(int id)
        {
            MF_Machine machine = db.MF_Machine.Find(id);
            return PartialView(machine);
        }

        [HttpPost]
        public JsonResult EliminarMachine(int id)
        {
            try
            {
                MF_Machine machine = db.MF_Machine.Find(id);
                db.MF_Machine.Remove(machine);
                db.SaveChanges();
                return Json(new { Success = true, Message = "The machine was successfully removed" }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(new { Success = false, Message = "Error deleting machine" }, JsonRequestBehavior.DenyGet);
            }
        }
    }

}
