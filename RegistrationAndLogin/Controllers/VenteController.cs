using RegistrationAndLogin.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace RegistrationAndLogin.Controllers
{
    public class VenteController : Controller
    {
        // GET: Vente
        MyDatabaseEntities24 dbObj = new MyDatabaseEntities24();
        // GET: Produit
        public ActionResult Vente(vente obj)
        {

            try
            {

                ViewBag.listeVente = dbObj.vente.ToList();
                ViewBag.listeProduit = dbObj.Prod.ToList();


      
                return View(obj);
            }
            catch (Exception e)
            {
                return HttpNotFound();
            }


        }


        [HttpPost]
        public ActionResult AddVente(vente model)
        {

            string message = "";

            if (ModelState.IsValid)
            {
                MyDatabaseEntities24 dbObj1 = new MyDatabaseEntities24();
                vente obj = new vente();
                obj.datevente = DateTime.Now;
                obj.titre = model.titre;
                obj.description = model.description;
                obj.IdUser = dbObj1.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().IdUser;
                
                obj.status = "en attente";
                obj.type = Request.Form["type"];
                obj.CODE_PRODUIT = model.CODE_PRODUIT;
                if (model.idvente == 0)
                {

                    dbObj.vente.Add(obj);
                    dbObj.SaveChanges();
                    message = "Ajout successfully done."; 
                   string accountSid = Environment.GetEnvironmentVariable("AC2ff03a1b9d39855883ec81fcb475361e");
                    string authToken = Environment.GetEnvironmentVariable("934692dc138b438d5c5b1765c40a98bf");

                    TwilioClient.Init("AC2ff03a1b9d39855883ec81fcb475361e", "934692dc138b438d5c5b1765c40a98bf");

                    var message1 = MessageResource.Create(
                        body: "vous avez une nouvelle demande de vente pour ton produit de la part de "+dbObj1.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().FirstName.ToString(),
                        from: new Twilio.Types.PhoneNumber("+14434291255"),
                        to: new Twilio.Types.PhoneNumber("+216"+ dbObj1.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().numtelephone.ToString())
                    );

                    Console.WriteLine(message1.Sid);  
                
                }
                else
                {
                    dbObj.Entry(obj).State = EntityState.Modified;
                    dbObj.SaveChanges();
                    message = "modification successfully done.";
                }

            }

          
      

            ModelState.Clear();

            ViewBag.message = message;
            return View("Vente");
        }



        public ActionResult ValidVente(int idvente)
        {

            string message = "";
            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {
              

              
                try
                {
                    var res = dc.vente.Where(x => x.idvente == idvente).First();
                    //   var res1 = dc.Users.Where(x => x.IdUser == IdUser);
                    var query = (from ord in dc.vente where ord.idvente == idvente select ord);
                    foreach (vente ord in query)
                    {
                        ord.status = "resolu";
                        res.status = ord.status;
                        ord.Prod.status = "resolu";
                        res.Prod.status = ord.Prod.status;
                        
                    }

                    //   dc.Entry(res).State = EntityState.Modified;
                    dc.Configuration.ValidateOnSaveEnabled = false;
                    dc.SaveChanges();


                    var res1 = dc.Prod.Where(x => x.CODE_PRODUIT == res.CODE_PRODUIT).First();
                    //   var res1 = dc.Users.Where(x => x.IdUser == IdUser);
                    var query1 = (from ord1 in dc.Prod where ord1.CODE_PRODUIT == res1.CODE_PRODUIT select ord1);
                    foreach (Prod ord1 in query1)
                    {
                        ord1.status = "resolu";
                        res1.status = ord1.status;
                        dc.Configuration.ValidateOnSaveEnabled = false;
                        dc.SaveChanges();

                    }
                  

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                    // Provide for exceptions.
                }



                var list = dc.vente.ToList();
                return View("Vente");
            }



        }


        public ActionResult VenteList()
        {
            var res = dbObj.vente.ToList();
            return View(res);


        }

        // pour l'espace admin 



        public ActionResult VenteAdmin(vente obj)
        {

            try
            {

                ViewBag.listeVente = dbObj.vente.ToList();
                ViewBag.listeProduit = dbObj.Prod.ToList();



                return View(obj);
            }
            catch (Exception e)
            {
                return HttpNotFound();
            }


        }


        [HttpPost]
        public ActionResult AddVenteAdmin(vente model)
        {

            string message = "";

            if (ModelState.IsValid)
            {
                MyDatabaseEntities24 dbObj1 = new MyDatabaseEntities24();
                vente obj = new vente();
                obj.datevente = DateTime.Now;
                obj.titre = model.titre;
                obj.description = model.description;
                obj.IdUser = dbObj1.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().IdUser;

                obj.status = "en attente";
                obj.type = Request.Form["type"];
                obj.CODE_PRODUIT = model.CODE_PRODUIT;
                if (model.idvente == 0)
                {

                    dbObj.vente.Add(obj);
                    dbObj.SaveChanges();
                    message = "Ajout successfully done.";
                }
                else
                {
                    dbObj.Entry(obj).State = EntityState.Modified;
                    dbObj.SaveChanges();
                    message = "modification successfully done.";
                }

            }




            ModelState.Clear();

            ViewBag.message = message;
            return View("VenteAdmin");
        }



        public ActionResult ValidVenteAdmin(int idvente)
        {

            string message = "";
            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {
                var res = dc.vente.Where(x => x.idvente == idvente).First();
                //   var res1 = dc.Users.Where(x => x.IdUser == IdUser);
                var query = (from ord in dc.vente where ord.idvente == idvente select ord);
                foreach (vente ord in query)
                {
                    ord.status = "resolu";
                    ord.Prod.status = "resolu";
                    res.status = ord.status;
                    res.Prod.status = ord.Prod.status;
                }

             
                try
                {


                    //   dc.Entry(res).State = EntityState.Modified;
                    dc.Configuration.ValidateOnSaveEnabled = false;
                    dc.SaveChanges();


                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                    // Provide for exceptions.
                }



                var list = dc.vente.ToList();
                return View("VenteAdmin");
            }



        }


        public ActionResult VenteListAdmin()
        {
            var res = dbObj.vente.ToList();
            return View(res);
        }

        public ActionResult Delete(int idvente)
        {
            var res = dbObj.vente.Where(x => x.idvente == idvente).First();
            dbObj.vente.Remove(res);
            dbObj.SaveChanges();
            var list = dbObj.vente.ToList();
            return View("VenteList", list);
        }

        public ActionResult DeleteAdmin(int idvente)
        {
            var res = dbObj.vente.Where(x => x.idvente == idvente).First();
            dbObj.vente.Remove(res);
            dbObj.SaveChanges();
            var list = dbObj.vente.ToList();
            return View("VenteListAdmin", list);
        }

        public ActionResult RefuseVenteAdmin(int idvente)
        {

            string message = "";
            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {
                var res = dc.vente.Where(x => x.idvente == idvente).First();
                //   var res1 = dc.Users.Where(x => x.IdUser == IdUser);
                var query = (from ord in dc.vente where ord.idvente == idvente select ord);
                foreach (vente ord in query)
                {
                    ord.status = "nonvalid";
                  //  ord.Prod.status = "resolu";
                    res.status = ord.status;
                 //   res.Prod.status = ord.Prod.status;
                }


                try
                {


                    //   dc.Entry(res).State = EntityState.Modified;
                    dc.Configuration.ValidateOnSaveEnabled = false;
                    dc.SaveChanges();


                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                    // Provide for exceptions.
                }



                var list = dc.vente.ToList();
                return View("VenteAdmin");
            }



        }


        public ActionResult RefuseVente(int idvente)
        {

            string message = "";
            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {
                var res = dc.vente.Where(x => x.idvente == idvente).First();
                //   var res1 = dc.Users.Where(x => x.IdUser == IdUser);
                var query = (from ord in dc.vente where ord.idvente == idvente select ord);
                foreach (vente ord in query)
                {
                    ord.status = "nonvalid";
                    //  ord.Prod.status = "resolu";
                    res.status = ord.status;
                    //   res.Prod.status = ord.Prod.status;
                }


                try
                {


                    //   dc.Entry(res).State = EntityState.Modified;
                    dc.Configuration.ValidateOnSaveEnabled = false;
                    dc.SaveChanges();


                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                    // Provide for exceptions.
                }



                var list = dc.vente.ToList();
                return View("Vente");
            }



        }

    }
        
}
    

