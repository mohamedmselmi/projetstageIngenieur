using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using RegistrationAndLogin.Models;

namespace RegistrationAndLogin.Controllers
{
    public class UserController : Controller
    {
        // GET: User
        MyDatabaseEntities24 dc = new MyDatabaseEntities24();
        //registration Action
        [HttpGet]
        public ActionResult Registration()
        {
            //    ViewBag.listeRoles = dc.Roles.ToList();
            return View();
        }


        //Registration POST action
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Registration([Bind(Exclude = "IsEmailVerified,ActivationCode")] Users user)
        {

            bool Status = false;
            string message = "";
            //
            // Model Validation 
            if (ModelState.IsValid)
            {
                MyDatabaseEntities24 dbObj1 = new MyDatabaseEntities24();
                #region //Email is already Exist 
                var isExist = IsEmailExist(user.EmailID);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return View(user);
                }
                #endregion

                #region Generate Activation Code 
                user.ActivationCode = Guid.NewGuid();
                #endregion

                #region  Password Hashing 
                user.Password = Crypto.Hash(user.Password);
                user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword); //
                user.datedesactivation = user.DateOfBirth;
                #endregion
                user.IsEmailVerified = false;
                var v = dc.Roles.Where(a => a.Rolename == "Client").FirstOrDefault();
                user.RoleID = v.RoleID;
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0]; //le nom de notre fichier
                    if (file != null)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Fichier"), fileName);

                        file.SaveAs(path);

