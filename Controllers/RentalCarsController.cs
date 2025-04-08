using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WALEED_TMS.DAL;
using WALEED_TMS.Models;

namespace WALEED_TMS.Controllers
{
    public class RentalCarsController : Controller
    {
        private RentalCarDAL rentalCarDAL = new RentalCarDAL();

        // GET: RentalCars
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult RentalRequests()
        {
            var rentals = rentalCarDAL.GetAllCarRentals();
            return View(rentals);
        }

        [HttpPost]
        public JsonResult UpdateRentalStatus(int bookingId, string status)
        {
            bool success = rentalCarDAL.UpdateRentalStatus(bookingId, status);
            return Json(new { success = success, message = success ? "Status updated successfully" : "Failed to update status" });
        }

        [HttpGet]
        public JsonResult CheckCarAvailability(int carId, DateTime fromDate, DateTime toDate)
        {
            var status = rentalCarDAL.CheckCarAvailability(carId, fromDate, toDate);
            string message = status == "Confirmed"
                ? "Car is not available for rental"
                : "Car is available for rental";

            return Json(new { success = true, message = message }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MyRentals()
        {
            string userEmail = Session["UserEmail"]?.ToString();
            if (string.IsNullOrEmpty(userEmail))
            {
                return RedirectToAction("Login", "Account");
            }

            UserDAL userDal = new UserDAL();
            int userId = userDal.GetUserIDByEmail(userEmail);

            // Get the rentals
            var rentals = rentalCarDAL.GetUserCarRentals(userId);

            // Convert CarRentalModel to CarRentalStatusModel
            var rentalStatusList = rentals.Select(r => new CarRentalStatusModel
            {
                BookingID = r.BookingID,
                UserID = r.UserID,
                CarID = r.CarID,
                CarName = r.CarName,
                FromDate = r.FromDate,
                ToDate = r.ToDate,
                PickupLocation = r.PickupLocation,
                DropLocation = r.DropLocation,
                NumberOfPassengers = r.NumberOfPassengers,
                TotalPrice = r.TotalPrice,
                Status = r.Status,
                BookingDate = r.BookingDate,
                UserName = userDal.GetUserNameByEmail(userEmail)
            }).ToList();

            return View(rentalStatusList);
        }



        public ActionResult BookCar(CarRentalModel model)
        {
            if (ModelState.IsValid)
            {
                string userEmail = Session["UserEmail"]?.ToString();
               

                UserDAL userDal = new UserDAL();
                model.UserID = userDal.GetUserIDByEmail(userEmail);

                if (rentalCarDAL.InsertCarRental(model))
                {
                    TempData["Success"] = "Car booked successfully!";
                    return RedirectToAction("MyRentals");
                }

                ModelState.AddModelError("", "Failed to book the car. Please try again.");
            }

            return View(model);
        }

        public ActionResult MyBookings()
        {
            if (Session["UserID"] == null)
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = Convert.ToInt32(Session["UserID"]);
            List<CarRentalBookingModel> bookings = rentalCarDAL.GetUserBookings(userId);

            return View(bookings);
        }

        public ActionResult Dashboard()
        {
            var dashboardData = rentalCarDAL.GetDashboardData();
            return View(dashboardData);
        }

        [HttpGet]
        public JsonResult GetDashboardData()
        {
            try
            {
                var data = rentalCarDAL.GetDashboardData();
                return Json(data, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }
    }
}