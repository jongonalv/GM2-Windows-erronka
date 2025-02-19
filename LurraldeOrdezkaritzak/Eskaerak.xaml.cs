using System;
using System.IO;
using Microsoft.Maui.Controls;
using lurraldeOrdezkaritzak;
using LurraldeOrdezkaritzak.ViewModels;


namespace LurraldeOrdezkaritzak;

public partial class Eskaerak : ContentPage
{
    EskaerakViewModel _viewModel;
            DBManager _dbmanager;

    public Eskaerak()
    {
        InitializeComponent();
        _viewModel = new EskaerakViewModel();
        BindingContext = _viewModel;
    }
    private async void datuakKargatuButton_Clicked(object sender, EventArgs e)
    {
   
    }
}
