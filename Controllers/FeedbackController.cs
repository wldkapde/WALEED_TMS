using System;
using System.Web.Mvc;
using WALEED_TMS.DAL;
using WALEED_TMS.Models;

namespace WALEED_TMS.Controllers
{
    public class FeedbackController : Controller
    {
        private readonly UserDAL _userDal = new UserDAL();

        [HttpGet]
        public ActionResult ViewFeedback()
        {
            var feedbackList = _userDal.GetAllFeedback();
            return View(feedbackList);
        }

        [HttpPost]
        //[Authorize]
        public ActionResult AddFeedback(int Rating, string FeedbackText)
        {
            try
            {
                // Get current user ID from session
                int userId = (int)Session["UserID"];
                
                // Insert feedback with "feedback:" prefix
                bool result = _userDal.InsertReview(userId, Rating, "feedback: " + FeedbackText);

                if (result)
                {
                    TempData["Message"] = "Feedback submitted successfully!";
                }
                else
                {
                    TempData["Error"] = "Failed to submit feedback. Please try again.";
                }
            }
            catch (Exception ex)
            {
                TempData["Error"] = "An error occurred: " + ex.Message;
            }

            return RedirectToAction("ViewFeedback");
        }

        [Authorize]
        public ActionResult FeedbackForm()
        {
            return View();
        }
    }
}