using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using RegistrationAndLogin.Models;
namespace RegistrationAndLogin.Controllers
{
    public class RoleController : Controller
    {
        // GET: Role

        public ActionResult Role(Roles obj)
        {
            return View(obj);
        }
        public ActionResult AddRole(Roles model)
        {
            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {
                string message = "";
                Roles obj = new Roles();
                if (ModelState.IsValid)
                {
                    obj.RoleID = model.RoleID;
                    obj.Rolename = model.Rolename;


                    if (model.RoleID == 0)
                    {
                        dc.Roles.Add(obj);
                        dc.SaveChanges();
                        message = "Ajout successfully done.";
                    }
                    else
                    {
                        dc.Entry(obj).State = EntityState.Modified;
                        dc.SaveChanges();
                        message = "Modification successfully done.";
                    }
                }
                ModelState.Clear();
                
                
                ViewBag.message = message;
                return View("Role");
            }
        }

        public ActionResult RoleList()
        {
            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {
                Roles obj = new Roles();

                var res = dc.Roles.ToList();
                return View(res);
            }
           
            
        }

        public ActionResult Delete(int RoleID)
        {
            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {
                var res = dc.Roles.Where(x => x.RoleID == RoleID).First();
                dc.Roles.Remove(res);
                dc.SaveChanges();
                var list = dc.Roles.ToList();
                return View("RoleList", list);
            }
        }
    }
}