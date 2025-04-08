using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WALEED_TMS.Models
{
    public class FeedbackModel
    {
        public int FeedbackID { get; set; }
        public int UserID { get; set; }
        public int Rating { get; set; }
        public string FeedbackText { get; set; }
        public DateTime CreatedDate { get; set; }
        public string UserName { get; set; }
    }
}