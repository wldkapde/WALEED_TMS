using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace WALEED_TMS.Models
{
    public class UserModel
    {
        public int UserID { get; set; }
        public string FullName { get; set; }
        public DateTime? DOB { get; set; }  // Make it nullable
        public string Email { get; set; }
        public string HearAboutUs { get; set; }
        public string PreferredContact { get; set; }
        public DateTime? CreatedDate { get; set; }  // Make it nullable
        public string IsActive { get; set; }
    }

}