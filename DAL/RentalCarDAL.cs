using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using WALEED_TMS.Models;

namespace WALEED_TMS.DAL
{
    public class RentalCarDAL
    {
        private string connectionString = ConfigurationManager.ConnectionStrings["DBCon"].ConnectionString;




        public bool InsertCarRental(CarRentalModel rental)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Tbl_CarRentalBookings 
                (UserID, CarID, CarName, FromDate, ToDate, PickupLocation, DropLocation, 
                 NumberOfPassengers, TotalPrice, Status, CreatedDate) 
                VALUES 
                (@UserID, @CarID, @CarName, @FromDate, @ToDate, @PickupLocation, @DropLocation, 
                 @NumberOfPassengers, @TotalPrice, 'Pending', GETDATE())";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", rental.UserID);
                        cmd.Parameters.AddWithValue("@CarID", rental.CarID);
                        cmd.Parameters.AddWithValue("@CarName", rental.CarName ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@FromDate", rental.FromDate);
                        cmd.Parameters.AddWithValue("@ToDate", rental.ToDate);
                        cmd.Parameters.AddWithValue("@PickupLocation", rental.PickupLocation ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@DropLocation", rental.DropLocation ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@NumberOfPassengers", rental.NumberOfPassengers);
                        cmd.Parameters.AddWithValue("@TotalPrice", rental.TotalPrice);

                        con.Open();
                        int rowsAffected = cmd.ExecuteNonQuery();
                        return rowsAffected > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                // Log the error (e.g., to a file, console, or a logging framework)
                Console.WriteLine("Error inserting car rental: " + ex.Message);
                return false;
            }
        }

        public List<CarRentalModel> GetUserCarRentals(int userId)
            {
            List<CarRentalModel> rentals = new List<CarRentalModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"SELECT BookingID, UserID, CarID, CarName, FromDate, ToDate, 
                           PickupLocation, DropLocation, NumberOfPassengers, TotalPrice, Status, CreatedDate
                           FROM Tbl_CarRentalBookings 
                           WHERE UserID = @UserID
                           ORDER BY CreatedDate DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                rentals.Add(new CarRentalModel
                                {
                                    BookingID = Convert.ToInt32(reader["BookingID"]),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CarID = Convert.ToInt32(reader["CarID"]),
                                    CarName = reader["CarName"].ToString(),
                                    FromDate = Convert.ToDateTime(reader["FromDate"]),
                                    ToDate = Convert.ToDateTime(reader["ToDate"]),
                                    PickupLocation = reader["PickupLocation"].ToString(),
                                    DropLocation = reader["DropLocation"].ToString(),
                                    NumberOfPassengers = Convert.ToInt32(reader["NumberOfPassengers"]),
                                    TotalPrice = Convert.ToDecimal(reader["TotalPrice"]),
                                    Status = reader["Status"].ToString(),
                                    BookingDate = Convert.ToDateTime(reader["CreatedDate"])
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting car rentals: " + ex.Message);
            }
            return rentals;
        }
        public bool BookCar(CarRentalBookingModel booking)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"INSERT INTO Tbl_CarRentalBookings 
                                   (CarID, UserID, FromDate, ToDate, TotalPrice, 
                                    NumberOfStops, StopLocations, BookingDate, Status) 
                                   VALUES 
                                   (@CarID, @UserID, @FromDate, @ToDate, @TotalPrice, 
                                    @NumberOfStops, @StopLocations, GETDATE(), @Status)";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@CarID", booking.CarID);
                        cmd.Parameters.AddWithValue("@UserID", booking.UserID);
                        cmd.Parameters.AddWithValue("@FromDate", booking.FromDate);
                        cmd.Parameters.AddWithValue("@ToDate", booking.ToDate);
                        cmd.Parameters.AddWithValue("@TotalPrice", booking.TotalPrice);
                        cmd.Parameters.AddWithValue("@NumberOfStops", booking.NumberOfStops);
                        cmd.Parameters.AddWithValue("@StopLocations", string.IsNullOrEmpty(booking.StopLocations) ? (object)DBNull.Value : booking.StopLocations);
                        cmd.Parameters.AddWithValue("@Status", "Pending");

                        con.Open();
                        int result = cmd.ExecuteNonQuery();
                        return result > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error booking rental car: " + ex.Message);
                return false;
            }
        }

        public List<CarRentalBookingModel> GetUserBookings(int userId)
        {
            List<CarRentalBookingModel> bookings = new List<CarRentalBookingModel>();

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"SELECT b.BookingID, b.CarID, b.UserID, b.FromDate, b.ToDate, 
                                   b.TotalPrice, b.NumberOfStops, b.StopLocations, b.BookingDate, b.Status,
                                   c.Make, c.Model, c.ImageUrl
                                   FROM Tbl_CarRentalBookings b
                                   INNER JOIN Tbl_RentalCars c ON b.CarID = c.CarID
                                   WHERE b.UserID = @UserID
                                   ORDER BY b.BookingDate DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@UserID", userId);
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                bookings.Add(new CarRentalBookingModel
                                {
                                    BookingID = Convert.ToInt32(reader["BookingID"]),
                                    CarID = Convert.ToInt32(reader["CarID"]),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    FromDate = Convert.ToDateTime(reader["FromDate"]),
                                    ToDate = Convert.ToDateTime(reader["ToDate"]),
                                    TotalPrice = Convert.ToDecimal(reader["TotalPrice"]),
                                    NumberOfStops = Convert.ToInt32(reader["NumberOfStops"]),
                                    StopLocations = reader["StopLocations"] != DBNull.Value ? reader["StopLocations"].ToString() : string.Empty,
                                    BookingDate = Convert.ToDateTime(reader["BookingDate"]),
                                    Status = reader["Status"].ToString(),
                                    Car = new RentalCarModel
                                    {
                                        CarID = Convert.ToInt32(reader["CarID"]),
                                        Make = reader["Make"].ToString(),
                                        Model = reader["Model"].ToString(),
                                        ImageUrl = reader["ImageUrl"] != DBNull.Value ? reader["ImageUrl"].ToString() : string.Empty
                                    }
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching user bookings: " + ex.Message);
            }

            return bookings;
        }

        public List<CarRentalModel> GetAllCarRentals()
        {
            List<CarRentalModel> rentals = new List<CarRentalModel>();
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"SELECT b.BookingID, b.UserID, b.CarID, b.CarName, 
                           b.FromDate, b.ToDate, b.Status, b.PickupLocation, b.DropLocation,
                           u.FullName as UserName
                           FROM Tbl_CarRentalBookings b
                           INNER JOIN tbl_UserRegistration u ON b.UserID = u.UserID
                           ORDER BY b.CreatedDate DESC";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        con.Open();
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                rentals.Add(new CarRentalModel
                                {
                                    BookingID = Convert.ToInt32(reader["BookingID"]),
                                    UserID = Convert.ToInt32(reader["UserID"]),
                                    CarID = Convert.ToInt32(reader["CarID"]),
                                    CarName = reader["CarName"].ToString(),
                                    FromDate = Convert.ToDateTime(reader["FromDate"]),
                                    ToDate = Convert.ToDateTime(reader["ToDate"]),
                                    Status = reader["Status"].ToString(),
                                    UserName = reader["UserName"].ToString(),
                                    PickupLocation = reader["PickupLocation"].ToString(),
                                    DropLocation = reader["DropLocation"].ToString()
                                });
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error getting rentals: " + ex.Message);
            }
            return rentals;
        }

        public bool UpdateRentalStatus(int bookingId, string status)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = "UPDATE Tbl_CarRentalBookings SET Status = @Status WHERE BookingID = @BookingID";
                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@Status", status);
                        cmd.Parameters.AddWithValue("@BookingID", bookingId);
                        con.Open();
                        return cmd.ExecuteNonQuery() > 0;
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error updating rental status: " + ex.Message);
                return false;
            }
        }

        public string CheckCarAvailability(int carId, DateTime fromDate, DateTime toDate)
        {
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    string query = @"SELECT TOP 1 Status 
                           FROM Tbl_CarRentalBookings 
                           WHERE CarID = @CarID 
                           AND Status = 'Confirmed'
                           AND ((FromDate BETWEEN @FromDate AND @ToDate) 
                           OR (ToDate BETWEEN @FromDate AND @ToDate)
                           OR (@FromDate BETWEEN FromDate AND ToDate))";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@CarID", carId);
                        cmd.Parameters.AddWithValue("@FromDate", fromDate);
                        cmd.Parameters.AddWithValue("@ToDate", toDate);

                        con.Open();
                        var result = cmd.ExecuteScalar();
                        return result?.ToString() ?? "Available";
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error checking car availability: " + ex.Message);
                return "Error";
            }
        }



        public CarRentalDashboardModel GetDashboardData()
        {
            var dashboard = new CarRentalDashboardModel
            {
                AcceptedRentals = new List<CarRentalBooking>(),
                RejectedRentals = new List<CarRentalBooking>()
            };

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    using (SqlCommand cmd = new SqlCommand("SP_GetCarRentalDashboard", con))
                    {
                        cmd.CommandType = CommandType.StoredProcedure;
                        con.Open();

                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            // First result set: Summary data
                            if (reader.Read())
                            {
                                dashboard.TotalEstimate = Convert.ToInt32(reader["TotalEstimate"]);
                                dashboard.AcceptedEstimate = Convert.ToInt32(reader["AcceptedEstimate"]);
                                dashboard.RejectedEstimate = Convert.ToInt32(reader["RejectedEstimate"]);
                                dashboard.TotalBookings = Convert.ToInt32(reader["TotalBookings"]);
                            }

                            // Second result set: Most booked car
                            reader.NextResult();
                            if (reader.Read())
                            {
                                dashboard.MostBookedCar = reader["CarName"].ToString();
                            }

                            // Third result set: Accepted rentals
                            reader.NextResult();
                            while (reader.Read())
                            {
                                dashboard.AcceptedRentals.Add(MapCarRentalBooking(reader));
                            }

                            // Fourth result set: Rejected rentals
                            reader.NextResult();
                            while (reader.Read())
                            {
                                dashboard.RejectedRentals.Add(MapCarRentalBooking(reader));
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine("Error fetching dashboard data: " + ex.Message);
            }

            return dashboard;
        }

        private CarRentalBooking MapCarRentalBooking(SqlDataReader reader)
        {
            return new CarRentalBooking
            {
                BookingID = Convert.ToInt32(reader["BookingID"]),
                UserID = Convert.ToInt32(reader["UserID"]),
                CarID = Convert.ToInt32(reader["CarID"]),
                CarName = reader["CarName"].ToString(),
                FromDate = Convert.ToDateTime(reader["FromDate"]),
                ToDate = Convert.ToDateTime(reader["ToDate"]),
                PickupLocation = reader["PickupLocation"].ToString(),
                DropLocation = reader["DropLocation"].ToString(),
                NumberOfPassengers = Convert.ToInt32(reader["NumberOfPassengers"]),
                TotalPrice = Convert.ToDecimal(reader["TotalPrice"]),
                Status = reader["Status"].ToString(),
                CreatedDate = Convert.ToDateTime(reader["CreatedDate"]),
                Username = reader["Username"].ToString()
            };
        }




        //public CarRentalAnalyticsModel GetMonthlyCarRentalAnalytics(DateTime startDate, DateTime endDate)
        //{
        //    var analytics = new CarRentalAnalyticsModel
        //    {
        //        Details = new List<CarRentalDetailModel>()
        //    };

        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(connectionString))
        //        {
        //            string query = @"
        //    SELECT 
        //        b.BookingID,
        //        b.CarName,
        //        b.FromDate,
        //        b.ToDate,
        //        b.Status,
        //        b.TotalPrice,
        //        u.FullName as UserName
        //    FROM Tbl_CarRentalBookings b
        //    INNER JOIN tbl_UserRegistration u ON b.UserID = u.UserID
        //    WHERE b.FromDate BETWEEN @StartDate AND @EndDate
        //    ORDER BY b.FromDate DESC";

        //            using (SqlCommand cmd = new SqlCommand(query, con))
        //            {
        //                cmd.Parameters.AddWithValue("@StartDate", startDate);
        //                cmd.Parameters.AddWithValue("@EndDate", endDate);

        //                con.Open();
        //                using (SqlDataReader reader = cmd.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        var detail = new CarRentalDetailModel
        //                        {
        //                            BookingID = Convert.ToInt32(reader["BookingID"]),
        //                            CarName = reader["CarName"].ToString(),
        //                            FromDate = Convert.ToDateTime(reader["FromDate"]),
        //                            ToDate = Convert.ToDateTime(reader["ToDate"]),
        //                            Status = reader["Status"].ToString(),
        //                            Estimate = Convert.ToDecimal(reader["TotalPrice"]),
        //                            UserName = reader["UserName"].ToString()
        //                        };

        //                        analytics.Details.Add(detail);

        //                        // Calculate totals
        //                        analytics.TotalBookings++;
        //                        analytics.TotalEstimate += detail.Estimate;

        //                        if (detail.Status.Equals("Confirmed", StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            analytics.AcceptedBookings++;
        //                            analytics.AcceptedEstimate += detail.Estimate;
        //                        }
        //                        else if (detail.Status.Equals("Rejected", StringComparison.OrdinalIgnoreCase))
        //                        {
        //                            analytics.RejectedBookings++;
        //                            analytics.RejectedEstimate += detail.Estimate;
        //                        }
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error getting car rental analytics: " + ex.Message);
        //    }

        //    return analytics;
        //}


        //public List<CarRentalStatusModel> GetUserCarRentals(int userId)
        //{
        //    List<CarRentalStatusModel> rentals = new List<CarRentalStatusModel>();
        //    try
        //    {
        //        using (SqlConnection con = new SqlConnection(connectionString))
        //        {
        //            string query = @"SELECT b.BookingID, b.UserID, b.CarID, b.CarName, b.FromDate, b.ToDate, 
        //                   b.PickupLocation, b.DropLocation, b.NumberOfPassengers, 
        //                   b.TotalPrice, b.Status, b.CreatedDate,
        //                   u.FullName as UserName
        //                   FROM Tbl_CarRentalBookings b
        //                   INNER JOIN tbl_UserRegistration u ON b.UserID = u.UserID
        //                   WHERE b.UserID = @UserID
        //                   ORDER BY b.CreatedDate DESC";

        //            using (SqlCommand cmd = new SqlCommand(query, con))
        //            {
        //                cmd.Parameters.AddWithValue("@UserID", userId);
        //                con.Open();
        //                using (SqlDataReader reader = cmd.ExecuteReader())
        //                {
        //                    while (reader.Read())
        //                    {
        //                        rentals.Add(new CarRentalStatusModel
        //                        {
        //                            BookingID = Convert.ToInt32(reader["BookingID"]),
        //                            UserID = Convert.ToInt32(reader["UserID"]),
        //                            CarID = Convert.ToInt32(reader["CarID"]),
        //                            CarName = reader["CarName"].ToString(),
        //                            FromDate = Convert.ToDateTime(reader["FromDate"]),
        //                            ToDate = Convert.ToDateTime(reader["ToDate"]),
        //                            PickupLocation = reader["PickupLocation"].ToString(),
        //                            DropLocation = reader["DropLocation"].ToString(),
        //                            NumberOfPassengers = Convert.ToInt32(reader["NumberOfPassengers"]),
        //                            Status = reader["Status"].ToString(),
        //                            TotalPrice = Convert.ToDecimal(reader["TotalPrice"]),
        //                            BookingDate = Convert.ToDateTime(reader["CreatedDate"])
        //                        });
        //                    }
        //                }
        //            }
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Console.WriteLine("Error getting car rentals: " + ex.Message);
        //    }
        //    return rentals;
        //}

    }
}