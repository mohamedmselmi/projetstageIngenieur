using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace RegistrationAndLogin.Models
{
    public class Mix
    {
        public int idvente { get; set; }
        public string titre { get; set; }
        public string description { get; set; }
        public System.DateTime datevente { get; set; }
        public int CODE_PRODUIT { get; set; }
        public string status { get; set; }
        public int IdUser { get; set; }
        public string type { get; set; }

        public virtual Prod Prod { get; set; }
        public virtual Users Users { get; set; }

        public int idrec { get; set; }
        public string titrerec { get; set; }
        public string descriptionrec { get; set; }
        public System.DateTime daterec { get; set; }

        public string statusrec { get; set; }

       


    }
}