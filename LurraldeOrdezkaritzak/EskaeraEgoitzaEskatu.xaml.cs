using SQLite;
using System;
using System.IO;
using System.Threading.Tasks;
using lurraldeOrdezkaritzak;
using LurraldeOrdezkaritzak.ItemViews;
using System.Xml.Linq;

namespace LurraldeOrdezkaritzak
{
    public partial class EskaeraEgoitzaEskatu : ContentPage
    {
        private readonly SQLiteAsyncConnection _database;

        public EskaeraEgoitzaEskatu()
        {
            InitializeComponent();
            BindingContext = new EskatuViewModel();

            // Inicializa la base de datos
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "lurraldeOrdezkaritzak.db3");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<EskaeraEgoitza>().Wait(); // Asegúrate de que la tabla exista
        }

        private async void OnEskatuButtonClickedAsync(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Artikuloa artikuloa)
            {
                int number = 0;
                bool isValid = false;

                while (!isValid)
                {
                    string result = await DisplayPromptAsync(
                        "Artikuloa",
                        $"Behar den kantitatea nahikoa izateko behar ditugu: {artikuloa.Kantitatea}",
                        "Jarraitu", "Atzera", "Zenbaki bat sartu...",
                        keyboard: Keyboard.Numeric);

                    if (string.IsNullOrEmpty(result))
                        return; // Si el usuario cancela, salir del bucle

                    if (int.TryParse(result, out number))
                    {
                        if (number >= artikuloa.Kantitatea)
                        {
                            bool answer = await DisplayAlert("Baieztatu", $"Zihur zaude produktuaren kantitatea {number} izatea?", "Bai", "Ez");

                            if (answer)
                            {
                                isValid = true; // Si el usuario confirma, salir del bucle
                                await GuardarEnBaseDeDatos(artikuloa, number); // Guardar los datos en la base de datos
                                await DisplayAlert("Eskaera Gordeta", "Eskaera SQLite-n gorde da.", "Bale");
                            }
                        }
                        else
                        {
                            await DisplayAlert("Errorea", $"Zenbakia {artikuloa.Kantitatea} baino handiagoa edo berdina izan behar da.", "Bale");
                        }
                    }
                    else
                    {
                        await DisplayAlert("Errorea", "Erantzuna ez da egokia. Zenbaki bat sartu.", "Bale");
                    }
                }
            }
        }

        private async Task GuardarEnBaseDeDatos(Artikuloa artikuloa, int kantitatea)
        {
            var nuevaEskaera = new EskaeraEgoitza
            {
                Kantitatea = kantitatea,
                IritsieraDataUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), // Asignamos la fecha y hora actual
                Entregatuta = false, // Suponemos que la entrega aún no ha sido realizada
                ArtikuloaId = artikuloa.Id // Relacionamos con el artículo
            };

            // Insertar el nuevo registro en la base de datos
            await _database.InsertAsync(nuevaEskaera);

            // Obtener el ID recién insertado
            int idEskaera = nuevaEskaera.Id;

            // Crear el XML con el nombre basado en el ID
            CrearXML(artikuloa, kantitatea, idEskaera);
        }

        private void CrearXML(Artikuloa artikuloa, int kantitatea, int idEskaera)
        {
            // Crear el nombre del archivo XML usando el ID de la EskaeraEgoitza
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"EskaeraEgoitza{idEskaera}.xml");

            // Cargar el XML existente, si lo hay
            XDocument xmlDocument;
            if (File.Exists(filePath))
            {
                xmlDocument = XDocument.Load(filePath);
            }
            else
            {
                xmlDocument = new XDocument(new XElement("Eskaera"));
            }

            // Agregar un nuevo elemento de producto a la lista de pedidos
            xmlDocument.Root?.Add(
                new XElement("Produktua",
                    new XElement("Izena", artikuloa.Izena),
                    new XElement("Kodea", artikuloa.Id),
                    new XElement("EskatutakoKantitatea", kantitatea)
                    
                )
            );

            // Guardar el XML actualizado
            xmlDocument.Save(filePath);
        }

    }
}
