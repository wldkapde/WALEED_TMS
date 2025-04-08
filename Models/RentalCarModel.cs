using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace WALEED_TMS.Models
{
    public class RentalCarModel
    {
        public int CarID { get; set; }
        public string Make { get; set; }
        public string Model { get; set; }
        public string Description { get; set; }
        public string Benefits { get; set; }
        public decimal DailyRate { get; set; }
        public string ImageUrl { get; set; }
        public bool IsAvailable { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class CarRentalModel
    {
        public int BookingID { get; set; }
        public int UserID { get; set; }
        public int CarID { get; set; }
        public string UserName { get; set; }

        public string CarName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string PickupLocation { get; set; }
        public string DropLocation { get; set; }
        public int NumberOfPassengers { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public DateTime BookingDate { get; set; }
    }

    public class CarRentalStatusModel
    {
        public int BookingID { get; set; }
        public int UserID { get; set; }
        public int CarID { get; set; }
        public string CarName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string PickupLocation { get; set; }
        public string DropLocation { get; set; }
        public int NumberOfPassengers { get; set; }
        public decimal TotalPrice { get; set; }
        public string Status { get; set; }
        public DateTime BookingDate { get; set; }
        public string UserName { get; set; }
    }
    public class CarRentalBookingModel
    {
        public int BookingID { get; set; }

        public int CarID { get; set; }

        public int UserID { get; set; }

        [Required(ErrorMessage = "From date is required")]
        public DateTime FromDate { get; set; }

        [Required(ErrorMessage = "To date is required")]
        public DateTime ToDate { get; set; }

        public decimal TotalPrice { get; set; }

        public int NumberOfStops { get; set; }

        public string StopLocations { get; set; }

        public DateTime BookingDate { get; set; }

        public string Status { get; set; }

        // Navigation property
        public RentalCarModel Car { get; set; }
    }

    public class CarRentalAnalyticsModel
    {
        public List<CarRentalDetailModel> Details { get; set; }
        public int TotalBookings { get; set; }
        public int AcceptedBookings { get; set; }
        public int RejectedBookings { get; set; }
        public decimal TotalEstimate { get; set; }
        public decimal AcceptedEstimate { get; set; }
        public decimal RejectedEstimate { get; set; }
    }

    public class CarRentalDetailModel
    {
        public int BookingID { get; set; }
        public string CarName { get; set; }
        public DateTime FromDate { get; set; }
        public DateTime ToDate { get; set; }
        public string Status { get; set; }
        public decimal Estimate { get; set; }
        public string UserName { get; set; }
    }
    //public class MonthlyReportViewModel
    //{
    //   // public TourAnalyticsModel TourData { get; set; }
    //    public CarRentalAnalyticsModel CarRentalData { get; set; }
    //    public DateTime StartDate { get; set; }
    //    public DateTime EndDate { get; set; }
    //}
}
