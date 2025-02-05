using System;
using System.IO;
using Microsoft.Maui.Controls;
using lurraldeOrdezkaritzak;
using LurraldeOrdezkaritzak.ViewModels;


namespace LurraldeOrdezkaritzak;

public partial class Eskaerak : ContentPage
{
    EskaerakViewModel _viewModel;

    public Eskaerak()
    {
        InitializeComponent();
        _viewModel = new EskaerakViewModel();
        BindingContext = _viewModel;
    }
}
