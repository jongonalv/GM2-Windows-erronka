namespace LurraldeOrdezkaritzak;

public partial class Eskaerak : ContentPage
{
	public Eskaerak()
	{
		InitializeComponent();
	}
    private async void Ikusi_clicked(object sender, EventArgs e)
    {
        await Navigation.PushAsync(new EskaerakIkusi());
    }
}