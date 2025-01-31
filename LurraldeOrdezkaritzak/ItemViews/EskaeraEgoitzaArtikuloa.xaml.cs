namespace LurraldeOrdezkaritzak.ItemViews;

public partial class EskaeraEgoitzaArtikuloa : ContentView
{
    public EskaeraEgoitzaArtikuloa()
    {
        InitializeComponent();
    }

    private void OnEntregatuClicked(object sender, EventArgs e)
    {
        var button = (Button)sender;
        button.Text = "Entregatuta";
        button.IsEnabled = false;
        button.BackgroundColor = Colors.Gray; 
        button.TextColor = Colors.White; 
    }

}