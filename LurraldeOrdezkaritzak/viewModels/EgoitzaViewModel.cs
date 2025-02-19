using LurraldeOrdezkaritzak.ViewModels;
using lurraldeOrdezkaritzak;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;

namespace LurraldeOrdezkaritzak.ViewModels
{
    /// <summary>
    /// Egoitzetako eskaerak kudeatzen dituen ViewModel-a.
    /// Eskaerak kargatu eta dataren arabera sailkatzeaz arduratzen da.
    /// </summary>
    public class EgoitzaViewModel : BaseViewModel
    {
        private readonly DBManager _dbManager;

        /// <summary>
        /// Uneko eskaeren zerrenda (gaurko data eta gero).
        /// </summary>
        public ObservableCollection<EskaeraEgoitzaItemViewModel> EskaeraEgoitza { get; set; } = new();

        /// <summary>
        /// Eskaera historikoen zerrenda (gaurko data baino lehen).
        /// </summary>
        public ObservableCollection<EskaeraEgoitzaItemViewModel> EskaeraEgoitzaHistoriala { get; set; } = new();

        /// <summary>
        /// ViewModel-aren eraikitzailea. DBManager-en instantzia lortzen du eta eskaerak asinkronoki kargatzen ditu.
        /// </summary>
        public EgoitzaViewModel()
        {
            _dbManager = DBManager.GetInstance;
            Task.Run(async () => await LoadEskaeraEgoitza());
        }

        /// <summary>
        /// Eskaerak datu-basean kargatzen dituen metodo asinkronoa.
        /// </summary>
        public async Task LoadEskaeraEgoitza()
        {
            var eskaeraList = await _dbManager.GetEskaeraEgoitzaAsync();
            EskaeraEgoitza.Clear();
            EskaeraEgoitzaHistoriala.Clear();

            var gaurkoData = DateTime.Today;

            foreach (var eskaeraEgoitza in eskaeraList)
            {
                string artikuluaIzena = await _dbManager.GetArtikuloaById(eskaeraEgoitza.ArtikuloaId);
                var itemViewModel = new EskaeraEgoitzaItemViewModel(eskaeraEgoitza, artikuluaIzena);

                var eskaeraData = DateTimeOffset.FromUnixTimeSeconds(eskaeraEgoitza.IritsieraDataUnix).DateTime;

                if (eskaeraData >= gaurkoData)
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

    /// <summary>
    /// Eskaera baten datuak gordetzen dituen ViewModel-a.
    /// </summary>
    public class EskaeraEgoitzaItemViewModel
    {
        /// <summary>
        /// Eskaeraren identifikatzailea.
        /// </summary>
        public int Id { get; set; }

        /// <summary>
        /// Eskaeraren kantitatea.
        /// </summary>
        public int Kantitatea { get; set; }

        /// <summary>
        /// Eskaeraren iritsiera-data.
        /// </summary>
        public DateTime IritsieraData { get; set; }

        /// <summary>
        /// Eskaera entregatu den ala ez adierazten du.
        /// </summary>
        public bool Entregatuta { get; set; }

        /// <summary>
        /// Eskaerari dagokion artikuluaren izena.
        /// </summary>
        public string ArtikuluaIzena { get; set; }

        /// <summary>
        /// Eskaera baten datuak gordetzen dituen eraikitzailea.
        /// </summary>
        /// <param name="eskaeraEgoitza">Eskaera objektua.</param>
        /// <param name="artikuluaIzena">Artikuluaren izena.</param>
        public EskaeraEgoitzaItemViewModel(EskaeraEgoitza eskaeraEgoitza, string artikuluaIzena)
        {
            Id = eskaeraEgoitza.Id;
            Kantitatea = eskaeraEgoitza.Kantitatea;
            IritsieraData = DateTimeOffset.FromUnixTimeSeconds(eskaeraEgoitza.IritsieraDataUnix).DateTime;
            Entregatuta = eskaeraEgoitza.Entregatuta;
            ArtikuluaIzena = artikuluaIzena;
        }
    }
}
