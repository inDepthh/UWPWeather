using System;
using System.Collections;
using System.Collections.Generic;
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
            var position = await LocationManager.GetPosition();

            if (autoDetect)
            {
                ResultTextBlock.Text = "Loading...";
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
                ResultTextBlock.Text = myWeather.name + " - " + ((int)myWeather.main.temp).ToString() + " - " + myWeather.weather[0].description;

            } catch(Exception ex)
            {
                Debug.WriteLine("Exception: " + ex.StackTrace);
                progressRing.IsActive = false;
                ResultTextBlock.Foreground = new SolidColorBrush(Colors.Red);
                ResultTextBlock.Text = "Error: Please check your spelling and formatting";
            }
                      

         

                       

            // string icon = String.Format("ms-appx:///Assets/Weather/{0}.png", myWeather.weather[0].icon);
            // ResultImage.Source = new BitmapImage(new Uri(icon, UriKind.Absolute));
        }



        public void InsertData()
        {
            //databaseHelper.InsertWeather("New York", 80);
            //databaseHelper.InsertWeather("Mars", 10000);
            //databaseHelper.InsertWeather("Canada", 3);
            //databaseHelper.InsertWeather("Australia", 800);
        }

        public void GetData()
        {
            //databaseHelper.fetchWeather();
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
    }
}
