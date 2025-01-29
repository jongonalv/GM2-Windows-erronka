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
            await Shell.Current.GoToAsync("//Hasiera");

        }
    }
}
