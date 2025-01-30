
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

        // Verificar si el usuario ingres� un valor y no cancel� el cuadro de di�logo
        if (!string.IsNullOrEmpty(result))
        {
            // Convertir el resultado a un n�mero (por ejemplo, int)
            if (int.TryParse(result, out int number))
            {
                // Mostrar cuadro de di�logo de confirmaci�n
                bool answer = await DisplayAlert("Baieztatu", $"Zihur zaude produktuaren kantitatea {number} izatea?", "Bai", "Ez");

                if (answer)
                {
                    // Acci�n a realizar si el usuario confirma
                }
                else
                {
                    // Acci�n a realizar si el usuario cancela
                }
            }
            else
            {
                // Manejar error de conversi�n
                await DisplayAlert("Errorea", "Erantzuna ez da egokia", "Bale");
            }
        }
        else
        {
            // Manejar cancelaci�n por parte del usuario
        }
    }
}