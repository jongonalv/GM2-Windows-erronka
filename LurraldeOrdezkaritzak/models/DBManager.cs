using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;

namespace lurraldeOrdezkaritzak
{
    public class DBManager
    {
        private readonly SQLiteAsyncConnection _database;
        private readonly XmlManager _xmlManager;


        public DBManager(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _xmlManager = new XmlManager();
            InitializeDatabase();
        }

        private async void InitializeDatabase()
        {
            try
            {
                await _database.CreateTableAsync<Artikuloa>();
                await _database.CreateTableAsync<Bazkidea>();
                await _database.CreateTableAsync<Eskaera>();
                await _database.CreateTableAsync<EskaeraArtikuloa>();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Errorea datu basea hasieratzerakoan: {ex.Message}");
            }
        }

        public Task<List<Artikuloa>> GetArtikuloakAsync()
        {
            return _database.Table<Artikuloa>().ToListAsync();
        }

        public Task<Artikuloa> GetArtikuloaAsync(int id)
        {
            return _database.Table<Artikuloa>().Where(a => a.Id == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveArtikuloaAsync(Artikuloa artikuloa)
        {
            return artikuloa.Id != 0 ? _database.UpdateAsync(artikuloa) : _database.InsertAsync(artikuloa);
        }

        public Task<int> DeleteArtikuloaAsync(Artikuloa artikuloa)
        {
            return _database.DeleteAsync(artikuloa);
        }

        public Task<List<Bazkidea>> GetBazkideakAsync()
        {
            return _database.Table<Bazkidea>().ToListAsync();
        }

        public Task<Bazkidea> GetBazkideaAsync(int id)
        {
            return _database.Table<Bazkidea>().Where(b => b.Id == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveBazkideaAsync(Bazkidea bazkidea)
        {
            return bazkidea.Id != 0 ? _database.UpdateAsync(bazkidea) : _database.InsertAsync(bazkidea);
        }

        public Task<int> DeleteBazkideaAsync(Bazkidea bazkidea)
        {
            return _database.DeleteAsync(bazkidea);
        }

        public Task<List<Eskaera>> GetEskaerakAsync()
        {
            return _database.Table<Eskaera>().ToListAsync();
        }

        public Task<Eskaera> GetEskaeraAsync(int id)
        {
            return _database.Table<Eskaera>().Where(e => e.Id == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveEskaeraAsync(Eskaera eskaera)
        {
            return eskaera.Id != 0 ? _database.UpdateAsync(eskaera) : _database.InsertAsync(eskaera);
        }

        public Task<int> DeleteEskaeraAsync(Eskaera eskaera)
        {
            return _database.DeleteAsync(eskaera);
        }

        public Task<List<EskaeraArtikuloa>> GetEskaeraArtikuloakAsync()
        {
            return _database.Table<EskaeraArtikuloa>().ToListAsync();
        }

        public Task<EskaeraArtikuloa> GetEskaeraArtikuloaAsync(int id)
        {
            return _database.Table<EskaeraArtikuloa>().Where(ea => ea.Id == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveEskaeraArtikuloaAsync(EskaeraArtikuloa eskaeraArtikuloa)
        {
            return eskaeraArtikuloa.Id != 0 ? _database.UpdateAsync(eskaeraArtikuloa) : _database.InsertAsync(eskaeraArtikuloa);
        }

        public Task<int> DeleteEskaeraArtikuloaAsync(EskaeraArtikuloa eskaeraArtikuloa)
        {
            return _database.DeleteAsync(eskaeraArtikuloa);
        }

        public async Task<List<string>> GetKategoriakAsync()
        {
            return await _database.QueryScalarsAsync<string>("SELECT DISTINCT Kategoria FROM Artikuloa");
        }

        public Task<List<Artikuloa>> GetArtikuloakByKategoriaAsync(string kategoria)
        {
            return _database.Table<Artikuloa>()
                .Where(a => a.Kategoria == kategoria)
                .ToListAsync();
        }

        /// <summary>
        ///    Egoitza nagusitik artikulo berriak badatozte eguneratzeko metodoa
        /// </summary>
        /// <returns></returns>
        public async Task XMLArtikuloakKargatu()
        {
            var customFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
    {
        { DevicePlatform.Android, new[] { "application/xml", "text/xml" } },
        { DevicePlatform.iOS, new[] { "public.xml" } },
        { DevicePlatform.WinUI, new[] { ".xml" } }
    });

            var pickResult = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = customFileType,
                PickerTitle = "Selecciona un archivo XML"
            });

            if (pickResult == null)
            {
                Debug.WriteLine("No se seleccionó ningún archivo.");
                return;
            }

            string filePath = pickResult.FullPath;

            // Cargar los artículos desde el archivo XML seleccionado
            var xmlArtikuloak = _xmlManager.LoadArtikuloakFromXml(filePath);

            if (xmlArtikuloak.Count == 0)
            {
                Debug.WriteLine("No se encontraron artículos en el archivo XML.");
                return;
            }

            // Obtener los artículos existentes en la base de datos
            var existingArtikuloak = await _database.Table<Artikuloa>().ToListAsync();
            var existingArtikuloakDict = existingArtikuloak.ToDictionary(a => (a.Izena, a.Kategoria));

            foreach (var xmlArtikuloa in xmlArtikuloak)
            {
                if (existingArtikuloakDict.TryGetValue((xmlArtikuloa.Izena, xmlArtikuloa.Kategoria), out var existingArtikuloa))
                {
                    // Actualizar solo si hay cambios
                    if (existingArtikuloa.Prezioa != xmlArtikuloa.Prezioa || existingArtikuloa.Stock != xmlArtikuloa.Stock)
                    {
                        existingArtikuloa.Prezioa = xmlArtikuloa.Prezioa;
                        existingArtikuloa.Stock = xmlArtikuloa.Stock;
                        await _database.UpdateAsync(existingArtikuloa);
                    }
                }
                else
                {
                    // Insertar si no existe
                    await _database.InsertAsync(xmlArtikuloa);
                }
            }
        }
    }
}
