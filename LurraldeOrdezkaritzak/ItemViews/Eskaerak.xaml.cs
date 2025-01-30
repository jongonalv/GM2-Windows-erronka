namespace LurraldeOrdezkaritzak.ItemViews;

public partial class Eskaerak : ContentView
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