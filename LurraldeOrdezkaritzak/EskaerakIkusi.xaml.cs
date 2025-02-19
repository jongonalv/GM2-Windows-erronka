using Ikaslea.KomertzialakApp.Models.Enums;
using lurraldeOrdezkaritzak;
using LurraldeOrdezkaritzak.ViewModels;
using System.Diagnostics;

namespace LurraldeOrdezkaritzak;

public partial class EskaerakIkusi : ContentPage
{
    /// <summary>
    /// EskaerakIkusi klasearen eraikitzailea.
    /// </summary>
    /// <param name="eskaeraViewModel">Eskaeren bistaratzea kudeatzen duen ViewModel-a.</param>
    public EskaerakIkusi(EskaeraViewModel eskaeraViewModel)
    {
        InitializeComponent();
        BindingContext = eskaeraViewModel;
    }

    /// <summary>
    /// Atzera botoia sakatzean aurreko orrira itzultzen da.
    /// </summary>
    private async void Atzera_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }

    /// <summary>
    /// "Prestatuta" egoerara aldatzeko metodoa.
    /// </summary>
    private async void Prestatuta_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is EskaeraViewModel viewModel)
        {
            Debug.WriteLine(viewModel.Egoera);
            if (viewModel.Egoera.Equals(Egoera.PRESTATZEN.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                /// Datu baseko instantzia eskuratu
                DBManager dbManager = DBManager.GetInstance;
                Eskaera eskaera = await dbManager.GetEskaeraAsync(viewModel.Id);

                /// Eskaera aurkitzen bada, egoera "Prestatuta" bezala eguneratu
                if (eskaera != null)
                {
                    eskaera.Egoera = Egoera.PRESTATUTA;
                    await dbManager.SaveEskaeraAsync(eskaera);

                    viewModel.Egoera = Egoera.PRESTATUTA.ToString();
                    await DisplayAlert("Egoera eguneratua", "Eskaeraren egoera 'Prestatuta'ra aldatu da.", "Ados");
                }
                else
                {
                    await DisplayAlert("Errorea", "Eskaera ez da aurkitu datu basean.", "Ados");
                }
            }
            else
            {
                await DisplayAlert("Ezin da aldatu", "Eskaeraren egoera ez da 'Prestatzen', ezin da 'Prestatuta'ra aldatu.", "Ados");
            }
        }
    }

    /// <summary>
    /// "Bidalita" egoerara aldatzeko metodoa.
    /// </summary>
    private async void Bidalita_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is EskaeraViewModel viewModel)
        {
            Debug.WriteLine(viewModel.Egoera);
            if (viewModel.Egoera.Equals(Egoera.PRESTATUTA.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                /// Datu baseko instantzia eskuratu
                DBManager dbManager = DBManager.GetInstance;
                Eskaera eskaera = await dbManager.GetEskaeraAsync(viewModel.Id);

                /// Eskaera aurkitzen bada, egoera "Bidalita" bezala eguneratu
                if (eskaera != null)
                {
                    eskaera.Egoera = Egoera.BIDALITA;
                    await dbManager.SaveEskaeraAsync(eskaera);

                    viewModel.Egoera = Egoera.BIDALITA.ToString();
                    await DisplayAlert("Egoera eguneratua", "Eskaeraren egoera 'BIDALITA'ra aldatu da.", "Ados");
                }
                else
                {
                    await DisplayAlert("Errorea", "Eskaera ez da aurkitu datu basean.", "Ados");
                }
            }
            else
            {
                await DisplayAlert("Ezin da aldatu", "Eskaeraren egoera ez da 'PRESTATUTA', ezin da 'BIDALITA'ra aldatu.", "Ados");
            }
        }
    }

    /// <summary>
    /// "Bukatuta" egoerara aldatzeko metodoa.
    /// </summary>
    private async void Bukatuta_Clicked(object sender, EventArgs e)
    {
        if (BindingContext is EskaeraViewModel viewModel)
        {
            Debug.WriteLine(viewModel.Egoera);
            if (viewModel.Egoera.Equals(Egoera.BIDALITA.ToString(), StringComparison.OrdinalIgnoreCase))
            {
                /// Datu baseko instantzia eskuratu
                DBManager dbManager = DBManager.GetInstance;
                Eskaera eskaera = await dbManager.GetEskaeraAsync(viewModel.Id);

                /// Eskaera aurkitzen bada, egoera "Bukatuta" bezala eguneratu
                if (eskaera != null)
                {
                    eskaera.Egoera = Egoera.BUKATUTA;
                    await dbManager.SaveEskaeraAsync(eskaera);

                    viewModel.Egoera = Egoera.BUKATUTA.ToString();
                    await DisplayAlert("Egoera eguneratua", "Eskaeraren egoera 'BUKATUTA'ra aldatu da.", "Ados");
                }
                else
                {
                    await DisplayAlert("Errorea", "Eskaera ez da aurkitu datu basean.", "Ados");
                }
            }
            else
            {
                await DisplayAlert("Ezin da aldatu", "Eskaeraren egoera ez da 'BIDALITA', ezin da 'BUKATUTA'ra aldatu.", "Ados");
            }
        }
    }
}
