using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WALEED_TMS.Models
{
        public class ReviewModel
        {
            public int ReviewID { get; set; }
            public int UserID { get; set; }
            public int Rating { get; set; }
            public string Comment { get; set; }
            public DateTime CreatedDate { get; set; }
            public string UserName { get; set; } // Add this property
        }
    
}