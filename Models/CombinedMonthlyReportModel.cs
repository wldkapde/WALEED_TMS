using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WALEED_TMS.Models
{
    public class CombinedMonthlyReportModel
    {
        public MonthlyReportModel TourReport { get; set; }
        public CarRentalAnalyticsModel CarRentalReport { get; set; }
    }
}