using RegistrationAndLogin.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RegistrationAndLogin.Controllers
{
    public class ContactController : Controller
    {
        MyDatabaseEntities24 dbObj1 = new MyDatabaseEntities24();
        // GET: Reclamation
        public ActionResult Contact(reclamation obj)
        {

            try
            {

                ViewBag.listeContact = dbObj1.reclamation.ToList();
                ViewBag.listeProduit = dbObj1.Prod.ToList();
               


                return View(obj);
            }
            catch (Exception e)
            {
                return HttpNotFound();
            }


        }


        [HttpPost]
        public ActionResult AddContact(reclamation model)
        {

            string message = "";

            if (ModelState.IsValid)
            {
                MyDatabaseEntities24 dbObj1 = new MyDatabaseEntities24();
                reclamation obj = new reclamation();
                obj.daterec = DateTime.Now;
                obj.titrerec = "message";
                obj.descriptionrec = model.descriptionrec;
                obj.IdUser = dbObj1.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().IdUser;
                obj.CODE_PRODUIT = model.CODE_PRODUIT;
              
                obj.statusrec = "contact";
                obj.idrec = model.idrec;
                if (model.idrec == 0)
                {

                    dbObj1.reclamation.Add(obj);
                    dbObj1.SaveChanges();
                    message = "Ajout successfully done.";
                }
                else
                {
                    dbObj1.Entry(obj).State = EntityState.Modified;
                    dbObj1.SaveChanges();
                    message = "modification successfully done.";
                }

            }




            ModelState.Clear();

            ViewBag.message = message;
            return View("Contact");
        }


        // espace admin


        public ActionResult ContactAdmin(reclamation obj)
        {

            try
            {

                ViewBag.listeContact = dbObj1.reclamation.ToList();
                ViewBag.listeProduit = dbObj1.Prod.ToList();



                return View(obj);
            }
            catch (Exception e)
            {
                return HttpNotFound();
            }


        }


        [HttpPost]
        public ActionResult AddContactAdmin(reclamation model)
        {

            string message = "";

            if (ModelState.IsValid)
            {
                MyDatabaseEntities24 dbObj1 = new MyDatabaseEntities24();
                reclamation obj = new reclamation();
                obj.daterec = DateTime.Now;
                obj.titrerec = "message";
                obj.descriptionrec = model.descriptionrec;
                obj.IdUser = dbObj1.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().IdUser;
                obj.CODE_PRODUIT = model.CODE_PRODUIT;
                obj.statusrec = "contact";
                obj.idrec = model.idrec;
                if (model.idrec == 0)
                {

                    dbObj1.reclamation.Add(obj);
                    dbObj1.SaveChanges();
                    message = "Ajout successfully done.";
                }
                else
                {
                    dbObj1.Entry(obj).State = EntityState.Modified;
                    dbObj1.SaveChanges();
                    message = "modification successfully done.";
                }

            }




            ModelState.Clear();

            ViewBag.message = message;
            return View("ContactAdmin");
        }

        //contact pour louer

        public ActionResult ContactLouer(reclamation obj)
        {

            try
            {

                ViewBag.listeContact = dbObj1.reclamation.ToList();
                ViewBag.listeProduit = dbObj1.Prod.ToList();



                return View(obj);
            }
            catch (Exception e)
            {
                return HttpNotFound();
            }


        }

        [HttpPost]
        public ActionResult AddContactLouer(reclamation model)
        {

            string message = "";

            if (ModelState.IsValid)
            {
                MyDatabaseEntities24 dbObj1 = new MyDatabaseEntities24();
                reclamation obj = new reclamation();
                obj.daterec = DateTime.Now;
                obj.titrerec = "messageLouer";
                obj.descriptionrec = model.descriptionrec;
                obj.IdUser = dbObj1.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().IdUser;
                obj.CODE_PRODUIT = model.CODE_PRODUIT;
                obj.statusrec = "contact";
                obj.idrec = model.idrec;
                if (model.idrec == 0)
                {

                    dbObj1.reclamation.Add(obj);
                    dbObj1.SaveChanges();
                    message = "Ajout successfully done.";
                }
                else
                {
                    dbObj1.Entry(obj).State = EntityState.Modified;
                    dbObj1.SaveChanges();
                    message = "modification successfully done.";
                }

            }




            ModelState.Clear();

            ViewBag.message = message;
            return View("ContactLouer");
        }

        //---------------------------------------------------------------------admin
        public ActionResult ContactAdminlouer(reclamation obj)
        {

            try
            {

                ViewBag.listeContact = dbObj1.reclamation.ToList();
                ViewBag.listeProduit = dbObj1.Prod.ToList();



                return View(obj);
            }
            catch (Exception e)
            {
                return HttpNotFound();
            }


        }


        [HttpPost]
        public ActionResult AddContactAdminLouer(reclamation model)
        {

            string message = "";

            if (ModelState.IsValid)
            {
                MyDatabaseEntities24 dbObj1 = new MyDatabaseEntities24();
                reclamation obj = new reclamation();
                obj.daterec = DateTime.Now;
                obj.titrerec = "messageLouer";
                obj.descriptionrec = model.descriptionrec;
                obj.IdUser = dbObj1.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().IdUser;
             //   var s = dbObj1.louer.Where(a => a.status == "en attente").Count().ToString();
                obj.CODE_PRODUIT = model.CODE_PRODUIT;
                obj.statusrec = "contact";
                obj.idrec = model.idrec;
                if (model.idrec == 0)
                {

                    dbObj1.reclamation.Add(obj);
                    dbObj1.SaveChanges();
                    message = "Ajout successfully done.";
                }
                else
                {
                    dbObj1.Entry(obj).State = EntityState.Modified;
                    dbObj1.SaveChanges();
                    message = "modification successfully done.";
                }

            }




            ModelState.Clear();

            ViewBag.message = message;
            return View("ContactAdminlouer");
        }


    }
}