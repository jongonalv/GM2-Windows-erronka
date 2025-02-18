using lurraldeOrdezkaritzak;
using LurraldeOrdezkaritzak.ViewModels;
using SQLite;

namespace LurraldeOrdezkaritzak.ItemViews
{
    public partial class EskaeraEgoitzaArtikuloa : ContentView
    {
        public EskaeraEgoitzaArtikuloa()
        {
            InitializeComponent();
        }

        private async void OnEntregatuClicked(object sender, EventArgs e)
        {
            var button = (Button)sender;
            var viewModel = (EskaeraEgoitzaItemViewModel)BindingContext;

            var dbManager = DBManager.GetInstance;

            try
            {
                // Obtener la solicitud completa
                var eskaeraEgoitza = await dbManager.GetEskaeraEgoitzaByIdAsync(viewModel.Id);
                if (eskaeraEgoitza == null)
                {
                    await Application.Current.MainPage.DisplayAlert("Error", "Eskaera ez da aurkitu.", "OK");
                    return;
                }

                // Verificar si ya está entregada
                if (eskaeraEgoitza.Entregatuta)
                {
                    await Application.Current.MainPage.DisplayAlert("Informazioa", "Eskaera hau entregatuta dago jadanik.", "OK");
                    return;
                }

                // Actualizar el stock del artículo
                await dbManager.UpdateArtikuloaStockAsync(eskaeraEgoitza.ArtikuloaId, viewModel.Kantitatea);

                // Marcar la solicitud como entregada
                await dbManager.MarkEskaeraEgoitzaAsEntregatutaAsync(viewModel.Id);

                // Actualizar la interfaz de usuario
                button.Text = "Entregatuta";
                button.IsEnabled = false;
                button.BackgroundColor = Colors.Gray;
                button.TextColor = Colors.White;

                // Actualizar el estado en el ViewModel
                viewModel.Entregatuta = true;
            }
            catch (Exception ex)
            {
                await Application.Current.MainPage.DisplayAlert("Error", ex.Message, "OK");
            }
        }
    }
}