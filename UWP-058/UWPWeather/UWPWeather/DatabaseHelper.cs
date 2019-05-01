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

        public void updateWeather(string Column, string ConnectionString)
        {
            SqlConnection cn = new SqlConnection();
            DataSet dataSet = new DataSet();
            SqlDataAdapter da;
            SqlCommandBuilder cmdBuilder;

            //cn.ConnectionString = "Data Source=DESKTOP-J1CRMQR;Initial Catalog=Weather;Integrated Security=SSPI";
            cn.ConnectionString = ConnectionString;
            cn.Open();

             da = new SqlDataAdapter("SELECT * FROM dbo.Data", cn);

            //the UpdateCommand, InsertCommand, and DeleteCommand properties of the SqlDataAdapter.
            cmdBuilder = new SqlCommandBuilder(da);

            //Populate the DataSet
            da.Fill(dataSet, "Data");

            //Temperature field before updating the data using the DataSet.
            Debug.WriteLine("Data before Update : " + dataSet.Tables["Data"].Rows[0][Column]);

            //Modify the value of the Temperature field.
            dataSet.Tables["Data"].Rows[0][Column] = 45;

            //Post the data modification to the database.
            da.Update(dataSet, "Data");
            
            Debug.WriteLine("Data updated successfully");
            cn.Close();
        }

        public ObservableCollection<WeatherDB> FetchWeather(string connectionString, string Location, float Temperature)
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
                                    var data = new WeatherDB(Location, Temperature);
                                    data.Location= reader.GetString(0);
                                    data.Temperature = reader.GetFloat(1);
                                }
                            }
                        }
                    }
                }
                return weatherData;
            }
            catch (Exception eSql)
            {
                Debug.WriteLine("Exception: " + eSql.Message);
            }
            return null;
        }

        public void InsertWeather(string Location, float Temperature, string connectionString)
        {
            //string strConnect = "Data Source=localhost;Initial Catalog=Weather;Integrated Security=True";
            SqlConnection conn = null;
            try
            {
                conn = new SqlConnection(connectionString);
                conn.Open();

                //populate Weather data
                WeatherDB weather1 = new WeatherDB(Location, Temperature);
                SaveWeather(new WeatherDB[] { weather1 }, conn);

            }
            catch (SqlException ex)
            {
                Debug.WriteLine("Failed: {0}", ex.Message.ToString());
                Debug.WriteLine("Failed: {0}", ex.ToString());
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
                comm.Parameters["@Location"].Value = t.Location;
                comm.Parameters["@Temperature"].Value = t.Temperature;
                comm.ExecuteNonQuery();

                Debug.Write("Inserted Location {0}", t.Location);
            }
        }
    }

    public class WeatherDB
    {
        public string Location { get; set; }
        public float Temperature { get; set; }

        public WeatherDB(string Location, float Temperature)
        {
            this.Location = Location;
            this.Temperature = Temperature;
        }
    }
}
