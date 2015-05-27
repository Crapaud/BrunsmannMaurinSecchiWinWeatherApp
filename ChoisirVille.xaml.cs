using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.ComponentModel;
using System.IO.IsolatedStorage;
using System.Runtime.CompilerServices;

namespace WinWeatherApp
{
    public partial class ChoisirVille : PhoneApplicationPage, INotifyPropertyChanged
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

        private List<string> listeVilles;
        public List<string> ListeVilles
        {
            get { return listeVilles; }
            set { NotifyPropertyChanged(ref listeVilles, value); }
        }

        public ChoisirVille()
        {
            InitializeComponent();
            DataContext = this;
        }

        protected override void OnNavigatedTo(System.Windows.Navigation.NavigationEventArgs e)
        {
            if (!IsolatedStorageSettings.ApplicationSettings.Contains("ListeVilles"))
            {
                MessageBox.Show("Vous devez ajouter des villes");
                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
            }
            else
            {
                ListeVilles = (List<string>)IsolatedStorageSettings.ApplicationSettings["ListeVilles"];
                if (ListeVilles.Count == 0)
                {
                    MessageBox.Show("Vous devez ajouter des villes");
                    if (NavigationService.CanGoBack)
                        NavigationService.GoBack();
                }

                if (IsolatedStorageSettings.ApplicationSettings.Contains("DerniereVille"))
                {
                    string ville = (string)IsolatedStorageSettings.ApplicationSettings["DerniereVille"];
                    int index = ListeVilles.IndexOf(ville);
                    if (index >= 0)
                        Liste.SelectedIndex = index;
                }
                Liste.SelectionChanged += Liste_SelectionChanged;
            }
            base.OnNavigatedTo(e);
        }

        protected override void OnNavigatedFrom(System.Windows.Navigation.NavigationEventArgs e)
        {
            Liste.SelectionChanged -= Liste_SelectionChanged;
            base.OnNavigatedFrom(e);
        }

        private void Liste_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (Liste.SelectedItem != null)
            {
                IsolatedStorageSettings.ApplicationSettings["DerniereVille"] = (string)Liste.SelectedItem;
            }
        }
    }
}