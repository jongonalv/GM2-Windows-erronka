using lurraldeOrdezkaritzak;
using LurraldeOrdezkaritzak;
using LurraldeOrdezkaritzak.ViewModels;
using Microsoft.Maui.Controls;

namespace LurraldeOrdezkaritzak;

public partial class Estadistikak : ContentPage
{
    private DBManager _dbmanager;

    public Estadistikak()
    {
        _dbmanager = new DBManager(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "lurraldeOrdezkaritzak.db3"));

        InitializeComponent();
        LoadData();
    }

    private async void LoadData()
    {
        // Obtén los datos de la base de datos
        var estadistikak = await _dbmanager.GetArtikuloaEstadistikakAsync();

        // Asigna los datos al CollectionView
        collectionView.ItemsSource = estadistikak;

        var estadistikak2 = await _dbmanager.GetBazkideakEskaeraEstadistikakAsync();

        // Asigna los datos a un CollectionView o cualquier otro control
        bazkideakCollectionView.ItemsSource = estadistikak2;

        // Obtén los datos de la base de datos
        var saldaerak = await _dbmanager.GetBazkideakArtikuloaSaldaeraAsync();

        // Asigna los datos a un CollectionView o cualquier otro control
        saldaeraCollectionView.ItemsSource = saldaerak;
    }

    public class ArtikuloaEstadistika
    {
        public int ArtikuloId { get; set; }
        public string ArtikuloIzena { get; set; }
        public int TotalSalduta { get; set; }
    }

    public class BazkideaEskaeraEstadistika
    {
        public int BazkideaId { get; set; } 
        public string BazkideaIzena { get; set; }
        public int EskaeraTotalak { get; set; } 
    }

    public class BazkideaArtikuloaSaldaera
    {
        public int BazkideaId { get; set; } 
        public string BazkideaIzena { get; set; } 
        public int ArtikuloaId { get; set; }
        public string ArtikuloaIzena { get; set; } 
        public int Unitateak { get; set; } 
        public decimal TotalSaldaera { get; set; } 
    }

}