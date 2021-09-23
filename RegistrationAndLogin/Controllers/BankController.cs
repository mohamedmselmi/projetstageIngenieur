using RegistrationAndLogin.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RegistrationAndLogin.Controllers
{
    public class BankController : Controller
    {
        // GET: Bank
        MyDatabaseEntities24 dbObj = new MyDatabaseEntities24();
        // GET: Categorie
        public ActionResult Bank(Bank obj)
        {
            return View(obj);
        }

        [HttpPost]
        public ActionResult AddBank(Bank model)
        {
            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {

                Bank obj = new Bank();
                string message = "";
                if (ModelState.IsValid)
                {
                    obj.idbank = model.idbank;
                    obj.namebank = model.namebank;
                    obj.interestrate = model.interestrate;

                    if (model.idbank == 0)
                    {
                        MyDatabaseEntities24 dbObj = new MyDatabaseEntities24();
                        System.Diagnostics.Debug.WriteLine("idbank : " + obj.idbank + " idbank : " + obj.namebank + "interestrate : " + obj.interestrate);
                        dc.Bank.Add(obj);
                        dc.SaveChanges();
                        message = "Ajout successfully done.";
                    }
                    else
                    {
                        MyDatabaseEntities24 dbObj = new MyDatabaseEntities24();
                        dc.Entry(obj).State = EntityState.Modified;
                        dc.SaveChanges();
                        message = "modification successfully done.";
                    }
                }
                ModelState.Clear();

                ViewBag.message = message;
                return View("Bank");
            }
        }


        public ActionResult BankList()
        {
            var res = dbObj.Bank.ToList();
            return View(res);
        }
        public ActionResult Delete(int idbank)
        {
            var res = dbObj.Bank.Where(x => x.idbank == idbank).First();
            dbObj.Bank.Remove(res);
            dbObj.SaveChanges();
            var list = dbObj.Bank.ToList();
            return View("BankList", list);
        }

    }
}