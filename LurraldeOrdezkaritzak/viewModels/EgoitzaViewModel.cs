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

        // Lista para las solicitudes actuales (fecha actual hacia adelante)
        public ObservableCollection<EskaeraEgoitzaItemViewModel> EskaeraEgoitza { get; set; } = new();

        // Lista para el historial de solicitudes (fecha actual hacia atrás)
        public ObservableCollection<EskaeraEgoitzaItemViewModel> EskaeraEgoitzaHistoriala { get; set; } = new();

        public EgoitzaViewModel()
        {
            _dbManager = DBManager.GetInstance;
            Task.Run(async () => await LoadEskaeraEgoitza());
        }

        public async Task LoadEskaeraEgoitza()
        {
            var eskaeraList = await _dbManager.GetEskaeraEgoitzaAsync();
            EskaeraEgoitza.Clear();
            EskaeraEgoitzaHistoriala.Clear();

            var fechaActual = DateTime.Today;

            foreach (var eskaeraEgoitza in eskaeraList)
            {
                string artikuluaIzena = await _dbManager.GetArtikuloaById(eskaeraEgoitza.ArtikuloaId);
                var itemViewModel = new EskaeraEgoitzaItemViewModel(eskaeraEgoitza, artikuluaIzena);

                // Filtrado por fecha
                if (eskaeraEgoitza.Iritsiera_data >= fechaActual)
                {
                    EskaeraEgoitza.Add(itemViewModel);
                }
                else
                {
                    EskaeraEgoitzaHistoriala.Add(itemViewModel);
                }
            }
        }
    }

    public class EskaeraEgoitzaItemViewModel
{
    public int Id { get; set; }
    public int Kantitatea { get; set; }
    public string IritsieraData { get; set; }
    public bool Entregatuta { get; set; }
    public string ArtikuluaIzena { get; set; }

    public EskaeraEgoitzaItemViewModel(EskaeraEgoitza eskaeraEgoitza, string artikuluaIzena)
    {
        Id = eskaeraEgoitza.Id;
        Kantitatea = eskaeraEgoitza.Kantitatea;
        IritsieraData = eskaeraEgoitza.Iritsiera_data.ToString("yyyy-MM-dd");
        Entregatuta = eskaeraEgoitza.Entregatuta;
        ArtikuluaIzena = artikuluaIzena;
    }
}
}