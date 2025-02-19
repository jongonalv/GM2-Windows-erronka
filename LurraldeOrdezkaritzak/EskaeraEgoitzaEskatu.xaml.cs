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

            // /// Datu-basea hasieratu ///
            string dbPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "lurraldeOrdezkaritzak.db3");
            _database = new SQLiteAsyncConnection(dbPath);
            _database.CreateTableAsync<EskaeraEgoitza>().Wait(); // /// Ziurtatu taula existitzen dela ///
        }

        private async void OnEskatuButtonClickedAsync(object sender, EventArgs e)
        {
            if (sender is Button button && button.BindingContext is Artikuloa artikuloa)
            {
                int number = 0;
                bool isValid = false;

                while (!isValid)
                {
                    // /// Erabiltzaileari kantitatea sartzeko eskaera egin ///
                    string result = await DisplayPromptAsync(
                        "Artikuloa",
                        $"Behar den kantitatea nahikoa izateko behar ditugu: {artikuloa.Kantitatea}",
                        "Jarraitu", "Atzera", "Zenbaki bat sartu...",
                        keyboard: Keyboard.Numeric);

                    if (string.IsNullOrEmpty(result))
                        return;

                    if (int.TryParse(result, out number))
                    {
                        if (number >= artikuloa.Kantitatea)
                        {
                            // /// Erabiltzaileari baieztapena eskatu ///
                            bool answer = await DisplayAlert("Baieztatu", $"Zihur zaude produktuaren kantitatea {number} izatea?", "Bai", "Ez");

                            if (answer)
                            {
                                isValid = true;
                                await GuardarEnBaseDeDatos(artikuloa, number);
                                await DisplayAlert("Eskaera Gordeta", "Eskaera SQLite-n gorde da.", "Bale");
                            }
                        }
                        else
                        {
                            // /// Errorea: zenbakia txikiegia da ///
                            await DisplayAlert("Errorea", $"Zenbakia {artikuloa.Kantitatea} baino handiagoa edo berdina izan behar da.", "Bale");
                        }
                    }
                    else
                    {
                        // /// Errorea: balioa ez da zenbaki bat ///
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
                IritsieraDataUnix = DateTimeOffset.UtcNow.ToUnixTimeSeconds(), // /// Uneko data eta ordua gorde ///
                Entregatuta = false, // /// Hasieran entrega ez da burutu ///
                ArtikuloaId = artikuloa.Id // /// Artikuloarekin erlazionatu ///
            };

            // /// Datu-basean erregistro berria sartu ///
            await _database.InsertAsync(nuevaEskaera);

            // /// Sartutako eskaeraren ID-a lortu ///
            int idEskaera = nuevaEskaera.Id;

            // /// Eskaeraren XML fitxategia sortu ///
            CrearXML(artikuloa, kantitatea, idEskaera);
        }

        private void CrearXML(Artikuloa artikuloa, int kantitatea, int idEskaera)
        {
            // /// XML fitxategiaren izena eskaeraren ID-a erabiliz sortu ///
            string filePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), $"EskaeraEgoitza{idEskaera}.xml");

            // /// XML-a kargatu, baldin badago ///
            XDocument xmlDocument;
            if (File.Exists(filePath))
            {
                xmlDocument = XDocument.Load(filePath);
            }
            else
            {
                xmlDocument = new XDocument(new XElement("Eskaera"));
            }

            // /// Produktu berria gehitu eskaeren zerrendara ///
            xmlDocument.Root?.Add(
                new XElement("Produktua",
                    new XElement("Izena", artikuloa.Izena),
                    new XElement("Kodea", artikuloa.Id),
                    new XElement("EskatutakoKantitatea", kantitatea)
                )
            );

            // /// XML eguneratua gorde ///
            xmlDocument.Save(filePath);
        }
    }
}
