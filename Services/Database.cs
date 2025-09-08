using System;
using System.Collections.Generic;
using System.Data.SqlClient;
using System.IO;
using CustomerApp.Models;
using CustomerApp.Services;

namespace CustomerApp.Services
{
    /// <summary>
    /// Klasse, der håndterer alle databaseoperationer for kunder og reoler.
    /// Den sørger for at hente data fra SQL Server, indsætte nye kunder,
    /// opdatere reolstatus og registrere salg. Offline data bruges som fallback
    /// hvis SQL Server ikke er tilgængelig.
    /// </summary>
    public class Database
    {
        // --- PRIVATE FIELDS ---
        /// <summary>
        /// Forbindelsesstreng til SQL Server.
        /// Indeholder server, database, bruger og password.
        /// </summary>
        private string connectionString =
            @"Server=192.168.154.160,1433;Database=CustomerDB;User Id=reoluser;Password=StrongPass123;Encrypt=False;";

        // --- PUBLIC PROPERTIES ---
        /// <summary>
        /// Seneste statusbesked, fx til visning i GUI.
        /// </summary>
        public string LatestStatus { get; private set; }

        // --- PRIVATE METHODS ---
        /// <summary>
        /// Hjælpemetode til at logge fejl til en tekstfil.
        /// Filen oprettes automatisk, hvis den ikke findes.
        /// </summary>
        /// <param name="ex">Exception objektet</param>
        private void LogError(Exception ex)
        {
            File.AppendAllText("errorlog.txt",
                $"{DateTime.Now}: {ex.Message}{Environment.NewLine}");
        }

        // --- PUBLIC METHODS ---

        /// <summary>
        /// Henter alle kunder fra databasen.
        /// Hvis databasen ikke er tilgængelig, returneres offline testdata.
        /// </summary>
        /// <returns>Liste af kunder</returns>
        public List<Customer> GetCustomers()
        {
            List<Customer> customers = new List<Customer>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "SELECT Id, Name, Email, Phone FROM Customers";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            customers.Add(new Customer
                            {
                                Id = reader.GetInt32(0),
                                Name = reader.GetString(1),
                                Email = reader.GetString(2),
                                Phone = reader.GetString(3)
                            });
                        }
                    }
                }
                LatestStatus = "Customers retrieved from DB.";
            }
            catch (Exception ex)
            {
                // Offline fallback data
                customers = new List<Customer>
                {
                    new Customer { Id = 1, Name = "Offline Customer 1", Email = "offline1@test.com", Phone = "11111111" },
                    new Customer { Id = 2, Name = "Offline Customer 2", Email = "offline2@test.com", Phone = "22222222" }
                };
                LatestStatus = $"Offline – {ex.Message}";
                LogError(ex);
            }
            return customers;
        }

        /// <summary>
        /// Tilføjer en ny kunde til databasen.
        /// Hvis databasen er offline, logges fejlen.
        /// </summary>
        /// <param name="newCustomer">Kundeobjekt der skal tilføjes</param>
        public void AddCustomer(Customer newCustomer)
        {
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    string sql = "INSERT INTO Customers (Name, Email, Phone) VALUES (@Name, @Email, @Phone)";
                    using (SqlCommand cmd = new SqlCommand(sql, conn))
                    {
                        cmd.Parameters.AddWithValue("@Name", newCustomer.Name);
                        cmd.Parameters.AddWithValue("@Email", newCustomer.Email);
                        cmd.Parameters.AddWithValue("@Phone", newCustomer.Phone);
                        cmd.ExecuteNonQuery();
                    }
                }
                LatestStatus = "Customer added.";
            }
            catch (Exception ex)
            {
                LatestStatus = $"Error while adding customer – offline ({ex.Message})";
                LogError(ex);
            }
        }

        /// <summary>
        /// Henter alle reoler fra databasen.
        /// Hvis databasen er offline, returneres offline testdata.
        /// </summary>
        /// <returns>Liste af reoler</returns>
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
                // Offline testdata
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
        /// Udlejer en reol til en kunde.
        /// Opdaterer kun databasen hvis SQL Server er tilgængelig.
        /// </summary>
        /// <param name="shelfId">ID på reolen</param>
        /// <param name="customerId">ID på kunden</param>
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
        /// Registrerer et salg på en reol.
        /// Beregner kommission og opdaterer LatestStatus.
        /// </summary>
        /// <param name="shelfId">ID på reolen</param>
        /// <param name="saleAmount">Salgsbeløb</param>
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

                // Beregn kommission med statisk metode i ShelfRental
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
