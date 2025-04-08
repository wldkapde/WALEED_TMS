using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Web.Mvc;
using WALEED_TMS.DAL;
using WALEED_TMS.Models;

public class TourController : Controller
{
    private readonly TourDAL _tourDAL;
    private readonly UserDAL _userDAL;

    public TourController()
    {
        _tourDAL = new TourDAL();
        _userDAL = new UserDAL(); // ✅ Fix: Proper initialization
    }

    [HttpPost]
    public JsonResult BookTour(TourBookingModel model)
    {
        string message;
        bool isSuccess = _tourDAL.BookTour(model, out message);

        return Json(new { success = isSuccess, message = message });
    }

    
}
