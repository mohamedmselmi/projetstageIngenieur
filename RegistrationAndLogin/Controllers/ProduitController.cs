using RegistrationAndLogin.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.IO;
using System.Linq;
using System.Security.Claims;
using System.Web;
using System.Web.Mvc;

namespace RegistrationAndLogin.Controllers
{
    public class ProduitController : Controller
    {

        MyDatabaseEntities24 dbObj = new MyDatabaseEntities24();
        // GET: Produit
        public ActionResult Produit(Prod obj)
        {
            
            try
            {

                ViewBag.listeProduit = dbObj.Prod.ToList();
                ViewBag.listeCategorie = dbObj.Categories.ToList();
                
               
                
                return View(obj);
            }
            catch (Exception e)
            {
                return HttpNotFound();
            }


        }
        [HttpPost]
        public ActionResult AddProduit(Prod model)
        {

           string message = "";
            
            if (ModelState.IsValid)
                {  MyDatabaseEntities24 dbObj1 = new MyDatabaseEntities24();
                    Prod obj = new Prod();
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0]; //le nom de notre fichier
                        if (file != null)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                           var path = Path.Combine(Server.MapPath("~/Fichier"), fileName);

                            file.SaveAs(path);
                    
                            model.IMAGE_PRODUIT = fileName;
                            model.URL_IMAGE_PRODUIT = path;
        
                        }
                      
                    }
                   
                    model.DATE_SAISIE = DateTime.Now;
                model.remise = 0;
                model.status = "en attente";
                model.vu = 0;
                model.datedebut = DateTime.Now;
                model.datefin = DateTime.Now;
                    model.type = Request.Form["Type"];
                    model.IdUser = dbObj1.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().IdUser;
                    //     model.LIBELLE_PRODUIT = "gg"
                  
