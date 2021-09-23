using RegistrationAndLogin.Models;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace RegistrationAndLogin.Controllers
{
    public class LoanController : Controller
    {
        // GET: Loan
        MyDatabaseEntities24 dbObj = new MyDatabaseEntities24();
        // GET: Categorie
        public ActionResult Loan(loan obj)
        {
            return View(obj);
        }

        [HttpPost]
        public ActionResult AddLoan(loan model)
        {
            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {

                loan obj = new loan();
                string message = "";
                if (ModelState.IsValid)
                {
                    obj.id = model.id;
                    obj.loanamont = model.loanamont;
                    obj.loanterme = Int32.Parse(Request.Form["Loanterm"]);
                    obj.monthlyincome = model.monthlyincome;
                    obj.IdUser = dc.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().IdUser;
                    obj.idbank = model.idbank;
                    if (model.id == 0)
                    {
                        MyDatabaseEntities24 dbObj = new MyDatabaseEntities24();
                        System.Diagnostics.Debug.WriteLine("id : " + obj.id+" loanamont : " + obj.loanamont + " loanterme : " + obj.loanterme + "monthlyincome : " + obj.monthlyincome);
                        dc.loan.Add(obj);
                        dc.SaveChanges();
                        message = "Ajout successfully done.";
                    }
                    else
                    {
                        MyDatabaseEntities24 dbObj = new MyDatabaseEntities24();
                        System.Diagnostics.Debug.WriteLine("id : " + obj.id + " loanamont : " + obj.loanamont + " loanterme : " + obj.loanterme + "monthlyincome : " + obj.monthlyincome);
                        dc.Entry(obj).State = EntityState.Modified;
                        dc.SaveChanges();
                        message = "modification successfully done.";
                    }
                }
                ModelState.Clear();

                ViewBag.message = message;
                return View("Loan");
            }
        }


        public ActionResult LoanList()
        {
            //       var query1 = (from ord1 in dbObj.vente where ord1.status == "resolu" select ord1.CODE_PRODUIT);
            MyDatabaseEntities24 dc = new MyDatabaseEntities24();
       ///     var s = dc.Users.Where(a => a.EmailID == User.Identity.Name).FirstOrDefault().IdUser;
            var res = dbObj.loan.ToList();
            return View(res);
        }

        public ActionResult Delete(int id)
        {
            var res = dbObj.loan.Where(x => x.id == id).First();
            dbObj.loan.Remove(res);
            dbObj.SaveChanges();
            var list = dbObj.loan.ToList();
            return View("LoanList", list);
        }
    }
}