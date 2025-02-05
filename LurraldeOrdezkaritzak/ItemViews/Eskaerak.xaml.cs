using LurraldeOrdezkaritzak.ViewModels;

namespace LurraldeOrdezkaritzak.ItemViews;

public partial class Eskaerak : ContentView
{
	public Eskaerak()
	{
		InitializeComponent();
	}
    private async void Ikusi_clicked(object sender, EventArgs e)
    {
        if (sender is Button button && button.BindingContext is EskaeraViewModel eskaera)
        {
            await Navigation.PushAsync(new EskaerakIkusi(eskaera));
        }
    }
}