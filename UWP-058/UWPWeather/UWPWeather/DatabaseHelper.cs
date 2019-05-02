using System.Diagnostics;
using System.Data.SqlClient;
using System.Data;
using System.Collections;
using System.Collections.ObjectModel;
using System;

namespace UWPWeather
{
    class DatabaseHelper
    {
        string connectionString = "Data Source=DESKTOP-J1CRMQR; Initial Catalog=Weather; User id=sa; Password=sapassword123;";
        public void updateWeather(string Column)
        {
            SqlConnection cn = new SqlConnection();
            DataSet dataSet = new DataSet();
            SqlDataAdapter da;
            SqlCommandBuilder cmdBuilder;

            cn.ConnectionString = connectionString;
            cn.Open();

             da = new SqlDataAdapter("SELECT * FROM dbo.Data", cn);

            cmdBuilder = new SqlCommandBuilder(da);

            //Populate the DataSet
            da.Fill(dataSet, "Data");

            //Temperature field before updating the data using the DataSet.
            Debug.WriteLine("Data before Update : " + dataSet.Tables["Data"].Rows[0][Column]);

            //Modify the value of the Temperature field.
            dataSet.Tables["Data"].Rows[0][Column] = 45;

            da.Update(dataSet, "Data");
            
            Debug.WriteLine("Data updated successfully");
            cn.Close();
        }

        public ObservableCollection<WeatherDB> fetchWeather()
        {
            const string GetWeatherQuery = "select Location, Temperature FROM Data";

            var weatherData = new ObservableCollection<WeatherDB>();
            try
            {
                using (SqlConnection conn = new SqlConnection(connectionString))
                {
                    conn.Open();
                    if (conn.State == System.Data.ConnectionState.Open)
                    {
                        using (SqlCommand cmd = conn.CreateCommand())
                        {
                            cmd.CommandText = GetWeatherQuery;
                            using (SqlDataReader reader = cmd.ExecuteReader())
                            {
                                while (reader.Read())
                                {
                                    string stringData = reader.GetString(0);
                                    double doubleData = reader.GetDouble(1);
                                    Debug.WriteLine(stringData);
                                    Debug.WriteLine(doubleData);

                                    var weather = new WeatherDB(stringData, doubleData);

                                   // weather.Location.Add(stringData);
                                   // weather.Temperature.Add(doubleData);

                                   // weatherData.Add(stringData);
                                    //Debug.WriteLine("Location: " + data.Location.Count + " Temperature: " + data.Temperature.Count);
                                }
                            }
                        }
                    }
                    conn.Close();
                }
                
                //return weatherData;
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.StackTrace);
            }
            return weatherData;
           
        }

        public void InsertWeather(string Location, double Temperature)
        {
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();

                //populate Weather data
                WeatherDB weather = new WeatherDB(Location, Temperature);
                SaveWeather(new WeatherDB[] { weather }, conn);

            }
            catch (SqlException ex)
            {
                Debug.WriteLine("Failed: {0}", ex.StackTrace);
                //Debug.WriteLine("Failed: {0}", ex.ToString());
            }
            finally
            {
                if (conn != null)
                    conn.Close();
            }
        }

        public void SaveWeather(WeatherDB[] aWeather, SqlConnection conn)
        {
            string SQL = "INSERT INTO Data (Location, Temperature) VALUES (@Location, @Temperature)";
            SqlCommand comm = new SqlCommand(SQL, conn);

            // Create parameters
            comm.Parameters.Add("@Location", SqlDbType.VarChar, 50);
            comm.Parameters.Add("@Temperature", SqlDbType.Float);
            // Step through each weather set in turn
            comm.Prepare();

            foreach (WeatherDB t in aWeather)
            {
                comm.Parameters["@Location"].Value = t.Location[0];
                comm.Parameters["@Temperature"].Value = t.Temperature[0];
                comm.ExecuteNonQuery();

                Debug.Write("Inserted Location {0}", t.Location[0].ToString());
            }
        }

        public void deleteWeather()
        {
            using (SqlConnection conn = new SqlConnection(connectionString))
            {
                conn.Open();
                SqlDataAdapter rdr = new SqlDataAdapter("SELECT Location FROM Data", conn);
                SqlCommandBuilder cmdBuilder = new SqlCommandBuilder(rdr);
                DataSet dataSet = new DataSet();
                SqlCommand cmd = new SqlCommand("DELETE FROM Data WHERE Location = @location", rdr.SelectCommand.Connection);
                cmd.Parameters.Add(new SqlParameter("@location", SqlDbType.VarChar, 50));
                cmd.Parameters["@location"].SourceVersion = DataRowVersion.Original;
                cmd.Parameters["@location"].SourceColumn = "location";
                rdr.DeleteCommand = cmd;
                rdr.Fill(dataSet, "Data");
                //Debug.WriteLine(cmdBuilder.GetDeleteCommand().CommandText);
                foreach (DataRow row in dataSet.Tables["Data"].Rows)
                {
                   // if (dataSet.Tables["Data"].Rows[0] == (DataRow) deleteLocation[0])
                  //  {
                        Debug.WriteLine("row: " + row[0]);
                        row.Delete();
                   // }
                    
                }
                rdr.Update(dataSet, "Data");
                conn.Close();
            }
        }
    }

    public class WeatherDB
    {
        public ObservableCollection<string> Location { get; set; }
        public ObservableCollection<double> Temperature{ get; set; }

        public WeatherDB(string Location, double Temperature)
        {
            this.Location = new ObservableCollection<string>();
            this.Temperature = new ObservableCollection<double>();

            this.Location.Add(Location);
            this.Temperature.Add(Temperature);
        }

        public WeatherDB()
        {
        }
    }
}
