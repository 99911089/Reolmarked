using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using CustomerApp.Models;

namespace CustomerApp.Services
{
    /// <summary>
    /// Klasse til håndtering af reoler og salg i databasen.
    /// </summary>
    public class DatabaseShelves
    {
        private string connectionString =
            @"Server=192.168.154.160,1433;Database=CustomerDB;User Id=reoluser;Password=StrongPass123;Encrypt=False;";

        public string LatestStatus { get; private set; }

        private void LogError(Exception ex)
        {
            File.AppendAllText("errorlog.txt",
                $"{DateTime.Now}: {ex.Message}{Environment.NewLine}");
        }

        /// <summary>
        /// Hent reoler fra DB, fallback til offline data
        /// </summary>
        public List<ShelfRental> GetShelves()
        {
            List<ShelfRental> shelves = new List<ShelfRental>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT Id, Number, HasHangerBar, IsRented, CustomerId FROM Shelves";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            shelves.Add(new ShelfRental
                            {
                                Id = reader.GetInt32(0),
                                Number = reader.GetInt32(1),
                                HasClothingRack = reader.GetBoolean(2),
                                IsRented = reader.GetBoolean(3),
                                CustomerId = reader.IsDBNull(4) ? null : reader.GetInt32(4)
                            });
                        }
                    }
                }
                LatestStatus = "Shelves retrieved from DB.";
            }
            catch (Exception ex)
            {
                shelves = new List<ShelfRental>
                {
                    new ShelfRental { Id = 1, Number = 1, HasClothingRack = false, IsRented = false },
                    new ShelfRental { Id = 2, Number = 2, HasClothingRack = true, IsRented = false },
                    new ShelfRental { Id = 3, Number = 3, HasClothingRack = false, IsRented = true, CustomerId = 1 }
                };
                LatestStatus = $"Offline – {ex.Message}";
                LogError(ex);
            }
            return shelves;
        }

        /// <summary>
        /// Udlej reol til kunde
        /// </summary>
        public void RentShelf(int shelfId, int customerId)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "UPDATE Shelves SET IsRented = 1, CustomerId = @CustomerId WHERE Id = @ShelfId";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ShelfId", shelfId);
                        cmd.Parameters.AddWithValue("@CustomerId", customerId);
                        cmd.ExecuteNonQuery();
                    }
                }
                LatestStatus = "Shelf rented.";
            }
            catch (Exception ex)
            {
                LatestStatus = $"Error while renting shelf – offline ({ex.Message})";
                LogError(ex);
            }
        }

        /// <summary>
        /// Registrer salg på reol
        /// </summary>
        public void RegisterSale(int shelfId, double saleAmount)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "INSERT INTO Sales (ShelfId, SaleAmount) VALUES (@ShelfId, @Amount)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@ShelfId", shelfId);
                        cmd.Parameters.AddWithValue("@Amount", saleAmount);
                        cmd.ExecuteNonQuery();
                    }
                }
                double commission = ShelfRental.CalculateCommission(saleAmount);
                LatestStatus = $"Sale registered. Commission: {commission} kr.";
            }
            catch (Exception ex)
            {
                LatestStatus = $"Error while registering sale – offline ({ex.Message})";
                LogError(ex);
            }
        }
    }
}
