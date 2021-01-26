using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace DemandMetalFab.Models
{
    public class LoginModel
    {
        public string _username="";
        public LoginModel()
        {

        }
        public string username { 
            get { return _username; } 
            set { _username = value; } 
        }
        public String password { get; set; }
        public int tipo { get; set; }

        
    }
}