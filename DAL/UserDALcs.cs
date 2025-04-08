using System;
using System.Data.SqlClient;
using System.Configuration;
using System.Collections.Generic;
using WALEED_TMS.Models;
using System.Data;

namespace WALEED_TMS.DAL
{
    public class UserDAL
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DBCon"].ConnectionString;

        public bool CheckUserExists(string email)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT COUNT(*) FROM tbl_UserRegistration WHERE Email = @Email";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);

                        con.Open();
                        int userCount = (int)cmd.ExecuteScalar();
                        return userCount > 0; 
                    }
                }
            }
            catch (Exception)
            {
                return false;
            }
        }

        public bool InsertUser(string fullName, string dob, string email, string password, string contactMethod, string hearAboutUs, string UserRole)
        {
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string query = "INSERT INTO tbl_UserRegistration (FullName, Dob, Email, Password, HearAboutUs, PreferredContact, UserRole, IsActive) " +
                                   "VALUES (@FullName, @Dob, @Email, @Password, @HearAboutUs, @PreferredContact, 'USER', 'Y')";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@FullName", fullName);
                        cmd.Parameters.AddWithValue("@Dob", dob);
                        cmd.Parameters.AddWithValue("@Email", email);
                        cmd.Parameters.AddWithValue("@Password", password);
                        cmd.Parameters.AddWithValue("@HearAboutUs", hearAboutUs);
                        cmd.Parameters.AddWithValue("@PreferredContact", contactMethod);

                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                }
                catch (Exception)
                {
                    return false;
                }
            }
        }

        public string GetUserRoleByEmail(string email)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT UserRole FROM tbl_UserRegistration WHERE Email = @Email";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        con.Open();
                        object result = cmd.ExecuteScalar();
                        return result != null ? result.ToString() : "USER"; // Default to USER if not found
                    }
                }
            }
            catch (Exception)
            {
                return "USER"; // Default role in case of error
            }
        }





        //-------------------s
        public string GetUserNameByEmail(string email)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT FullName FROM tbl_UserRegistration WHERE Email = @Email";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        con.Open();
                        object result = cmd.ExecuteScalar();
                        return result != null ? result.ToString() : "User"; // Default value if name not found
                    }
                }
            }
            catch (Exception)
            {
                return "User"; 
            }
        }


        //----------------Login User Method
        public string AuthenticateUser(string email, string enteredPassword)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT UserID, Password, IsActive FROM tbl_UserRegistration WHERE Email = @Email";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Email", email);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            if (reader.Read())
                            {
                                int userId = Convert.ToInt32(reader["UserID"]);
                                string storedPassword = reader["Password"].ToString();
                                string isActive = reader["IsActive"].ToString();

                                if (isActive != "Y")
                                {
                                    return $"User is not active. UserID: {userId}. Please contact the administrator.";
                                }

                                if (enteredPassword == storedPassword)
                                {
                                    return $"Success. UserID: {userId}";
                                }
                                return $"Invalid password. UserID: {userId}";
                            }
                            return "User not found";
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error: " + ex.Message);
                return "An error occurred during authentication";
            }
        }

        public int GetUserIDByEmail(string email)
        {
            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCon"].ConnectionString))
            {
                string query = "SELECT UserID FROM tbl_UserRegistration WHERE Email = @Email";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@Email", email);
                    con.Open();
                    object result = cmd.ExecuteScalar();
                    return result != null ? Convert.ToInt32(result) : 0;
                }
            }
        }

        public List<UserModel> GetAllUsers()
        {
            List<UserModel> users = new List<UserModel>();

            using (SqlConnection con = new SqlConnection(ConfigurationManager.ConnectionStrings["DBCon"].ConnectionString))
            {
                string query = "SELECT UserID, FullName, DOB, Email, HearAboutUs, PreferredContact, CreatedDate, IsActive FROM tbl_UserRegistration";
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            users.Add(new UserModel
                            {
                                UserID = Convert.ToInt32(reader["UserID"]),
                                FullName = reader["FullName"].ToString(),
                                DOB = reader["DOB"] != DBNull.Value ? Convert.ToDateTime(reader["DOB"]) : DateTime.MinValue, // Default to MinValue
                                Email = reader["Email"].ToString(),
                                HearAboutUs = reader["HearAboutUs"].ToString(),
                                PreferredContact = reader["PreferredContact"].ToString(),
                                CreatedDate = reader["CreatedDate"] != DBNull.Value ? Convert.ToDateTime(reader["CreatedDate"]) : DateTime.Now, // Default to Now
                                IsActive = reader["IsActive"] != DBNull.Value ? reader["IsActive"].ToString() : "N"
                            });

                        }
                    }
                }
            }
            return users;
        }


        public bool UpdateUserActiveStatus(int userId, string status)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DBCon"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE tbl_UserRegistration SET IsActive = @Status WHERE UserID = @UserID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@Status", status);
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();
                    Console.WriteLine($"UserID: {userId}, New Status: {status}, Rows Affected: {rowsAffected}");
                    return rowsAffected > 0;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error in UpdateUserActiveStatus: " + ex.Message);
                return false;
            }
        }


        public UserModel GetUserDataByUserID(int userId)
        {
            UserModel user = null;
            string connectionString = ConfigurationManager.ConnectionStrings["DBCon"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT UserID, FullName, DOB, Email, HearAboutUs, PreferredContact, CreatedDate, IsActive FROM tbl_UserRegistration WHERE UserID = @UserID";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    if (reader.Read())
                    {
                        user = new UserModel
                        {
                            UserID = Convert.ToInt32(reader["UserID"]),
                            FullName = reader["FullName"].ToString(),
                            DOB = Convert.ToDateTime(reader["DOB"]),
                            Email = reader["Email"].ToString(),
                            HearAboutUs = reader["HearAboutUs"].ToString(),
                            PreferredContact = reader["PreferredContact"].ToString(),
                            CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                            IsActive = Convert.ToString(reader["IsActive"])
                        };
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                // Log error (use a logging framework or store in a DB table)
                Console.WriteLine("Error fetching user data: " + ex.Message);
            }

            return user;
        }


        public bool UpdateUserData(UserModel user)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["DBCon"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE tbl_UserRegistration SET FullName = @FullName, DOB = @DOB, Email = @Email, HearAboutUs = @HearAboutUs, PreferredContact = @PreferredContact WHERE UserID = @UserID";

                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@FullName", user.FullName);
                    cmd.Parameters.AddWithValue("@DOB", user.DOB);
                    cmd.Parameters.AddWithValue("@Email", user.Email);
                    cmd.Parameters.AddWithValue("@HearAboutUs", user.HearAboutUs);
                    cmd.Parameters.AddWithValue("@PreferredContact", user.PreferredContact);
                    cmd.Parameters.AddWithValue("@UserID", user.UserID);

                    con.Open();
                    int rowsAffected = cmd.ExecuteNonQuery();

                    return rowsAffected > 0; // True if update is successful
                }
            }
            catch (Exception ex)
            {
                // Log error (use a logging framework or store in a DB table)
                Console.WriteLine("Error updating user data: " + ex.Message);
                return false;
            }
        }


        public List<string> GetHearAboutUsOptions()
        {
            List<string> hearAboutUsOptions = new List<string>();
            string connectionString = ConfigurationManager.ConnectionStrings["DBCon"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT HearAboutUs FROM Tbl_HearAboutUs";
                    SqlCommand cmd = new SqlCommand(query, con);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        hearAboutUsOptions.Add(reader["HearAboutUs"].ToString());
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching HearAboutUs options: " + ex.Message);
            }

            return hearAboutUsOptions ?? new List<string>();
        }

        public List<string> GetPreferredContactOptions()
        {
            List<string> preferredContactOptions = new List<string>();
            string connectionString = ConfigurationManager.ConnectionStrings["DBCon"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "SELECT Contact FROM Tbl_PreferredWaytoContact";
                    SqlCommand cmd = new SqlCommand(query, con);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        preferredContactOptions.Add(reader["Contact"].ToString());
                    }

                    reader.Close();
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching Preferred Contact options: " + ex.Message);
            }

            return preferredContactOptions ?? new List<string>();
        }

        public List<ConfirmedTourModel> GetConfirmedTours(int userId)
        {
            List<ConfirmedTourModel> tours = new List<ConfirmedTourModel>();
            string connectionString = ConfigurationManager.ConnectionStrings["DBCon"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    // Modified query to include ESTIMATE column or set a default value if it doesn't exist
                    string query = @"SELECT j.JOURNEYID, j.USERID, j.TOURLOCATION, j.CREATIONDATE,
                           ISNULL(j.ESTIMATE, 0) AS ESTIMATE
                           FROM Tbl_UserJourneyDetails j
                           WHERE j.TourStatus = 'Confirmed' AND j.USERID = @UserID";
                    SqlCommand cmd = new SqlCommand(query, con);
                    cmd.Parameters.AddWithValue("@UserID", userId);

                    con.Open();
                    SqlDataReader reader = cmd.ExecuteReader();

                    while (reader.Read())
                    {
                        tours.Add(new ConfirmedTourModel
                        {
                            JourneyID = Convert.ToInt32(reader["JOURNEYID"]),
                            TourLocation = reader["TOURLOCATION"].ToString(),
                            CreationDate = Convert.ToDateTime(reader["CREATIONDATE"]),
                            // Add the estimate value
                            Estimate = reader["ESTIMATE"] != DBNull.Value ? Convert.ToDecimal(reader["ESTIMATE"]) : 0
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching confirmed tours: " + ex.Message);
            }

            return tours;
        }

        public List<FeedbackModel> GetAllFeedback()
        {
            List<FeedbackModel> feedbackList = new List<FeedbackModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"SELECT r.ReviewID AS FeedbackID, r.UserID, r.Rating, r.Comment AS FeedbackText, 
                   r.CreatedDate, u.FullName AS UserName
                   FROM Tbl_Reviews r
                   INNER JOIN tbl_UserRegistration u ON r.UserID = u.UserID
                   ORDER BY r.CreatedDate DESC";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                feedbackList.Add(new FeedbackModel
                                {
                                    FeedbackID = Convert.ToInt32(reader["FeedbackID"]),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    Rating = Convert.ToInt32(reader["Rating"]),
                                    FeedbackText = reader["FeedbackText"].ToString(),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    UserName = reader["UserName"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting feedback: " + ex.Message);
            }
            return feedbackList;
        }

        public bool InsertReview(int userId, int rating, string comment)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Tbl_Reviews 
                    (UserID, Rating, Comment, CreatedDate) 
                    VALUES (@UserID, @Rating, @Comment, GETDATE())";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        cmd.Parameters.AddWithValue("@Rating", rating);
                        cmd.Parameters.AddWithValue("@Comment", comment);

                        con.Open();
                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error inserting review: " + ex.Message);
                return false;
            }
        }

        public List<ReviewModel> GetReviews()
        {
            List<ReviewModel> reviews = new List<ReviewModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"SELECT r.ReviewID, r.UserID, r.Rating, r.Comment, r.CreatedDate, u.FullName AS UserName
                    FROM Tbl_Reviews r
                    INNER JOIN tbl_UserRegistration u ON r.UserID = u.UserID
                    ORDER BY r.CreatedDate DESC";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                reviews.Add(new ReviewModel
                                {
                                    ReviewID = Convert.ToInt32(reader["ReviewID"]),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    Rating = Convert.ToInt32(reader["Rating"]),
                                    Comment = reader["Comment"].ToString(),
                                    CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                                    UserName = reader["UserName"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting reviews: " + ex.Message);
            }
            return reviews;
        }

        public MonthlyReportModel GetMonthlyReport(int? selectedMonth = null)
        {
            var report = new MonthlyReportModel
            {
                TotalTours = 0,
                ToursAccepted = 0,
                ToursRejected = 0,
                TotalEstimate = 0,
                AcceptedEstimate = 0,
                RejectedEstimate = 0,
                AcceptedTours = new List<TourDetail>(),
                RejectedTours = new List<TourDetail>()
            };

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("GetMonthlyReport", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        cmd.Parameters.AddWithValue("@SelectedMonth", selectedMonth ?? DateTime.Now.Month);

                        con.Open();
                        using (var reader = cmd.ExecuteReader())
                        {
                            // First result set: Summary data
                            if (reader.Read())
                            {
                                report.TotalTours = reader["TotalTours"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TotalTours"]);
                                report.ToursAccepted = reader["ToursAccepted"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ToursAccepted"]);
                                report.ToursRejected = reader["ToursRejected"] == DBNull.Value ? 0 : Convert.ToInt32(reader["ToursRejected"]);
                                report.TotalEstimate = reader["TotalEstimate"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["TotalEstimate"]);
                                report.AcceptedEstimate = reader["AcceptedEstimate"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["AcceptedEstimate"]);
                                report.RejectedEstimate = reader["RejectedEstimate"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["RejectedEstimate"]);
                            }

                            // Second result set: Accepted tours
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    report.AcceptedTours.Add(new TourDetail
                                    {
                                        TourID = reader["TourID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TourID"]),
                                        Date = reader["Date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["Date"]),
                                        Estimate = reader["Estimate"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Estimate"])
                                    });
                                }
                            }

                            // Third result set: Rejected tours
                            if (reader.NextResult())
                            {
                                while (reader.Read())
                                {
                                    report.RejectedTours.Add(new TourDetail
                                    {
                                        TourID = reader["TourID"] == DBNull.Value ? 0 : Convert.ToInt32(reader["TourID"]),
                                        Date = reader["Date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["Date"]),
                                        Estimate = reader["Estimate"] == DBNull.Value ? 0 : Convert.ToDecimal(reader["Estimate"])
                                    });
                                }
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error
                Console.WriteLine($"Error in GetMonthlyReport: {ex.Message}");
                throw; // Rethrow to handle in controller
            }

            return report;
        }

    }
}
