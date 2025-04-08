using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using WALEED_TMS.DAL;
using System.Net;
using System.Net.Mail;
using WALEED_TMS.Models;
using System.Linq;

namespace WALEED_TMS.Controllers
{
    public class ClientAndAdminController : Controller
    {
        private readonly TourDAL _tourDAL = new TourDAL();
        private readonly UserDAL _userDAL = new UserDAL();

        public ActionResult Dashboard()
        {
            return View();
        }

        public ActionResult AdminDashboard()
        {
            List<TourBookingModel> pendingTours = _tourDAL.GetPendingTours(); // Fetch pending tours
            return View(pendingTours); 
        }

        #region [Accept Tour]

        [HttpPost]
        public JsonResult AcceptTour(int tourID)
        {
            string userEmail = _tourDAL.UpdateTourStatus(tourID, "Confirmed");

            if (!string.IsNullOrEmpty(userEmail))
            {
                int userId = _userDAL.GetUserIDByEmail(userEmail); // 🔴 UserID fetch kar rahe hain
                if (userId > 0)
                {
                    SendConfirmationEmail(userEmail, userId); // ✅ Email ab dynamic data ke saath jayega!
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false, message = "User email not found." });
        }




        private void SendConfirmationEmail(string toEmail, int userId)
        {
            try
            {
                TourBookingModel tour = _tourDAL.GetTourDetailsByUser1(userId); // ✅ Ab ek hi record milega

                if (tour == null)
                {
                    System.Diagnostics.Debug.WriteLine("No tours found for email: " + toEmail);
                    return; // ❌ Agar koi tour nahi mila toh email nahi bhejna
                }

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("waleedkapde786@gmail.com");
                mail.To.Add(toEmail);
                mail.Subject = "Tour Confirmation - Waleed Tourism Management System";

                // ✅ Ab GetEmailTemplate ko ek single tour pass karenge
                mail.Body = GetEmailTemplate(tour);
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("waleedkapde786@gmail.com", "fnvsmqlxpokcfbyx");
                smtp.EnableSsl = true;

                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Email Error: " + ex.Message);
            }
        }






