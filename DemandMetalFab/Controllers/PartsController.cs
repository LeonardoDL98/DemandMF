using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DemandMetalFab.Models;
using ExcelDataReader;

namespace DemandMetalFab.Controllers
{
    public class PartsController : Controller
    {
        List<int> id_maquina = new List<int>();
        List<int> id_maquinahoja1 = new List<int>();
        List<string> num_parthoja1 = new List<string>();
        int columnahoja1 = 1, columnahoja2 = 1;
        private DemandDBEntities db = new DemandDBEntities();

        // GET: Parts
        public ActionResult Index()
        {
            var part = db.MF_Part.Include(p => p.MF_Machine).Include(p => p.MF_Project);
            return View(part.ToList());
        }

        // GET: Parts/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MF_Part part = db.MF_Part.Find(id);
            if (part == null)
            {
                return HttpNotFound();
            }
            return View(part);
        }

        // GET: Parts/Create
        public ActionResult Create()
        {
            ViewBag.Id_Machine = new SelectList(db.MF_Machine, "Id_Machine", "Machine");
            ViewBag.Id_Project = new SelectList(db.MF_Project, "Id_Project", "Project");
            return View();
        }

        // POST: Parts/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create([Bind(Include = "Id_Part,Num_Part,Id_Project,Id_Machine")] MF_Part part)
        {
            if (ModelState.IsValid)
            {
                db.MF_Part.Add(part);
                db.SaveChanges();
                return RedirectToAction("Index");
            }

            ViewBag.Id_Machine = new SelectList(db.MF_Machine, "Id_Machine", "Machine", part.Id_Machine);
            ViewBag.Id_Project = new SelectList(db.MF_Project, "Id_Project", "Project", part.Id_Project);
            return View(part);
        }

        // GET: Parts/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MF_Part part = db.MF_Part.Find(id);
            List<MF_OpMachine> op = new List<MF_OpMachine>();
            op = db.MF_OpMachine.Where(x => x.Id_Part == id).ToList();    
                

