using System;
using Microsoft.Maui.Controls;

namespace LurraldeOrdezkaritzak
{
    public partial class MainPage : ContentPage
    {
        public MainPage()
        {
            InitializeComponent();
        }

        private async void OnSaioaHasiClicked(object sender, EventArgs e)
        {
            string erabilztailea = ErabiltzaileaEntry.Text;
            string pasahitza = PasahitzaEntry.Text;

            if (erabilztailea == "admin" && pasahitza == "admin")
            {
                await Shell.Current.GoToAsync("//Hasiera");
            }
            else
            {
                await DisplayAlert("Errorea", "Erabiltzailea edo pasahitza okerrak", "OK");
            }
        }
    }
}
