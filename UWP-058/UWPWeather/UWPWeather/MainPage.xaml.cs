using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices.WindowsRuntime;
using Windows.Foundation;
using Windows.Foundation.Collections;
using Windows.UI;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Controls.Primitives;
using Windows.UI.Xaml.Data;
using Windows.UI.Xaml.Input;
using Windows.UI.Xaml.Media;
using Windows.UI.Xaml.Media.Imaging;
using Windows.UI.Xaml.Navigation;

namespace UWPWeather
{
    public sealed partial class MainPage : Page
    {
        public MainPage()
        {
            this.InitializeComponent();
            

        }

        DatabaseHelper databaseHelper = new DatabaseHelper();
        bool autoDetect = false;
        bool exception = false;

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            
            RootObject myWeather = null;

            string city = textBoxCity.Text;
            Debug.WriteLine("City: " + city);

            string country = textBoxCountry.Text;
            Debug.WriteLine("Country: " + country);

            progressRing.IsActive = true;
            ResultTextBlock.Text = "Loading...";
            var position = await LocationManager.GetPosition();

            if (autoDetect)
            {
               
                myWeather = await OpenWeatherMapProxy.GetWeather(position.Coordinate.Latitude, position.Coordinate.Longitude);
            }
            else
            {
               myWeather = await OpenWeatherMapProxy.GetWeather(city, country);
            }

            try
            {
                progressRing.IsActive = false;
                ResultTextBlock.Foreground = new SolidColorBrush(Colors.Black);
                ResultTextBlock.Text = "Location: " + myWeather.name + ", "+ myWeather.sys.country + ", Temperature: " + ((int)myWeather.main.temp).ToString() + ", " + myWeather.weather[0].description +
                    ", Humidity: " + myWeather.main.humidity + ", Wind Speed: " + myWeather.wind.speed + ", Sunrise: " + myWeather.sys.sunrise + ", Sunset: " + myWeather.sys.sunset;

            } catch(Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.StackTrace);
                progressRing.IsActive = false;
                ResultTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                ResultTextBlock.Text = "Error: Please check your spelling and format";
            }
        }

        public void InsertData(string data)
        {
            databaseHelper.InsertWeather(data, 80);
        }

        public ObservableCollection<WeatherDB> GetData()
        {
            return databaseHelper.fetchWeather();
        }

        public void UpdateData()
        {
            //databaseHelper.updateWeather("Temperature");
        }

        public void DeleteData()
        {
            //databaseHelper.deleteWeather();
        }

        private void CheckBox_Checked(object sender, RoutedEventArgs e)
        {
            autoDetect = true;
        }

        private void CheckBox_Unchecked(object sender, RoutedEventArgs e)
        {
            autoDetect = false;
        }

        private void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string city = textBoxCity.Text;
            Debug.WriteLine("City: " + city);

            string country = textBoxCountry.Text;
            Debug.WriteLine("Country: " + country);

            string data = city + ", " + country;
           // InsertData(data);

            //string fetch = GetData()[0].ToString();
            WeatherDB weatherdb = new WeatherDB();
            //string location = weatherdb.Location[0].ToString();
            Debug.WriteLine("Fetched location: " + weatherdb.Location.Count);

            //SavedAddresses saveAddress = new SavedAddresses();
           // saveAddress.Address.Add(location);
           
            listView.ItemsSource = GetData();

            

            
        //List<string> saveAdress = (runtimeObject) new List<string>();
        //saveAddress.Address.Add("one");
        //    saveAddress.Address.Add("two");
        //    saveAddress.Address.Add("three");
        //    saveAddress.Address.Add("four");
        //    saveAddress.Address.Add("five");
            //Debug.WriteLine("saveAddresses: " + saveAddress.Address);
        }
    }
}
