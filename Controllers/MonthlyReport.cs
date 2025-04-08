using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using WALEED_TMS.DAL;
using WALEED_TMS.Models;

namespace WALEED_TMS.Controllers
{
    public class MonthlyReportController : Controller
    {
        private readonly UserDAL _userDal = new UserDAL();

        public ActionResult MonthlyReportAndAnalytics()
        {
            int currentMonth = DateTime.Now.Month;
            ViewBag.CurrentMonth = currentMonth;
            var report = _userDal.GetMonthlyReport(currentMonth);

            // Add debug check
            if (report != null)
            {
                System.Diagnostics.Debug.WriteLine($"Accepted Tours Count: {report.AcceptedTours?.Count}");
                System.Diagnostics.Debug.WriteLine($"Rejected Tours Count: {report.RejectedTours?.Count}");
            }

            return View(report);
        }

        [HttpGet]
        public JsonResult GetMonthlyReportData(int selectedMonth)
        {
            try
            {
                var report = _userDal.GetMonthlyReport(selectedMonth);

                // Debug the data
                System.Diagnostics.Debug.WriteLine($"Selected Month: {selectedMonth}");
                System.Diagnostics.Debug.WriteLine($"Accepted Tours: {report.AcceptedTours?.Count}");
                System.Diagnostics.Debug.WriteLine($"Rejected Tours: {report.RejectedTours?.Count}");

                var result = new
                {
                    success = true,
                    data = new
                    {
                        totalTours = report.TotalTours,
                        toursAccepted = report.ToursAccepted,
                        toursRejected = report.ToursRejected,
                        totalEstimate = report.TotalEstimate,
                        acceptedEstimate = report.AcceptedEstimate,
                        rejectedEstimate = report.RejectedEstimate,
                        acceptedTours = report.AcceptedTours.Select(t => new
                        {
                            TourID = t.TourID,
                            Date = t.Date,
                            Estimate = t.Estimate
                        }),
                        rejectedTours = report.RejectedTours.Select(t => new
                        {
                            TourID = t.TourID,
                            Date = t.Date,
                            Estimate = t.Estimate
                        })
                    }
                };

                return Json(result, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error: {ex.Message}");
                return Json(new { success = false, message = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

    }
}