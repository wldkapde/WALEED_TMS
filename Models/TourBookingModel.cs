using System;

namespace WALEED_TMS.Models
{
    public class TourBookingModel
    {
        public int TourID { get; set; }
        public int UserID { get; set; }
        public string UserName { get; set; }
        public string UserEmail { get; set; }
        public string TourLocation { get; set; }
        public DateTime TourStartDate { get; set; }
        public DateTime TourEndDate { get; set; }
        public int Adults { get; set; }
        public int Children { get; set; }
        public decimal Estimate { get; set; }
        public string TourStatus { get; set; }
        public DateTime CreationDate { get; set; } // ✅ Added for correct sorting
    }
}
