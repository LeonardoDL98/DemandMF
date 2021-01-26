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
    public class ProjectsController : Controller
    {
        private DemandDBEntities db = new DemandDBEntities();

        // GET: Projects
        public ActionResult Index()
        {
            var project = db.MF_Project.Include(p => p.MF_Demand).Where(x=>x.Id_Proceso==Datos.proceso);
            return View(project.ToList());
        }

        public PartialViewResult AddprojectPartial()
        {
            ViewBag.Id_Demand = new SelectList(db.MF_Demand, "Id_Demand", "Demand");
            return PartialView();
        }
        [HttpPost]
        public JsonResult AgregarProject(string project, int demand)
        {
            int item;
            try
            {
                item = (int)db.MF_Project.Where(x => x.Id_Proceso == Datos.proceso).OrderByDescending(x => x.Id_Project).First().Item;
                MF_Project pro = new MF_Project()
                {
                    Item=item,
                    Project = project,
                    Id_Demand = demand,
                    Id_Proceso=Datos.proceso
                };
                db.MF_Project.Add(pro);
                db.SaveChanges();
                return Json(new { Success = true, Message = "The project was successfully added" }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception e)
            {
                return Json(new { Success = false, Message = "The project could not be added" }, JsonRequestBehavior.DenyGet);
            }
        }

        public PartialViewResult EditprojectPartial(int id)
        {
            MF_Project project = db.MF_Project.Find(id);
            ViewBag.Id_Demand = new SelectList(db.MF_Demand, "Id_Demand", "Demand", project.Id_Demand);
            return PartialView(project);
        }
        [HttpPost]
        public JsonResult EditarProject(int id,string project,int demand)
        {
            try
            {
                MF_Project pro = db.MF_Project.Find(id);
                pro.Project = project;
                pro.Id_Demand = demand;
                db.SaveChanges();
                return Json(new { Success = true, Message = "Project successfully modified" }, JsonRequestBehavior.DenyGet);
            }
            catch(Exception e)
            {
                return Json(new { Success = false, Message = "Error modifying the project" }, JsonRequestBehavior.DenyGet);
            }

        }

        public PartialViewResult DeleteprojectPartial(int id)
        {
            MF_Project project = db.MF_Project.Find(id);
            return PartialView(project);
        }

        [HttpPost]
        public JsonResult EliminarProject(int id)
        {
            try
            {
                MF_Project project = db.MF_Project.Find(id);
                db.MF_Project.Remove(project);
                db.SaveChanges();
                return Json(new { Success = true, Message = "The project was successfully removed" }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(new { Success = true, Message = "Error deleting project" }, JsonRequestBehavior.DenyGet);
            }
        }
        
    }
}
