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
        public ObservableCollection<Artikuloa> Artikuloak { get; } = new ObservableCollection<Artikuloa>();

        /// <summary>
        /// Kategoriak gordetzen dituen behagarri bilduma (ObservableCollection).
        /// </summary>
        public ObservableCollection<string> Kategoriak { get; } = new ObservableCollection<string>();

        /// <summary>
        /// Hautatutako kategoria gordetzen duen propietatea.
        /// </summary>
        private string _selectedKategoria;
        private string _searchText;
        private List<Artikuloa> _allArtikuloak = new List<Artikuloa>();
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

            var artikuloak = string.IsNullOrEmpty(SearchText)
                ? _allArtikuloak  // Artikulu guztiak kargatu
                : _allArtikuloak.Where(a => a.Izena.Contains(SearchText, StringComparison.OrdinalIgnoreCase)).ToList();

            foreach (var artikulo in artikuloak)
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
                    LoadArtikuloakByKategoria(_selectedKategoria);
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

            // Artikulu guztiak lehenetsita kargatzen ditu
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
        private async Task LoadAllArtikuloak()
        {
            Artikuloak.Clear();
            _allArtikuloak.Clear();

            var artikuloak = await _dbManager.GetArtikuloakAsync();
            _allArtikuloak.AddRange(artikuloak);

            foreach (var artikulo in artikuloak)
            {
                Artikuloak.Add(artikulo);
            }
        }


        /// <summary>
        /// Hautatutako kategoriaren arabera artikuluak kargatzen ditu. 
        /// Kategoria hutsik badago, artikulu guztiak kargatzen dira.
        /// </summary>
        /// <param name="kategoria">Kategoria hautatua.</param>
        private async Task LoadArtikuloakByKategoria(string kategoria)
        {
            if (string.IsNullOrEmpty(kategoria))
            {
                await LoadAllArtikuloak();
                return;
            }

            Artikuloak.Clear();
            var filteredArtikuloak = await _dbManager.GetArtikuloakByKategoriaAsync(kategoria);
            foreach (var artikulo in filteredArtikuloak)
            {
                Artikuloak.Add(artikulo);
            }
        }
    }
}
