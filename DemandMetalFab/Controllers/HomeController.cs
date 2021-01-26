using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.DirectoryServices;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using DemandMetalFab.Models;
using ExcelDataReader;
using Newtonsoft.Json;

namespace DemandMetalFab.Controllers
{
    public class HomeController : Controller
    {
        private DemandDBEntities db = new DemandDBEntities();

        List<int> id_titulomaquina = new List<int>();
        List<int> id_maquinahoja1 = new List<int>();
        List<int> id_projecthoja1 = new List<int>();
        List<string> num_parthoja1 = new List<string>();
        int constantevalidar = 0;
        int columnahoja1 = 1, columnahoja2 = 1, id = 0, item = 1;
        bool succe = true;

        public ActionResult Index()
        {
          
            return View();
        }


     


        public PartialViewResult ProcesosPartial()
        {
            var resultado = db.Database.SqlQuery<PorcentajeProceso>("Exec MF_Porcentaje_Proceso_Nuevo").ToList();
            ViewBag.proceso = resultado;
            return PartialView();
        }

        public ActionResult About()
        {
            ViewBag.Message = "Your application description page.";

            return View();
        }

        public ActionResult Contact()
        {
            ViewBag.Message = "Your contact page.";

            return View();
        }

        public PartialViewResult GraficaPartial()
        {
            DataChartModel chart = new DataChartModel();
            DataSets datasets = new DataSets();
            DataSetDetail datasetdetails;
            Options option = new Options();
            Scales scales = new Scales();
            Axes xAxes = new Axes() { stacked = true };
            Axes yAxes = new Axes() { stacked = true };
            

            var resultado = db.Database.SqlQuery<PorcentajeProceso>("Exec MF_Porcentaje_Proceso_Nuevo").ToList();

            foreach (var item in resultado)
            {
                chart.labels.Add(item.Proceso);
                datasets.data.Add(100);
            }

            datasets.type = "line";
            datasets.label = "LIMIT";
            datasets.borderColor = "red";
            datasets.fill = false;

            chart.datasets.Add(datasets);
            datasetdetails = new DataSetDetail();
            datasetdetails.type = "bar";
            datasetdetails.label = "Process percentage";
            datasetdetails.backgroundColor = "aqua";
            foreach (var res in resultado.OrderBy(x => x.Id_Proceso).ToList())
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

        public PartialViewResult AddProcessPartial()
        {
            return PartialView();
        }
       
        [HttpPost]
        public JsonResult AgregarProceso(int valor,string proceso, string sector)
        {
            int ejemplo;
            string mensaje;
            string fileName="";
            if (Request.Files.Count > 0)
            {
                try
                {
                    HttpFileCollectionBase files = Request.Files;
                    
                    HttpPostedFileBase file = files[0];
                    if (!file.FileName.EndsWith(".xls") && !file.FileName.EndsWith(".xlsx") && !file.FileName.EndsWith(".xlsm"))
                        return Json(new { Success = false, Message = "Error the excel extension is not correct" }, JsonRequestBehavior.DenyGet);

                    fileName = DateTime.Now.ToString("dd-MM-yyyy HH.mm tt ") + file.FileName.ToString();
                    //var fileName = DateTime.Now.ToString("yyyyMMddHHmm.") + file.FileName.Split(new[] { '.' }, StringSplitOptions.RemoveEmptyEntries).Last();
                    SavesFileActual(file, fileName);
                    mensaje = UploadsRecordsToDataBaseActual(fileName, valor, proceso,sector);
                    //string fullPath = System.IO.Path.Combine(@"\\AGUNTE808\metalFab\DemandMF", fileName);
                    //string fullPath = Request.MapPath("~/Content/Files/" + fileName);
                    string fullPath = System.IO.Path.Combine(Datos.Ruta, fileName);
                    if (!succe)
                    {
                        if (System.IO.File.Exists(fullPath))
                            System.IO.File.Delete(fullPath);
                    }
                    else
                    {
                        MF_ExcelProceso excel = new MF_ExcelProceso()
                        {
                            Id_Proceso=Datos.proceso,
                            Nombre=fileName,
                            Ruta=fullPath
                        };
                        db.MF_ExcelProceso.Add(excel);
                        db.SaveChanges();
                    }
                    return Json(new { Success = succe, Message = mensaje }, JsonRequestBehavior.DenyGet);

                }

                catch (TypeInitializationException e)
                {
                    string fullPath= "";
                    //string fullPath = System.IO.Path.Combine(@"\\AGUNTE808\metalFab\DemandMF", fileName);
                    fullPath = System.IO.Path.Combine(Datos.Ruta, fileName);
                    if (System.IO.File.Exists(fullPath))
                    {
                        System.IO.File.Delete(fullPath);
                    }
                    return Json(new { Success = false, Message = e.Message }, JsonRequestBehavior.DenyGet);
                    
                }
            }

            return Json(new { Success = true, Message = "Funciona" }, JsonRequestBehavior.DenyGet);
        }

        private void SavesFileActual(HttpPostedFileBase file, string fileName)
        {
            //var path = System.IO.Path.Combine(Server.MapPath("~/Content/Files/"), fileName);
            //var path = System.IO.Path.Combine(@"\\AGUNTE808\metalFab\DemandMF", fileName);
            var path = System.IO.Path.Combine(Datos.Ruta, fileName);
            var data = new byte[file.ContentLength];
            file.InputStream.Read(data, 0, file.ContentLength);
            using (var sw = new System.IO.FileStream(path, System.IO.FileMode.Create))
            {
                sw.Write(data, 0, data.Length);
            }
        }

       
        private string UploadsRecordsToDataBaseActual(string fileName, int op, string process, string idsectores)
        {

            DataTable dt = new DataTable();
            dt.TableName = "MF_Part";

            dt.Columns.Add(new DataColumn("Item", typeof(int)));
            dt.Columns.Add(new DataColumn("Num_Part", typeof(string)));
            dt.Columns.Add(new DataColumn("Id_Project", typeof(int)));
            dt.Columns.Add(new DataColumn("Id_Machine", typeof(int)));
            dt.Columns.Add(new DataColumn("Set_Up", typeof(decimal)));
            dt.Columns.Add(new DataColumn("Cycle", typeof(decimal)));
            dt.Columns.Add(new DataColumn("Quantity", typeof(int)));
            dt.Columns.Add(new DataColumn("Id_Proceso", typeof(int)));


            DataTable dts = new DataTable();
            dts.TableName = "MF_OpMachine";
            dts.Columns.Add(new DataColumn("Item", typeof(int)));
            dts.Columns.Add(new DataColumn("Id_Part", typeof(int)));
            dts.Columns.Add(new DataColumn("Id_Machine", typeof(int)));
            dts.Columns.Add(new DataColumn("Id_Proceso", typeof(int)));

            string connection = "data source=AGUNT805;user id=Metales_db;password=M3t4l3s.123;";
            SqlConnection con = new SqlConnection(connection);
            SqlBulkCopy objbulk = new SqlBulkCopy(con);

            var records = new List<MF_Part>();
            var opmach = new List<MF_OpMachine>();
            string error = "An error has occurred";
            string falla = "";
            int proyecto = 0, maquina = 0, demanda = 0, cliente = 0, sect = 0, itemdem = 1, itempro = 1;
            string project = null, num = null, machine = null, demand = null, customer = null, sector = null,lugar="";
            double set_up = 0, cycle = 0, qty = 0;
            int  numcol = 0, cont = 0, ani = 0, numceldas = 0;
            string mes = null, anio = null;
            List<int> month = new List<int>();
            List<int> year = new List<int>();
            List<int> workdays = new List<int>();
            List<int> workweek = new List<int>();
            List<int> idwork = new List<int>();
            string nombre;

            using (DbContextTransaction dbTran = db.Database.BeginTransaction())
            {

                EliminarAsignardatos(op, process, idsectores);

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
                                                    ani =Convert.ToInt32(reader.GetDateTime(i).ToString("yyyy"));
                                                    month.Add(db.MF_DemandMonth.Where(x => x.DemandMonth == mes).First().Id_DemandMonth);
                                                    
                                                    if (db.MF_DemandYear.Where(x => x.DemandYear ==ani).Count()== 0)
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
                                                try { workweek.Add(Int32.Parse(reader.GetValue(i).ToString())); } catch (Exception) { workweek.Add(0);}
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
                                                        Item=item,
                                                        Id_Demand=demanda,
                                                        Quantity=cantidad,
                                                        Id_WorkDate= idwork[i - 3]
                                                    };
                                                    db.MF_DemandDate.Add(nuevo);
                                                }
                                                //dtDemandDetalle.Rows.Add(item, demanda, cantidad, idwork[i - 3], Datos.proceso);
                                            }
                                            item++;
                                            itemdem++;
                                        }
                                        cont++;

                                    }
                                    //objbulk.DestinationTableName = "DemandDate";
                                    //objbulk.ColumnMappings.Add("Item", "Item");
                                    //objbulk.ColumnMappings.Add("Id_Demand", "Id_Demand");
                                    //objbulk.ColumnMappings.Add("Quantity", "Quantity");
                                    //objbulk.ColumnMappings.Add("Id_WorkDate", "Id_WorkDate");
                                    //objbulk.ColumnMappings.Add("Id_Proceso", "Id_Proceso");
                                    //con.Open();
                                    //objbulk.WriteToServer(dtDemandDetalle);
                                    //con.Close();
                                    db.SaveChanges();
                                }
                                if (nombre == "SECTOR MAQUINA")
                                {
                                    item = 1;
                                    falla = "hoja de sector maquina";
                                    int c = 0;
                                    while (reader.Read())
                                    {
                                        if (c >= 1)
                                        {

                                            try { machine = reader.GetValue(0).ToString().ToUpper().TrimEnd(); } catch (Exception) { break; }
                                            try { sector = reader.GetValue(1).ToString().ToUpper().TrimEnd(); } catch (Exception) { break; }
                                            sect = sectorid(sector);
                                            if (machine != null || machine == "")
                                            {
                                                MF_Machine mac = new MF_Machine()
                                                {
                                                    Item = item,
                                                    Machine = machine,
                                                    Id_Proceso = Datos.proceso,
                                                    Id_Sector = sect
                                                };
                                                db.MF_Machine.Add(mac);
                                                item++;
                                            }
                                            else
                                            {
                                                break;
                                            }


                                        }
                                        c++;
                                    }
                                    db.SaveChanges();
                                }

                                if (nombre == "MATRIZ TIEMPOS")
                                {
                                    lugar = "hoja de matriz";
                                    falla = "hoja de matriz";
                                    item = 1;
                                    while (reader.Read())
                                    {
                                        demand = "";
                                        project = "";
                                        machine = "";
                                        if (columnahoja1 != 1)
                                        {

                                            try { id = Convert.ToInt32(reader.GetValue(0)); } catch (Exception) { id = 0; }
                                            try
                                            {
                                                demand = reader.GetValue(2).ToString().ToUpper().TrimEnd();
                                               
                                                if (!db.MF_Demand.Any(x => x.Demand == demand ))
                                                {
                                                    dbTran.Dispose();
                                                    succe = false;
                                                    return "Cannot add excel because demand '" + demand + "' does not exists";
                                                }
                                                demanda = demandaid(demand);
                                                error = id.ToString();
                                            }
                                            catch (Exception)
                                            {
                                                if (id != 0)
                                                {
                                                    dbTran.Dispose();
                                                    succe = false;
                                                    error = id.ToString();
                                                    return "Cannot add excel because demand '" + demand + "' does not exists";
                                                }
                                                else
                                                {
                                                    demanda = 0;
                                                }
                                            }

                                            try
                                            {
                                                try { project = reader.GetValue(3).ToString().ToUpper().TrimEnd(); } catch (Exception e) { project = demand; }
                                                if (project == "" || project == null)
                                                {
                                                    proyecto = 0;
                                                }
                                                else
                                                {
                                                    if (db.MF_Project.Where(x => x.Project.ToUpper() == project && x.Id_Proceso == Datos.proceso).Count() == 0)
                                                    {
                                                        MF_Project pro = new MF_Project()
                                                        {
                                                            Item = itempro,
                                                            Project = project,
                                                            Id_Demand = demanda,
                                                            Id_Proceso = Datos.proceso

                                                        };
                                                        db.MF_Project.Add(pro);
                                                        db.SaveChanges();
                                                        itempro++;
                                                    }
                                                    proyecto = proyectoid(project);
                                                }
                                            }
                                            catch (Exception)
                                            {
                                                if (id != 0)
                                                {
                                                    dbTran.Dispose();
                                                    succe = false;
                                                    return "Cannot add excel because project '" + project + "' does not exists";
                                                }
                                                else
                                                {
                                                    proyecto = 0;
                                                }
                                            }
                                            if (id == 0 && demanda == 0 && proyecto == 0) break;
                                            try
                                            {
                                                try { num = reader.GetString(4).TrimEnd(); } catch (Exception e) { num = (Int32.Parse(reader.GetValue(4).ToString())).ToString(); }
                                            }
                                            catch (Exception e)
                                            {
                                                dbTran.Dispose();
                                                succe = false;
                                                return "Cannot add excel because part in column "+columnahoja1+" is empty";
                                            }
                                            try {
                                                machine = reader.GetString(5).TrimEnd(); 
                                            }catch(Exception e) {
                                                dbTran.Dispose();
                                                succe = false;
                                                return "The machine cannot be empty (Error in row "+columnahoja1+")"; 
                                            }
                                            try { 
                                                maquina = maquinaid(machine); 
                                            } catch (Exception) {
                                                dbTran.Dispose();
                                                succe = false;
                                                return "Cannot add excel because Machine '" + reader.GetString(5).TrimEnd() + "' does not exists in row "+columnahoja1; 
                                            }
                                            try { set_up = double.Parse(reader.GetValue(6).ToString()); } catch (Exception e) { set_up = 0; }
                                            try { cycle = double.Parse(reader.GetValue(7).ToString()); } catch (Exception e) { cycle = 0; }
                                            try { qty = double.Parse(reader.GetValue(8).ToString()); } catch (Exception e) { qty = 0; }

                                            id_maquinahoja1.Add(maquina);
                                            num_parthoja1.Add(num);
                                            if (num == null)
                                            {
                                                dbTran.Dispose();
                                                succe = false;
                                                return "Parts cannot be updated because there are empty part numbers";
                                            }
                                            if (db.MF_Part.Any(x => x.Num_Part == num && x.Id_Project == proyecto && x.Id_Proceso == Datos.proceso))
                                            {
                                                dbTran.Dispose();
                                                succe = false;
                                                return "Parts cannot be updated because there are duplicate part numbers (" + num + ")";
                                            }
                                            falla = num;
                                
                                            dt.Rows.Add(item, num, proyecto, maquina, Convert.ToDecimal(set_up), Convert.ToDecimal(cycle), qty, Datos.proceso);
                                            id_projecthoja1.Add(proyecto);
                                            item++;
                                        }
                                        columnahoja1++;
                                    }
                                    objbulk = new SqlBulkCopy(con);  
                                    objbulk.DestinationTableName = "MF_Part";
                                    objbulk.ColumnMappings.Add("Item", "Item");
                                    objbulk.ColumnMappings.Add("Num_Part", "Num_Part");
                                    objbulk.ColumnMappings.Add("Id_Project", "Id_Project");
                                    objbulk.ColumnMappings.Add("Id_Machine", "Id_Machine");
                                    objbulk.ColumnMappings.Add("Set_Up", "Set_Up");
                                    objbulk.ColumnMappings.Add("Cycle", "Cycle");
                                    objbulk.ColumnMappings.Add("Quantity", "Quantity");
                                    objbulk.ColumnMappings.Add("Id_Proceso", "Id_Proceso");
                                    con.Open();
                                    objbulk.WriteToServer(dt);
                                    con.Close();
                                }
                                if (nombre == "ASIGNACION MAQUINA")
                                {
                                    lugar = "asignacion";
                                    item = 1;
                                    while (reader.Read())
                                    {
                                        if (columnahoja2 == 1)
                                        {
                                            for (int i = 6; i < reader.FieldCount; i++)
                                            {
                                                try { machine = reader.GetValue(i).ToString().ToUpper().TrimEnd(); } catch (Exception) { break; }
                                                if (machine == null || machine == "")break;
                                                
                                                if (db.MF_Machine.Any(x => x.Machine == machine && x.Id_Proceso == Datos.proceso)) 
                                                id_titulomaquina.Add(maquinaid(machine));
                                            }
                                        }
                                        else
                                        {
                                            List<int> ids_maquina = new List<int>();
                                            try { id = Convert.ToInt32(reader.GetValue(0)); } catch (Exception) { id = 0; }
                                            try
                                            {
                                                try { num = reader.GetString(4).TrimEnd(); } catch (Exception e) { num = (Int32.Parse(reader.GetValue(4).ToString())).ToString(); }
                                            }
                                            catch (Exception e)
                                            {
                                                if (id != 0)
                                                {
                                                    dbTran.Dispose();
                                                    succe = false;
                                                    return "Cannot add excel because part in column " + columnahoja1 + " is empty";
                                                }
                                                else
                                                {
                                                    num = null;
                                                }
                             
                                            }
                                            falla = num;
                                            try
                                            {
                                                try { project = reader.GetString(3).ToUpper().TrimEnd(); } catch (Exception) { project = reader.GetString(2).ToUpper().TrimEnd(); }
                                            }catch(Exception e)
                                            {
                                                project = null;
                                            }
                                            try { 
                                                machine = reader.GetString(5).TrimEnd(); 
                                            } catch (Exception) {
                                                if (id != 0)
                                                {
                                                    succe = false;
                                                    return "The Machine can´t be empty ERROR in row " + columnahoja2;
                                                }
                                                else
                                                {
                                                    machine = null;
                                                }
                                            }

                                            if (id==0 && project == null && num == null && machine==null) break;
                                            if (project == null)
                                            {
                                                dbTran.Dispose();
                                                succe = false;
                                                return "Parts cannot be added because there are empty some Projects";
                                            }
                                            try { proyecto = proyectoid(project); }catch(Exception e) { return "Subproject does not exist, verify matching time matrix with assignment Error in row " + columnahoja2; }
                                            maquina = maquinaid(machine);
                                            if (num == null) error = "Parts cannot be updated because there are empty part numbers";
                                            if (!num_parthoja1.Any(x => x == num))
                                            {
                                                dbTran.Dispose();
                                                succe = false;
                                                return "Excel part numbers don't match (Error in row " + columnahoja2 + " " + num + " )";
                                            }
                                            if (id_maquinahoja1[columnahoja2 - 2] != maquina)
                                            {
                                                dbTran.Dispose();
                                                succe = false;
                                                return "The machines of the excel part numbers do not match (Error in row " + (columnahoja2 - 1) + " )";
                                            }
                                            
                                            if (id_projecthoja1[columnahoja2-2] != proyecto)
                                            {
                                                dbTran.Dispose();
                                                succe = false;
                                                return "The project in matrix time does not match with machine assignment Error ( Row " + (columnahoja2 - 1) + " )";
                                            }

                                            MF_Part part = db.MF_Part.Where(x => x.Num_Part == num && x.Id_Project == proyecto && x.Id_Machine == maquina && x.Id_Proceso == Datos.proceso).First();
                                            int cons = 6;
                                            for (int i = 0; i < id_titulomaquina.Count; i++)
                                            {
                                                machine = reader.GetString(cons);

                                                if (machine != null)
                                                {
                                                    ids_maquina.Add(id_titulomaquina[i]);
                                                    dts.Rows.Add(item, part.Id_Part, id_titulomaquina[i], Datos.proceso);
                                                }
                                                cons++;
                                            }

                                            if (!ids_maquina.Any(x => x == maquina))
                                            {
                                                dbTran.Dispose();
                                                succe = false;
                                                return "The machine of the part number '" + num + "' is not assigned correctly in row " + columnahoja2;
                                            }
                                        }

                                        columnahoja2++;
                                    }
                                    objbulk = new SqlBulkCopy(con); 
                                    objbulk.DestinationTableName = "MF_OpMachine";
                                    objbulk.ColumnMappings.Add("Item", "Item");
                                    objbulk.ColumnMappings.Add("Id_Part", "Id_Part");
                                    objbulk.ColumnMappings.Add("Id_Machine", "Id_Machine");
                                    objbulk.ColumnMappings.Add("Id_Proceso", "Id_Proceso");
                                    con.Open();
                                    objbulk.WriteToServer(dts);
                                    con.Close();
                                }
                            } while (reader.NextResult());
                            if (columnahoja1 != columnahoja2)
                            {
                                succe = false;
                                dbTran.Dispose();
                                return "Excel does not have the same quantity of part numbers";
                            }
                        }
                    }
                    dbTran.Commit();
                }
                catch (Exception e)
                {

                    succe = false;
                    return "error columna " + columnahoja2 + " " + falla+" "+lugar;
                }
            }

            return "The part numbers were updated successfully";

        }
        public int maquinaid(string machine)
        {
            return db.MF_Machine.Where(x=>x.Machine==machine && x.Id_Proceso==Datos.proceso).First().Id_Machine;
        }

        public int proyectoid(string project)
        {
            return db.MF_Project.Where(x => x.Project == project && x.Id_Proceso==Datos.proceso).First().Id_Project;
        }

        public int demandaid(string demand)
        {
            return db.MF_Demand.Where(x => x.Demand == demand).First().Id_Demand ;
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

        public bool ValidarProject(int p)
        {
            return id_projecthoja1[constantevalidar] == p ? true : false;
        }

        public void EliminarAsignardatos(int op,string process,string idsectores)
        {

            if (op != 1)
            {
                db.Database.ExecuteSqlCommand("EXEC MF_DeleteProcess {0},{1}", Datos.proceso, 2);
            }
            if (op == 1)
            {
                List<string> valores = idsectores.Split(',').ToList() ;

                MF_Proceso proc = new MF_Proceso()
                {
                    Proceso = process,
                    DiasIndustrial = valorenbool(valores[0])==true?6:0,
                    DiasMedical = valorenbool(valores[1]) == true ? 6 : 0,
                    DiasProvee1 = valorenbool(valores[2]) == true ? 6 : 0,
                    DiasProvee2 = valorenbool(valores[3]) == true ? 6 : 0,
                    Id_DemandMonth = db.MF_DemandMonth.First().Id_DemandMonth,
                    Id_DemandYear = db.MF_DemandYear.First().Id_DemandYear,
                    TurnoIndustrial = valorenbool(valores[0]) == true ? 21.5 : 0,
                    TurnoMedical = valorenbool(valores[1]) == true ? 21.5 : 0,
                    TurnoProvee1 = valorenbool(valores[3]) == true ? 21.5 : 0,
                    TurnoProvee2 = valorenbool(valores[3]) == true ? 21.5 : 0,
                    EnaIndu = valorenbool(valores[0]),
                    EnaMed = valorenbool(valores[1]),
                    EnaPro1 = valorenbool(valores[2]),
                    EnaPro2 = valorenbool(valores[3])
                };
                db.MF_Proceso.Add(proc);
                db.SaveChanges();
                var idproc = db.MF_Proceso.Where(x => x.Proceso == process).First();
                Datos.proceso = idproc.Id_Proceso;
            }
        }

        public PartialViewResult DeleteProcessPartial()
        {
            ViewBag.Id_Proceso = new SelectList(db.MF_Proceso, "Id_Proceso", "Proceso");
            return PartialView();
        }
        [HttpPost]
        public JsonResult EliminarProceso(int idproceso)
        {
            string fullPath = "";
            try
            {   
                foreach(var item in db.MF_ExcelProceso.Where(x => x.Id_Proceso == idproceso).ToList())
                {
                    //fullPath = System.IO.Path.Combine(@"\\AGUNTE808\metalFab\DemandMF", item.Nombre);
                    fullPath = System.IO.Path.Combine( Datos.Ruta ,item.Nombre);
                    if (System.IO.File.Exists(fullPath))
                        System.IO.File.Delete(fullPath);
                }
                db.MF_ExcelProceso.RemoveRange(db.MF_ExcelProceso.Where(x => x.Id_Proceso == idproceso).ToList());
                db.SaveChanges();
                db.Database.ExecuteSqlCommand("EXEC MF_DeleteProcess {0},{1}", idproceso, 1);
                return Json(new { Success = true, Message = "The process was successfully removed" }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception)
            {
                return Json(new { Success = false, Message = "The process could not be removed" }, JsonRequestBehavior.DenyGet);
            }
        }

        [HttpPost]
        public JsonResult AsignacionDatos(int proceso)
        {
            var datosproceso = db.MF_Proceso.Where(x => x.Id_Proceso == proceso).First();
            Datos.diaindustrial = Convert.ToDouble(datosproceso.DiasIndustrial);
            Datos.diamedical = Convert.ToDouble(datosproceso.DiasMedical);
            Datos.diaprove1 = Convert.ToDouble(datosproceso.DiasProvee1);
            Datos.diaprove2 = Convert.ToDouble(datosproceso.DiasProvee2);
            Datos.horasindustrial = Convert.ToDouble(datosproceso.TurnoIndustrial);
            Datos.horasmedical = Convert.ToDouble(datosproceso.TurnoMedical);
            Datos.horasprove1 = Convert.ToDouble(datosproceso.TurnoProvee1);
            Datos.horasprove2 = Convert.ToDouble(datosproceso.TurnoProvee2);
            Datos.mes = Convert.ToInt32(datosproceso.Id_DemandMonth);
            Datos.anio = Convert.ToInt32(datosproceso.Id_DemandYear);
            Datos.proceso = Convert.ToInt32(datosproceso.Id_Proceso);
            Datos.turnosimpleindustrial = turnoid(Datos.horasindustrial);
            Datos.turnosimplemedical = turnoid(Datos.horasmedical);
            Datos.turnosimpleprovee1 = turnoid(Datos.horasprove1);
            Datos.turnosimpleprovee2 = turnoid(Datos.horasprove2);
            Datos.statusindu = (bool)datosproceso.EnaIndu;
            Datos.statusmedic= (bool)datosproceso.EnaMed;
            Datos.statuspro1 = (bool)datosproceso.EnaPro1;
            Datos.statuspro2 = (bool)datosproceso.EnaPro2;
            return Json(new { Success = true, Message = "The process was successfully removed" }, JsonRequestBehavior.DenyGet);
        }


        public static int turnoid(double valor)
        {
            int res = 0;
            if (valor == 21.5) res = 3;
            if (valor == 15.5) res = 2;
            if (valor == 8) res = 1;
            return res;
        }

        public bool valorenbool(string val)
        {
            return val == "1" ? true : false;
        }

        public PartialViewResult EditProcessPartial()
        {
            return PartialView();
        }

        public PartialViewResult BusquedaPartial(int id)
        {
            List<int> valores = new List<int>();
            MF_Proceso proce = db.MF_Proceso.Where(x => x.Id_Proceso == id).First();
            ViewBag.nombre = proce.Proceso;
            valores.Add((proce.EnaIndu == true) ? 1 : 0);
            valores.Add((proce.EnaMed == true) ? 1 : 0);
            valores.Add((proce.EnaPro1 == true) ? 1 : 0);
            valores.Add((proce.EnaPro2 == true) ? 1 : 0);
            ViewBag.ids = valores;
            return PartialView();
        }

        [HttpPost]
        public JsonResult EditarProceso(int id,string process,string idsectores)
        {
            try
            {
                List<string> valores = idsectores.Split(',').ToList();
                MF_Proceso proces = db.MF_Proceso.Where(x => x.Id_Proceso == id).First();


                proces.Proceso = process;
                proces.DiasIndustrial = valorenbool(valores[0]) == true ? 6 : 0;
                proces.DiasMedical = valorenbool(valores[1]) == true ? 6 : 0;
                proces.DiasProvee1 = valorenbool(valores[2]) == true ? 6 : 0;
                proces.DiasProvee2 = valorenbool(valores[3]) == true ? 6 : 0;
                proces.TurnoIndustrial = valorenbool(valores[0]) == true ? 21.5 : 0;
                proces.TurnoMedical = valorenbool(valores[1]) == true ? 21.5 : 0;
                proces.TurnoProvee1 = valorenbool(valores[2]) == true ? 21.5 : 0;
                proces.TurnoProvee2 = valorenbool(valores[3]) == true ? 21.5 : 0;
                proces.EnaIndu = valorenbool(valores[0]);
                proces.EnaMed = valorenbool(valores[1]);
                proces.EnaPro1 = valorenbool(valores[2]);
                proces.EnaPro2 = valorenbool(valores[3]);
                db.SaveChanges();
                return Json(new { Success = true, Message = "The process was successfully edited" }, JsonRequestBehavior.DenyGet);

            }
            catch (Exception)
            {
                return Json(new { Success = false, Message = "An error occurred while editing the process" }, JsonRequestBehavior.DenyGet);

            }
        }
        public JsonResult ActualizarProcesoDropDown(int id)
        {
            string mensaje="";

                try
                {
                    string fileName = db.MF_ExcelProceso.Where(x => x.Id_ExcelProceso == id).First().Nombre;
                    mensaje = UploadsRecordsToDataBaseActual(fileName, 2, "nada", null);
                    return Json(new { Success = succe, Message = mensaje }, JsonRequestBehavior.DenyGet);
                }
                catch (TypeInitializationException e)
                {
                    return Json(new { Success = false, Message = e.Message }, JsonRequestBehavior.DenyGet);
                }
        }

        public List<Users> UsuariosAD()
        {
            try
            {
                List<Users> Lista = new List<Users>();
                DirectoryEntry entry = new DirectoryEntry("LDAP://" + "americas" + ".ad.flextronics.com");
                DirectorySearcher search = new DirectorySearcher(entry);
                search.Filter = "(&(objectClass=user)(l=Aguascalientes))";
                search.PropertiesToLoad.Add("samaccountname");
                search.PropertiesToLoad.Add("mail");
                search.PropertiesToLoad.Add("department");
                search.PropertiesToLoad.Add("displayname");
                SearchResult result;
                SearchResultCollection resultCol = search.FindAll();
                if (resultCol != null)
                {
                    for (int counter = 0; counter < resultCol.Count; counter++)
                    {
                        result = resultCol[counter];
                        if (result.Properties.Contains("department"))
                        {
                            Users objUsuario = new Users();
                            objUsuario.nombre = (string)result.Properties["samaccountname"][0];
                            if (result.Properties.Contains("mail"))
                            {
                                objUsuario.correo = (string)result.Properties["mail"][0];
                                objUsuario.displayname = (string)result.Properties["displayname"][0];
                            }
                            Lista.Add(objUsuario);
                        }
                    }
                }
                return Lista;
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public PartialViewResult AddUsersPartial()
        {
            ViewBag.users = UsuariosAD();
            return PartialView();
        }
        [HttpPost]
        public JsonResult BuscarUsuario(string user)
        {
            foreach(var item in UsuariosAD())
            {
                if (item.nombre == user)
                {
                    return Json(new { email = item.correo, nombre = item.displayname }, JsonRequestBehavior.DenyGet);
                }
            }
            return Json(new { email = "", nombre = "" }, JsonRequestBehavior.DenyGet);
        }

        [HttpPost]
        public JsonResult AgregarUsuario(string nombre, string cuenta, string email, int tipo)
        {
            try
            {
                if (db.MF_Usuario.Any(x => x.Cuenta == cuenta))
                {
                    return Json(new { Success = false, Message = "The user has already been registered" }, JsonRequestBehavior.DenyGet);
                }
                MF_Usuario user = new MF_Usuario()
                {
                    Nombre = nombre,
                    Cuenta = cuenta,
                    Correo = email,
                    Id_TipoUsuario = tipo,
                    Fecha = DateTime.Now
                };
                db.MF_Usuario.Add(user);
                db.SaveChanges();
                return Json(new { Success = true, Message = "The user was added correctly" }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception e)
            {
                return Json(new { Success = false, Message = e.Message }, JsonRequestBehavior.DenyGet);
            }
        }
        public PartialViewResult EditUsersPartial()
        {
            ViewBag.users = db.MF_Usuario;
            return PartialView();
        }
        [HttpPost]
        public JsonResult EditarUsuario(string cuenta, int tipo)
        {
            try
            {

                MF_Usuario user = db.MF_Usuario.Where(x => x.Cuenta == cuenta).First();
                user.Id_TipoUsuario = tipo;
                db.SaveChanges();
                return Json(new { Success = true, Message = "The user was edited successfully" }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception e)
            {
                return Json(new { Success = false, Message = "The user doesnt not exist" }, JsonRequestBehavior.DenyGet);
            }
        }
        public PartialViewResult DeleteUsersPartial()
        {
            ViewBag.users = UsuariosAD();
            return PartialView();
        }
        [HttpPost]
        public JsonResult EliminarUsuario(string cuenta)
        {
            try
            {
                db.MF_Usuario.Remove(db.MF_Usuario.Where(x => x.Cuenta == cuenta).First());
                db.SaveChanges();
                return Json(new { Success = true, Message = "The user was deleted successfully" }, JsonRequestBehavior.DenyGet);
            }
            catch (Exception e)
            {
                return Json(new { Success = false, Message = "The user doesnt not exist" }, JsonRequestBehavior.DenyGet);
            }
        }

        public ActionResult Graphics()
        {
            List<int> ids = new List<int>();
            List<string> nombres = new List<string>();
            foreach (var item in db.MF_Proceso)
            {
                ids.Add(item.Id_Proceso);
                nombres.Add(item.Proceso);
            }
            ViewBag.ids = JsonConvert.SerializeObject(ids);
            return View();
        }

        public PartialViewResult GraficaIndividual(int id)
        {
            string fecha = "", mes="";
            int anio;
            DataChartModel chart = new DataChartModel();
            DataSets datasets = new DataSets();
            DataSetDetail datasetdetails;
            Options option = new Options();
            Scales scales = new Scales();
            Axes xAxes = new Axes() { stacked = true };
            Axes yAxes = new Axes() { stacked = true };

            var val = db.MF_Proceso.Where(x => x.Id_Proceso == id).First();

            var resultado = db.Database.SqlQuery<Grafica>("EXEC MF_Porcentaje_Maquina {0},{1},{2},{3},{4},{5},{6},{7},{8},{9},{10}", val.DiasIndustrial, val.DiasMedical, val.DiasProvee1, val.DiasProvee2, val.Id_DemandMonth, val.Id_DemandYear, val.TurnoIndustrial, val.TurnoMedical, val.TurnoProvee1, val.TurnoProvee2, id).ToList();

            foreach (var item in db.MF_Machine.Where(x => x.Id_Proceso == id).ToList())
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
            ViewBag.id = JsonConvert.SerializeObject(id);
            ViewBag.nombre = val.Proceso;

            mes = db.MF_DemandMonth.Where(x => x.Id_DemandMonth == val.Id_DemandMonth).First().DemandMonth;
            anio= db.MF_DemandYear.Where(x => x.Id_DemandYear == val.Id_DemandYear).First().DemandYear;
            fecha = mes +"-"+ anio.ToString();
            ViewBag.fecha = fecha;
            return PartialView();
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


    }
}