namespace LurraldeOrdezkaritzak;

public partial class Hasiera : ContentPage
{
	public Hasiera()
	{
		InitializeComponent();
		Guarroman.Source = "https://www.google.com/maps?q=38.184470,-3.687691";

    }
    private async void emailKlikatzerakoan(object sender, EventArgs e)
    {
        var email = "mailto:kevintodotuercas@gmail.com";
        await Launcher.OpenAsync(new Uri(email));
    }

}