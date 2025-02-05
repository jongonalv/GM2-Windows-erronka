using System;
using System.IO;
using Microsoft.Maui.Controls;
using lurraldeOrdezkaritzak;
using LurraldeOrdezkaritzak.ViewModels;

namespace LurraldeOrdezkaritzak
{
    public partial class Stock : ContentPage
    {
        private readonly StockViewModel _viewModel;
        DBManager _dbmanager;

        public Stock()
        {
            InitializeComponent();

            _dbmanager = new DBManager(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "lurraldeOrdezkaritzak.db3"));
            _viewModel = new StockViewModel(_dbmanager);

            BindingContext = _viewModel;
            _viewModel.LoadKategoriakCommand.Execute(null);
        }

        private async void datuakKargatuButton_Clicked(object sender, EventArgs e)
        {
            await _dbmanager.XMLArtikuloakKargatu();
            await _viewModel.LoadAllArtikuloak();
        }
    }
}
