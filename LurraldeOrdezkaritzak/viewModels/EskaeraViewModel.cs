using lurraldeOrdezkaritzak;
using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;

namespace LurraldeOrdezkaritzak.ViewModels
{
    public class EskaerakViewModel : BaseViewModel
    {
        private readonly DBManager _dbManager;

        public ObservableCollection<EskaeraViewModel> Eskaerak { get; set; } = new();
        public ObservableCollection<EskaeraViewModel> EskaerakFiltroak { get; set; } = new();

        private ObservableCollection<string> _filtroak;
        public ObservableCollection<string> Filtroak
        {
            get => _filtroak;
            set
            {
                _filtroak = value;
                OnPropertyChanged();
            }
        }

        private string _filtroAukeratua;
        public string FiltroAukeratua
        {
            get => _filtroAukeratua;
            set
            {
                if (_filtroAukeratua != value)
                {
                    _filtroAukeratua = value;
                    OnPropertyChanged();
                    FiltroaAplikatu();
                }
            }
        }

        private ObservableCollection<Bazkidea> _bazkideak;
        public ObservableCollection<Bazkidea> Bazkideak
        {
            get => _bazkideak;
            set
            {
                _bazkideak = value;
                OnPropertyChanged();
            }
        }

        private Bazkidea _bazkideaAukeratua;
        public Bazkidea BazkideaAukeratua
        {
            get => _bazkideaAukeratua;
            set
            {
                if (_bazkideaAukeratua != value)
                {
                    _bazkideaAukeratua = value;
                    OnPropertyChanged();
                    FiltroaAplikatu();
                }
            }
        }

        public EskaerakViewModel()
        {
            _dbManager = DBManager.GetInstance;
            Filtroak = new ObservableCollection<string>
            {
                "Eskaerak guztiak",
                "Hilabeteko eskarak"
            };

            Task.Run(async () => await LoadEskaerak());
            Task.Run(async () => await LoadBazkideak());
        }

        /// <summary>
        ///     Eskaerak kargatzeko metodoa
        /// </summary>
        /// <returns></returns>
        public async Task LoadEskaerak()
        {
            var eskaeraList = await _dbManager.GetEskaerakAsync();
            Eskaerak.Clear();

            foreach (var eskaera in eskaeraList)
            {
                var bazkidea = await _dbManager.GetBazkideaByIdAsync(eskaera.BazkideaId);
                var artikuloakList = await _dbManager.GetArtikuloakByEskaeraIdAsync(eskaera.Id);

                var artikuloakObservable = new ObservableCollection<Artikuloa>(artikuloakList);

                Eskaerak.Add(new EskaeraViewModel(eskaera, bazkidea, artikuloakObservable));
            }

            // Inicialmente, mostrar todas las Eskaerak
            FiltroaAplikatu();
        }

        /// <summary>
        ///     Bazkideak kargatzeko metodoa
        /// </summary>
        /// <returns></returns>
        public async Task LoadBazkideak()
        {
            var bazkideakList = await _dbManager.GetBazkideakAsync();
            Bazkideak = new ObservableCollection<Bazkidea>(bazkideakList);

            // Agregar la opción "Guztiak" al principio de la lista
            Bazkideak.Insert(0, new Bazkidea { Id = -1, Izena = "Guztiak" });
        }

        private void FiltroaAplikatu()
        {
            var eskaerakFiltroak = Eskaerak.AsEnumerable();

            // Aplicar filtro por fecha
            if (FiltroAukeratua == "Hilabeteko eskarak")
            {
                var gaurkoData = DateTime.Now;
                eskaerakFiltroak = eskaerakFiltroak.Where(e => DateTime.Parse(e.EskaeraData).Month == gaurkoData.Month && DateTime.Parse(e.EskaeraData).Year == gaurkoData.Year);
            }

            // Aplicar filtro por Bazkidea
            if (BazkideaAukeratua != null && BazkideaAukeratua.Id != -1) // Ignorar si se selecciona "Guztiak"
            {
                eskaerakFiltroak = eskaerakFiltroak.Where(e => e.Izena == BazkideaAukeratua.Izena);
            }

            EskaerakFiltroak.Clear();
            foreach (var eskaera in eskaerakFiltroak)
            {
                EskaerakFiltroak.Add(eskaera);
            }
        }
    }

    public class EskaeraViewModel : INotifyPropertyChanged
    {
        public int Id { get; set; }
        public string Izena { get; set; }
        public string Kontzeptua { get; set; }
        public string EskaeraData { get; set; }
        public string Egoera { get; set; }
        public double Guztira { get; set; }

        // Eskaeraren artikuloak azaltzeko zerrenda
        public ObservableCollection<Artikuloa> Artikuloak { get; set; }

        // Stock falta duten artikuloak azaltzeko zerrenda
        public ObservableCollection<Artikuloa> stockFaltaArtikuloak
        => new ObservableCollection<Artikuloa>(Artikuloak.Where(a => a.Kantitatea > a.Stock));


        // Propietate berri bat stock-a falta bada
        public bool StockFalta => Artikuloak.Any(a => a.Kantitatea > a.Stock);

        // Nueva propiedad para habilitar/deshabilitar botones
        public bool EstanBotonesHabilitados
        {
            get { return stockFaltaArtikuloak == null || stockFaltaArtikuloak.Count == 0; }
        }

        public EskaeraViewModel(Eskaera eskaera, Bazkidea bazkidea, ObservableCollection<Artikuloa> artikuloak)
        {
            Id = eskaera.Id;
            Izena = bazkidea?.Izena ?? "Ezezaguna";
            Kontzeptua = eskaera.Kontzeptua;
            EskaeraData = DateTimeOffset.FromUnixTimeSeconds(eskaera.EskaeraDataTimestamp).DateTime.ToString("yyyy-MM-dd");
            Egoera = eskaera.Egoera.ToString();
            Guztira = eskaera.Guztira;
            Artikuloak = artikuloak ?? new ObservableCollection<Artikuloa>();
        }
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}