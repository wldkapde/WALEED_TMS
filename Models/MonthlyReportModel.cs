using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WALEED_TMS.Models
{


    public class MonthlyReportModel
    {
        public int TotalTours { get; set; }
        public int ToursAccepted { get; set; }
        public int ToursRejected { get; set; }
        public decimal TotalEstimate { get; set; }
        public decimal AcceptedEstimate { get; set; }
        public decimal RejectedEstimate { get; set; }
        public List<TourDetail> AcceptedTours { get; set; }
        public List<TourDetail> RejectedTours { get; set; }
        public int SelectedMonth { get; set; }

        public MonthlyReportModel()
        {
            AcceptedTours = new List<TourDetail>();
            RejectedTours = new List<TourDetail>();
            SelectedMonth = DateTime.Now.Month;
        }
    }

    public class TourDetail
    {
        public int TourID { get; set; }
        public DateTime Date { get; set; }
        public decimal Estimate { get; set; }
    }
}