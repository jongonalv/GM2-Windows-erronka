using System.Collections.ObjectModel;
using System.Threading.Tasks;
using System.Windows.Input;
using Microsoft.Maui.Controls;
using System.Linq;
using System.Collections.Generic;
using lurraldeOrdezkaritzak;

namespace LurraldeOrdezkaritzak.ViewModels
{
    public class StockViewModel : BaseViewModel
    {
        /// <summary>
        /// Datu-basearekin elkarreragiteko DBManager instantzia gordetzen du.
        /// </summary>
        private readonly DBManager _dbManager;

        /// <summary>
        /// Artikuluen zerrenda gordetzen duen behagarri bilduma (ObservableCollection).
        /// </summary>
        private ObservableCollection<Artikuloa> _artikuloak = new ObservableCollection<Artikuloa>();
        public ObservableCollection<Artikuloa> Artikuloak
        {
            get => _artikuloak;
            set
            {
                if (_artikuloak != value)
                {
                    _artikuloak = value;
                    OnPropertyChanged();
                }
            }
        }

        /// <summary>
        /// Kategoriak gordetzen dituen behagarri bilduma (ObservableCollection).
        /// </summary>
        public ObservableCollection<string> Kategoriak { get; } = new ObservableCollection<string>();

        /// <summary>
        /// Hautatutako kategoria gordetzen duen propietatea.
        /// </summary>
        private string _selectedKategoria;
        private string _searchText;
        private ObservableCollection<Artikuloa> _allArtikuloak = new ObservableCollection<Artikuloa>();

        public string SearchText
        {
            get => _searchText;
            set
            {
                if (_searchText != value)
                {
                    _searchText = value;
                    OnPropertyChanged();
                    FilterArtikuloak();
                }
            }
        }
        /// <summary>
        /// Fitxategi guztien bilaketa egiten du testuaren arabera.
        /// </summary>
        private void FilterArtikuloak()
        {
            Artikuloak.Clear();

            var filteredArtikuloak = string.IsNullOrEmpty(SelectedKategoria)
                ? _allArtikuloak.ToList()
                : _allArtikuloak.Where(a => a.Kategoria == SelectedKategoria).ToList();

            if (!string.IsNullOrEmpty(SearchText))
            {
                filteredArtikuloak = filteredArtikuloak
                    .Where(a => a.Izena.Contains(SearchText, StringComparison.OrdinalIgnoreCase))
                    .ToList();
            }

            foreach (var artikulo in filteredArtikuloak)
            {
                Artikuloak.Add(artikulo);
            }
        }

        public string SelectedKategoria
        {
            get => _selectedKategoria;
            set
            {
                if (_selectedKategoria != value)
                {
                    _selectedKategoria = value;
                    OnPropertyChanged();
                    FilterArtikuloak();
                }
            }
        }

        /// <summary>
        /// Kategoriak kargatzeko agindua (Command).
        /// </summary>
        public ICommand LoadKategoriakCommand { get; }

        /// <summary>
        /// Artikulu guztiak kargatzeko agindua (Command).
        /// </summary>
        public ICommand LoadArtikuloakCommand { get; }

        /// <summary>
        /// StockViewModel instantzia sortzen du eta aginduak (Command) inicializatzen ditu.
        /// </summary>
        /// <param name="dbManager">Datu-base kudeatzailea.</param>
        public StockViewModel(DBManager dbManager)
        {
            _dbManager = dbManager;
            LoadKategoriakCommand = new Command(async () => await LoadKategoriak());
            LoadArtikuloakCommand = new Command(async () => await LoadAllArtikuloak());

            Task.Run(async () => await LoadAllArtikuloak());
        }

        /// <summary>
        /// Datu-baseko kategoriak kargatzen ditu eta Kategoriak bilduman gordetzen ditu.
        /// </summary>
        private async Task LoadKategoriak()
        {
            Kategoriak.Clear();
            var kategoriak = await _dbManager.GetKategoriakAsync();
            foreach (var kategoria in kategoriak)
            {
                Kategoriak.Add(kategoria);
            }
        }

        /// <summary>
        /// Datu-baseko artikulu guztiak kargatzen ditu eta Artikuloak bilduman gordetzen ditu.
        /// </summary>
        public async Task LoadAllArtikuloak()
        {
            var artikuloak = await _dbManager.GetArtikuloakAsync();

            _allArtikuloak.Clear();
            foreach (var artikulo in artikuloak)
            {
                _allArtikuloak.Add(artikulo);
            }

            FilterArtikuloak();
        }
    }
}
