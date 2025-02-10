using LurraldeOrdezkaritzak.ViewModels;
using lurraldeOrdezkaritzak;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LurraldeOrdezkaritzak.ViewModels
{
    public class EgoitzaViewModel : BaseViewModel
    {
        private readonly DBManager _dbManager;

       public ObservableCollection<EskaeraEgoitzaItemViewModel> EskaeraEgoitza { get; set; } = new();


        public EgoitzaViewModel()
        {
            _dbManager = DBManager.GetInstance;
            Task.Run(async () => await LoadEskaeraEgoitza());
        }

        public async Task LoadEskaeraEgoitza()
        {
            var eskaeraList = await _dbManager.GetEskaeraEgoitzaAsync();
            EskaeraEgoitza.Clear();

            foreach (var eskaeraEgoitza in eskaeraList)
            {
               
                var artikuloakList = await _dbManager.GetArtikuloakByEskaeraEgoitzaIdAsync(eskaeraEgoitza.Id);

                var artikuloakObservable = new ObservableCollection<Artikuloa>(artikuloakList);

                EskaeraEgoitza.Add(new EskaeraEgoitzaItemViewModel(eskaeraEgoitza, artikuloakObservable.FirstOrDefault()));

            }
        }
    }

    public class EskaeraEgoitzaItemViewModel
    {
        public int Id { get; set; }
        public int Kantitatea { get; set; }
        public string IritsieraData { get; set; }
        public bool Entregatuta { get; set; }
        public int ArtikuloaId { get; set; }

        public EskaeraEgoitzaItemViewModel(EskaeraEgoitza eskaeraEgoitza, Artikuloa artikuloa)
        {
            Id = eskaeraEgoitza.Id;
            Kantitatea = eskaeraEgoitza.Kantitatea;
            IritsieraData = eskaeraEgoitza.Iritsiera_data.ToString("yyyy-MM-dd");
            Entregatuta = eskaeraEgoitza.Entregatuta;
            ArtikuloaId = eskaeraEgoitza.ArtikuloaId;
        }
    }
}