            if (part == null)
            {
                return HttpNotFound();
            }
            ViewBag.Id_machine = new SelectList(op, "Machine.Id_Machine", "Machine.Machine", part.Id_Machine);
            ViewBag.Id_Machines = new SelectList(db.MF_Machine, "Id_Machine", "Machine", part.Id_Machine);
            ViewBag.Id_Project = new SelectList(db.MF_Project, "Id_Project", "Project", part.Id_Project);
            return View(part);
        }

        // POST: Parts/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see https://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include = "Id_Part,Num_Part,Id_Project,Id_Machine")] MF_Part part)
        {
            if (ModelState.IsValid)
            {
                db.Entry(part).State = EntityState.Modified;
                db.SaveChanges();
                return RedirectToAction("Index");
            }
            ViewBag.Id_Machine = new SelectList(db.MF_Machine, "Id_Machine", "Machine", part.Id_Machine);
            ViewBag.Id_Project = new SelectList(db.MF_Project, "Id_Project", "Projec1", part.Id_Project);
            return View(part);
        }

        // GET: Parts/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            MF_Part part = db.MF_Part.Find(id);
            if (part == null)
            {
                return HttpNotFound();
            }
            return View(part);
        }

        // POST: Parts/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            MF_Part part = db.MF_Part.Find(id);
            db.MF_Part.Remove(part);
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
        public ActionResult Asignacion()
        {
            return View();
        }
        public PartialViewResult FiltroAsignacionParcial()
        {
            return PartialView();
        }
        public PartialViewResult GridAsignacionParcial(string maquinas)
        {
            List<MF_Part> Asignacion_maquinas = new List<MF_Part>();
            try
            {
                List<Int32> Ids_maquinas = new List<int>();

                if (!String.IsNullOrEmpty(maquinas))
                    Ids_maquinas = maquinas.Split(',').ToList().Select(Int32.Parse).ToList();
                else
                    Ids_maquinas = db.MF_Machine.Select(x => x.Id_Machine).ToList();
                //Obtenemos el filtro de los datos seleccionados y mostramos

                Asignacion_maquinas = db.MF_Part
                    .Where(x =>Ids_maquinas.Contains(x.MF_Machine.Id_Machine) && x.Id_Proceso==Datos.proceso).Distinct().ToList();



            }
            catch (Exception e)
            {

            }

            return PartialView(Asignacion_maquinas);
        }
        [HttpPost]
        public JsonResult agregar(int idparte, string idmaquina)
        {
            List<int> id_maquina = idmaquina.Split(',').ToList().Select(Int32.Parse).ToList();
            
            List<MF_OpMachine> opcion= id_maquina.Select(x=>new MF_OpMachine() {Id_Part= x, Id_Machine=x }).ToList();
            db.MF_OpMachine.AddRange(opcion);
            return Json(new { },JsonRequestBehavior.DenyGet);
        }

        public PartialViewResult DropdownMachineParcial()
        {

            return PartialView();
        }

        public JsonResult Actualizarpartes(string Idparte, string Idmaquina)
        {
            MF_Part partes = db.MF_Part.Find(Int32.Parse(Idparte));
            partes.Id_Machine = Int32.Parse(Idmaquina);
            db.SaveChanges();
            return Json(new { actualizar = true }, JsonRequestBehavior.DenyGet);
           
        }
        public JsonResult AddeleOpmachine(string Idparte, string Idmaquina, int opc)
        {
            int idp, idm;
            MF_OpMachine OpMaquina;
            idp = Int32.Parse(Idparte);
            idm = Int32.Parse(Idmaquina);
            switch (opc)
            {
                case 1:
                    OpMaquina = new MF_OpMachine()
                    {
                        Id_Part = idp,
                        Id_Machine = idm
                    };
                    db.MF_OpMachine.Add(OpMaquina);
                    db.SaveChanges();
                    break;
                case 2:
                    OpMaquina = db.MF_OpMachine.Where(x => x.Id_Part == idp && x.Id_Machine == idm).First();
                    db.MF_OpMachine.Remove(OpMaquina);
                    db.SaveChanges();
                    break;
            }
            return Json(new { actualizar = true }, JsonRequestBehavior.DenyGet);
        }


        //vistas time machine
        [HttpPost]
        public JsonResult guardarDatos(string campo, string valoractual, string tipo, int id)
        {
            MF_Part timemachine = db.MF_Part.Find(id);
            decimal valordecimal = 0;
            int valorint = 0;
            switch (tipo)
            {
                case "decimal":
                    valordecimal = Decimal.Parse(valoractual);
                    break;
                case "int":
                    valorint = Int32.Parse(valoractual);
                    break;

            }
            switch (campo)
            {

                case "Setup":
                    timemachine.Set_Up = valordecimal;
                    break;
                case "Cycle":
                    timemachine.Cycle = valordecimal;
                    break;
                case "Qty":
                    timemachine.Quantity = valorint;
                    break;
            }
            db.SaveChanges();
            return Json(new { actualizar = true }, JsonRequestBehavior.DenyGet);
        }
        [HttpPost]
        public JsonResult guardarmaquina(int idparte, int idmaquina)
        {
            try
            {
                MF_Part partes = db.MF_Part.Find(idparte);
                partes.Id_Machine = idmaquina;
                db.SaveChanges();
                return Json(new { actualizar = true }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {

                return Json(new { actualizar = false }, JsonRequestBehavior.DenyGet);
            }
        }
        public ActionResult TimeMachinePart()
        {
            
            //  try
            //  {
            //      TempData["UserMessage"] = new AlertValidation() { CssClassName = "alert-success", Title = "Success!", Message = "Operation Done." };
            //  //    return RedirectToAction("Success");
            //  }
            //  catch (Exception e)
            //  {
            //      TempData["UserMessage"] = new AlertValidation() { CssClassName = "alert-error", Title = "Error!", Message = "Operation Failed." };
            ////      return RedirectToAction("Error");
            //  }

            return View();
        }

        public PartialViewResult FiltroTimeParcial()
        {
            return PartialView();
        }

        public PartialViewResult GridTimeParcial(string clientes, string proyectos, string maquinas)
        {
            List<MF_Part> matriz_tiempos = new List<MF_Part>();
            try
            {
                List<Int32> Ids_clientes = new List<int>();
                List<Int32> Ids_proyectos = new List<int>();
                List<Int32> Ids_maquinas = new List<int>();
                //Checamos si esta vacio o no la variable clientes
                if (!String.IsNullOrEmpty(clientes))
                    Ids_clientes = clientes.Split(',').ToList().Select(Int32.Parse).ToList();
                else
                    Ids_clientes = db.MF_Customer.Select(x => x.Id_Customer).ToList();
                if (!String.IsNullOrEmpty(proyectos))
                    Ids_proyectos = proyectos.Split(',').ToList().Select(Int32.Parse).ToList();
                else
                    Ids_proyectos = db.MF_Project.Select(x => x.Id_Project).ToList();
                if (!String.IsNullOrEmpty(maquinas))
                    Ids_maquinas = maquinas.Split(',').ToList().Select(Int32.Parse).ToList();
                else
                    Ids_maquinas = db.MF_Machine.Select(x => x.Id_Machine).ToList();
                //Obtenemos el filtro de los datos seleccionados y mostramos

                matriz_tiempos = db.MF_Part
                    .Where(x =>
                           Ids_clientes.Contains(x.MF_Project.MF_Demand.MF_Customer.Id_Customer)
                           && Ids_proyectos.Contains(x.MF_Project.Id_Project)
                           && Ids_maquinas.Contains(x.MF_Machine.Id_Machine)
                           && x.Id_Proceso==Datos.proceso).Distinct().ToList();



            }
            catch (Exception e)
            {

            }

            return PartialView(matriz_tiempos);
        }
        public PartialViewResult AddTime()
        {
            ViewBag.Id_Customer = new SelectList(db.MF_Customer, "Id_Customer", "Customer");
            return PartialView();
        }

        [HttpPost]
        public JsonResult AgregarParte(TimeMachineParts tmaquina)
        {
            int item;
            using (DbContextTransaction dbTran = db.Database.BeginTransaction())
            {
                MF_Part partes = new MF_Part();
                MF_OpMachine opmachine = new MF_OpMachine();

                try
                {
                    item = (int)db.MF_Part.Where(x => x.Id_Proceso == Datos.proceso).OrderByDescending(x => x.Id_Part).First().Item + 1;
                    tmaquina.partes.Item = item;
                    tmaquina.partes.Id_Proceso = Datos.proceso;
                    db.MF_Part.Add(tmaquina.partes);
                    db.SaveChanges();
                    dbTran.Commit();
                    var id = db.MF_Part.Where(x => x.Num_Part == tmaquina.partes.Num_Part).First();
                    List<int> id_maquina = tmaquina.opmaquina.Split(',').ToList().Select(Int32.Parse).ToList();
                    List<MF_OpMachine> opcion = id_maquina.Select(x => new MF_OpMachine() { Id_Part = id.Id_Part,Item=item, Id_Machine = x,Id_Proceso=Datos.proceso }).ToList();
                    db.MF_OpMachine.AddRange(opcion);

                    db.SaveChanges();
                    return Json(new { Success = true, Message = "Part successfully added" }, JsonRequestBehavior.DenyGet);


                }
                catch (Exception e)
                {
                    return Json(new { Success = false, Message = "Error: Part could not be added" }, JsonRequestBehavior.DenyGet);
                }


            }
            

        }
        public PartialViewResult DemandshowPartial(string id, int? idparte)
        {

            if (!(String.IsNullOrEmpty(id) || id == "null"))
            {
                int cons = Int32.Parse(id);
                List<MF_Project> projec = db.MF_Project.Where(x => x.MF_Demand.Id_Customer == cons).ToList();
                if (idparte == null)
                {
                    ViewBag.Id_Project = new SelectList(projec, "Id_Project", "Project");
                }
                else
                {
                    MF_Part part = db.MF_Part.Find(idparte);
                    ViewBag.Id_Project = new SelectList(projec, "Id_Project", "Project", part.Id_Project);
                }
            }
            else
                ViewBag.Id_Project = new SelectList("", "Id_Project", "Project");



            return PartialView();
        }


        public PartialViewResult MachineshowPartial(string id, int? idparte)
        {
            List<Int32> Ids_maquinas = new List<int>();
            List<MF_Machine> maquina;

            if (idparte == null)
            {
                if (!(String.IsNullOrEmpty(id) || id == "null"))
                    Ids_maquinas = id.Split(',').ToList().Select(Int32.Parse).ToList();
                else
                    ViewBag.Id_Machine = new SelectList("", "Id_Machine", "Machine");
                maquina = db.MF_Machine.Where(x => Ids_maquinas.Contains(x.Id_Machine)).ToList();
                ViewBag.Id_Machine = new SelectList(maquina, "Id_Machine", "Machine");
            }
            else
            {
                List<MF_OpMachine> opciones = db.MF_OpMachine.Where(x => x.Id_Part == idparte).ToList();
                MF_Part part = db.MF_Part.Find(idparte);
                ViewBag.Id_Machine = new SelectList(opciones, "MF_Machine.Id_Machine", "MF_Machine.Machine", part.Id_Machine);
            }

            return PartialView();
        }

        public PartialViewResult EditMachinePartial(int id)
        {
            MF_Part timeMachine = db.MF_Part.Find(id);

            ViewBag.Id_Customer = new SelectList(db.MF_Customer, "Id_Customer", "Customer", timeMachine.MF_Project.MF_Demand.Id_Customer);

            return PartialView(timeMachine);
        }
        [HttpPost]
        public JsonResult EditarParte(TimeMachineParts tmaquina)
        {
            using (DbContextTransaction dbTran = db.Database.BeginTransaction())
            {
                MF_Part partes = new MF_Part();
                MF_OpMachine opmachine = new MF_OpMachine();

                try
                {
                    partes = db.MF_Part.Where(x => x.Id_Part == tmaquina.partes.Id_Part).First();
                    partes.Num_Part = tmaquina.partes.Num_Part;
                    partes.Id_Project = tmaquina.partes.Id_Project;
                    partes.Id_Machine = tmaquina.partes.Id_Machine;
                    partes.Set_Up = tmaquina.partes.Set_Up;
                    partes.Cycle = tmaquina.partes.Cycle;
                    partes.Quantity = tmaquina.partes.Quantity;
                    dbTran.Commit();
                    //Eliminamos las maquinas de la parte

                    db.Database.ExecuteSqlCommand("EXEC MF_DeleteOpMachine {0}", tmaquina.partes.Id_Part);
                    //
                    List<int> id_maquina = tmaquina.opmaquina.Split(',').ToList().Select(Int32.Parse).ToList();
                    List<MF_OpMachine> opcion = id_maquina.Select(x => new MF_OpMachine() { Id_Part = tmaquina.partes.Id_Part,Item=partes.Item, Id_Machine = x,Id_Proceso=Datos.proceso }).ToList();
                    db.MF_OpMachine.AddRange(opcion);
                    db.SaveChanges();
                    return Json(new { Success = true, Message = "Part successfully edited" }, JsonRequestBehavior.DenyGet);
                }
                catch (Exception e)
                {

                    return Json(new { Success = false, Message = "Part could not be edited" }, JsonRequestBehavior.DenyGet);
                }
            }
        }

        public PartialViewResult DeletePart(int id)
        {
            MF_Part timemachine = db.MF_Part.Find(id);
            return PartialView(timemachine);
        }
        [HttpPost]
        public JsonResult EliminarParte(int id)
        {
            try
            {
                db.Database.ExecuteSqlCommand("EXEC MF_DeletePart {0}", id);
                return Json(new { Success = true, Message = "The part was successfully removed" }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception e)
            {
                return Json(new { Success = false, Message = "The part could not be removed" }, JsonRequestBehavior.DenyGet);
            }
        }
        public PartialViewResult DroProjectPartial(string clientes)
        {
            List<Int32> Ids_clientes = new List<int>();
            List<MF_Project> proyect;

            if (!(String.IsNullOrEmpty(clientes) || clientes == "null"))
                Ids_clientes = clientes.Split(',').ToList().Select(Int32.Parse).ToList();
            else
                Ids_clientes = db.MF_Customer.Select(x => x.Id_Customer).ToList();
            proyect = db.MF_Project.Where(x => Ids_clientes.Contains(x.MF_Demand.Id_Customer) && x.Id_Proceso==Datos.proceso).Distinct().ToList();

            ViewBag.droproyect = proyect;
            return PartialView();
        }


    }
}
