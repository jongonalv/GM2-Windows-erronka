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

        public Stock()
        {
            InitializeComponent();

            var dbManager = new DBManager(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "lurraldeOrdezkaritzak.db3"));
            _viewModel = new StockViewModel(dbManager);

            BindingContext = _viewModel;
            _viewModel.LoadKategoriakCommand.Execute(null);
        }
    }
}
