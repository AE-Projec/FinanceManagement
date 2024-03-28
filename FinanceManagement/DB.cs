using System.Configuration;
using System.Data;
using System.Data.SqlClient;
using System.Globalization;
using System.IO;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Xml.Linq;

namespace FinanceManagement
{
    public class DB
    {
        private string? connectionString;
        public event Action? DataUpdated;
        public event Action? DataShouldBeReloaded;
        public event EventHandler? RecordAdded;
        public event EventHandler? RecordRemoved;


        //for the datagrid Budget get Updatedated, Important
        /*
        public void ReadData(DataGrid budgetsDataGrid)
        {

            connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
            List<BudgetLimits> budgetLimits = new List<BudgetLimits>();
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
                            var budgetLimit = new BudgetLimits()
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
        }*/


        /*---------------------
        START OF GENERIC DB METHODS 

         ---------------------*/
        //for datagrid
        public void ReadData<T>(DataGrid dataGrid) where T : class, new()
        {
            connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
            List<T> items = new List<T>();

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                try
                {
                    con.Open();
                    string tableName = typeof(T).Name;

                    using (SqlCommand cmd = new SqlCommand($"select * from {tableName}", con))
                    {
                        SqlDataReader reader = cmd.ExecuteReader();
                        while (reader.Read())
                        {
                            T item = new T();

                            foreach (var property in typeof(T).GetProperties())
                            {
                                if (reader[property.Name] != null)
                                {
                                    property.SetValue(item, reader[property.Name]);
                                }
                            }
                            items.Add(item);
                        }
                        reader.Close();
                    }
                }
                catch (Exception ex)
                {
                    MessageBox.Show($"Ein Fehler ist aufgetreten beim Zugriff auf die Datenbank: {ex.Message}");
                }
            }

            dataGrid.Dispatcher.Invoke(() =>
            {
                dataGrid.ItemsSource = items;
            });
        }

