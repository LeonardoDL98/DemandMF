using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemandMetalFab.Models
{
    public class Grafica
    {

        public int Id_machine { get; set; }
        public string Machine { get; set; }
        public decimal Matto { get; set; }
        public decimal Tiempo_Total { get; set; }
        public decimal Tiempo_Total_MTTO { get; set; }
        public double Disponibilidad { get; set; }
        public decimal Ocupacion_Maquina { get; set; }
        public decimal Total_Ocupacion { get; set; }
    }
    public class GraficaSinApilar
    {
        public string Jsonmaquinas { get; set; }
        public string JsonPorcentajemaquina { get; set; }
    }
    public class PorcentajeDemanda
    {
        public int Item { get; set; }
        public string Machine { get; set; }
        public int Id_Sector { get; set; }
        public int Id_Demand { get; set; }
        public string Demand { get; set; }
        public double Tiempo_Maquina { get; set; }
        public double Porcentaje_Demanda { get; set; }
        public double Porcentaje_Noventa { get; set; }
    }

    public class PorcentajeTresMeses
    {
        public int Id_turno { get; set; }
        public int Id_Proceso { get; set; }
        public int Id_machine { get; set; }
        public string Machine { get; set; }
        public decimal Matto { get; set; }
        public decimal Tiempo_Total { get; set; }
        public decimal Tiempo_Total_MTTO { get; set; }
        public double Disponibilidad { get; set; }
        public decimal Ocupacion_Maquina { get; set; }
        public decimal Total_Ocupacion { get; set; }
        public int Mes { get; set; }
        public int Anio { get; set; }
    }
   
    public class PorcentajeCliente
    {
        public int Item { get; set; }
        public string Machine { get; set; }
        public int Id_Sector { get; set; }
        public int Id_Customer { get; set; }
        public string Customer { get; set; }
        public double Ocupacion_Cliente { get; set; }
        public double Ocupacion_Noventa { get; set; }
    }
   
    public class BusquedaId
    {
        public int Id_Part { get; set; }
    }

    public class PorcentajeProceso
    {
        public int Id_Proceso { get; set; }
        public string Proceso { get; set; }
        public string Machine { get; set; }
        public decimal Total_Ocupacion { get; set; }
        public string DemandMonth { get; set; }
        public int DemandYear { get; set; }
        public double DiaIndustrial { get; set; }
        public double DiaMedical { get; set; }
        public double DiaProve1 { get; set; }
        public double DiaProve2 { get; set; }
        public double TurnoIndustrial { get; set; }
        public double TurnoMedical { get; set; }
        public double TurnoProve1 { get; set; }
        public double TurnoProve2 { get; set; }
        public int m { get; set; }
    }
}