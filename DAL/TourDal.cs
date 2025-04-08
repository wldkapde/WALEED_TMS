using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Web;
using WALEED_TMS.Models;

namespace WALEED_TMS.DAL
{
    public class TourDAL
    {
        private readonly string connectionString = ConfigurationManager.ConnectionStrings["DBCon"].ConnectionString;

        // ✅ Book a Tour
        public List<TourBookingModel> GetTourDetailsByUser(int userId)
        {
            List<TourBookingModel> tourList = new List<TourBookingModel>();

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetTourDetailsByUser", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tourList.Add(new TourBookingModel
                            {
                                TourLocation = reader["TourLocation"].ToString(),
                                TourStartDate = Convert.ToDateTime(reader["TourStartDate"]), // ✅ Convert properly
                                TourEndDate = Convert.ToDateTime(reader["TourEndDate"]),   // ✅ Convert properly
                                Adults = Convert.ToInt32(reader["Adults"]),
                                Children = Convert.ToInt32(reader["Children"]),
                                Estimate = Convert.ToDecimal(reader["Estimate"]),
                                TourStatus = reader["TourStatus"].ToString()
                            });
                        }
                    }
                }
            }
            return tourList;
        }

        public TourBookingModel GetTourDetailsByUser1(int userId) // 🔥 Ab List nahi, ek hi record return karega
        {
            TourBookingModel tour = null; // ✅ Single record ke liye null initialize

            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetTourDetailsByUser1", conn))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    conn.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        if (reader.Read()) // ✅ Sirf ek record hi milega
                        {
                            tour = new TourBookingModel
                            {
                                TourLocation = reader["TourLocation"].ToString(),
                                TourStartDate = Convert.ToDateTime(reader["TourStartDate"]),
                                TourEndDate = Convert.ToDateTime(reader["TourEndDate"]),
                                Adults = Convert.ToInt32(reader["Adults"]),
                                Children = Convert.ToInt32(reader["Children"]),
                                Estimate = Convert.ToDecimal(reader["Estimate"]),
                                TourStatus = reader["TourStatus"].ToString()
                            };
                        }
                    }
                }
            }
            return tour; 
        }




        public int GetUserIDByEmail(string email)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCon"].ConnectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT UserID FROM Users WHERE Email = @Email", con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        public bool BookTour(TourBookingModel model, out string message)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("INSERTTourDetails", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    // Retrieve values from session
                    int userID = Convert.ToInt32(HttpContext.Current.Session["UserID"]);
                    string userName = HttpContext.Current.Session["UserName"]?.ToString();
                    string userEmail = HttpContext.Current.Session["UserEmail"]?.ToString();

                    if (userID == 0 || string.IsNullOrEmpty(userName) || string.IsNullOrEmpty(userEmail))
                    {
                        message = "User session expired or invalid.";
                        return false;
                    }

                    cmd.Parameters.AddWithValue("@UserID", userID);
                    cmd.Parameters.AddWithValue("@TourLocation", model.TourLocation);
                    cmd.Parameters.AddWithValue("@TourStartDate", model.TourStartDate);
                    cmd.Parameters.AddWithValue("@TourEndDate", model.TourEndDate);
                    cmd.Parameters.AddWithValue("@Adults", model.Adults);
                    cmd.Parameters.AddWithValue("@Children", model.Children);
                    cmd.Parameters.AddWithValue("@Estimate", model.Estimate);

                    try
                    {
                        con.Open();
                        cmd.ExecuteNonQuery();
                        message = "Booking successful!";
                        return true;
                    }
                    catch (Exception ex)
                    {
                        message = "Error: " + ex.Message;
                        return false;
                    }
                }
            }
        }



        //public int GetUserIDByEmail(string email)
        //{
        //    using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCon"].ConnectionString))
        //    {
        //        using (SqlCommand cmd = new SqlCommand("SELECT UserID FROM Users WHERE Email = @Email", con))
        //        {
        //            cmd.Parameters.AddWithValue("@Email", email);
        //            con.Open();
        //            object result = cmd.ExecuteScalar();
        //            return result != null ? Convert.ToInt32(result) : 0;
        //        }
        //    }
        //}

        // ✅ Get UserID from Session or DB
        private int GetUserIDFromSessionOrDB()
        {
            if (HttpContext.Current.Session["UserID"] != null)
            {
                return Convert.ToInt32(HttpContext.Current.Session["UserID"]);
            }

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("SELECT UserID FROM Users WHERE Email = @Email", con))
                {
                    cmd.Parameters.AddWithValue("@Email", HttpContext.Current.Session["UserEmail"]);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        // ✅ Fetch Pending Tours
        public List<TourBookingModel> GetPendingTours()
        {
            List<TourBookingModel> tours = new List<TourBookingModel>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("GetPendingTours", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            tours.Add(new TourBookingModel
                            {
                                TourID = reader.GetInt32(reader.GetOrdinal("JourneyID")),
                                UserID = reader.GetInt32(reader.GetOrdinal("UserID")),
                                UserName = reader["UserName"].ToString(),
                                UserEmail = reader["UserEmail"].ToString(),
                                TourLocation = reader["TourLocation"].ToString(),
                                TourStartDate = reader["TourStartDate"] as DateTime? ?? DateTime.MinValue,
                                TourEndDate = reader["TourEndDate"] as DateTime? ?? DateTime.MinValue,

                                Adults = reader.GetInt32(reader.GetOrdinal("Adults")),
                                Children = reader.GetInt32(reader.GetOrdinal("Children")),
                                Estimate = reader.GetDecimal(reader.GetOrdinal("Estimate")),
                                TourStatus = reader["TourStatus"].ToString(),
                                CreationDate = Convert.ToDateTime(reader["CreationDate"])
                            });
                        }
                    }
                }
            }
            return tours;
        }

        public string UpdateTourStatus(int tourID, string status)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("UpdateTourStatus", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;
                    cmd.Parameters.AddWithValue("@JourneyID", tourID);
                    cmd.Parameters.AddWithValue("@Status", status);

                    SqlParameter emailParam = new SqlParameter("@UserEmail", SqlDbType.NVarChar, 255);
                    emailParam.Direction = ParameterDirection.Output;
                    cmd.Parameters.Add(emailParam);

                    con.Open();
                    cmd.ExecuteNonQuery();  // `ExecuteNonQuery()` se rows affected count nahi lena hoga

                    // Directly return email value, NULL check handle karna zaroori hai
                    return emailParam.Value != DBNull.Value ? emailParam.Value.ToString() : null;
                }
            }
        }

        //public TourBookingModel GetJourneyDetailsById(int journeyId)
        //{
        //    TourBookingModel tour = null;

        //    using (SqlConnection conn = new SqlConnection(connectionString))
        //    {
        //        using (SqlCommand cmd = new SqlCommand("GetJourneyDetailsById", conn)) // ✅ Stored Procedure Call
        //        {
        //            cmd.CommandType = CommandType.StoredProcedure;
        //            cmd.Parameters.AddWithValue("@JourneyID", journeyId);

        //            conn.Open();
        //            using (SqlDataReader reader = cmd.ExecuteReader())
        //            {
        //                if (reader.Read())  // ✅ Sirf ek record read karega
        //                {
        //                    tour = new TourBookingModel
        //                    {
        //                        TourID = Convert.ToInt32(reader["JourneyID"]),  // ✅ JourneyID as TourID
        //                        UserID = Convert.ToInt32(reader["UserID"]),
        //                        UserName = reader["UserName"].ToString(),
        //                        UserEmail = reader["UserEmail"].ToString(),
        //                        TourLocation = reader["TourLocation"].ToString(),
        //                        TourStartDate = Convert.ToDateTime(reader["TourStartDate"]),
        //                        TourEndDate = Convert.ToDateTime(reader["TourEndDate"]),
        //                        Adults = Convert.ToInt32(reader["Adults"]),
        //                        Children = Convert.ToInt32(reader["Children"]),
        //                        Estimate = Convert.ToDecimal(reader["Estimate"]),
        //                        TourStatus = reader["TourStatus"].ToString(),
        //                        CreationDate = Convert.ToDateTime(reader["CreationDate"])
        //                    };
        //                }
        //            }
        //        }
        //    }
        //    return tour;  // ✅ Sirf ek journey ka data return karega
        //}





    }
}