        //for table values from the database
        public List<T> ReadData<T>(string tableName) where T : new()
        {
            var results = new List<T>();
            connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
            var query = $"select * from {tableName}";

            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        using (var reader = command.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                var item = CreateItemFromReader<T>(reader);
                                results.Add(item);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ein fehler ist aufgetreten: {ex.Message}");
                    }
                }
            }
            return results;
        }

        public T? ReadFirstEntry<T>(string tableName) where T : new()
        {
            T? result = default;
            //tableName = $"{typeof(T).Name}s";

            connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
            var query = $"select top 1 * from {tableName} order by 1 asc";

            using (var connection = new SqlConnection(connectionString))
            {
                using (var command = new SqlCommand(query, connection))
                {
                    try
                    {
                        connection.Open();
                        SqlDataReader reader = command.ExecuteReader();
                        if (reader.Read())
                        {
                            result = CreateItemFromReader<T>(reader);

                        }
                    }
                    catch (Exception ex)
                    {
                        MessageBox.Show($"Ein fehler ist aufgetreten: {ex.Message}");
                    }
                }
                return result;
            }
        }

        public void InsertIntoData<T>(string tableName, T item)
        {
            connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                // gibt alles aus der tabelle aus was keine ID ist(0 = keine ID , 1 = ID)
                string query = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}' AND COLUMNPROPERTY(OBJECT_ID(TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 0";

                List<string> columns = new List<string>();
                using (SqlCommand cmdColumns = new SqlCommand(query, con))
                {
                    using (SqlDataReader reader = cmdColumns.ExecuteReader())
                    {
                        while (reader.Read())
                        {
                            columns.Add(reader.GetString(0));
                        }
                    }
                }

                string columnNames = string.Join(", ", columns);
                string parameters = string.Join(", ", columns.Select(col => $"@{col}"));

                string insertQuery = $"INSERT INTO {tableName} ({columnNames}) VALUES ({parameters})";

                using (SqlCommand cmd = new SqlCommand(insertQuery, con))
                {
                    foreach (var col in columns)
                    {
                        var prop = typeof(T).GetProperty(col);
                        object value = prop?.GetValue(item) ?? DBNull.Value;
                        if (value == null || value == DBNull.Value)
                        {
                            DateTime? dateValue = null;
                            if (IsDateColumn(tableName, col))
                            {
                                DateTime? dateOnlyValue = (DateTime?)prop.GetValue(item);
                                if (dateOnlyValue.HasValue)
                                {
                                    dateValue = new DateTime(dateOnlyValue.Value.Year, dateOnlyValue.Value.Month, dateOnlyValue.Value.Day);
                                }
                                else
                                {
                                    dateValue = DateTime.Now.Date; // Aktuelles Datum ohne Uhrzeit
                                }
                                cmd.Parameters.AddWithValue($"@{col}", dateValue);
                            }
                            else
                            {
                                cmd.Parameters.AddWithValue($"@{col}", value);
                            }
                        }
                        else
                        {
                            cmd.Parameters.AddWithValue($"@{col}", value);
                        }

                    }

                    cmd.ExecuteNonQuery();
                }
                RecordAdded?.Invoke(this, EventArgs.Empty);
                con.Close();
            }
        }

        public void UpdateData<T>(string tableName, T item) where T : class
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;

            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();

                    // Spalten abrufen, die keine IDs sind
                    string query = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}' AND COLUMNPROPERTY(OBJECT_ID(TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 0";

                    List<string> columns = new List<string>();
                    using (SqlCommand cmdColumns = new SqlCommand(query, con))
                    {
                        using (SqlDataReader reader = cmdColumns.ExecuteReader())
                        {
                            while (reader.Read())
                            {
                                columns.Add(reader.GetString(0));
                            }
                        }
                    }

                    string setClause = string.Join(", ", columns.Select(col => $"{col} = @{col}"));

                    // WHERE-Klausel basierend auf der ID-Spalte erstellen
                    var idColumnName = GetIdColumnName(tableName);
                    string whereClause = $" WHERE {idColumnName} = @{idColumnName}";

                    string updateQuery = $"UPDATE {tableName} SET {setClause}{whereClause}";

                    // UPDATE-Abfrage ausführen
                    using (SqlCommand cmd = new SqlCommand(updateQuery, con))
                    {
                        foreach (var col in columns)
                        {
                            var prop = typeof(T).GetProperty(col);
                            object value = prop?.GetValue(item) ?? DBNull.Value;

                            if (value is DateOnly dateOnlyValue)
                            {
                                value = dateOnlyValue.ToDateTime(default);
                            }
                            cmd.Parameters.AddWithValue($"@{col}", value == null || value == DBNull.Value ? GetDefaultValue(tableName, col) : value);
                        }

                        // ID-Parameter hinzufügen
                        var idProp = typeof(T).GetProperties().FirstOrDefault(prop => prop.Name.Equals(idColumnName, StringComparison.OrdinalIgnoreCase));
                        if (idProp != null)
                        {
                            var idValue = idProp.GetValue(item);
                            cmd.Parameters.AddWithValue($"@{idColumnName}", idValue ?? DBNull.Value);
                        }
                        cmd.ExecuteNonQuery();
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }

        public void DeleteData<T>(string tableName, string idColumnName, object idValue) where T : class
        {
            connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
            try
            {
                using (SqlConnection con = new SqlConnection(connectionString))
                {
                    con.Open();
                    // Spalte abrufen, die ein ID ist
                    using (SqlCommand cmd = new SqlCommand(
                            $"DELETE FROM {tableName} WHERE {idColumnName} = @ID", con))
                    {
                        cmd.Parameters.AddWithValue("@ID", idValue);

                        cmd.ExecuteNonQuery();
                    }

                    RecordRemoved?.Invoke(this, EventArgs.Empty);
                    con.Close();
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message);
            }
        }


        /*-------------------------------------
         END OF GENERIC DATABASE METHODS         

         -------------------------------------*/

        //Hilfsmethode für ReadData für die richtige ausgabe des Datums(Date)
        private T? CreateItemFromReader<T>(SqlDataReader reader) where T : new()
        {
            T item = new T();
            foreach (var property in typeof(T).GetProperties())
            {
                if (!reader.IsDBNull(reader.GetOrdinal(property.Name)))
                {
                    if (property.PropertyType == typeof(DateOnly?) || property.PropertyType == typeof(DateTime))
                    {
                        var dateValue = reader.GetDateTime(reader.GetOrdinal(property.Name));
                        property.SetValue(item, DateOnly.FromDateTime(dateValue));
                    }
                    else
                    {
                        var value = reader[property.Name];
                        property.SetValue(item, value);
                    }

                }
            }
            return item;
        }

        // Hilfsmethode zum Überprüfen, ob eine Spalte ein Datumsfeld ist
        private bool IsDateColumn(string tableName, string columnName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
            // Gibt den datentypen innerhalb der Tabelle und spalte aus
            string query = $"SELECT DATA_TYPE FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}' AND COLUMN_NAME = '{columnName}'";
            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();
                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    string dataType = (string)cmd.ExecuteScalar();
                    return dataType == "date" || dataType == "datetime";
                }
            }
        }

        //todo : Datum bei eintragung in die DB über UI in deutsch anzeigen
        /*
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

        }*/

        // Methode zum Abrufen des ID-Spaltennamens
        private string? GetIdColumnName(string tableName)
        {
            string connectionString = ConfigurationManager.ConnectionStrings["MyDBConnectionString"].ConnectionString;
            string query = $"SELECT COLUMN_NAME FROM INFORMATION_SCHEMA.COLUMNS WHERE TABLE_NAME = '{tableName}' AND COLUMNPROPERTY(OBJECT_ID(TABLE_NAME), COLUMN_NAME, 'IsIdentity') = 1";

            using (SqlConnection con = new SqlConnection(connectionString))
            {
                con.Open();

                using (SqlCommand cmd = new SqlCommand(query, con))
                {
                    return cmd.ExecuteScalar()?.ToString();
                }
            }
        }

        // Methode zum Abrufen des Standardwerts für eine bestimmte Spalte
        private object GetDefaultValue(string tableName, string columnName)
        {
            if (IsDateColumn(tableName, columnName))
            {
                return DateTime.Now;
            }
            // Weitere Logik zum Bestimmen des Standardwerts für andere Spaltentypen
            return DBNull.Value;
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

    }
}
