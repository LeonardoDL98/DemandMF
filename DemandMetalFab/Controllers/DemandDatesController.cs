using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Drawing;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using DemandMetalFab.Models;
using ExcelDataReader;
using Newtonsoft.Json;

namespace DemandMetalFab.Controllers
{
    public class DemandDatesController : Controller
    {
        private DemandDBEntities db = new DemandDBEntities();


        public ActionResult Index()
        {
            var demandDate = db.MF_DemandDate.Include(d => d.MF_Demand).Include(d => d.MF_WorkDate);
            return View(demandDate.ToList());
        }
        public PartialViewResult GridDemandaPartial(string inicio, string fin)
        {
 
            string[] iniciofecha = inicio.Split(' ');
            string[] finalfecha = fin.Split(' ');
            int iniciomes, inicioanio, finmes, finanio;

            iniciomes = Convert.ToInt32(iniciofecha[0]);
            inicioanio = Convert.ToInt32(iniciofecha[1]);
            finmes = Convert.ToInt32(finalfecha[0]);
            finanio = Convert.ToInt32(finalfecha[1]);

            var idini = db.MF_WorkDate.Where(x => x.Id_DemandMonth == iniciomes && x.Id_DemandYear == inicioanio).First();
            var idfin = db.MF_WorkDate.Where(x => x.Id_DemandMonth == finmes && x.Id_DemandYear == finanio).First();

            ViewBag.idini = idini.Id_WorkDate;
            ViewBag.idfin = idfin.Id_WorkDate;
            ViewBag.iniciomes = iniciomes;
            ViewBag.finmes = finmes;
            ViewBag.inicioanio = inicioanio;
            ViewBag.finanio = finanio;

            ViewBag.demandamensual = db.MF_DemandDate.Where(x => x.Id_WorkDate >= idini.Id_WorkDate && x.Id_WorkDate <= idfin.Id_WorkDate).ToList();
            return PartialView();
        }
        public PartialViewResult Grafica_MaquinaPartial(int mes, int anio, int turnoindustrial, double diasindustrial, int turnomedical, double diasmedical, int turnoprovee1, double diasprovee1, int turnoprovee2, double diasprovee2)
        {
            DataChartModel chart = new DataChartModel();
            DataSets datasets = new DataSets();
            DataSetDetail datasetdetails;
            Options option = new Options();
            Scales scales = new Scales();
            Axes xAxes = new Axes() { stacked = true };
            Axes yAxes = new Axes() { stacked = true };

            ActualizarDatos(mes,anio,turnoindustrial,diasindustrial,turnomedical,diasmedical,turnoprovee1,diasprovee1,turnoprovee2,diasprovee2);

            var resultado = db.Database.SqlQuery<PorcentajeDemanda>("EXEC MF_Porcentaje_Demanda {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", Datos.diaindustrial, Datos.diamedical, Datos.diaprove1, Datos.diaprove2, Datos.mes, Datos.anio, Datos.horasindustrial, Datos.horasmedical, Datos.horasprove1, Datos.horasprove2,Datos.proceso).ToList();

            foreach (var item in db.MF_Machine.Where(x=>x.Id_Proceso==Datos.proceso).ToList())
            {
                chart.labels.Add(item.Machine);
                datasets.data.Add(100);
            }

            datasets.type = "line";
            datasets.label = "LIMIT";
            datasets.borderColor = "red";
            datasets.fill = false;

            chart.datasets.Add(datasets);

            foreach (var dem in db.MF_Demand.ToList())
            {
                datasetdetails = new DataSetDetail();
                datasetdetails.type = "bar";
                datasetdetails.label = dem.Demand;
                datasetdetails.backgroundColor = BarColor((int)dem.Item);
                foreach (var res in resultado.Where(x => x.Id_Demand == dem.Id_Demand).OrderBy(x=>x.Item).ToList())
                {
                    if (res.Porcentaje_Noventa == 0)
                    {
                        datasetdetails.data.Add("");
                    }
                    else
                    {
                        datasetdetails.data.Add(res.Porcentaje_Noventa.ToString());
                    }
                }
                chart.datasets.Add(datasetdetails);
            }

            scales.xAxes.Add(xAxes);
            scales.yAxes.Add(yAxes);

            ViewBag.JsonDataSet = JsonConvert.SerializeObject(chart);


            return PartialView();
        }

