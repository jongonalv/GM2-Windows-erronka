using LurraldeOrdezkaritzak.ViewModels;
using lurraldeOrdezkaritzak;
using System.Collections.ObjectModel;
using System.Windows.Input;

public class EskatuViewModel : BaseViewModel
{
    private readonly DBManager _dbManager;

    /// <summary>
    /// Stock gabe dauden artikuluen zerrenda gordetzen du.
    /// </summary>
    public ObservableCollection<Artikuloa> ArtikuloakOutOfStock { get; set; } = new();

    /// <summary>
    /// Hautatutako artikuluarekin erlazionatutako eskaeren zerrenda gordetzen du.
    /// </summary>
    public ObservableCollection<Eskaera> EskaerakRelacionados { get; set; } = new();

    /// <summary>
    /// Artikulo bat klik egitean ekintza bat burutzeko komandoa.
    /// </summary>
    public ICommand ArtikuloaClickedCommand { get; }

    /// <summary>
    /// ViewModel-aren eraikitzailea. DBManager-en instantzia lortzen du eta datuak asinkronoki kargatzen ditu.
    /// </summary>
    public EskatuViewModel()
    {
        _dbManager = DBManager.GetInstance;
        ArtikuloaClickedCommand = new Command<Artikuloa>(OnArtikuloaClicked);
        Task.Run(async () => await LoadArtikuloakOutOfStock());
    }

    /// <summary>
    /// Hautatutako artikuluaren arabera eskaera erlazionatuak kargatzen ditu.
    /// </summary>
    /// <param name="artikuloa">Hautatutako artikuloa.</param>
    private async void OnArtikuloaClicked(Artikuloa artikuloa)
    {
        if (artikuloa == null) return;

        var eskaerak = await _dbManager.GetEskaerakByArtikuloaIdAsync(artikuloa.Id);

        EskaerakRelacionados.Clear();

        foreach (var eskaera in eskaerak)
        {
            EskaerakRelacionados.Add(eskaera);
        }
    }

    /// <summary>
    /// Stock gabe dauden artikuluak kargatzen dituen metodo asinkronoa.
    /// </summary>
    public async Task LoadArtikuloakOutOfStock()
    {
        var artikuloak = await _dbManager.GetArtikuloakAsync();
        var eskaeraArtikuloak = await _dbManager.GetEskaeraArtikuloakAsync();

        var artikuloaKantitateak = new Dictionary<int, int>();

        foreach (var eskaeraArtikuloa in eskaeraArtikuloak)
        {
            if (artikuloaKantitateak.ContainsKey(eskaeraArtikuloa.ArtikuloaId))
            {
                artikuloaKantitateak[eskaeraArtikuloa.ArtikuloaId] += eskaeraArtikuloa.Kantitatea;
            }
            else
            {
                artikuloaKantitateak[eskaeraArtikuloa.ArtikuloaId] = eskaeraArtikuloa.Kantitatea;
            }
        }

        foreach (var artikuloa in artikuloak)
        {
            if (artikuloaKantitateak.TryGetValue(artikuloa.Id, out var kantitatea))
            {
                artikuloa.Kantitatea = kantitatea;
                if (artikuloa.StockFalta > 0)
                {
                    ArtikuloakOutOfStock.Add(artikuloa);
                }
            }
        }
    }
}
