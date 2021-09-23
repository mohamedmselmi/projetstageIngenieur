using RegistrationAndLogin.Models;
using System.Collections.Generic;

namespace RegistrationAndLogin.Controllers
{
    public class ViewModel
    {

        public IEnumerable<Prod> Prods { get; set; }
        public IEnumerable<vente> ventes { get; set; }
        public string titre { get; set; }
        public string description { get; set; }
        public System.DateTime datevente { get; set; }
        public int CODE_PRODUIT { get; set; }
        public string status { get; set; }
        public int IdUser { get; set; }
        public string type { get; set; }

        public virtual Prod Prod { get; set; }
        public virtual Users Users { get; set; }
    }
    }
     
 