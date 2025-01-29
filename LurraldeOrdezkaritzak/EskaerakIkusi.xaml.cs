namespace LurraldeOrdezkaritzak;

public partial class EskaerakIkusi : ContentPage
{
	public EskaerakIkusi()
	{
		InitializeComponent();
	}

    private async void Atzera_Clicked(object sender, EventArgs e)
    {
        await Navigation.PopAsync();
    }
}