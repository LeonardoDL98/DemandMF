using DemandMetalFab.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace DemandMetalFab.GlobalCode
{
    public class Settings
    {
        private static DemandDBEntities _db;
        public static DemandDBEntities db
        {
            get
            {
                if (_db == null)
                {
                    _db = new DemandDBEntities();
                }
                return _db;
            }
            set
            {
                _db = value;
            }
        }

        public static LoginModel LoggedUser
        {
            get
            {
                return (LoginModel)HttpContext.Current.Session["USR_Session"];
            }
            set
            {
                HttpContext.Current.Session["USR_Session"] = value;
            }
        }


    }

}