using lurraldeOrdezkaritzak;
using System;
using System.Collections.ObjectModel;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace LurraldeOrdezkaritzak.ViewModels
{
    public class EskaerakViewModel : BaseViewModel
    {
        private readonly DBManager _dbManager;

        public ObservableCollection<EskaeraViewModel> Eskaerak { get; set; } = new();

        public EskaerakViewModel()
        {
            _dbManager = DBManager.GetInstance;
            Task.Run(async () => await LoadEskaerak());
        }

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
        }
    }

    public class EskaeraViewModel
    {
        public int Id { get; set; }
        public string Izena { get; set; }
        public string Kontzeptua { get; set; }
        public string EskaeraData { get; set; }
        public string Egoera { get; set; }
        public double Guztira { get; set; }

        public ObservableCollection<Artikuloa> Artikuloak { get; set; }

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
    }

}