        public PartialViewResult Ocupacion_maquinasParcial()
        {
            ViewBag.ocupacion = db.Database.SqlQuery<Grafica>("EXEC MF_Porcentaje_Maquina {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", Datos.diaindustrial, Datos.diamedical,Datos.diaprove1,Datos.diaprove2, Datos.mes, Datos.anio, Datos.horasindustrial, Datos.horasmedical,Datos.horasprove1,Datos.horasprove2,Datos.proceso).ToList();
            return PartialView();
        }
        public ActionResult ShowDemand()
        {
            var demandDate = db.MF_DemandDate.Include(d => d.MF_Demand).Include(d => d.MF_WorkDate);
            return View(demandDate.ToList());
        }

        public ActionResult DemandDetalle()
        {
           
            var resultado = db.Database.SqlQuery<PorcentajeDemanda>("EXEC MF_Porcentaje_Demanda {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", Datos.diaindustrial, Datos.diamedical, Datos.diaprove1, Datos.diaprove2, Datos.mes, Datos.anio, Datos.horasindustrial, Datos.horasmedical, Datos.horasprove1, Datos.horasprove2,Datos.proceso).ToList();
            var total = db.Database.SqlQuery<Grafica>("EXEC MF_Porcentaje_Maquina  {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", Datos.diaindustrial, Datos.diamedical, Datos.diaprove1, Datos.diaprove2, Datos.mes, Datos.anio, Datos.horasindustrial, Datos.horasmedical, Datos.horasprove1, Datos.horasprove2,Datos.proceso).ToList();
            ViewBag.valores = resultado;
            ViewBag.ocupacion = total;
            
            return View();
        }

    
        public PartialViewResult GraficanoapiladaPartial(int mes, int anio, int turnoindustrial, double diasindustrial, int turnomedical, double diasmedical,int turnoprovee1,double diasprovee1, int turnoprovee2, double diasprovee2)
        {
            DataChartModel chart = new DataChartModel();
            DataSets datasets = new DataSets();
            DataSetDetail datasetdetails;
            Options option = new Options();
            Scales scales = new Scales();
            Axes xAxes = new Axes() { stacked = true };
            Axes yAxes = new Axes() { stacked = true };
            
            ActualizarDatos(mes, anio, turnoindustrial, diasindustrial, turnomedical, diasmedical,turnoprovee1,diasprovee1,turnoprovee2,diasprovee2);
           
            
            var resultado = db.Database.SqlQuery<Grafica>("EXEC MF_Porcentaje_Maquina {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", Datos.diaindustrial, Datos.diamedical, Datos.diaprove1, Datos.diaprove2, Datos.mes, Datos.anio, Datos.horasindustrial, Datos.horasmedical, Datos.horasprove1, Datos.horasprove2,Datos.proceso).ToList();
            
            foreach (var item in db.MF_Machine.Where(x => x.Id_Proceso == Datos.proceso).ToList())
            {
                chart.labels.Add(item.Machine);
                datasets.data.Add(100);
            }

            datasets.type = "line";
            datasets.label = "LIMIT";
            datasets.borderColor = "red";
            datasets.fill = false;

            chart.datasets.Add(datasets);

            datasetdetails = new DataSetDetail();
            datasetdetails.type = "bar";
            datasetdetails.label = "Machine Percentage";
            datasetdetails.backgroundColor = BarColor(1);
            foreach (var res in resultado.OrderBy(x => x.Id_machine).ToList())
            {
                if (res.Total_Ocupacion == 0)
                {
                    datasetdetails.data.Add("");
                }
                else
                {
                    datasetdetails.data.Add(res.Total_Ocupacion.ToString());
                }
            }
            chart.datasets.Add(datasetdetails);
            scales.xAxes.Add(xAxes);
            scales.yAxes.Add(yAxes);

            ViewBag.JsonDataSet = JsonConvert.SerializeObject(chart);

            return PartialView();
        }
        [HttpPost]
        public JsonResult EditarQty(int id_DemandDate, int qty)
        {
          
            try
            {
                MF_DemandDate Demandat = db.MF_DemandDate.Find(id_DemandDate);
                Demandat.Quantity = qty;
                db.SaveChanges();
                return Json(new { Success = true }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception e)
            {
                return Json(new { Success = false }, JsonRequestBehavior.DenyGet);
            }
        }

        public PartialViewResult addDemand()
        {
            return PartialView();
        }
        public JsonResult agregardemandas(string valores, int mes, int anio, int dias, int semanas)
        {
            int item;
            using (DbContextTransaction dbTran = db.Database.BeginTransaction())
            {
                try
                {
                    MF_WorkDate work = new MF_WorkDate()
                    {
                        Id_DemandMonth = mes,
                        Id_DemandYear = anio,
                        WorkDays = dias,
                        WorkWeeks = semanas
                    };
                    db.MF_WorkDate.Add(work);
                    db.SaveChanges();
                   
                    item = (int)db.MF_DemandDate.OrderByDescending(x => x.Id_DemandDate).First().Item;
                    var id_workdate = db.MF_WorkDate.Where(x => x.Id_DemandMonth == mes && x.Id_DemandYear == anio).First();
                    List<Int32> valor_demanda = new List<int>();
                    valor_demanda = valores.Split(',').ToList().Select(Int32.Parse).ToList();
                    int y = (int)db.MF_Demand.First().Id_Demand;
                    foreach (var valor in valor_demanda)
                    {
                        MF_DemandDate demdate = new MF_DemandDate()
                        {
                            Item=item,
                            Id_Demand = y,
                            Quantity = valor,
                            Id_WorkDate = id_workdate.Id_WorkDate
                        };
                        db.MF_DemandDate.Add(demdate);
                       
                        y++;
                    }
                    db.SaveChanges();
                    dbTran.Commit();
                    return Json(new { Success = true, Message = "Demand was successfully added" }, JsonRequestBehavior.DenyGet);
                }
                catch (Exception e)
                {
                    dbTran.Dispose();
                    return Json(new { Success = false, Message = "Error adding demand" }, JsonRequestBehavior.DenyGet);
                }
            }

        }
        public PartialViewResult mesesdisponibles(int id)
        {
            var meses = db.MF_DemandMonth.Where(x => !db.MF_WorkDate.Where(es => es.Id_DemandMonth == x.Id_DemandMonth && es.Id_DemandYear == id).Any()).ToList();
            ViewBag.mes = meses;
            return PartialView();
        }

        public PartialViewResult mesindexPartial(int id)
        {
            var meses = db.MF_DemandMonth.Where(x => db.MF_WorkDate.Where(es => es.Id_DemandMonth == x.Id_DemandMonth && es.Id_DemandYear == id).Any()).ToList();
            ViewBag.mes = meses;
            return PartialView();
        }
        public PartialViewResult mesesdeletePartial(int id)
        {
            var meses = db.MF_DemandMonth.Where(x => db.MF_WorkDate.Where(es => es.Id_DemandMonth == x.Id_DemandMonth && es.Id_DemandYear == id).Any()).ToList();
            ViewBag.mes = meses;
            return PartialView();
        }

        public PartialViewResult ShoworkdaysPartial(int mes, int anio)
        {

            var dias = db.MF_WorkDate.Where(x => x.Id_DemandYear == anio && x.Id_DemandMonth == mes).First();
            ViewBag.dias = dias.WorkDays;
            ViewBag.meses = dias.WorkWeeks;
            return PartialView(dias);
        }
        public JsonResult modificardiasemanas(int id, int valor, int op)
        {
            try
            {
                MF_WorkDate work = db.MF_WorkDate.Find(id);
                switch (op)
                {
                    case 1:
                        work.WorkDays = valor;
                        break;
                    case 2:
                        work.WorkWeeks = valor;
                        break;
                }
                db.SaveChanges();
                return Json(new { actualizar = true }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception e)
            {
                return Json(new { actualizar = false }, JsonRequestBehavior.DenyGet);
            }
        }

        public PartialViewResult DeleteDemandDatesPartial()
        {
            return PartialView();
        }
        public PartialViewResult showGridnoeditPartial(int mes, int anio)
        {
            var demandDate = db.MF_DemandDate.Include(d => d.MF_Demand).Include(d => d.MF_WorkDate).Where(x => x.MF_WorkDate.Id_DemandMonth == mes && x.MF_WorkDate.Id_DemandYear == anio);
            return PartialView(demandDate.ToList());
        }

        [HttpPost]
        public JsonResult EliminarDemandaMensual(int mes, int anio)
        {
            try
            {
                var work = db.MF_WorkDate.Where(x => x.Id_DemandMonth == mes && x.Id_DemandYear == anio).First();
                db.Database.ExecuteSqlCommand("EXEC MF_DeleteDemandMonthly {0}, {1}, {2}", work.Id_WorkDate, mes, anio);
                return Json(new { Success = true, Message = "The demand was successfully dropped" }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(new { Success = true, Message = "Error removing monthly demand" }, JsonRequestBehavior.DenyGet);
            }

        }

        public PartialViewResult CustomerGraficaPartial(int mes, int anio, int turnoindustrial, double diasindustrial, int turnomedical, double diasmedical, int turnoprovee1, double diasprovee1, int turnoprovee2, double diasprovee2)
        {

            ActualizarDatos(mes, anio, turnoindustrial, diasindustrial, turnomedical, diasmedical, turnoprovee1, diasprovee1, turnoprovee2, diasprovee2);
            DataChartModel chart = new DataChartModel();
            DataSets datasets = new DataSets();
            DataSetDetail datasetdetails;
            Options option = new Options();
            Scales scales = new Scales();
            Axes xAxes = new Axes() { stacked = true };
            Axes yAxes = new Axes() { stacked = true };
            var resultado = db.Database.SqlQuery<PorcentajeCliente>("EXEC MF_Porcentaje_Customer  {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", Datos.diaindustrial, Datos.diamedical,Datos.diaprove1,Datos.diaprove2, Datos.mes, Datos.anio, Datos.horasindustrial, Datos.horasmedical,Datos.horasprove1,Datos.horasprove2,Datos.proceso).ToList();

            foreach (var item in db.MF_Machine.Where(x => x.Id_Proceso == Datos.proceso).ToList())
            {
                chart.labels.Add(item.Machine);
                datasets.data.Add(100);
            }

            datasets.type = "line";
            datasets.label = "LIMIT";
            datasets.borderColor = "red";
            datasets.fill = false;

            chart.datasets.Add(datasets);

            foreach (var cus in db.MF_Customer)
            {
                datasetdetails = new DataSetDetail();
                datasetdetails.type = "bar";
                datasetdetails.label = cus.Customer;
                datasetdetails.backgroundColor = BarColor(cus.Id_Customer);
                foreach (var res in resultado.Where(x => x.Id_Customer == cus.Id_Customer).OrderBy(x => x.Item).ToList())
                {
                    if (res.Ocupacion_Noventa == 0)
                    {
                        datasetdetails.data.Add("");
                    }
                    else
                    {
                        datasetdetails.data.Add(res.Ocupacion_Noventa.ToString());
                    }
                }
                chart.datasets.Add(datasetdetails);
            }

            scales.xAxes.Add(xAxes);
            scales.yAxes.Add(yAxes);

            ViewBag.JsonDataSet = JsonConvert.SerializeObject(chart);

            return PartialView();
        }

        public ActionResult Graficas()
        {
            
            return View();
        }

        public PartialViewResult AddExcelPartial()
        {
            return PartialView();
        }

        [HttpPost]
        public JsonResult Cargar()
        {
            string mensaje;
            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;

                    HttpPostedFileBase file = files[0];
                    if (!file.FileName.EndsWith(".xls") && !file.FileName.EndsWith(".xlsx") && !file.FileName.EndsWith(".xlsm"))
                        return Json(new { Success = false, Message = "Error the excel extension is not correct" }, JsonRequestBehavior.DenyGet);


                    var fileName = DateTime.Now.ToString("yyyyMMddHHmm.") + file.FileName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Last();
                    SavesFile(file, fileName);
                    mensaje = UploadsRecordsToDataBase(fileName);
                    string fullPath = System.IO.Path.Combine(Datos.Ruta, fileName);
                    if (System.IO.File.Exists(fullPath))
                          System.IO.File.Delete(fullPath);
                    
                    return Json(new { Success = true, Message = mensaje }, JsonRequestBehavior.DenyGet);

                }

                catch (Exception e)
                {
                    return Json("error" + e.Message);
                }
            }

            return Json("No files were selected !");
        }

        private void SavesFile(HttpPostedFileBase file, string fileName)
        {
            //var path = System.IO.Path.Combine(@"\\AGUNTE808\metalFab\DemandMF", fileName);
            var path = System.IO.Path.Combine(Datos.Ruta, fileName);
            var data = new byte[file.ContentLength];
            file.InputStream.Read(data, 0, file.ContentLength);

            using (var sw = new System.IO.FileStream(path, System.IO.FileMode.Create))
            {
                sw.Write(data, 0, data.Length);
            }
        }
        private string UploadsRecordsToDataBase(string fileName)
        {
            var records = new List<MF_Part>();
            var opmach = new List<MF_OpMachine>();
            string error = "An error has occurred";
            int demanda = 0, cliente = 0, sect = 0, itemdem = 1;
            string  demand = null, customer = null, falla;
            double set_up = 0, cycle = 0, qty = 0;
            int numcol = 0, cont = 0, ani = 0, numceldas = 0;
            string mes = null, anio = null;
            List<int> month = new List<int>();
            List<int> year = new List<int>();
            List<int> workdays = new List<int>();
            List<int> workweek = new List<int>();
            List<int> idwork = new List<int>();
            string nombre;
            int item=1;
            using (DbContextTransaction dbTran = db.Database.BeginTransaction())
            {
                try
                {
                    //using (var stream = System.IO.File.Open(Path.Combine(@"\\AGUNTE808\metalFab\DemandMF", fileName), FileMode.Open, FileAccess.Read))
                    using (var stream = System.IO.File.Open(Path.Combine(Datos.Ruta, fileName), FileMode.Open, FileAccess.Read))
                    {
                        using (var reader = ExcelReaderFactory.CreateReader(stream))
                        {
                            do
                            {
                                nombre = reader.Name.ToUpper();

                                if (nombre == "DEMANDA ANUAL")
                                {
                                    while (reader.Read())
                                    {
                                        numcol = reader.FieldCount;
                                        int numeros;
                                        if (cont == 0)
                                        {
                                            for (int i = 3; i < numcol; i++)
                                            {
                                                try
                                                {
                                                    falla = "hoja de demanda";
                                                    mes = reader.GetDateTime(i).ToString("MMMM", CultureInfo.InvariantCulture);
                                                    ani = Convert.ToInt32(reader.GetDateTime(i).ToString("yyyy"));
                                                    month.Add(db.MF_DemandMonth.Where(x => x.DemandMonth == mes).First().Id_DemandMonth);

                                                    if (db.MF_DemandYear.Where(x => x.DemandYear == ani).Count() == 0)
                                                    {
                                                        MF_DemandYear demyear = new MF_DemandYear()
                                                        {
                                                            DemandYear = ani
                                                        };
                                                        db.MF_DemandYear.Add(demyear);
                                                        db.SaveChanges();
                                                    }
                                                    int cfe;
                                                    cfe = db.MF_DemandYear.Where(x => x.DemandYear == ani).Count();
                                                    year.Add(db.MF_DemandYear.Where(x => x.DemandYear == ani).First().Id_DemandYear);
                                                    numceldas = numcol;
                                                }
                                                catch (Exception e)
                                                {
                                                    numceldas = i - 1;
                                                }

                                            }
                                        }
                                        if (cont == 1)
                                        {
                                            for (int i = 3; i < numceldas; i++)
                                            {
                                                try { workweek.Add(Int32.Parse(reader.GetValue(i).ToString())); } catch (Exception) { workweek.Add(0); }
                                            }
                                        }
                                        if (cont == 2)
                                        {
                                            int m, y, d, e;
                                            for (int i = 3; i < numceldas; i++)
                                            {
                                                workdays.Add(Int32.Parse(reader.GetValue(i).ToString()));
                                            }
                                            for (int i = 0; i < month.Count; i++)
                                            {
                                                m = month[i];
                                                y = year[i];
                                                d = workdays[i];
                                                e = workweek[i];

                                                MF_WorkDate work;

                                                if (db.MF_WorkDate.Where(x => x.Id_DemandMonth == m && x.Id_DemandYear == y && x.WorkDays == d && x.WorkWeeks == e).Count() == 1)
                                                {
                                                    idwork.Add(db.MF_WorkDate.Where(x => x.Id_DemandMonth == m && x.Id_DemandYear == y && x.WorkDays == d && x.WorkWeeks == e).First().Id_WorkDate);
                                                }
                                                else
                                                {
                                                    if (db.MF_WorkDate.Any(x => x.Id_DemandMonth == m && x.Id_DemandYear == y))
                                                    {
                                                        work = db.MF_WorkDate.Where(x => x.Id_DemandMonth == m && x.Id_DemandYear == y).First();
                                                        work.WorkDays = d;
                                                        work.WorkWeeks = e;
                                                        idwork.Add(work.Id_WorkDate);
                                                    }
                                                    else
                                                    {
                                                        work = new MF_WorkDate
                                                        {
                                                            Id_DemandMonth = m,
                                                            Id_DemandYear = y,
                                                            WorkDays = d,
                                                            WorkWeeks = e
                                                        };
                                                        db.MF_WorkDate.Add(work);
                                                        db.SaveChanges();
                                                        idwork.Add(db.MF_WorkDate.Where(x => x.Id_DemandMonth == m && x.Id_DemandYear == y && x.WorkDays == d && x.WorkWeeks == e).First().Id_WorkDate);
                                                    }

                                                }
                                            }
                                        }
                                        if (cont >= 3)
                                        {
                                            int cantidad;
                                            customer = reader.GetValue(1).ToString().ToUpper().TrimEnd();
                                            if (!db.MF_Customer.Any(x => x.Customer == customer.ToUpper()))
                                            {
                                                MF_Customer cus = new MF_Customer()
                                                {
                                                    Customer = customer
                                                };
                                                db.MF_Customer.Add(cus);
                                                db.SaveChanges();
                                            }
                                            cliente = clienteid(customer);
                                            sect = sectorid(reader.GetValue(0).ToString().ToUpper().TrimEnd());
                                            demand = reader.GetValue(2).ToString().ToUpper().TrimEnd();

                                            if (db.MF_Demand.Where(x => x.Demand == demand).Count() == 0)
                                            {
                                                MF_Demand demandas = new MF_Demand()
                                                {
                                                    Item = itemdem,
                                                    Demand = demand,
                                                    Id_Customer = cliente,
                                                    Id_Sector = sect
                                                };
                                                db.MF_Demand.Add(demandas);
                                                db.SaveChanges();
                                            }
                                            demanda = demandaid(demand);


                                            for (int i = 3; i < numceldas; i++)
                                            {
                                                int workdate = 0;
                                                workdate = idwork[i - 3];
                                                try { cantidad = Int32.Parse(reader.GetValue(i).ToString()); } catch (Exception) { cantidad = 0; }
                                                try
                                                {

                                                    MF_DemandDate ddate = db.MF_DemandDate.Where(x => x.Id_Demand == demanda && x.Id_WorkDate == workdate).First();
                                                    ddate.Quantity = cantidad;
                                                }
                                                catch (Exception)
                                                {
                                                    MF_DemandDate nuevo = new MF_DemandDate()
                                                    {
                                                        Item = item,
                                                        Id_Demand = demanda,
                                                        Quantity = cantidad,
                                                        Id_WorkDate = idwork[i - 3]
                                                    };
                                                    db.MF_DemandDate.Add(nuevo);
                                                }
                                            }
                                            item++;
                                            itemdem++;
                                        }
                                        cont++;

                                    }
                                    db.SaveChanges();
                                }
                            } while (reader.NextResult());

                        }
                    }
                    dbTran.Commit();
                }

                catch (Exception e)
                {
                    return error;
                }
            }

            return "The demand was successfully updated";

        }


        public void ActualizarDatos(int mes, int anio, int turnoindustrial, double diasindustrial, int turnomedical, double diasmedical, int turnoprovee1, double diasprovee1, int turnoprovee2, double diasprovee2)
        {
            double turnoindu = 0, turnomedic = 0,turnopro1=0,turnopro2=0;
            if (turnoindustrial == 1) turnoindu = 8;
            if (turnoindustrial == 2) turnoindu = 15.5;
            if (turnoindustrial == 3) turnoindu = 21.5;
            if (turnomedical == 1) turnomedic = 8;
            if (turnomedical == 2) turnomedic = 15.5;
            if (turnomedical == 3) turnomedic = 21.5;
            if (turnoprovee1 == 1) turnopro1 = 8;
            if (turnoprovee1 == 2) turnopro1 = 15.5;
            if (turnoprovee1 == 3) turnopro1 = 21.5;
            if (turnoprovee2 == 1) turnopro2 = 8;
            if (turnoprovee2 == 2) turnopro2 = 15.5;
            if (turnoprovee2 == 3) turnopro2 = 21.5;

            Datos.diaindustrial = diasindustrial;
            Datos.diamedical = diasmedical;
            Datos.mes = mes;
            Datos.anio = anio;
            Datos.horasindustrial = turnoindu;
            Datos.horasmedical = turnomedic;
            Datos.diaprove1 = diasprovee1;
            Datos.diaprove2 = diasprovee2;
            Datos.horasprove1 = turnopro1;
            Datos.horasprove2 = turnopro2;
            if (DemandMetalFab.GlobalCode.Settings.LoggedUser != null)
            {
                MF_Proceso proces = db.MF_Proceso.Where(x => x.Id_Proceso == Datos.proceso).First();
                proces.DiasIndustrial = diasindustrial;
                proces.DiasMedical = diasmedical;
                proces.DiasProvee1 = diasprovee1;
                proces.DiasProvee2 = diasprovee2;
                proces.TurnoIndustrial = turnoindu;
                proces.TurnoMedical = turnomedic;
                proces.TurnoProvee1 = turnopro1;
                proces.TurnoProvee2 = turnopro2;
                proces.Id_DemandMonth = mes;
                proces.Id_DemandYear = anio;
                db.SaveChanges();
            }
        }


        public string BarColor(int num)
        {
            string color = "";
            if (num == 1) color = "#45D5F5";
            if (num == 2) color = "#AACEF9";
            if (num == 3) color = "#9EFD92";
            if (num == 4) color = "yellow";
            if (num == 5) color = "#45F58B";
            if (num == 6) color = "#FAE6AB ";
            if (num == 7) color = "pink";
            if (num == 8) color = "aqua";
            if (num == 9) color = "orange";
            if (num == 10) color = "fuchsia";
            if (num == 11) color = "#F3F79B";
            if (num == 12) color = "lime";
            if (num == 13) color = "gray";
            if (num == 14) color = "#F1CEFA";
            if (num == 15) color = "silver";
            if (num == 16) color = "#F77D7D";
            if (num == 17) color = "#F788AF";
            if (num == 18) color = "#FAC89D";
            if (num == 19) color = "#FAEE9D";
            if (num == 20) color = "#9DFAEB";
            if (num == 21) color = "#D097FB";
            if (num == 22) color = "#FB97BE";
            if (num == 23) color = "#F5DBCB";
            if (num == 24) color = "#C99186";
            if (num == 25) color = "#F5B33A";
            if (num == 26) color = "#3AA5F5";
            if (num == 27) color = "#8374BF";
            if (num == 28) color = "#6D7389";
            if (num == 29) color = "#CCF5E4";
            if (num == 30) color = "#A7F634";
            return color;
        }

        public int demandaid(string demand)
        {
            return db.MF_Demand.Where(x => x.Demand == demand).First().Id_Demand;
        }

        public int clienteid(string client)
        {
            return db.MF_Customer.Where(x => x.Customer == client).First().Id_Customer;
        }
        public int sectorid(string sector)
        {
            if (!db.MF_Sector.Any(x => x.Sector == sector))
            {
                MF_Sector sectores = new MF_Sector()
                {
                    Sector = sector
                };
                db.MF_Sector.Add(sectores);
                db.SaveChanges();
            }
            return db.MF_Sector.Where(x => x.Sector == sector).First().Id_Sector;
        }

        public ActionResult ProcesoTresMeses()
        {
            return View();
        }

        public PartialViewResult TablaProcesoTresPartial(string mesanio)
        {
            List<string> fecha = mesanio.Split('/').ToList();
            List<string> texto = new List<string>();
            List<string> idsfecha = new List<string>();
            string nuevafecha = "", nuevoid = "";
            int mes = 0;
            int anio = 0;
            MF_DemandMonth demmonth = new MF_DemandMonth();
            MF_DemandYear demyear = new MF_DemandYear();
            for (int i = 0; i < 3; i++)
            {
                mes = Convert.ToInt32(fecha[0]) + i;
                anio = Convert.ToInt32(fecha[1]);
                if (mes >= 1 && mes <= 12)
                {
                    demmonth = db.MF_DemandMonth.Where(x => x.Id_DemandMonth == mes).First();
                    demyear = db.MF_DemandYear.Where(x => x.Id_DemandYear == anio).First();
                    nuevafecha = demmonth.DemandMonth.Substring(0, 3) + "_" + demyear.DemandYear.ToString().Substring(anio.ToString().Length - 2);
                    nuevoid = demmonth.Id_DemandMonth + "/" + demyear.Id_DemandYear;
                }
                else
                {
                    mes = mes - 12;
                    anio += 1;
                    try
                    {
                        demmonth = db.MF_DemandMonth.Where(x => x.Id_DemandMonth == mes).First();
                        demyear = db.MF_DemandYear.Where(x => x.Id_DemandYear == anio).First();
                        nuevafecha = demmonth.DemandMonth.Substring(0, 3) + "_" + demyear.DemandYear.ToString().Substring(anio.ToString().Length - 2);
                        nuevoid = demmonth.Id_DemandMonth + "/" + demyear.Id_DemandYear;
                    }
                    catch (Exception)
                    {
                        nuevafecha = "";
                        nuevoid = "";
                    }
                }
                texto.Add(nuevafecha);
                idsfecha.Add(nuevoid);
            }
            ViewBag.idsfecha = idsfecha;
            ViewBag.textofecha = texto;
            return PartialView();
        }
        public PartialViewResult GraficaTresMesesPartial(string fecha,string turno, string dia)
        {
            List<string> fechastres = fecha.Split(',').ToList();
            List<string> turnostres = turno.Split(',').ToList();
            List<string> diastres = dia.Split(',').ToList();
            int mes1, mes2, mes3,an1,an2,an3,mes,anio, tu1, tu2, tu3;
            double  d1, d2, d3;

            mes1 =Convert.ToInt32(fechastres[0].Split('/')[0]);
            an1= Convert.ToInt32(fechastres[0].Split('/')[1]);
            tu1= Convert.ToInt32(turnostres[0]);
            d1 = Convert.ToDouble(diastres[0]);
            mes2 = Convert.ToInt32(fechastres[1].Split('/')[0]);
            an2 = Convert.ToInt32(fechastres[1].Split('/')[1]);
            tu2 = Convert.ToInt32(turnostres[1]);
            d2 = Convert.ToDouble(diastres[1]);
            mes3 = Convert.ToInt32(fechastres[2].Split('/')[0]);
            an3 = Convert.ToInt32(fechastres[2].Split('/')[1]);
            tu3 = Convert.ToInt32(turnostres[2]);
            d3 = Convert.ToDouble(diastres[2]);

            DataChartModel chart = new DataChartModel();
            DataSets datasets = new DataSets();
            DataSetDetail datasetdetails;
            Options option = new Options();
            Scales scales = new Scales();
            Axes xAxes = new Axes() { stacked = false };
            Axes yAxes = new Axes() { stacked = false };

 
           
            var resultado = db.Database.SqlQuery<PorcentajeTresMeses>("EXEC MF_Porcentaje_Tres_Meses {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10},{11},{12}", mes1, mes2, mes3, an1, an2, an3, d1, d2, d3, Datos.simpleturno(tu1), Datos.simpleturno(tu2), Datos.simpleturno(tu3), Datos.proceso).ToList();
            //fecha
            for (int z = 0; z < 3; z++)
            {
                mes = Convert.ToInt32(fechastres[z].Split('/')[0]);
                anio = Convert.ToInt32(fechastres[z].Split('/')[1]);
                chart.labels.Add(db.MF_DemandMonth.Where(x => x.Id_DemandMonth == mes).First().DemandMonth + "-" + db.MF_DemandYear.Where(x=>x.Id_DemandYear==anio).First().DemandYear);
                datasets.data.Add(100);
            }

            datasets.type = "line";
            datasets.label = "LIMIT";
            datasets.borderColor = "red";
            datasets.fill = false;

            chart.datasets.Add(datasets);

            foreach (var maq in db.MF_Machine.Where(x => x.Id_Proceso == Datos.proceso).ToList())
            {
                datasetdetails = new DataSetDetail();
                datasetdetails.type = "bar";
                datasetdetails.label = maq.Machine;
                datasetdetails.backgroundColor = BarColor((int)maq.Item);
                foreach (var res in resultado.Where(x => x.Id_machine == maq.Id_Machine).OrderBy(x => x.Id_turno).ToList())
                {
                    if (res.Total_Ocupacion == 0)
                    {
                        datasetdetails.data.Add("");
                    }
                    else
                    {
                        datasetdetails.data.Add(res.Total_Ocupacion.ToString());
                    }
                }
                chart.datasets.Add(datasetdetails);
            }

            scales.xAxes.Add(xAxes);
            scales.yAxes.Add(yAxes);

            ViewBag.JsonDataSet = JsonConvert.SerializeObject(chart);
            return PartialView();
        }
        [HttpPost]
        public JsonResult EditarWD(int Id_Work,int valor,int op)
        {
            try
            {
                MF_WorkDate work = db.MF_WorkDate.Where(x => x.Id_WorkDate == Id_Work).First();
                if (op == 1)
                {
                    work.WorkWeeks = valor;
                }
                else
                {
                    work.WorkDays = valor;
                }
                
                db.SaveChanges();
                return Json(new { Success = true }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception e)
            {
                return Json(new { Success = false }, JsonRequestBehavior.DenyGet);
            }
        }

        public PartialViewResult UpdateProcessExcel()
        {
            return PartialView();
        }
        public PartialViewResult DeleteExcelServerPartial()
        {
            return PartialView();
        }
        [HttpPost]
        public JsonResult EliminarExcelServer(string idsexcel)
        {
            string fullPath = ""; 
            List<MF_ExcelProceso> Seleccion_Excel = new List<MF_ExcelProceso>();
            try
            {
                List<Int32> ids = new List<int>();
                if (String.IsNullOrEmpty(idsexcel))
                {
                    return Json(new { Success = false, Message = "There is no excel to delete" }, JsonRequestBehavior.DenyGet);
                }
                ids = idsexcel.Split(',').ToList().Select(Int32.Parse).ToList();
                Seleccion_Excel = db.MF_ExcelProceso.Where(x => ids.Contains(x.Id_ExcelProceso) && x.Id_Proceso == Datos.proceso).Distinct().ToList();
                foreach(var item in Seleccion_Excel)
                {
                    fullPath = System.IO.Path.Combine(Datos.Ruta, item.Nombre);
                    //fullPath = Request.MapPath("~/Content/Files/" + item.Nombre);
                    if (System.IO.File.Exists(fullPath))
                        System.IO.File.Delete(fullPath);
                }
                db.MF_ExcelProceso.RemoveRange(Seleccion_Excel);
                db.SaveChanges();
                return Json(new { Success = true, Message = "The excel was successfully deleted" }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception e)
            {
                return Json(new { Success = false, Message = "Error deleting excel" }, JsonRequestBehavior.DenyGet);
            }
        }

    }
}


