using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WALEED_TMS.Models
{
    public class ConfirmedTourModel
    {
        public int JourneyID { get; set; }
        public string TourLocation { get; set; }
        public DateTime CreationDate { get; set; }
        public decimal Estimate { get; set; }

    }
}