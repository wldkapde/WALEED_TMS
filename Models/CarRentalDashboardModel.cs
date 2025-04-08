using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WALEED_TMS.Models
{
    public class CarRentalDashboardModel
    {
        public int TotalEstimate { get; set; }
        public int AcceptedEstimate { get; set; }
        public int RejectedEstimate { get; set; }
        public int TotalBookings { get; set; }
        public string MostBookedCar { get; set; }
        public List<CarRentalBooking> AcceptedRentals { get; set; }
        public List<CarRentalBooking> RejectedRentals { get; set; }
    }

    public class CarRentalBooking
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
        public DateTime CreatedDate { get; set; }
        public string Username { get; set; }
    }
}