using System.Configuration;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;

namespace FinanceManagement
{
    public class DB
    {
        private string? connectionString;
        public event Action? DataUpdated;
        public event Action? DataShouldBeReloaded;
        public event EventHandler? RecordAdded;

        public void ReadData(DataGrid budgetsDataGrid)
        {

            connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
            List<BudgetLimit> budgetLimits = new List<BudgetLimit>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("Select * from BudgetLimits", con))
                    {

                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            var budgetLimit = new BudgetLimit()
                            {
                                BudgetID = reader["BudgetID"] as int?,
                                Budget_Amount = reader["Budget_Amount"] as int?,
                                Budget_Limit_Year = reader["Budget_Limit_Year"] as int?,
                                Budget_Category = reader["Budget_Category"] as string,
                                Creation_Date = reader["Creation_Date"] != DBNull.Value ? DateOnly.FromDateTime((DateTime)reader["Creation_Date"]) : (DateOnly?)null,
                                Budget_Status = reader["Budget_Status"] as string,
                                Approved_By = reader["Approved_By"] as string,
                                Comment = reader["Comment"] as string,
                                Currency = reader["Currency"] as string

                            };
                            budgetLimits.Add(budgetLimit);
                            DataUpdated?.Invoke();
                        }
                        reader.Close();
                    }

                }

                catch (SqlException ex)
                {

                    MessageBox.Show($"Ein Fehler ist aufgetreten beim Zugriff auf die Datenbank: {ex.Message}");
                }
            }
            // Aktualisiere das DataGrid im UI-Thread
            budgetsDataGrid.Dispatcher.Invoke(() =>
                {

                    budgetsDataGrid.ItemsSource = budgetLimits;
                });
        }

        public void ReadData(DataGrid budgetsDataGrid, Action<int> onLoadedCallback)
        {

            connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
            List<BudgetLimit> budgetLimits = new List<BudgetLimit>();
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("Select * from BudgetLimits", con))
                {
                    SqlDataReader reader = cmd.ExecuteReader();
                    while (reader.Read())
                    {
                        var budgetLimit = new BudgetLimit()
                        {
                            BudgetID = reader["BudgetID"] as int?,
                            Budget_Amount = reader["Budget_Amount"] as int?,
                            Budget_Limit_Year = reader["Budget_Limit_Year"] as int?,
                            Budget_Category = reader["Budget_Category"] as string,
                            Creation_Date = reader["Creation_Date"] != DBNull.Value ? DateOnly.FromDateTime((DateTime)reader["Creation_Date"]) : (DateOnly?)null,
                            Budget_Status = reader["Budget_Status"] as string,
                            Approved_By = reader["Approved_By"] as string,
                            Comment = reader["Comment"] as string,
                            Currency = reader["Currency"] as string

                        };
                        budgetLimits.Add(budgetLimit);
                        DataUpdated?.Invoke();
                    }
                    reader.Close();
                }
            }
            // Aktualisiere das DataGrid im UI-Thread
            budgetsDataGrid.Dispatcher.Invoke(() =>
            {

                budgetsDataGrid.ItemsSource = budgetLimits;
                onLoadedCallback?.Invoke(budgetLimits.Count);
            });
        }

        public List<BudgetLimit> ReadData()
        {
            List<BudgetLimit> results = new List<BudgetLimit>();
            connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    using (SqlCommand cmd = new SqlCommand("SELECT * FROM BudgetLimits", con))
                    {
                        using (SqlDataReader reader = cmd.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var budgetLimit = new BudgetLimit()
                                {
                                    BudgetID = reader["BudgetID"] as int?,
                                    Budget_Amount = reader["Budget_Amount"] as int?,
                                    Budget_Limit_Year = reader["Budget_Limit_Year"] as int?,
                                    Budget_Category = reader["Budget_Category"] as string,
                                    Creation_Date = reader["Creation_Date"] != DBNull.Value ? DateOnly.FromDateTime((DateTime)reader["Creation_Date"]) : (DateOnly?)null,
                                    Budget_Status = reader["Budget_Status"] as string,
                                    Approved_By = reader["Approved_By"] as string,
                                    Comment = reader["Comment"] as string,
                                    Currency = reader["Currency"] as string
                                };
                                results.Add(budgetLimit);
                            }
                        }
                    }
                }

                catch (SqlException)
                {

                    MessageBox.Show($"Ein Fehler ist aufgetreten beim Zugriff auf die Datenbank");
                }
                return results;
            }


        }

        //todo : Datum bei eintragung in die DB über UI in deutsch anzeigen
        public void InsertData(string budgetAmount, int yearLimit, string budgetCategory, string creationDate, string budgetStatus, string approvedBy, string comment, string currency)
        {
            connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(
                    "INSERT INTO BudgetLimits " +
                    "(Budget_Amount," +
                    " Budget_Limit_Year," +
                    " Budget_Category," +
                    (string.IsNullOrEmpty(creationDate) ? "" : "Creation_Date,") +
                    " Budget_Status," +
                    " Approved_By," +
                    " Comment," +
                    " Currency)" +
                    " VALUES" +
                    " (@BudgetAmount," +
                    " @YearLimit," +
                    " @BudgetCategory," +
                    (string.IsNullOrEmpty(creationDate) ? "" : "@CreationDate,") +
                    " @BudgetStatus," +
                    " @ApprovedBy" +
                    ",@Comment," +
                    " @Currency)",
                    con))
                {
                    cmd.Parameters.AddWithValue("@BudgetAmount", budgetAmount ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@YearLimit", yearLimit);
                    cmd.Parameters.AddWithValue("@BudgetCategory", budgetCategory ?? (object)DBNull.Value);
                    if (!string.IsNullOrEmpty(creationDate) && DateTime.TryParse(creationDate, out DateTime parsedDate))
                    {
                        cmd.Parameters.AddWithValue("@CreationDate", parsedDate);
                    }
                    cmd.Parameters.AddWithValue("@BudgetStatus", budgetStatus ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@ApprovedBy", approvedBy ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Comment", comment ?? (object)DBNull.Value);
                    cmd.Parameters.AddWithValue("@Currency", currency ?? (object)DBNull.Value);


                    cmd.ExecuteNonQuery();

                }
                RecordAdded?.Invoke(this, EventArgs.Empty);
                con.Close();
            }
        }


        /*
        public void UpdateData(int budgetId, string budgetAmount, int yearLimit, string budgetCategory, string creationDate, string budgetStatus, string approvedBy, string comment, string currency)
        {
            connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(
                        "Update BudgetLimits Set " +
                        "Budget_Amount = @BudgetAmount, " +
                        "Budget_Limit_Year = @YearLimit, " +
                        "Budget_Category = @BudgetCategory, " +
                        "Creation_Date = @CreationDate, " +
                        "Budget_Status = @BudgetStatus, " +
                        "Approved_By = @ApprovedBy, " +
                        "Comment = @Comment, " +
                        "Currency = @Currency " +
                        "Where BudgetID = @ID",
                        con))
                {
                    cmd.Parameters.AddWithValue("@ID", budgetId);
                    cmd.Parameters.AddWithValue("@BudgetAmount", budgetAmount);
                    cmd.Parameters.AddWithValue("@YearLimit", yearLimit);
                    cmd.Parameters.AddWithValue("@BudgetCategory", budgetCategory);
                    cmd.Parameters.AddWithValue("@CreationDate", creationDate);
                    cmd.Parameters.AddWithValue("@BudgetStatus", budgetStatus);
                    cmd.Parameters.AddWithValue("@ApprovedBy", approvedBy);
                    cmd.Parameters.AddWithValue("@Comment", comment);
                    cmd.Parameters.AddWithValue("@Currency", currency);

                    cmd.ExecuteNonQuery();
                }
            }

        }*/

        public BudgetLimit GetFirstBudgetEntry()
        {
            connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (var cmd = new SqlCommand("SELECT TOP 1 * FROM BudgetLimits ORDER BY BudgetID ASC", con))
                {
                    using (var reader = cmd.ExecuteReader())
                    {
                        if (reader.Read())
                        {
                            var budgetEntry = new BudgetLimit
                            {
                                BudgetID = reader.GetInt32(reader.GetOrdinal("BudgetID")),
                                Budget_Amount = reader.IsDBNull(reader.GetOrdinal("Budget_Amount")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("Budget_Amount")),
                                Budget_Limit_Year = reader.IsDBNull(reader.GetOrdinal("Budget_Limit_Year")) ? null : (int?)reader.GetInt32(reader.GetOrdinal("Budget_Limit_Year")),
                                Budget_Category = reader.IsDBNull(reader.GetOrdinal("Budget_Category")) ? null : reader.GetString(reader.GetOrdinal("Budget_Category")),
                                Creation_Date = reader.IsDBNull(reader.GetOrdinal("Creation_Date")) ? (DateOnly?)null : DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("Creation_Date"))),
                                Budget_Status = reader.IsDBNull(reader.GetOrdinal("Budget_Status")) ? null : reader.GetString(reader.GetOrdinal("Budget_Status")),
                                Approved_By = reader.IsDBNull(reader.GetOrdinal("Approved_By")) ? null : reader.GetString(reader.GetOrdinal("Approved_By")),
                                Comment = reader.IsDBNull(reader.GetOrdinal("Comment")) ? null : reader.GetString(reader.GetOrdinal("Comment")),
                                Currency = reader.IsDBNull(reader.GetOrdinal("Currency")) ? null : reader.GetString(reader.GetOrdinal("Currency")),

                            };
                            return budgetEntry;
                        }
                    }
                }
            }
            return null;

        }
        public void UpdateData(BudgetLimit budgetLimit)
        {
            connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();

                    string query = @"UPDATE BudgetLimits SET 
                    Budget_Amount = @BudgetAmount, 
                    Budget_Limit_Year = @YearLimit, 
                    Budget_Category = @BudgetCategory, 
                    Creation_Date = @CreationDate, 
                    Budget_Status = @BudgetStatus, 
                    Approved_By = @ApprovedBy, 
                    Comment = @Comment, 
                    Currency = @Currency 
                    WHERE BudgetID = @ID";

                    using (SqlCommand cmd = new SqlCommand(query, con))
                    {
                        cmd.Parameters.AddWithValue("@ID", budgetLimit.BudgetID);
                        cmd.Parameters.AddWithValue("@BudgetAmount", budgetLimit.Budget_Amount ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@YearLimit", budgetLimit.Budget_Limit_Year ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@BudgetCategory", budgetLimit.Budget_Category ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@CreationDate", (object?)budgetLimit.Creation_Date?.ToDateTime(default) ?? DBNull.Value);
                        cmd.Parameters.AddWithValue("@BudgetStatus", budgetLimit.Budget_Status ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@ApprovedBy", budgetLimit.Approved_By ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Comment", budgetLimit.Comment ?? (object)DBNull.Value);
                        cmd.Parameters.AddWithValue("@Currency", budgetLimit.Currency ?? (object)DBNull.Value);

                        cmd.ExecuteNonQuery();

                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.Message);
                }
                con.Close();
            }
        }

        public void DeleteData(int budgetId)
        {
            connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(
                        "Delete from BudgetLimits where BudgetID = @ID ", con))
                {
                    cmd.Parameters.AddWithValue("@ID", budgetId);
                    cmd.ExecuteNonQuery();
                }
            }

        }

        public bool DoesBudgetIDExist(int budgetId)
        {
            connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand("Select count(*) from BudgetLimits where BudgetID = @BudgetID", con))
                {
                    cmd.Parameters.AddWithValue("@BudgetID", budgetId);
                    int count = (int)cmd.ExecuteScalar();
                    return count > 0;

                }

            }


        }

        public List<BudgetLimit> SearchData(string columnName, string searchValue)
        {
            List<BudgetLimit> results = new List<BudgetLimit>();
            connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;

            if (!ValidateColumnName(columnName))
            {
                throw new ArgumentException("Ungültiger Spaltenname", nameof(columnName));
            }
            string query = $"SELECT * FROM BudgetLimits WHERE [{columnName}] LIKE @searchValue";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    cmd.Parameters.AddWithValue("@searchValue", $"%{searchValue}%");
                    con.Open();
                    using (SqlDataReader reader = cmd.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            var budgetLimit = new BudgetLimit()
                            {
                                BudgetID = reader.GetInt32(reader.GetOrdinal("BudgetID")),
                                Budget_Amount = reader.IsDBNull(reader.GetOrdinal("Budget_Amount")) ? null : reader.GetInt32(reader.GetOrdinal("Budget_Amount")),
                                Budget_Limit_Year = reader.IsDBNull(reader.GetOrdinal("Budget_Limit_Year")) ? null : reader.GetInt32(reader.GetOrdinal("Budget_Limit_Year")),
                                Budget_Category = reader.IsDBNull(reader.GetOrdinal("Budget_Category")) ? null : reader.GetString(reader.GetOrdinal("Budget_Category")),
                                Creation_Date = reader.IsDBNull(reader.GetOrdinal("Creation_Date")) ? null : (DateOnly?)DateOnly.FromDateTime(reader.GetDateTime(reader.GetOrdinal("Creation_Date"))),
                                Budget_Status = reader.IsDBNull(reader.GetOrdinal("Budget_Status")) ? null : reader.GetString(reader.GetOrdinal("Budget_Status")),
                                Approved_By = reader.IsDBNull(reader.GetOrdinal("Approved_By")) ? null : reader.GetString(reader.GetOrdinal("Approved_By")),
                                Comment = reader.IsDBNull(reader.GetOrdinal("Comment")) ? null : reader.GetString(reader.GetOrdinal("Comment")),
                                Currency = reader.IsDBNull(reader.GetOrdinal("Currency")) ? null : reader.GetString(reader.GetOrdinal("Currency"))

                            };
                            results.Add(budgetLimit);
                        }
                    }
                    con.Close();

                }
            }
            return results;
        }

        //Wenn es ohne dictionary funktioniert, anpassen
        /*
        public void InsertDataIntoDB_csv(List<Dictionary<string, string>> records)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();

                foreach (var record in records)
                {
                    var commandText =
                    "INSERT INTO BudgetLimits " +
                    "(Budget_Amount," +
                    " Budget_Limit_Year," +
                    " Budget_Category," +
                    " Creation_Date," +
                    " Budget_Status," +
                    " Approved_By," +
                    " Comment," +
                    " Currency)" +
                    " VALUES" +
                    " (@BudgetAmount," +
                    " @YearLimit," +
                    " @BudgetCategory," +
                    " @CreationDate," +
                    " @BudgetStatus," +
                    " @ApprovedBy," +
                    " @Comment," +
                    " @Currency)";
                    using (var command = new SqlCommand(commandText, connection))
                    {
                        command.Parameters.AddWithValue("@BudgetAmount", record.ContainsKey(CsvToDbMapping.ColumnMappings["Betrag"]) ? record[CsvToDbMapping.ColumnMappings["Betrag"]] : DBNull.Value);
                        command.Parameters.AddWithValue("@BudgetCategory", record.ContainsKey(CsvToDbMapping.ColumnMappings["Kategorie"]) ? record[CsvToDbMapping.ColumnMappings["Währung"]] : DBNull.Value);
                        command.Parameters.AddWithValue("@YearLimit", record.ContainsKey(CsvToDbMapping.ColumnMappings["Limit des Jahres"]) ? record[CsvToDbMapping.ColumnMappings["Limit des Jahres"]] : DBNull.Value);
                        command.Parameters.AddWithValue("@CreationDate", record.ContainsKey(CsvToDbMapping.ColumnMappings["Erstellt am"]) ? record[CsvToDbMapping.ColumnMappings["Erstellt am"]] : DBNull.Value);
                        command.Parameters.AddWithValue("@BudgetStatus", record.ContainsKey(CsvToDbMapping.ColumnMappings["Status"]) ? record[CsvToDbMapping.ColumnMappings["Status"]] : DBNull.Value);
                        command.Parameters.AddWithValue("@ApprovedBy", record.ContainsKey(CsvToDbMapping.ColumnMappings["Genehmigt von"]) ? record[CsvToDbMapping.ColumnMappings["Genehmigt von"]] : DBNull.Value);
                        command.Parameters.AddWithValue("@Comment", record.ContainsKey(CsvToDbMapping.ColumnMappings["Kommentar"]) ? record[CsvToDbMapping.ColumnMappings["Kommentar"]] : DBNull.Value);
                        command.Parameters.AddWithValue("@Currency", record.ContainsKey(CsvToDbMapping.ColumnMappings["Währung"]) ? record[CsvToDbMapping.ColumnMappings["Währung"]] : DBNull.Value);
                        command.ExecuteNonQuery();
                    }
                    RecordAdded?.Invoke(this, EventArgs.Empty);
                    connection.Close();
                }
            }
        }
        */
        public void ReadCsvAndInsertInToDB(string filepath)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
            using (var connection = new SqlConnection(connectionString))
            {
                connection.Open();
                var lines = File.ReadAllLines(filepath);

                for (int i = 1; i < lines.Length; i++)
                {
                    var values = lines[i].Split(',');
                    bool dateProvided = !string.IsNullOrWhiteSpace(values[4]);
                    var sql = @"
                    INSERT INTO BudgetLimits 
                    (Budget_Amount, 
                    Currency,
                    Budget_Limit_Year,
                    Budget_Category," +
                    (dateProvided ? "Creation_Date, " : "") +
                    @"Budget_Status,
                    Approved_By,
                    Comment) 
                    VALUES (
                    @BudgetAmount,
                    @Currency,
                    @YearLimit,
                    @BudgetCategory," +
                    (dateProvided ? "@CreationDate, " : "GETDATE(),") +
                    @"@Status,
                    @ApprovedBy,
                    @Comment)";

                    using (var command = new SqlCommand(sql, connection))
                    {
                        command.Parameters.AddWithValue("@BudgetAmount", string.IsNullOrWhiteSpace(values[0]) ? DBNull.Value : (object)int.Parse(values[0]));
                        command.Parameters.AddWithValue("@Currency", values[1]);
                        command.Parameters.AddWithValue("@YearLimit", string.IsNullOrWhiteSpace(values[2]) ? DBNull.Value : (object)int.Parse(values[2]));
                        command.Parameters.AddWithValue("@BudgetCategory", values[3]);
                        if (dateProvided)
                        {
                            command.Parameters.AddWithValue(@"CreationDate", DateTime.Parse(values[4].Trim()));
                        }
                        command.Parameters.AddWithValue("@Status", values[5]);
                        command.Parameters.AddWithValue("@ApprovedBy", values[6]);
                        command.Parameters.AddWithValue("@Comment", values[7]);
                        // Führe den Befehl aus
                        command.ExecuteNonQuery();
                    }
                }
            }
        }
        private bool ValidateColumnName(string columnName)
        {
            return true;
        }
    }
}
