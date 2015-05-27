using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using WinWeatherApp.Resources;
using System.IO.IsolatedStorage;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using Newtonsoft.Json;

namespace WinWeatherApp
{
    public partial class MainPage : PhoneApplicationPage, INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        private void NotifyPropertyChanged(String propertyName)
        {
            PropertyChangedEventHandler handler = PropertyChanged;
            if (null != handler)
            {
                handler(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        private bool NotifyPropertyChanged<T>(ref T variable, T valeur, [CallerMemberName] string nomPropriete = null)
        {
            if (object.Equals(variable, valeur)) return false;

            variable = valeur;
            NotifyPropertyChanged(nomPropriete);
            return true;
        }

        private List<Meteo> listeMeteo;
        public List<Meteo> ListeMeteo
        {
            get { return listeMeteo; }
            set { NotifyPropertyChanged(ref listeMeteo, value); }
        }

        private bool chargementEnCours;
        public bool ChargementEnCours
        {
            get { return chargementEnCours; }
            set { NotifyPropertyChanged(ref chargementEnCours, value); }
        }

        private string nomVille;
        public string NomVille
        {
            get { return nomVille; }
            set { NotifyPropertyChanged(ref nomVille, value); }
        }

        public MainPage()
        {
            InitializeComponent();
            DataContext = this;
        }

        protected async override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (IsolatedStorageSettings.ApplicationSettings.Contains("DerniereVille"))
            {
                Information.Visibility = Visibility.Collapsed;
                ChargementEnCours = true;
                NomVille = (string)IsolatedStorageSettings.ApplicationSettings["DerniereVille"];
                WebClient client = new WebClient();
                try
                {
                    ChargementEnCours = false;
                    string resultatMeteo = await client.DownloadStringTaskAsync(new Uri(string.Format("http://free.worldweatheronline.com/feed/weather.ashx?q={0}&format=json&num_of_days=5&key=f9f4e82a4ba4ccb2ec8faf3c7a3f3", NomVille.Replace(' ', '+')), UriKind.Absolute));

                    RootObject resultat = JsonConvert.DeserializeObject<RootObject>(resultatMeteo);
                    List<Meteo> liste = new List<Meteo>();
                    foreach (Weather temps in resultat.data.weather.OrderBy(w => w.date))
                    {
                        Meteo meteo = new Meteo { TemperatureMax = temps.tempMaxC + " °C", TemperatureMin = temps.tempMinC + " °C" };
                        DateTime date;
                        if (DateTime.TryParse(temps.date, out date))
                        {
                            meteo.Date = date.ToString("dddd dd MMMM");
                            meteo.Temps = GetTemps(temps.weatherCode);
                            WeatherIconUrl2 url = temps.weatherIconUrl.FirstOrDefault();
                            if (url != null)
                            {
                                meteo.Url = new Uri(url.value, UriKind.Absolute);
                            }
                        }
                        liste.Add(meteo);
                    }
                    ListeMeteo = liste;
                }
                catch (Exception)
                {
                    MessageBox.Show("Impossible de récupérer les informations de météo, vérifiez votre connexion internet");
                }
            }
            else
                Information.Visibility = Visibility.Visible;

            base.OnNavigatedTo(e);
        }

        private string GetTemps(string code)
        {
            // à compléter ...
            switch (code)
            {
                case "113":
                    return "Clair / Ensoleillé";
                case "116":
                    return "Partiellement nuageux";
                case "119":
                    return "Nuageux";
                case "296":
                    return "Faible pluie";
                case "353":
                    return "Pluie";
                default:
                    return "";
            }
        }

        private void Ajouter_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/Ajouter.xaml", UriKind.Relative));
        }

        private void Choisir_Click(object sender, EventArgs e)
        {
            NavigationService.Navigate(new Uri("/ChoisirVille.xaml", UriKind.Relative));
        }
    }
}