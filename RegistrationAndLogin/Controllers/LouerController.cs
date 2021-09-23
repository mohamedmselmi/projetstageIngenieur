using RegistrationAndLogin.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RegistrationAndLogin.Controllers
{
    public class LouerController : Controller
    {
        // GET: Louer
        MyDatabaseEntities24 dbObj = new MyDatabaseEntities24();
        public ActionResult Louer(louer obj)
        {

            try
            {

                ViewBag.listeLouer = dbObj.louer.ToList();
                ViewBag.listeProduit = dbObj.Prod.ToList();



                return View(obj);
            }
            catch (Exception e)
            {
                return HttpNotFound();
            }


        }

        [HttpPost]
        public ActionResult AddLouer(louer model)
        {

            string message = "";

            if (ModelState.IsValid)
            {
                MyDatabaseEntities24 dbObj1 = new MyDatabaseEntities24();
                louer obj = new louer();
                obj.datedebut = model.datedebut;
                obj.datefin = model.datefin;
                obj.titre = model.titre;
                obj.description = model.description;
                obj.IdUser = dbObj1.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().IdUser;
            //    var s = dbObj1.louer.Where(a => a.status == "en attente").Count().ToString();
                obj.status = "en attente";
                obj.type = Request.Form["type"];
                obj.CODE_PRODUIT = model.CODE_PRODUIT;
                if (model.idlouer == 0)
                {

                    dbObj.louer.Add(obj);
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
            return View("Louer");
        }


        public ActionResult LouerList()
        {
            MyDatabaseEntities24 dbObj1 = new MyDatabaseEntities24();
            var res = dbObj1.louer.ToList();
       //     var Tuple = Tuple < reclamation, List<louer> >(new LoginViewModel(), new RegisterViewModel());
            return View(res);
        }



        // louer pour admin 



        public ActionResult LouerAdmin(louer obj)
        {

            try
            {

                ViewBag.listeLouer = dbObj.louer.ToList();
                ViewBag.listeProduit = dbObj.Prod.ToList();



                return View(obj);
            }
            catch (Exception e)
            {
                return HttpNotFound();
            }


        }

        [HttpPost]
        public ActionResult AddLouerAdmin(louer model)
        {

            string message = "";

            if (ModelState.IsValid)
            {
                MyDatabaseEntities24 dbObj1 = new MyDatabaseEntities24();
                louer obj = new louer();
                obj.datedebut = model.datedebut;
                obj.datefin = model.datefin;
                obj.titre = model.titre;
                obj.description = model.description;
                obj.IdUser = dbObj1.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().IdUser;

                obj.status = "en attente";
                obj.type = Request.Form["type"];
                obj.CODE_PRODUIT = model.CODE_PRODUIT;
                if (model.idlouer == 0)
                {

                    dbObj.louer.Add(obj);
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
            return View("Louer");
        }


        public ActionResult LouerListAdmin()
        {
            MyDatabaseEntities24 dbObj1 = new MyDatabaseEntities24();
            var res = dbObj1.louer.ToList();
            return View(res);
        }

        public ActionResult ValidLouerAdmin(int idlouer)
        {

            string message = "";
            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {
                var res = dc.louer.Where(x => x.idlouer == idlouer).First();
                //   var res1 = dc.Users.Where(x => x.IdUser == IdUser);
                var query = (from ord in dc.louer where ord.idlouer == idlouer select ord);
                foreach (louer ord in query)
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



                var list = dc.louer.ToList();
                return View("LouerListAdmin");
            }



        }

        public ActionResult ValidLouer(int idlouer)
        {

            string message = "";
            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {
                var res = dc.louer.Where(x => x.idlouer == idlouer).First();
                //   var res1 = dc.Users.Where(x => x.IdUser == IdUser);
                var query = (from ord in dc.louer where ord.idlouer == idlouer select ord);
                foreach (louer ord in query)
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



                var list = dc.louer.ToList();
                return View("LouerList");
            }



        }

        public ActionResult RefuseLouerAdmin(int idlouer)
        {

            string message = "";
            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {
                var res = dc.louer.Where(x => x.idlouer == idlouer).First();
                //   var res1 = dc.Users.Where(x => x.IdUser == IdUser);
                var query = (from ord in dc.louer where ord.idlouer == idlouer select ord);
                foreach (louer ord in query)
                {
                    ord.status = "nonvalid";
                  //  ord.Prod.status = "resolu";
                    res.status = ord.status;
                  //  res.Prod.status = ord.Prod.status;
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



                var list = dc.louer.ToList();
                return View("LouerListAdmin");
            }



        }

        public ActionResult RefuseLouer(int idlouer)
        {

            string message = "";
            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {
                var res = dc.louer.Where(x => x.idlouer == idlouer).First();
                //   var res1 = dc.Users.Where(x => x.IdUser == IdUser);
                var query = (from ord in dc.louer where ord.idlouer == idlouer select ord);
                foreach (louer ord in query)
                {
                    ord.status = "nonvalid";
                    //  ord.Prod.status = "resolu";
                    res.status = ord.status;
                    //  res.Prod.status = ord.Prod.status;
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



                var list = dc.louer.ToList();
                return View("LouerList");
            }



        }

        public ActionResult Delete(int idlouer)
        {
            var res = dbObj.louer.Where(x => x.idlouer == idlouer).First();
            dbObj.louer.Remove(res);
            dbObj.SaveChanges();
            var list = dbObj.louer.ToList();
            return View("LouerList", list);
        }


        public ActionResult DeleteAdmin(int idlouer)
        {
            var res = dbObj.louer.Where(x => x.idlouer == idlouer).First();
            dbObj.louer.Remove(res);
            dbObj.SaveChanges();
            var list = dbObj.louer.ToList();
            return View("LouerListAdmin", list);
        }


    }
}