using DemandMetalFab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemandMetalFab
{
   
    public class Datos
    {

        public static int _proceso = 0;
        public static int _mes = 1;
        public static int _anio = 22;
        public static double _diamedical = 6;
        public static double _diaindustrial = 6;
        public static double _diaprove1 = 6;
        public static double _diaprove2 = 6;
        public static double _horasmedical = 21.5;
        public static double _horasindustrial = 21.5;
        public static double _horasprove1 = 21.5;
        public static double _horasprove2 = 21.5;
        public static int _turnosimpleindutrial = 3;
        public static int _turnosimplemedical = 3;
        public static int _turnosimpleprovee1 = 3;
        public static int _turnosimpleprovee2 = 3;
        public static bool _statusindu = true;
        public static bool _statusmedic = true;
        public static bool _statuspro1 = true;
        public static bool _statuspro2 = true;

        //public static string Ruta = HttpContext.Current.Server.MapPath("~/Content/Files/");
        public static string Ruta = @"\\AGUNTE808\metalFab\DemandMF";
        public static int proceso
        {
            get { return _proceso; }
            set { _proceso = value; }
        }
        public static int mes
        {
            get{ return _mes; }
            set{ _mes = value; }
        }
        public static int anio
        {
            get{ return _anio; }
            set{ _anio = value; }
        }
        public static double diamedical
        {
            get { return _diamedical; }
            set { _diamedical = value; }
        }
        public static double diaindustrial
        {
            get { return _diaindustrial; }
            set { _diaindustrial = value; }
        }
        public static double diaprove1
        {
            get { return _diaprove1; }
            set { _diaprove1 = value; }
        }
        public static double diaprove2
        {
            get { return _diaprove2; }
            set { _diaprove2 = value; }
        }
        public static double horasmedical
        {
            get { return _horasmedical; }
            set { _horasmedical = value; }
        }
        public static double horasindustrial
        {
            get { return _horasindustrial; }
            set { _horasindustrial = value; }
        }
        public static double horasprove1
        {
            get { return _horasprove1; }
            set { _horasprove1 = value; }
        }
        public static double horasprove2
        {
            get { return _horasprove2; }
            set { _horasprove2 = value; }
        }
        public static int turnosimpleindustrial
        {
            get { return _turnosimpleindutrial; }
            set { _turnosimpleindutrial = value; }
        }
        public static int turnosimplemedical
        {
            get { return _turnosimplemedical; }
            set { _turnosimplemedical = value; }
        }
        public static int turnosimpleprovee1
        {
            get { return _turnosimpleprovee1; }
            set { _turnosimpleprovee1 = value; }
        }
        public static int turnosimpleprovee2
        {
            get { return _turnosimpleprovee2; }
            set { _turnosimpleprovee2 = value; }
        }
    

        public static bool statusindu
        {
            get { return _statusindu; }
            set { _statusindu = value; }
        }
        public static bool statusmedic
        {
            get { return _statusmedic; }
            set { _statusmedic = value; }
        }
        public static bool statuspro1
        {
            get { return _statuspro1; }
            set { _statuspro1 = value; }
        }
        public static bool statuspro2
        {
            get { return _statuspro2; }
            set { _statuspro2 = value; }
        }
        public static int turnoid(double valor)
        {
            int res = 0;

            if (valor == 21.5) res = 3;
            if (valor == 15.5) res = 2;
            if (valor == 8) res = 1;
            return res;
        }

        public static double simpleturno(int valor)
        {
            double res = 0;
            if (valor == 1) res = 8;
            if (valor == 2) res = 15.5;
            if (valor == 3) res = 21.5;
            return res;
        }

        public static void RegistroBitacora(string descripcion)
        {
            
        }

    }
}