                    if (model.CODE_PRODUIT == 0)
                    {
                       
                        dbObj.Prod.Add(model);
                        dbObj.SaveChanges();
                      message = "Ajout successfully done.";
                    }
                    else
                    {
                        dbObj.Entry(model).State = EntityState.Modified;
                        dbObj.SaveChanges();
                        message = "modification successfully done.";
                    }
                }
            
            ModelState.Clear();

            ViewBag.message = message;
            return View("Produit");
            
            

        }

        public ActionResult ProduitList()
        {
     //       var query1 = (from ord1 in dbObj.vente where ord1.status == "resolu" select ord1.CODE_PRODUIT);
            
            var res = dbObj.Prod.ToList();
            foreach (var item in res)
            {

                string p = item.datedebut.Day.ToString() + "/" + item.datedebut.Month.ToString() + "/" + item.datedebut.Year.ToString();
                string p1 = item.datefin.Day.ToString() + "/" + item.datefin.Month.ToString() + "/" + item.datefin.Year.ToString();

                string datenow = DateTime.Now.Day.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString();
                DateTime deb = DateTime.Parse(p);
                DateTime fin = DateTime.Parse(p1);
                DateTime now = DateTime.Parse(datenow);
                TimeSpan DiffRemise = fin - deb;
                TimeSpan DiffRemisef = now - deb;
                if (item.status != "remise" && DateTime.Now.Day - DateTime.Parse(item.datedebut.Day.ToString() + "/" + item.datedebut.Month.ToString() + "/" + item.datedebut.Year.ToString()).Day >= 0 && DateTime.Parse(item.datefin.Day.ToString() + "/" + item.datefin.Month.ToString() + "/" + item.datefin.Year.ToString()).Day - DateTime.Now.Day >= 0)
                {
                    var t = item.Prix / 100;
                    item.Prix = t * (100 - item.remise);
                    item.status = "remise";
                }

                if (item.status == "remise" && DateTime.Parse(item.datefin.Day.ToString() + "/" + item.datefin.Month.ToString() + "/" + item.datefin.Year.ToString()).Day - DateTime.Now.Day < 0)
                {
                    var t = item.Prix / (100 - item.remise);
                    item.Prix = t * 100;
                    item.status = "en attente";
                }
                dbObj.Entry(item).State = EntityState.Modified;
                dbObj.SaveChanges();


            }
            return View(res);
        }

        public ActionResult Delete(int CODE_PRODUIT)
        {
            var res = dbObj.Prod.Where(x => x.CODE_PRODUIT == CODE_PRODUIT).First();
            dbObj.Prod.Remove(res);
            dbObj.SaveChanges();
            var list = dbObj.Prod.ToList();
            return View("ProduitList", list);
        }

        public ActionResult ModifierProduit(int id)
        {
            try
            {
                ViewBag.listeCategorie = dbObj.Categories.ToList();
                ViewBag.listeProduit = dbObj.Prod.ToList();
                Prod produit = dbObj.Prod.Find(id);
                if (produit != null)
                {
                    //   dbObj.Entry(produit).State = EntityState.Modified;
                    //  dbObj.SaveChanges();
                    return View("Produit", produit);
                }
                return RedirectToAction("Produit");

            }
            catch (Exception e)
            {
                return HttpNotFound();
            }
        }

        [HttpPost]
        public ActionResult ModifierProduit(Prod produit)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    dbObj.Entry(produit).State = EntityState.Modified;
                    dbObj.SaveChanges();
                }
                return RedirectToAction("Produit");
            }
            catch (Exception e)
            {
                return HttpNotFound();
            }
        }


        public ActionResult Catalogue()
        {
            ViewBag.listeCategorie = dbObj.Categories.ToList().OrderBy(r => r.LIBELLE_CATEGORIE);
            return View();
        }
        public ActionResult CatalogueTest()
        {
            ViewBag.listeCategorie = dbObj.Categories.ToList().OrderBy(r => r.LIBELLE_CATEGORIE);
            return View();
        }

        public ActionResult CatalogueClient()
        {
            ViewBag.listeCategorie = dbObj.Categories.ToList().OrderBy(r => r.LIBELLE_CATEGORIE);
            ViewModel mymodel = new ViewModel();
            mymodel.Prods = dbObj.Prod.ToList();
            mymodel.ventes = dbObj.vente.ToList();
            return View(mymodel);
        }


        // les methodes de la partie client


        public ActionResult ProduitClient(Prod obj)
        {

            try
            {

                ViewBag.listeProduit = dbObj.Prod.ToList();
                ViewBag.listeCategorie = dbObj.Categories.ToList();



                return View(obj);
            }
            catch (Exception e)
            {
                return HttpNotFound();
            }


        }
        [HttpPost]
        public ActionResult AddProduitClient(Prod model)
        {

            string message = "";

            if (ModelState.IsValid)
            {
                MyDatabaseEntities24 dbObj1 = new MyDatabaseEntities24();
                Prod obj = new Prod();
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0]; //le nom de notre fichier
                    if (file != null)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Fichier"), fileName);

                        file.SaveAs(path);

                        model.IMAGE_PRODUIT = fileName;
                        model.URL_IMAGE_PRODUIT = path;

                    }

                }

                model.DATE_SAISIE = DateTime.Now;
             //   model.remise = 0;
                model.status = "en attente";
                model.vu = 0;
               model.datedebut = DateTime.Now;
               model.datefin = DateTime.Now;
                model.type = Request.Form["Type"];
                model.IdUser = dbObj1.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().IdUser;  



                obj.DATE_SAISIE = model.DATE_SAISIE;
                obj.remise = model.remise;
                obj.status = "en attente";
                obj.DATE_SAISIE = DateTime.Now;
                obj.vu = model.vu;
                obj.datedebut = model.datedebut;
                obj.datefin = model.datefin;
                obj.type = model.type;
                obj.IdUser = model.IdUser;
                obj.LIBELLE_PRODUIT = model.LIBELLE_PRODUIT;
                obj.DESCRIPTION_PRODUIT = model.DESCRIPTION_PRODUIT;
                obj.IMAGE_PRODUIT = model.IMAGE_PRODUIT;
                obj.URL_IMAGE_PRODUIT = model.URL_IMAGE_PRODUIT;
                obj.CODE_CATEGORIE = model.CODE_CATEGORIE;
                obj.CODE_PRODUIT = model.CODE_PRODUIT;
                obj.Prix = model.Prix;
          //      obj.remise = model.remise;
                //     model.LIBELLE_PRODUIT = "gg"

                if (model.CODE_PRODUIT == 0)
                {

                    dbObj.Prod.Add(obj);
                    dbObj.SaveChanges();
                    message = "Ajout successfully done.";
                }
                else
                {
                    obj.remise = dbObj1.Prod.Where(a => a.CODE_PRODUIT == obj.CODE_PRODUIT).FirstOrDefault().remise;
                    dbObj.Entry(obj).State = EntityState.Modified;
                    dbObj.SaveChanges();
                    message = "modification successfully done.";
                }
            }

            ModelState.Clear();

            ViewBag.message = message;
            return View("ProduitClient");



        }

        public ActionResult ProduitListClient()
        {
          
            var res = dbObj.Prod.ToList();
            foreach (var item in res)
            {

                string p = item.datedebut.Day.ToString() + "/" + item.datedebut.Month.ToString() + "/" + item.datedebut.Year.ToString();
                string p1 = item.datefin.Day.ToString() + "/" + item.datefin.Month.ToString() + "/" + item.datefin.Year.ToString();

                string datenow = DateTime.Now.Day.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString();
                DateTime deb = DateTime.Parse(p);
                DateTime fin = DateTime.Parse(p1);
                DateTime now = DateTime.Parse(datenow);
                TimeSpan DiffRemise = fin - deb;
                TimeSpan DiffRemisef = now - deb;
                if (item.status != "remise" && DateTime.Now.Day - DateTime.Parse(item.datedebut.Day.ToString() + "/" + item.datedebut.Month.ToString() + "/" + item.datedebut.Year.ToString()).Day >= 0 && DateTime.Parse(item.datefin.Day.ToString() + "/" + item.datefin.Month.ToString() + "/" + item.datefin.Year.ToString()).Day - DateTime.Now.Day >= 0)
                {
                    var t = item.Prix / 100;
                    item.Prix = t * (100 - item.remise);
                    item.status = "remise";
                }

                if (item.status == "remise" && DateTime.Parse(item.datefin.Day.ToString() + "/" + item.datefin.Month.ToString() + "/" + item.datefin.Year.ToString()).Day - DateTime.Now.Day < 0)
                {
                    var t = item.Prix / (100 - item.remise);
                    item.Prix = t * 100;
                   item.status = "en attente";
                }
                dbObj.Entry(item).State = EntityState.Modified;
                dbObj.SaveChanges();
               

            }
            return View(res);
        }
        public ActionResult DeleteClient(int CODE_PRODUIT)
        {
            var res = dbObj.Prod.Where(x => x.CODE_PRODUIT == CODE_PRODUIT).First();
            dbObj.Prod.Remove(res);
            dbObj.SaveChanges();
            var list = dbObj.Prod.ToList();
            return View("ProduitListClient", list);
        }


        public ActionResult ProduitClientREMISE(Prod obj)
        {

            try
            {

                ViewBag.listeProduit = dbObj.Prod.ToList();
                ViewBag.listeCategorie = dbObj.Categories.ToList();



                return View(obj);
            }
            catch (Exception e)
            {
                return HttpNotFound();
            }


        }


        [HttpPost]
        public ActionResult AddRemiseProduitClient(Prod model)
        {

            string message = "";
            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {
                if (ModelState.IsValid)
                {
                    MyDatabaseEntities24 dbObj1 = new MyDatabaseEntities24();
                    Prod obj = new Prod();
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0]; //le nom de notre fichier
                        if (file != null)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var path = Path.Combine(Server.MapPath("~/Fichier"), fileName);

                            file.SaveAs(path);

                            model.IMAGE_PRODUIT = fileName;
                            model.URL_IMAGE_PRODUIT = path;

                        }

                    }
                //    var res = dc.Prod.Where(x => x.CODE_PRODUIT == id).First();
                    model.DATE_SAISIE = DateTime.Now;
                    //   model.remise = 0;
                    model.status = "en attente";
                    model.vu = 0;
               //     model.datedebut = DateTime.Now;
               //     model.datefin = DateTime.Now;
                    model.type = Request.Form["Type"];
                    model.IdUser = dbObj1.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().IdUser;



                    obj.DATE_SAISIE = model.DATE_SAISIE;
                    obj.remise = model.remise;
                    obj.status = "en attente";
                    obj.DATE_SAISIE = DateTime.Now;
                    obj.vu = model.vu;
                    obj.datedebut = model.datedebut;
                    obj.datefin = model.datefin;
                    obj.type = dbObj1.Prod.Where(a => a.CODE_PRODUIT == model.CODE_PRODUIT).FirstOrDefault().type;

                    obj.IdUser = model.IdUser;
                    obj.CODE_CATEGORIE = dbObj1.Prod.Where(a => a.CODE_PRODUIT == model.CODE_PRODUIT).FirstOrDefault().CODE_CATEGORIE;
                    obj.CODE_PRODUIT = model.CODE_PRODUIT;
                    obj.LIBELLE_PRODUIT = dbObj1.Prod.Where(a => a.CODE_PRODUIT == model.CODE_PRODUIT).FirstOrDefault().LIBELLE_PRODUIT;
                    obj.DESCRIPTION_PRODUIT = dbObj1.Prod.Where(a => a.CODE_PRODUIT == model.CODE_PRODUIT).FirstOrDefault().DESCRIPTION_PRODUIT;
                    obj.IMAGE_PRODUIT = dbObj1.Prod.Where(a => a.CODE_PRODUIT == model.CODE_PRODUIT).FirstOrDefault().IMAGE_PRODUIT;
                    obj.URL_IMAGE_PRODUIT = dbObj1.Prod.Where(a => a.CODE_PRODUIT == model.CODE_PRODUIT).FirstOrDefault().URL_IMAGE_PRODUIT;
                   
                    obj.Prix = dbObj1.Prod.Where(a => a.CODE_PRODUIT == model.CODE_PRODUIT).FirstOrDefault().Prix;
                    obj.remise = model.remise;
                    obj.DESCRIPTION_PRODUIT = dbObj1.Prod.Where(a => a.CODE_PRODUIT == model.CODE_PRODUIT).FirstOrDefault().DESCRIPTION_PRODUIT;
                    //   model.LIBELLE_PRODUIT = "gg"

                    if (model.CODE_PRODUIT == 0)
                    {

                        dbObj.Prod.Add(obj);
                        dbObj.SaveChanges();
                        message = "Ajout successfully done.";
                    }
                    else
                    {

                        //   var res = dc.Prod.Where(x => x.CODE_PRODUIT == model.CODE_PRODUIT).First();
                        //        dbObj.Entry(obj).State = EntityState.Modified;

                        string p = obj.datedebut.Day.ToString() + "/" + obj.datedebut.Month.ToString() + "/" + obj.datedebut.Year.ToString();
                        string p1 = obj.datefin.Day.ToString() + "/" + obj.datefin.Month.ToString() + "/" + obj.datefin.Year.ToString();

                        string datenow = DateTime.Now.Day.ToString() + "/" + DateTime.Now.Month.ToString() + "/" + DateTime.Now.Year.ToString();
                        DateTime deb = DateTime.Parse(p);
                        DateTime fin = DateTime.Parse(p1);
                        DateTime now = DateTime.Parse(datenow);
                        TimeSpan DiffRemise = fin - deb;
                        TimeSpan DiffRemisef = now - deb;
                        System.Diagnostics.Debug.WriteLine("DiffRemise : " + DiffRemise.Days + "DiffRemisef : " + DiffRemisef.Days);
                  /*      if (obj.remise != 0 && DateTime.Now.Day - DateTime.Parse(obj.datedebut.Day.ToString() + "/" + obj.datedebut.Month.ToString() + "/" + obj.datedebut.Year.ToString()).Day >= 0 && DateTime.Parse(obj.datefin.Day.ToString() + "/" + obj.datefin.Month.ToString() + "/" + obj.datefin.Year.ToString()).Day - DateTime.Now.Day >= 0)
                        {
                            var t = obj.Prix / 100;
                            obj.Prix = t * (100 - obj.remise);
                        }
                        else
                        {

                        }
                  */
                        dbObj.Entry(obj).State = EntityState.Modified;
                        dbObj.SaveChanges();
                        message = "modification successfully done.";
                    }
                }
            }

            ModelState.Clear();

            ViewBag.message = message;
            return View("ProduitClientREMISE");



        }

        [HttpPost]
        public ActionResult AddRemiseProduit(int id,int p)
        {

            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {
                var message = "";
              
                   
               




                try
                {
                    var res = dc.Prod.Where(x => x.CODE_PRODUIT == id).First();
                    //   var res1 = dc.Users.Where(x => x.IdUser == IdUser);
                    //   Users ord = new Users();

                    var query = (from ord in dc.Prod where ord.CODE_PRODUIT == id select ord);

                    //    var element = document.getElementById('product');
                    res.remise = p;
                    System.Diagnostics.Debug.WriteLine("id:  " + res.CODE_PRODUIT + "firstname:  " + p);

                 //   dc.Entry(res).State = EntityState.Modified;
                    dc.Configuration.ValidateOnSaveEnabled = false;
                    dc.SaveChanges();

                    message = "modification avec succées";
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                    // Provide for exceptions.
                }



                ModelState.Clear();

                ViewBag.message = message;
                return View("AddRemiseProduit");



            }

            



        }


        //hhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhhh


        public ActionResult ProduitClientREMISE1(Prod obj)
        {

            try
            {

                ViewBag.listeProduit = dbObj.Prod.ToList();
                ViewBag.listeCategorie = dbObj.Categories.ToList();



                return View(obj);
            }
            catch (Exception e)
            {
                return HttpNotFound();
            }


        }


        [HttpPost]
        public ActionResult AddRemiseProduitClient1(Prod model)
        {

            string message = "";
            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {
                if (ModelState.IsValid)
                {
                    MyDatabaseEntities24 dbObj1 = new MyDatabaseEntities24();
                    Prod obj = new Prod();
                    if (Request.Files.Count > 0)
                    {
                        var file = Request.Files[0]; //le nom de notre fichier
                        if (file != null)
                        {
                            var fileName = Path.GetFileName(file.FileName);
                            var path = Path.Combine(Server.MapPath("~/Fichier"), fileName);

                            file.SaveAs(path);

                            model.IMAGE_PRODUIT = fileName;
                            model.URL_IMAGE_PRODUIT = path;

                        }

                    }
                    //    var res = dc.Prod.Where(x => x.CODE_PRODUIT == id).First();
                    model.DATE_SAISIE = DateTime.Now;
                    //   model.remise = 0;
                    model.status = "en attente";
                    model.vu = 0;
                    model.datedebut = DateTime.Now;
                    model.datefin = DateTime.Now;
                    model.type = Request.Form["Type"];
                    model.IdUser = dbObj1.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().IdUser;



                    obj.DATE_SAISIE = model.DATE_SAISIE;
                    obj.remise = model.remise;
                    obj.status = "en attente";
                    obj.DATE_SAISIE = DateTime.Now;
                    obj.vu = model.vu;
                    obj.datedebut = model.datedebut;
                    obj.datefin = model.datefin;
                    obj.type = dbObj1.Prod.Where(a => a.CODE_PRODUIT == model.CODE_PRODUIT).FirstOrDefault().type;

                    obj.IdUser = model.IdUser;
                    obj.CODE_CATEGORIE = dbObj1.Prod.Where(a => a.CODE_PRODUIT == model.CODE_PRODUIT).FirstOrDefault().CODE_CATEGORIE;
                    obj.CODE_PRODUIT = model.CODE_PRODUIT;
                    obj.LIBELLE_PRODUIT = dbObj1.Prod.Where(a => a.CODE_PRODUIT == model.CODE_PRODUIT).FirstOrDefault().LIBELLE_PRODUIT;
                    obj.DESCRIPTION_PRODUIT = dbObj1.Prod.Where(a => a.CODE_PRODUIT == model.CODE_PRODUIT).FirstOrDefault().DESCRIPTION_PRODUIT;
                    obj.IMAGE_PRODUIT = dbObj1.Prod.Where(a => a.CODE_PRODUIT == model.CODE_PRODUIT).FirstOrDefault().IMAGE_PRODUIT;
                    obj.URL_IMAGE_PRODUIT = dbObj1.Prod.Where(a => a.CODE_PRODUIT == model.CODE_PRODUIT).FirstOrDefault().URL_IMAGE_PRODUIT;

                    obj.Prix = dbObj1.Prod.Where(a => a.CODE_PRODUIT == model.CODE_PRODUIT).FirstOrDefault().Prix;
                    obj.remise = model.remise;
                    obj.DESCRIPTION_PRODUIT = dbObj1.Prod.Where(a => a.CODE_PRODUIT == model.CODE_PRODUIT).FirstOrDefault().DESCRIPTION_PRODUIT;
                    //   model.LIBELLE_PRODUIT = "gg"

                    if (model.CODE_PRODUIT == 0)
                    {

                        dbObj.Prod.Add(obj);
                        dbObj.SaveChanges();
                        message = "Ajout successfully done.";
                    }
                    else
                    {

                        //   var res = dc.Prod.Where(x => x.CODE_PRODUIT == model.CODE_PRODUIT).First();
                        //        dbObj.Entry(obj).State = EntityState.Modified;
                        dbObj.Entry(obj).State = EntityState.Modified;
                        dbObj.SaveChanges();
                        message = "modification successfully done.";
                    }
                }
            }

            ModelState.Clear();

            ViewBag.message = message;
            return View("ProduitClientREMISE");



        }


        //nbre de vu incremente

        public ActionResult AddVu(int CODE_PRODUIT)
        {
          
                if (ModelState.IsValid)
            {
                MyDatabaseEntities24 dc = new MyDatabaseEntities24();
                //   var res1 = dc.Users.Where(x => x.IdUser == IdUser);
                var res = dc.Prod.Where(x => x.CODE_PRODUIT == CODE_PRODUIT).First();
                //   var res1 = dc.Users.Where(x => x.IdUser == IdUser);
                var query = (from ord in dc.Prod where ord.CODE_PRODUIT == CODE_PRODUIT select ord);
                foreach (Prod ord in query)
                {
                   
                    ord.vu +=1;
                    res.vu = ord.vu;
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
                //   obj.remise = model.remise;
                //     obj.DESCRIPTION_PRODUIT = dc.Prod.Where(a => a.CODE_PRODUIT == model.CODE_PRODUIT).FirstOrDefault().DESCRIPTION_PRODUIT;


                }
                ModelState.Clear();
                ViewBag.listeProduit = dbObj.Prod.ToList();
                ViewBag.listeCategorie = dbObj.Categories.ToList();
                return View("CatalogueClient");



            


        }

    }
}
