using LurraldeOrdezkaritzak.ItemViews;

namespace LurraldeOrdezkaritzak
{
    public partial class EskaeraEgoitzaEskatu : ContentPage
    {
        public EskaeraEgoitzaEskatu()
        {
            InitializeComponent();

            // Encuentra el ContentView en el XAML
            var contentView = this.FindByName<LurraldeOrdezkaritzak.ItemViews.EskaeraEgoitzaEskatu>("EskaeraView");

            // Suscríbete al evento del ContentView
            if (contentView != null)
            {
                contentView.EskatuButtonClicked += OnEskatuButtonClickedAsync;
            }
        }

        private async void OnEskatuButtonClickedAsync(object sender, EventArgs e)
        {
            string result = await DisplayPromptAsync("Artikuloa", "Behar den kantitatea nahikoa izateko behar ditugu: 69", "Jarraitu", "Atzera", "Zenbaki bat sartu...", keyboard: Keyboard.Numeric);

            if (!string.IsNullOrEmpty(result))
            {
                if (int.TryParse(result, out int number))
                {
                    bool answer = await DisplayAlert("Baieztatu", $"Zihur zaude produktuaren kantitatea {number} izatea?", "Bai", "Ez");

                    if (answer)
                    {
                        // Acción a realizar si el usuario confirma
                    }
                    else
                    {
                        // Acción a realizar si el usuario cancela
                    }
                }
                else
                {
                    await DisplayAlert("Errorea", "Erantzuna ez da egokia", "Bale");
                }
            }
        }
    }
}