        private string GetEmailTemplate(TourBookingModel tour) 
        {
            string tableRow = $@"
        <tr>
            <td>{tour.TourLocation}</td>
            <td>{tour.TourStartDate:dd-MM-yyyy}</td> <!-- ✅ Updated Format -->
            <td>{tour.TourEndDate:dd-MM-yyyy}</td>   <!-- ✅ Updated Format -->
            <td>{tour.Adults}</td>
            <td>{tour.Children}</td>
            <td><b style='color: {(tour.TourStatus == "Confirmed" ? "green" : "red")};'>{tour.TourStatus}</b></td>
        </tr>";

            return $@"
    <html>
    <head>
        <style>
            .email-container {{
                font-family: Arial, sans-serif;
                padding: 20px;
                background-color: #f4f4f4;
            }}
            .email-content {{
                background: white;
                padding: 20px;
                border-radius: 8px;
                box-shadow: 0px 0px 10px #ccc;
            }}
            .email-header {{
                text-align: center;
                font-size: 20px;
                font-weight: bold;
                color: linear-gradient(to right, #cc2b5e, #753a88)!important;
            }}
            .email-table {{
                width: 100%;
                border-collapse: collapse;
                margin-top: 20px;
            }}
            .email-table th, .email-table td {{
                border: 1px solid #ddd;
                padding: 8px;
                text-align: center;
            }}
            .email-table th {{
                  //background: linear-gradient(to right, #cc2b5e, #753a88);
                color: white;
            }}
  thead {{
            background: linear-gradient(to right, #cc2b5e, #753a88);
        }}

            .email-footer {{
                text-align: center;
                margin-top: 20px;
                font-size: 14px;
                color: #555;
            }}

        </style>
    </head>
    <body>
        <div class='email-container'>
            <div class='email-content'>
                <div class='email-header'>Tour Booking Confirmation</div>
                <p>Dear User,</p>
                <p>Your tour has been successfully confirmed. Below are your tour details:</p>
                <table class='email-table'>
<thead>
 <tr>
                        <th>Tour Location</th>
                        <th>Start Date</th>
                        <th>End Date</th>
                        <th>Adults</th>
                        <th>Children</th>
                        <th>Status</th>
                    </tr>
</thead>
                   
                    {tableRow}  
                </table>
                <p>If you have any queries, please contact our support team or the admin directly.</p>
                    <p><strong>Admin Contact:</strong></p>
                    <p>Name: Waleed Kapade</p>
                    <p>Email: <a href='mailto:wldkapde@gmail.com'>wldkapde@gmail.com</a> / <a href='mailto:waleedkapde786@gmail.com'>waleedkapde786@gmail.com</a></p>
                    <p>Phone: +91 9022879875</p>
                    <div class='email-footer'>Waleed Tourism Management System</div>
            </div>
        </div>
    </body>
    </html>";
        }

        #endregion


        #region [Reject Tour]
        [HttpPost]
        public JsonResult RejectTour(int tourID)
        {
            string userEmail = _tourDAL.UpdateTourStatus(tourID, "Rejected");

            if (!string.IsNullOrEmpty(userEmail))
            {
                int userId = _userDAL.GetUserIDByEmail(userEmail);  
                if (userId > 0)
                {
                    SendRejectionEmail(userEmail, userId); 
                    return Json(new { success = true });
                }
            }

            return Json(new { success = false, message = "User email not found." });
        }

       
        private void SendRejectionEmail(string toEmail, int userId)
        {
            try
            {
                TourBookingModel tour = _tourDAL.GetTourDetailsByUser1(userId); 

                if (tour == null)
                {
                    System.Diagnostics.Debug.WriteLine("No tours found for email: " + toEmail);
                    return;
                }

                MailMessage mail = new MailMessage();
                mail.From = new MailAddress("waleedkapde786@gmail.com");
                mail.To.Add(toEmail);
                mail.Subject = "Tour Rejected - Waleed Tourism Management System";

                // ✅ Use same template but add rejection message
                mail.Body = GetRejectionEmailTemplate(tour);
                mail.IsBodyHtml = true;

                SmtpClient smtp = new SmtpClient("smtp.gmail.com", 587);
                smtp.UseDefaultCredentials = false;
                smtp.Credentials = new NetworkCredential("waleedkapde786@gmail.com", "fnvsmqlxpokcfbyx");
                smtp.EnableSsl = true;

                smtp.Send(mail);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine("Email Error: " + ex.Message);
            }
        }

        private string GetRejectionEmailTemplate(TourBookingModel tour)
        {
            string tableRow = $@"
        <tr>
            <td>{tour.TourLocation}</td>
            <td>{tour.TourStartDate:dd-MM-yyyy}</td>
            <td>{tour.TourEndDate:dd-MM-yyyy}</td>
            <td>{tour.Adults}</td>
            <td>{tour.Children}</td>
            <td><b style='color: red;'>{tour.TourStatus}</b></td>
        </tr>";

            return $@"
    <html>
    <head>
        <style>
            .email-container {{
                font-family: Arial, sans-serif;
                padding: 20px;
                background-color: #f4f4f4;
            }}
            .email-content {{
                background: white;
                padding: 20px;
                border-radius: 8px;
                box-shadow: 0px 0px 10px #ccc;
            }}
            .email-header {{
                text-align: center;
                font-size: 20px;
                font-weight: bold;
                color: red;
            }}
            .email-table {{
                width: 100%;
                border-collapse: collapse;
                margin-top: 20px;
            }}
            .email-table th, .email-table td {{
                border: 1px solid #ddd;
                padding: 8px;
                text-align: center;
            }}
            .email-table th {{
                color: white;
            }}
            thead {{
                background: linear-gradient(to right, #cc2b5e, #753a88);
            }}
            .email-footer {{
                text-align: center;
                margin-top: 20px;
                font-size: 14px;
                color: #555;
            }}
        </style>
    </head>
    <body>
        <div class='email-container'>
            <div class='email-content'>
                <div class='email-header'>Tour Rejected</div>
                <p>Dear User,</p>
                <p>We regret to inform you that your tour booking has been rejected. Below are your tour details:</p>
                <table class='email-table'>
                    <thead>
                        <tr>
                            <th>Tour Location</th>
                            <th>Start Date</th>
                            <th>End Date</th>
                            <th>Adults</th>
                            <th>Children</th>
                            <th>Status</th>
                        </tr>
                    </thead>
                    {tableRow}
                </table>
                <p>For further assistance, please contact our support team.</p>
                <p><strong>Admin Contact:</strong></p>
                <p>Name: Waleed Kapade</p>
                <p>Email: <a href='mailto:wldkapde@gmail.com'>wldkapde@gmail.com</a> / <a href='mailto:waleedkapde786@gmail.com'>waleedkapde786@gmail.com</a></p>
                <p>Phone: +91 9022879875</p>
                <div class='email-footer'>Waleed Tourism Management System</div>
            </div>
        </div>
    </body>
    </html>";
        }

        #endregion


        #region [View Status]
        public ActionResult ViewStatus()
        {
            string userEmail = Session["UserEmail"] as string;
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Index", "LoginAndRegister"); 
            }

            int userId = _userDAL.GetUserIDByEmail(userEmail);
            Debug.WriteLine("Fetched UserID: " + userId); // 🔴 Debugging

            if (userId == 0)
            {
                return RedirectToAction("Index", "LoginAndRegister");
            }

            List<TourBookingModel> tours = _tourDAL.GetTourDetailsByUser(userId);
            Debug.WriteLine("Number of Tours Found: " + tours.Count); // 🔴 Debugging

            return View(tours); 
        }
        #endregion

        #region [Manage User]
        public ActionResult ManageUsers()
        {
            var users = _userDAL.GetAllUsers(); // We'll create this method in UserDAL
            return View(users);
        }

        [HttpPost]
        public JsonResult UpdateUserStatus(int userId, bool isActive)
        {
            try
            {
                bool success = _userDAL.UpdateUserActiveStatus(userId, isActive ? "Y" : "N");
                return Json(new { success });
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating user status: " + ex.Message);
                return Json(new { success = false });
            }
        }


        public ActionResult EditUser(int userId)
        {
            var user = _userDAL.GetUserDataByUserID(userId);
            if (user == null)
            {
                return HttpNotFound();
            }

            var hearAboutUsOptions = _userDAL.GetHearAboutUsOptions();
            var preferredContactOptions = _userDAL.GetPreferredContactOptions();

            ViewBag.HearAboutUsOptions = hearAboutUsOptions.Any() ? hearAboutUsOptions : new List<string> { "No options available" };
            ViewBag.PreferredContactOptions = preferredContactOptions.Any() ? preferredContactOptions : new List<string> { "No options available" };

            return View(user);
        }

        [HttpPost]
        public ActionResult EditUser(UserModel user)
        {
            if (ModelState.IsValid)
            {
                bool success = _userDAL.UpdateUserData(user);
                if (success)
                {
                    return RedirectToAction("ManageUsers");
                }
            }
            return View(user);
        }
        #endregion


        #region [View Invoice]
        //public ActionResult DownloadInvoice()
        //{
        //    string userEmail = Session["UserEmail"] as string;
        //    if (string.IsNullOrEmpty(userEmail))
        //    {
        //        return RedirectToAction("Index", "LoginAndRegister");
        //    }

        //    int userId = _userDAL.GetUserIDByEmail(userEmail);
        //    if (userId == 0)
        //    {
        //        return RedirectToAction("Index", "LoginAndRegister");
        //    }

        //    var confirmedTours = _userDAL.GetConfirmedTours(userId);
        //    return View(confirmedTours);
        //}

        public ActionResult DownloadInvoice()
        {
            
            int userId = Convert.ToInt32(Session["UserID"]);
            string userEmail = Session["UserEmail"]?.ToString();

            UserDAL userDal = new UserDAL();

            // Get user's name
            string userName = userDal.GetUserNameByEmail(userEmail);
            ViewBag.UserName = userName;

            // Get confirmed tours for the user with estimate data
            List<ConfirmedTourModel> confirmedTours = userDal.GetConfirmedTours(userId);

            return View(confirmedTours);
        }

        private decimal GetTourEstimate(int journeyId)
        {
           
            Random random = new Random();
            return random.Next(100, 500);
        }
        #endregion



    }


}



