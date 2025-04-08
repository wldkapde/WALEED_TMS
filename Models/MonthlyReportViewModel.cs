using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WALEED_TMS.Models
{
    public class MonthlyReportViewModel
    {
        public MonthlyReportModel TourReport { get; set; }
        public CarRentalAnalyticsModel CarRentalReport { get; set; }
        public DateTime StartDate { get; set; }
        public DateTime EndDate { get; set; }
    }
}