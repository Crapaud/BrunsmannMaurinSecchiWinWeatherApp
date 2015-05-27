using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Navigation;
using Microsoft.Phone.Controls;
using Microsoft.Phone.Shell;
using System.IO.IsolatedStorage;

namespace WinWeatherApp
{
    public partial class Ajouter : PhoneApplicationPage
    {
        public Ajouter()
        {
            InitializeComponent();
        }

        private void Button_Tap(object sender, System.Windows.Input.GestureEventArgs e)
        {
            if (string.IsNullOrEmpty(NomVille.Text))
            {
                MessageBox.Show("Veuillez saisir un nom de ville");
            }
            else
            {
                IsolatedStorageSettings.ApplicationSettings["DerniereVille"] = NomVille.Text;
                List<string> nomVilles;
                if (IsolatedStorageSettings.ApplicationSettings.Contains("ListeVilles"))
                    nomVilles = (List<string>)IsolatedStorageSettings.ApplicationSettings["ListeVilles"];
                else
                    nomVilles = new List<string>();
                nomVilles.Add(NomVille.Text);
                IsolatedStorageSettings.ApplicationSettings["ListeVilles"] = nomVilles;

                if (NavigationService.CanGoBack)
                    NavigationService.GoBack();
            }
        }
    }
}