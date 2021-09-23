using RegistrationAndLogin.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RegistrationAndLogin.Controllers
{
    public class CategorieController : Controller
    {
        MyDatabaseEntities24 dbObj = new MyDatabaseEntities24();
        // GET: Categorie
        public ActionResult Categorie(Categories obj)
        {
            return View(obj);
        }

        [HttpPost]
        public ActionResult AddCategorie(Categories model)
        {
            Categories obj = new Categories();
            string message = "";
            if (ModelState.IsValid)
            {
                obj.CODE_CATEGORIE = model.CODE_CATEGORIE;
                obj.LIBELLE_CATEGORIE = model.LIBELLE_CATEGORIE;
                obj.DATE_SAISIE = DateTime.Now;

                if (model.CODE_CATEGORIE == 0)
                {
                    dbObj.Categories.Add(obj);
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
            return View("Categorie");
        }


        public ActionResult CategorieList()
        {
            var res = dbObj.Categories.ToList();
            return View(res);
        }
        public ActionResult Delete(int CODE_CATEGORIE)
        {
            var res = dbObj.Categories.Where(x => x.CODE_CATEGORIE == CODE_CATEGORIE).First();
            dbObj.Categories.Remove(res);
            dbObj.SaveChanges();
            var list = dbObj.Categories.ToList();
            return View("CategorieList", list);
        }
    }
}