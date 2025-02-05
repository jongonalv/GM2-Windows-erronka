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

        // Datu basea kudeatzeko aldagaia
        private readonly DBManager _dbManager;

        // Artikuloak ikusteko zerrenda
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

        public string SelectedArtikuloIzena => SelectedArtikuloa != null
                    ? $"{SelectedArtikuloa.Id}    {SelectedArtikuloa.Izena}"
                    : "Id:     Produktua:";

        // Aukeratutako artikuloa gordetzeko
        private Artikuloa _selectedArtikuloa;
        public Artikuloa SelectedArtikuloa
        {
            get => _selectedArtikuloa;
            set
            {
                if (_selectedArtikuloa != value)
                {
                    _selectedArtikuloa = value;
                    OnPropertyChanged();
                    OnPropertyChanged(nameof(SelectedArtikuloIzena));
                    eskaerakKargatu();
                }
            }
        }

        // Eskaerak ikusteko zerrenda
        private ObservableCollection<Eskaera> _eskaerak = new ObservableCollection<Eskaera>();
        public ObservableCollection<Eskaera> Eskaerak
        {
            get => _eskaerak;
            set
            {
                _eskaerak = value;
                OnPropertyChanged();
            }
        }

        // Kategoriak ikusteko zerrenda
        public ObservableCollection<string> Kategoriak { get; } = new ObservableCollection<string>();

        // Aukeratutako kategoria gordetzeko
        private string _selectedKategoria;

        // Bilaketak egiteko filtroak erabiliz
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
        ///     Kategoriak exekutatzeko komandoa, komando hau exekutatzerakoan,
        ///     pantailan kategoria guztiak kargatzen dira
        /// </summary>
        public ICommand LoadKategoriakCommand { get; }

        /// <summary>
        ///     Artikulo guztiak kargatzeko komandoa, exekutatzerakoan, pantailan 
        ///     artikulo guztiak kargatzen dira
        /// </summary>
        public ICommand LoadArtikuloakCommand { get; }

        /// <summary>
        ///     Aukeratutako artikuloaren gertakariak kontrolatzeko
        ///     erabiliko den komandoa
        /// </summary>
        public ICommand ArtikuloaAukeratuaCommand { get; }


        /// <summary>
        ///     Eraikitzailea, deitzerakoan komandoak exekutatzeko eta datu
        ///     basea kargatzeko.
        /// </summary>
        /// <param name="dbManager"></param>
        public StockViewModel(DBManager dbManager)
        {
            _dbManager = dbManager;
            LoadKategoriakCommand = new Command(async () => await LoadKategoriak());
            LoadArtikuloakCommand = new Command(async () => await LoadAllArtikuloak());
            ArtikuloaAukeratuaCommand = new Command<Artikuloa>(artikulo => SelectedArtikuloa = artikulo);
            Task.Run(async () => await LoadAllArtikuloak());
        }

        /// <summary>
        ///     Metodo bat aukeratutako artikuluaren arabera, eskaerak
        ///     kargatzeko pantailan
        /// </summary>
        private async void eskaerakKargatu()
        {
            if (SelectedArtikuloa != null)
            {
                var eskaerak = await _dbManager.GetEskaerakByArtikuloaAsync(SelectedArtikuloa.Id);
                Eskaerak = new ObservableCollection<Eskaera>(eskaerak);
            }
        }

        /// <summary>
        ///     Kategoriak kargatzeko pantailan
        /// </summary>
        /// <returns></returns>
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
        ///     Artikulu guztiak kargatzeko pantailan ireki eta gero
        /// </summary>
        /// <returns></returns>
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

        /// <summary>
        ///     Artikuluen filtroa egiteko, searchText arabera, zerrendan
        ///     artikulu bat bilatzen du eta beste zerrenda batean gehitzen da
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
    }
}