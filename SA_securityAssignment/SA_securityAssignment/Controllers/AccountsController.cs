using System;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using BusinessLogic;
using Common;
using Newtonsoft.Json;

namespace SA_securityAssignment.Controllers
{
    public class AccountsController : Controller
    {
        #region Login

        public ActionResult Login()
        {
            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken] 
        public ActionResult Login(string username, string password) {

            try
            {
                UsersBL uBL = new UsersBL();

                //if user & pass are valid an Auth ticket will be created for this user and will be redircted to the index.
                if (uBL.Login(username, password))
                {
                    //checks if user is blocked.
                    if (uBL.IsUserBlocked(username) && uBL.NumOfAttemps(username) >= 3)
                    {
                        throw new CustomException("This Account is Blocked!");
                    }
                    else
                    {
                        if (uBL.NumOfAttemps(username) < 3)
                        {

                            uBL.ResetAttemps(username);

                            FormsAuthentication.SetAuthCookie(username, true);
                            Logger.Log(username, Request.Path, "Successfully logged in");
                            return RedirectToAction("index", "Tracks"); //method and controller names
                        }

                        return View();
                    }

                }
                else
                {
                    //Manual validation.
                    if (username == "")
                    {
                        TempData["errormessage"] = "Please Enter Username";
                        return View();
                    }

                    else if (password == "")
                    {
                        TempData["errormessage"] = "Please Enter Password";
                        return View();
                    }

                    //if user available in DB
                    User UserAvailableInDB = uBL.GetUser(username);
                    if (UserAvailableInDB != null) //user available in db
                    {
                        uBL.IncreaseAttemps(username);

                        if (uBL.NumOfAttemps(username) >= 3)
                        {
                            uBL.BlockUser(username);
                            Logger.Log(username, Request.Path, "This Account is Blocked!");
                            throw new CustomException("This Account is Blocked!");
                        }

                        throw new CustomException("Login failed");
                        //TempData["errormessage"] = "login failed";
                        //return View();
                    }
                  
                    throw new CustomException("Invalid credentials");
                }

            }
            catch (CustomException ex)
            {
                TempData["errormessage"] = ex.Message;
                return View();
            }

            catch (Exception ex)
            {
                TempData["errormessage"] = ex.Message;
                return View();
            }

        }
        #endregion

        public ActionResult Signout()
        {
            FormsAuthentication.SignOut();
            Logger.Log(User.Identity.Name, Request.Path, "User Signed Out");
            return RedirectToAction("Login");
        }



        #region Register

        public ActionResult Register()
        { 
            ViewBag.Message = "Register";
            return View(); 
        }

    
        [HttpPost]
        public ActionResult Register(User u)
        {
            try
            {
                if (u.Username == null || u.Password == null || u.Email == null || u.Country == null)
                {
                    throw new CustomException("Please fill all fields");
                }
                else
                {
                    UsersBL uBl = new UsersBL();
                    if (uBl.UserAlreadyExists(u.Username))
                    {
                        throw new CustomException("Username Already Exists");
                    }
                    else if (uBl.EmailAlreadyExists(u.Email))
                    {
                        throw new CustomException("Email Already Exists");
                    }
                    else
                    {
                        CaptchaResponse response = ValidateCaptcha(Request.Form["g-recaptcha-response"]);


                        if (response.Success && ModelState.IsValid) //ModelState.isValid: if an input passed is not like the model it will return false //response.Success (captcha resposne)
                        {
                            uBl.Register(u);
                            TempData["message"] = "Account Successfully Registered.";
                            Logger.Log("Guest", Request.Path, "Registered as " + u.Username);
                            return RedirectToAction("Login", "Accounts");
                        }
                        else {
                            throw new CustomException("CAPTCHA Error: " + response.ErrorMessage[0].ToString());
                        }
                    }


                }
            }

            catch (CustomException ex) {
                TempData["errormessage"] = ex.Message;
                Logger.Log(User.Identity.Name, Request.Path, ex.Message);
                return View();
            }

            catch (Exception ex)
            {
                TempData["errormessage"] = ex.Message;
                Logger.Log(User.Identity.Name, Request.Path, ex.Message);
                return View();
            }
        }

        #endregion

        #region Captcha Methods
        //Captcha methods
        public static CaptchaResponse ValidateCaptcha(string response)
        {
            string secret = System.Web.Configuration.WebConfigurationManager.AppSettings["secretKey"];
            var client = new WebClient();
            var jsonResult = client.DownloadString(string.Format("https://www.google.com/recaptcha/api/siteverify?secret={0}&response={1}", secret, response));
            return JsonConvert.DeserializeObject<CaptchaResponse>(jsonResult.ToString());
        }


        //Model for JSON fields.
        public class CaptchaResponse
        {
            //api link.
            //https://www.google.com/recaptcha/api/siteverify

            [JsonProperty("success")]
            public bool Success { get; set; }

            [JsonProperty("error-codes")]
            public List<string> ErrorMessage { get; set; }
        }


        #endregion
    }
}