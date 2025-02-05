using LurraldeOrdezkaritzak.ViewModels;

namespace LurraldeOrdezkaritzak;

public partial class EskaerakIkusi : ContentPage
{
    public EskaerakIkusi(EskaeraViewModel eskaeraViewModel)
    {
        InitializeComponent();
        BindingContext = eskaeraViewModel;
    }

    private async void Atzera_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}