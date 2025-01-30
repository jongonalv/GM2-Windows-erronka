
using System;

namespace LurraldeOrdezkaritzak;

public partial class EskaeraEgoitzaEskatu : ContentPage
{
	public EskaeraEgoitzaEskatu()
	{
		InitializeComponent();
	}

    private async void OnEskatuButtonClickedAsync(object sender, EventArgs e)
    {
        string result = await DisplayPromptAsync("Artikuloa", "Behar den kantitatea nahikoa izateko behar ditugu: 69", "Jarraitu", "Atzera", "Zenbaki bat sartu...", keyboard: Keyboard.Numeric);

        // Verificar si el usuario ingresó un valor y no canceló el cuadro de diálogo
        if (!string.IsNullOrEmpty(result))
        {
            // Convertir el resultado a un número (por ejemplo, int)
            if (int.TryParse(result, out int number))
            {
                // Mostrar cuadro de diálogo de confirmación
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
                // Manejar error de conversión
                await DisplayAlert("Errorea", "Erantzuna ez da egokia", "Bale");
            }
        }
        else
        {
            // Manejar cancelación por parte del usuario
        }
    }
}