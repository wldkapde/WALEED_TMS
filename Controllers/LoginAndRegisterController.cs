using System;
using System.Web;
using System.Web.Mvc;
using WALEED_TMS.DAL;

namespace WALEED_TMS.Controllers
{
    public class LoginAndRegisterController : Controller
    {
        private readonly UserDAL _userDAL;

       
        public LoginAndRegisterController()
        {
            _userDAL = new UserDAL();
        }

        // ✅ Login Page (No Redirect Loop)
        public ActionResult Index()
        {
            if (Session["UserEmail"] != null)
            {
                return RedirectToAction("Dashboard", "LoginAndRegister"); 
            }
            return View(); 
        }

        // ✅ Registration Page
        public ActionResult Registeration()
        {
            return View();
        }

        [HttpPost]
        public ActionResult RegisterUser(string FullName, string Dob, string Email, string Password, string ContactMethod, string hearAboutUs, string UserRole)
        {
            try
            {
                var userExists = _userDAL.CheckUserExists(Email);
                if (userExists)
                {
                    return Json(new { success = false, message = "User with this email already exists!" });
                }

                bool isInserted = _userDAL.InsertUser(FullName, Dob, Email, Password, ContactMethod, hearAboutUs, UserRole);

                if (isInserted)
                {
                    return Json(new { success = true, message = "Registration Successful! Your account is now active." });
                }
                else
                {
                    return Json(new { success = false, message = "Registration failed. Please try again." });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }


        [HttpPost]
        public JsonResult LoginUser(string email, string password)
        {
            try
            {
                int userID = _userDAL.GetUserIDByEmail(email);
                string userName = _userDAL.GetUserNameByEmail(email);
                string authResult = _userDAL.AuthenticateUser(email, password);
                string userRole = _userDAL.GetUserRoleByEmail(email);

                if (authResult.StartsWith("Success"))
                {
                    Session["UserEmail"] = email;
                    Session["UserName"] = userName;
                    Session["UserID"] = userID;
                    Session["UserRole"] = userRole;

                    System.Diagnostics.Debug.WriteLine("User Role: " + userRole);

                    string redirectUrl = (userRole == "ADMIN")? Url.Action("AdminDashboard", "ClientAndAdmin"): Url.Action("Dashboard", "ClientAndAdmin");

                    System.Diagnostics.Debug.WriteLine("Redirect URL: " + redirectUrl);

                    return Json(new { success = true, message = "Login successful!", userID = userID, redirectUrl = redirectUrl });
                }
                else
                {
                    return Json(new { success = false, message = authResult });
                }
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "An error occurred: " + ex.Message });
            }
        }








        public ActionResult Dashboard()
        {
            if (Session["UserEmail"] == null)
            {
                return RedirectToAction("Index", "LoginAndRegister"); 
            }
            return View(); 
        }

        
        public ActionResult Logout()
        {
            Session.Clear();
            Session.Abandon();
            HttpContext.Session.RemoveAll();

            Response.Cache.SetExpires(DateTime.UtcNow.AddMinutes(-1));
            Response.Cache.SetCacheability(HttpCacheability.NoCache);
            Response.Cache.SetNoStore();

            return RedirectToAction("Index", "LoginAndRegister");
        }

    }
}