                        user.IMAGE_USER = fileName;
                        user.URL_IMAGE_USER = path;
                        System.Diagnostics.Debug.WriteLine("num telephone:  " + user.numtelephone + "URL_IMAGE_USER: " + user.URL_IMAGE_USER + "IMAGE_USER: " + user.IMAGE_USER);

                    }

                }
                #region Save to Database

                dbObj1.Users.Add(user);
                dbObj1.SaveChanges();

                //Send Email to User
                SendVerificationLinkEmail(user.EmailID, user.ActivationCode.ToString());
                message = "Registration successfully done. Account activation link " +
                    " has been sent to your email id:" + user.EmailID;
                Status = true;

                #endregion
            }
            else
            {
                message = "Invalid Request";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(user);
        }
        // admin registration
        [HttpGet]
        public ActionResult RegistrationAdmin()
        {

            ViewBag.listeRoles = dc.Roles.ToList();
            return View();

        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult RegistrationAdmin([Bind(Exclude = "IsEmailVerified,ActivationCode")] Users user)
        {

            bool Status = false;
            string message = "";
            //
            // Model Validation 
            if (ModelState.IsValid)
            {
                MyDatabaseEntities24 dbObj1 = new MyDatabaseEntities24();
                #region //Email is already Exist 
                var isExist = IsEmailExist(user.EmailID);
                if (isExist)
                {
                    ModelState.AddModelError("EmailExist", "Email already exist");
                    return View(user);
                }
                #endregion

                #region Generate Activation Code 
                user.ActivationCode = Guid.NewGuid();
                #endregion

                #region  Password Hashing 
                user.Password = Crypto.Hash(user.Password);
                user.ConfirmPassword = Crypto.Hash(user.ConfirmPassword); //
                user.datedesactivation = user.DateOfBirth;
                #endregion
                user.IsEmailVerified = false;
                if (Request.Files.Count > 0)
                {
                    Users obj = new Users();
                    var file = Request.Files[0]; //le nom de notre fichier
                    if (file != null)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Fichier"), fileName);

                        file.SaveAs(path);

                        user.IMAGE_USER = fileName;
                        user.URL_IMAGE_USER = path;
                        System.Diagnostics.Debug.WriteLine("num telephone:  " + user.numtelephone + "URL_IMAGE_USER: " + user.URL_IMAGE_USER + "IMAGE_USER: " + user.IMAGE_USER);

                    }

                }

                #region Save to Database
                if (user.IdUser == 0)
                {
                    dbObj1.Users.Add(user);
                    dbObj1.SaveChanges();
                }
                else
                {

                    //   System.Diagnostics.Debug.WriteLine("id:  " + res.IdUser + "firstname:  " + firstname);

                    //   dc.Entry(res).State = EntityState.Modified;
                    dc.Entry(user).State = EntityState.Modified;
                    dc.SaveChanges();

                    dc.Configuration.ValidateOnSaveEnabled = false;
                    dc.SaveChanges();
                }

                //Send Email to User
                SendVerificationLinkEmail(user.EmailID, user.ActivationCode.ToString());
                message = "Registration successfully done. Account activation link " +
                    " has been sent to your email id:" + user.EmailID;
                Status = true;

                #endregion
            }
            else
            {
                message = "Invalid Request";
            }

            ViewBag.Message = message;
            ViewBag.Status = Status;
            return View(user);
        }

        //verify account
        [HttpGet]
        public ActionResult VerifyAccount(string id)
        {
            bool Status = false;
            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {
                dc.Configuration.ValidateOnSaveEnabled = false; // this line I have added here to avoid
                                                                //Confirme password does not mutch issue on save changes
                var v = dc.Users.Where(a => a.ActivationCode == new Guid(id)).FirstOrDefault();

                if (v != null)
                {
                    v.IsEmailVerified = true;
                    dc.SaveChanges();
                    Status = true;
                }
                else
                {
                    ViewBag.Message = "Invalid Request";
                }

            }
            ViewBag.Status = Status;
            return View();
        }
        //login

        [HttpGet]
        public ActionResult Login()
        {
            return View();
        }
        //login POST
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Login(UserLogin login, string ReturnUrl = "")
        {
            string message = "";
            string message1 = "";
            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {
                var v = dc.Users.Where(a => a.EmailID == login.EmailID).FirstOrDefault();
                var ts = dc.Users.Where(a => a.RoleID == 21).Count();
                Console.Clear();
                System.Diagnostics.Debug.WriteLine("resultat : " + ts);

                var dtStart = v.datedesactivation.Day;
                var datef = DateTime.Now;
                DateTime dtStart1 = new DateTime(v.datedesactivation.Year, v.datedesactivation.Month, v.datedesactivation.Day);
                DateTime datef1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day);

                TimeSpan Diff_dates = datef1.Subtract(dtStart1);

                string p = v.datedesactivation.Day.ToString() + "/" + v.datedesactivation.Month.ToString() + "/" + v.datedesactivation.Year.ToString();
                DateTime deb = DateTime.Parse(p);
                DateTime fin = DateTime.Now;
                TimeSpan Diff = fin - deb;
                System.Diagnostics.Debug.WriteLine("diffenrece : " + Diff.Days);

                //   var z = 3 - (datef - v.datedesactivation.Day);
                if (v != null)
                {
                    if (!v.IsEmailVerified)
                    {
                        ViewBag.Message = "Please verify your email first";
                        return View();
                    }
                    if (string.Compare(Crypto.Hash(login.Password), v.Password) == 0)
                    {
                        int timeout = login.RememberMe ? 525600 : 20; // 525600 min = 1 year
                        var ticket = new FormsAuthenticationTicket(login.EmailID, login.RememberMe, timeout);
                        string encrypted = FormsAuthentication.Encrypt(ticket);
                        var cookie = new HttpCookie(FormsAuthentication.FormsCookieName, encrypted);
                        cookie.Expires = DateTime.Now.AddMinutes(timeout);
                        cookie.HttpOnly = true;
                        Response.Cookies.Add(cookie);



                        if (Url.IsLocalUrl(ReturnUrl))
                        {
                            return Redirect(ReturnUrl);
                        }

                        else if ((2 - Diff.Days) > 0)
                        {
                            var t = 2 - Diff.Days;
                            message1 = "votre compte est desactivé pour une periode de :" + t + " jours";
                        }
                        else if (v.RoleID == 21)
                        {
                            return RedirectToAction("Index", "Home");
                        }
                        else if (v.RoleID == 22)
                        {
                            return RedirectToAction("IndexClient", "Home");
                        }
                    }
                    else
                    {

                        message = "Invalid credential provided";
                    }
                }
                else
                {

                    message = "Invalid credential provided";
                }
            }
            ViewBag.Message = message;
            ViewBag.Message1 = message1;
            return View();
        }

        //Logout

        [Authorize]
        [HttpPost]
        public ActionResult Logout()
        {
            FormsAuthentication.SignOut();
            return RedirectToAction("Login", "User");
        }

        [NonAction]
        public bool IsEmailExist(string emailID)
        {

            var v = dc.Users.Where(a => a.EmailID == emailID).FirstOrDefault();
            return v != null;

        }

        [NonAction]
        public void SendVerificationLinkEmail(string emailID, string activationCode, string emailFor = "VerifyAccount")
        {
            var verifyUrl = "/User/" + emailFor + "/" + activationCode;
            var link = Request.Url.AbsoluteUri.Replace(Request.Url.PathAndQuery, verifyUrl);

            var fromEmail = new MailAddress("mohamedmselmi407@gmail.com", "Dotnet Awesome");
            var toEmail = new MailAddress(emailID);
            var fromEmailPassword = "mohamedMSELMI07226997"; // Replace with actual password

            string subject = "";
            string body = "";
            if (emailFor == "VerifyAccount")
            {
                subject = "Your account is successfully created!";
                body = "<br/><br/>We are excited to tell you that your Dotnet Awesome account is" +
                    " successfully created. Please click on the below link to verify your account" +
                    " <br/><br/><a href='" + link + "'>" + link + "</a> ";
            }
            else if (emailFor == "ResetPassword")
            {
                subject = "Reset Password";
                body = "Hi,<br/>br/>We got request for reset your account password. Please click on the below link to reset your password" +
                    "<br/><br/><a href=" + link + ">Reset Password link</a>";
            }


            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com",
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                UseDefaultCredentials = false,
                Credentials = new NetworkCredential(fromEmail.Address, fromEmailPassword)
            };

            using (var message = new MailMessage(fromEmail, toEmail)
            {
                Subject = subject,
                Body = body,
                IsBodyHtml = true
            })
                smtp.Send(message);
        }



        public ActionResult getprofil()
        {

            Users obj = new Users();

            var res = dc.Users.ToList();
            return View("getprofil", res);

        }
        public ActionResult getprofilClient()
        {

            Users obj = new Users();

            var res = dc.Users.ToList();
            return View("getprofilClient", res);

        }
        public ActionResult ListUsers()
        {

            Users obj = new Users();

            var res = dc.Users.ToList();
            return View("ListUsers", res);

        }

        //parte 3 -forget password

        [HttpGet]
        public ActionResult ForgotPassword()
        {
            return View();
        }


        [HttpPost]
        public ActionResult ForgotPassword(string EmailID)
        {
            //Verify Email ID
            //Generate Reset password link 
            //Send Email 
            string message = "";
            bool status = false;

            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {
                var account = dc.Users.Where(a => a.EmailID == EmailID).FirstOrDefault();
                if (account != null)
                {
                    //Send email for reset password
                    string resetCode = Guid.NewGuid().ToString();
                    SendVerificationLinkEmail(account.EmailID, resetCode, "ResetPassword");
                    account.ResetPasswordCode = resetCode;
                    //This line I have added here to avoid confirm password not match issue , as we had added a confirm password property 
                    //in our model class in part 1
                    dc.Configuration.ValidateOnSaveEnabled = false;
                    dc.SaveChanges();
                    message = "Reset password link has been sent to your email id.";
                }
                else
                {
                    message = "Account not found";
                }
            }
            ViewBag.Message = message;
            return View();
        }

        public ActionResult ResetPassword(string id)
        {
            //Verify the reset password link
            //Find account associated with this link
            //redirect to reset password page
            if (string.IsNullOrWhiteSpace(id))
            {
                return HttpNotFound();
            }

            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {
                var user = dc.Users.Where(a => a.ResetPasswordCode == id).FirstOrDefault();
                if (user != null)
                {
                    ResetPasswordModel model = new ResetPasswordModel();
                    model.ResetCode = id;
                    return View(model);
                }
                else
                {
                    return HttpNotFound();
                }
            }
        }


        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            var message = "";
            if (ModelState.IsValid)
            {
                using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
                {
                    var user = dc.Users.Where(a => a.ResetPasswordCode == model.ResetCode).FirstOrDefault();
                    if (user != null)
                    {
                        user.Password = Crypto.Hash(model.NewPassword);
                        user.ResetPasswordCode = "";
                        dc.Configuration.ValidateOnSaveEnabled = false;
                        dc.SaveChanges();
                        message = "New password updated successfully";
                    }
                }
            }
            else
            {
                message = "Something invalid";
            }
            ViewBag.Message = message;
            return View(model);
        }

        public ActionResult Delete(int IdUser)
        {
            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {
                var message = "";
                var t = dc.Users.Where(x => x.RoleID == 21).Count();
                var res = dc.Users.Where(x => x.IdUser == IdUser).First();
                if (t == 1 && res.RoleID == 21)
                {
                    message = "il faut avoir au moin un admin";

                }
                else
                {
                    dc.Users.Remove(res);
                    dc.SaveChanges();
                    message = "suppression successfully done.";
                }
                var list = dc.Users.ToList();
                ViewBag.Message = message;
                return View("ListUsers", list);
            }


        }


        public ActionResult desactivation(int IdUser)
        {
            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {
                var res = dc.Users.Where(x => x.IdUser == IdUser).First();
                //   var res1 = dc.Users.Where(x => x.IdUser == IdUser);
                var query = (from ord in dc.Users where ord.IdUser == IdUser select ord);
                foreach (Users ord in query)
                {
                    ord.datedesactivation = DateTime.Now;
                    res.datedesactivation = ord.datedesactivation;
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



                var list = dc.Users.ToList();
                return View("ListUsers", list);



            }


        }


        public ActionResult modif(Users model)
        {
            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {

              //  var res = dc.Users.Where(x => x.IdUser == iduser).First();
                //   var res1 = dc.Users.Where(x => x.IdUser == IdUser);
                //   Users ord = new Users();
                Users obj = new Users();
             //   MyDatabaseEntities21 dbObj1 = new MyDatabaseEntities21();
                //    var res = dc.Prod.Where(x => x.CODE_PRODUIT == id).First();




                obj.IdUser = model.IdUser;
                obj.IMAGE_USER = dc.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().IMAGE_USER;
                obj.FirstName = model.FirstName;
                obj.LastName = model.LastName;
                obj.DateOfBirth = dc.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().DateOfBirth;
                obj.IsEmailVerified = dc.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().IsEmailVerified;
                obj.EmailID = dc.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().EmailID;
                obj.Password = dc.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().Password;
                obj.numtelephone = model.numtelephone;
                var query = (from ord in dc.Users where ord.IdUser == obj.IdUser select ord);
               




                try
                {
                    //      System.Diagnostics.Debug.WriteLine("--------firstname:  " + obj.FirstName);

                    //     dc.Entry(obj).State = EntityState.Modified;
                    //     dc.Configuration.ValidateOnSaveEnabled = false;
                    //      dc.SaveChanges();
                    var res = dc.Users.ToList();
                    foreach (Users ord in res)
                    {
                        //    var element = document.getElementById('product');
                        if (ord.IdUser == obj.IdUser)
                        {
                            System.Diagnostics.Debug.WriteLine("boucle----firstname:  " + obj.FirstName);
                            ord.FirstName = obj.FirstName;
                            ord.LastName = obj.LastName;
                            ord.EmailID = obj.EmailID;
                            ord.numtelephone = obj.numtelephone;
                            dc.Configuration.ValidateOnSaveEnabled = false;
                            dc.SaveChanges();
                        }


                        //res.FirstName = ord.FirstName;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                    // Provide for exceptions.
                }



                var list = dc.Users.ToList();
                return View("getprofilClient", list);



            }


        }


        public ActionResult modifTest(Users model)
        {
            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {
                Users obj = new Users();
              
                //  string message = "";
                if (ModelState.IsValid)
                {
                    obj.IdUser = model.IdUser;
                    obj.FirstName = model.FirstName;
                    obj.LastName = model.LastName;
                    obj.numtelephone = model.numtelephone;

                    if (model.IdUser == 0)
                    {
                        dc.Users.Add(obj);
                        dc.SaveChanges();
                     //   message = "Ajout successfully done.";
                    }
                    else
                    {
                        model.numtelephone = 20251825;
                        dc.Configuration.ValidateOnSaveEnabled = false;
                        dc.SaveChanges();
                        //   message = "modification successfully done.";
                    }
                }
                ModelState.Clear();

                var list = dc.Users.ToList();
                return View("modifTest", model);
            }
        }



        //modif compte 

        public ActionResult modifClientProfil(Users obj)
        {

            try { 

            ViewBag.listeRoles = dc.Roles.ToList();
            return View(obj);
            }
            catch (Exception e)
            {
                return HttpNotFound();
            }



        }


        [HttpPost]
        public ActionResult AddmodifClientProfil(Users model)
        {

            string message = "";
            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {
                if (ModelState.IsValid)
                {
                    MyDatabaseEntities24 dbObj1 = new MyDatabaseEntities24();
                    Users obj = new Users();
                   
                    //    var res = dc.Prod.Where(x => x.CODE_PRODUIT == id).First();
     



                    obj.IdUser = model.IdUser;
                   obj.IMAGE_USER= dbObj1.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().IMAGE_USER;
                    obj.FirstName = model.FirstName;
                    obj.LastName = dbObj1.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().LastName;
                    obj.DateOfBirth = dbObj1.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().DateOfBirth;
                    obj.IsEmailVerified = dbObj1.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().IsEmailVerified;
                    obj.EmailID = dbObj1.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().EmailID;
                    obj.Password = dbObj1.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().Password;
                    obj.numtelephone = model.numtelephone;
                    //   model.LIBELLE_PRODUIT = "gg"

                    if (model.IdUser == 0)
                    {

                        dbObj1.Users.Add(obj);
                        dbObj1.SaveChanges();
                        message = "Ajout successfully done.";
                    }
                    else
                    {

                        //   var res = dc.Prod.Where(x => x.CODE_PRODUIT == model.CODE_PRODUIT).First();
                        //        dbObj.Entry(obj).State = EntityState.Modified;



                        dbObj1.Entry(obj).State = EntityState.Modified;
                        dbObj1.SaveChanges();
                        message = "modification successfully done.";
                    }
                }
            }

            ModelState.Clear();

            ViewBag.message = message;
            return View("modifClientProfil");



        }


        //modif profil image

        public ActionResult modifImageProfil(Users obj)
        {

            try
            {

                ViewBag.listeRoles = dc.Roles.ToList();
                return View(obj);
            }
            catch (Exception e)
            {
                return HttpNotFound();
            }



        }


        public ActionResult modifphoto(Users model)
        {
            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {

                //  var res = dc.Users.Where(x => x.IdUser == iduser).First();
                //   var res1 = dc.Users.Where(x => x.IdUser == IdUser);
                //   Users ord = new Users();
                Users obj = new Users();
                //   MyDatabaseEntities21 dbObj1 = new MyDatabaseEntities21();
                //    var res = dc.Prod.Where(x => x.CODE_PRODUIT == id).First();
                obj.IdUser = model.IdUser;
                //       obj.IMAGE_USER = model.IMAGE_USER;
                //   obj.URL_IMAGE_USER = model.URL_IMAGE_USER;
                obj.FirstName = dc.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().FirstName;
                obj.LastName = dc.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().LastName;
                obj.DateOfBirth = dc.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().DateOfBirth;
                obj.IsEmailVerified = dc.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().IsEmailVerified;
                obj.EmailID = dc.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().EmailID;
                obj.Password = dc.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().Password;
                obj.numtelephone = model.numtelephone;
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0]; //le nom de notre fichier
                    if (file != null)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Fichier"), fileName);

                        file.SaveAs(path);

                       obj.IMAGE_USER = fileName;
                        obj.URL_IMAGE_USER = path;
                //       System.Diagnostics.Debug.WriteLine("num telephone:  " + user.numtelephone + "URL_IMAGE_USER: " + user.URL_IMAGE_USER + "IMAGE_USER: " + user.IMAGE_USER);

                    }

                }


           
                var query = (from ord in dc.Users where ord.IdUser == obj.IdUser select ord);





                try
                {
                    //      System.Diagnostics.Debug.WriteLine("--------firstname:  " + obj.FirstName);

                    //     dc.Entry(obj).State = EntityState.Modified;
                    //     dc.Configuration.ValidateOnSaveEnabled = false;
                    //      dc.SaveChanges();
                    var res = dc.Users.ToList();
                    foreach (Users ord in res)
                    {
                        //    var element = document.getElementById('product');
                        if (ord.IdUser == obj.IdUser)
                        {
                            System.Diagnostics.Debug.WriteLine("boucle----IMAGE URL :  " + obj.URL_IMAGE_USER);
                            ord.IMAGE_USER = obj.IMAGE_USER;
                            ord.URL_IMAGE_USER = obj.URL_IMAGE_USER;
                          
                            dc.Configuration.ValidateOnSaveEnabled = false;
                            dc.SaveChanges();
                        }


                        //res.FirstName = ord.FirstName;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                    // Provide for exceptions.
                }



                var list = dc.Users.ToList();
                return View("getprofilClient", list);



            }


        }



        //update pour admin 

        public ActionResult modifImageProfilAdmin(Users obj)
        {

            try
            {

                ViewBag.listeRoles = dc.Roles.ToList();
                return View(obj);
            }
            catch (Exception e)
            {
                return HttpNotFound();
            }



        }


        public ActionResult modifphotoAdmin(Users model)
        {
            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {

                //  var res = dc.Users.Where(x => x.IdUser == iduser).First();
                //   var res1 = dc.Users.Where(x => x.IdUser == IdUser);
                //   Users ord = new Users();
                Users obj = new Users();
                //   MyDatabaseEntities21 dbObj1 = new MyDatabaseEntities21();
                //    var res = dc.Prod.Where(x => x.CODE_PRODUIT == id).First();
                obj.IdUser = model.IdUser;
                //       obj.IMAGE_USER = model.IMAGE_USER;
                //   obj.URL_IMAGE_USER = model.URL_IMAGE_USER;
                obj.FirstName = dc.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().FirstName;
                obj.LastName = dc.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().LastName;
                obj.DateOfBirth = dc.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().DateOfBirth;
                obj.IsEmailVerified = dc.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().IsEmailVerified;
                obj.EmailID = dc.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().EmailID;
                obj.Password = dc.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().Password;
                obj.numtelephone = model.numtelephone;
                if (Request.Files.Count > 0)
                {
                    var file = Request.Files[0]; //le nom de notre fichier
                    if (file != null)
                    {
                        var fileName = Path.GetFileName(file.FileName);
                        var path = Path.Combine(Server.MapPath("~/Fichier"), fileName);

                        file.SaveAs(path);

                        obj.IMAGE_USER = fileName;
                        obj.URL_IMAGE_USER = path;
                        //       System.Diagnostics.Debug.WriteLine("num telephone:  " + user.numtelephone + "URL_IMAGE_USER: " + user.URL_IMAGE_USER + "IMAGE_USER: " + user.IMAGE_USER);

                    }

                }



                var query = (from ord in dc.Users where ord.IdUser == obj.IdUser select ord);





                try
                {
                    //      System.Diagnostics.Debug.WriteLine("--------firstname:  " + obj.FirstName);

                    //     dc.Entry(obj).State = EntityState.Modified;
                    //     dc.Configuration.ValidateOnSaveEnabled = false;
                    //      dc.SaveChanges();
                    var res = dc.Users.ToList();
                    foreach (Users ord in res)
                    {
                        //    var element = document.getElementById('product');
                        if (ord.IdUser == obj.IdUser)
                        {
                            System.Diagnostics.Debug.WriteLine("boucle----IMAGE URL :  " + obj.URL_IMAGE_USER);
                            ord.IMAGE_USER = obj.IMAGE_USER;
                            ord.URL_IMAGE_USER = obj.URL_IMAGE_USER;

                            dc.Configuration.ValidateOnSaveEnabled = false;
                            dc.SaveChanges();
                        }


                        //res.FirstName = ord.FirstName;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                    // Provide for exceptions.
                }



                var list = dc.Users.ToList();
                return View("getprofil", list);



            }


        }

        public ActionResult modifAdminProfil(Users obj)
        {

            try
            {

                ViewBag.listeRoles = dc.Roles.ToList();
                return View(obj);
            }
            catch (Exception e)
            {
                return HttpNotFound();
            }



        }


        [HttpPost]
        public ActionResult AddmodifAdminProfil(Users model)
        {

            string message = "";
            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {
                if (ModelState.IsValid)
                {
                    MyDatabaseEntities24 dbObj1 = new MyDatabaseEntities24();
                    Users obj = new Users();

                    //    var res = dc.Prod.Where(x => x.CODE_PRODUIT == id).First();




                    obj.IdUser = model.IdUser;
                    obj.IMAGE_USER = dbObj1.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().IMAGE_USER;
                    obj.FirstName = model.FirstName;
                    obj.LastName = dbObj1.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().LastName;
                    obj.DateOfBirth = dbObj1.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().DateOfBirth;
                    obj.IsEmailVerified = dbObj1.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().IsEmailVerified;
                    obj.EmailID = dbObj1.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().EmailID;
                    obj.Password = dbObj1.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().Password;
                    obj.numtelephone = model.numtelephone;
                    //   model.LIBELLE_PRODUIT = "gg"

                    if (model.IdUser == 0)
                    {

                        dbObj1.Users.Add(obj);
                        dbObj1.SaveChanges();
                        message = "Ajout successfully done.";
                    }
                    else
                    {

                        //   var res = dc.Prod.Where(x => x.CODE_PRODUIT == model.CODE_PRODUIT).First();
                        //        dbObj.Entry(obj).State = EntityState.Modified;



                        dbObj1.Entry(obj).State = EntityState.Modified;
                        dbObj1.SaveChanges();
                        message = "modification successfully done.";
                    }
                }
            }

            ModelState.Clear();

            ViewBag.message = message;
            return View("modifAdminProfil");



        }


        public ActionResult modifAdmin(Users model)
        {
            using (MyDatabaseEntities24 dc = new MyDatabaseEntities24())
            {

                //  var res = dc.Users.Where(x => x.IdUser == iduser).First();
                //   var res1 = dc.Users.Where(x => x.IdUser == IdUser);
                //   Users ord = new Users();
                Users obj = new Users();
                //   MyDatabaseEntities21 dbObj1 = new MyDatabaseEntities21();
                //    var res = dc.Prod.Where(x => x.CODE_PRODUIT == id).First();




                obj.IdUser = model.IdUser;
                obj.IMAGE_USER = dc.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().IMAGE_USER;
                obj.FirstName = model.FirstName;
                obj.LastName = model.LastName;
                obj.DateOfBirth = dc.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().DateOfBirth;
                obj.IsEmailVerified = dc.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().IsEmailVerified;
                obj.EmailID = dc.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().EmailID;
                obj.Password = dc.Users.Where(a => a.IdUser == obj.IdUser).FirstOrDefault().Password;
                obj.numtelephone = model.numtelephone;
                var query = (from ord in dc.Users where ord.IdUser == obj.IdUser select ord);





                try
                {
                    //      System.Diagnostics.Debug.WriteLine("--------firstname:  " + obj.FirstName);

                    //     dc.Entry(obj).State = EntityState.Modified;
                    //     dc.Configuration.ValidateOnSaveEnabled = false;
                    //      dc.SaveChanges();
                    var res = dc.Users.ToList();
                    foreach (Users ord in res)
                    {
                        //    var element = document.getElementById('product');
                        if (ord.IdUser == obj.IdUser)
                        {
                            System.Diagnostics.Debug.WriteLine("boucle----firstname:  " + obj.FirstName);
                            ord.FirstName = obj.FirstName;
                            ord.LastName = obj.LastName;
                            ord.EmailID = obj.EmailID;
                            ord.numtelephone = obj.numtelephone;
                            dc.Configuration.ValidateOnSaveEnabled = false;
                            dc.SaveChanges();
                        }


                        //res.FirstName = ord.FirstName;
                    }

                }
                catch (Exception e)
                {
                    Console.WriteLine(e);

                    // Provide for exceptions.
                }



                var list = dc.Users.ToList();
                return View("getprofil", list);



            }


        }


    }
}

    