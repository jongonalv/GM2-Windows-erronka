using LurraldeOrdezkaritzak.ViewModels;
using lurraldeOrdezkaritzak;
using System.Collections.ObjectModel;
using System.Windows.Input;

public class EskatuViewModel : BaseViewModel
{
    private readonly DBManager _dbManager;

    public ObservableCollection<Artikuloa> ArtikuloakOutOfStock { get; set; } = new();
    public ObservableCollection<Eskaera> EskaerakRelacionados { get; set; } = new();

    public ICommand ArtikuloaClickedCommand { get; }

    public EskatuViewModel()
    {
        _dbManager = DBManager.GetInstance;
        ArtikuloaClickedCommand = new Command<Artikuloa>(OnArtikuloaClicked);
        Task.Run(async () => await LoadArtikuloakOutOfStock());
    }

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