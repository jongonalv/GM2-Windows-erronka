using Ikaslea.KomertzialakApp.Models.Enums;
using LurraldeOrdezkaritzak.ItemViews;
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
        private static DBManager _instance;

        public static DBManager GetInstance
        {
            get
            {             
                if (_instance == null)
                    {
                    _instance = new DBManager(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "lurraldeOrdezkaritzak.db3"));
                    }
                return _instance;
            }
        }

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
                await _database.CreateTableAsync<EskaeraEgoitza>();
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
        ///     Bazkide bat lortzeko bere ID-a pasata
        /// </summary>
        /// <param name="bazkideaId"></param>
        /// <returns></returns>
        public Task<Bazkidea> GetBazkideaByIdAsync(int bazkideaId)
        {
            return _database.FindAsync<Bazkidea>(bazkideaId);
        }

        public async Task<List<Artikuloa>> GetArtikuloakByEskaeraIdAsync(int eskaeraId)
        {
            var artikuloak = await _database.QueryAsync<Artikuloa>(
                @"SELECT a.Id, a.Izena, a.Stock, a.Prezioa, ea.kantitatea 
          FROM Artikuloa a
          INNER JOIN EskaeraArtikuloa ea ON a.Id = ea.artikuloa_id
          WHERE ea.eskaera_id = ?", eskaeraId);

            foreach (var artikulo in artikuloak)
            {
                artikulo.Kantitatea = await _database.ExecuteScalarAsync<int>(
                    "SELECT kantitatea FROM EskaeraArtikuloa WHERE artikuloa_id = ? AND eskaera_id = ?",
                    artikulo.Id, eskaeraId);
            }

            return artikuloak;
        }




        /// <summary>
        /// Eskaerak lortzeko metodoa artikuloaren arabera
        /// </summary>
        /// <param name="artikuloaId"></param>
        /// <returns></returns>
        public async Task<List<Eskaera>> GetEskaerakByArtikuloaAsync(int artikuloaId)
        {
            string query = @"
                            SELECT E.* 
                            FROM Eskaera E
                            INNER JOIN EskaeraArtikuloa EA ON E.Id = EA.eskaera_id
                            WHERE EA.artikuloa_id = ?";

            var eskaerak = await _database.QueryAsync<Eskaera>(query, artikuloaId);

            foreach (var eskaera in eskaerak)
            {
                eskaera.EskaeraArtikuloak = await _database.Table<EskaeraArtikuloa>()
                    .Where(ea => ea.EskaeraId == eskaera.Id)
                    .ToListAsync();
            }

            return eskaerak;
        }



        public Task<List<EskaeraEgoitza>> GetEskaeraEgoitzaAsync()
        {
            return _database.Table<EskaeraEgoitza>().ToListAsync();
        }

        public Task<EskaeraEgoitza> GetEskaeraEgoitzaAsync(int id)
        {
            return _database.Table<EskaeraEgoitza>().Where(e => e.Id == id).FirstOrDefaultAsync();
        }

        public Task<int> SaveEskaeraEgoitzaAsync(EskaeraEgoitza eskaeraEgoitza)
        {
            return eskaeraEgoitza.Id != 0 ? _database.UpdateAsync(eskaeraEgoitza) : _database.InsertAsync(eskaeraEgoitza);
        }

        public Task<int> DeleteEskaeraEgoitzaAsync(EskaeraEgoitza eskaeraEgoitza)
        {
            return _database.DeleteAsync(eskaeraEgoitza);
        }
        public async Task<List<Artikuloa>> GetArtikuloakByEskaeraEgoitzaIdAsync(int eskaeraEgoitzaId)
        {
            var artikuloak = await _database.QueryAsync<Artikuloa>(
                @"SELECT a.Id, a.Izena, a.Stock, a.Prezioa, ea.kantitatea 
          FROM Artikuloa a
          INNER JOIN EskaeraEgoitza ee ON a.id = ee.ArtikuloaId
          WHERE ee.eskaera_id = ?", eskaeraEgoitzaId);

            foreach (var artikulo in artikuloak)
            {
                artikulo.Kantitatea = await _database.ExecuteScalarAsync<int>(
                    "SELECT kantitatea FROM EskaeraArtikuloa WHERE artikuloa_id = ? AND eskaera_id = ?",
                    artikulo.Id, eskaeraEgoitzaId);
            }

            return artikuloak;
        }


        /// <summary>
        ///     metodo tenporala bazkideak sartzeko
        /// </summary>
        public void InsertBazkideak()
        {
            using (var db = new SQLiteConnection(Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "lurraldeOrdezkaritzak.db3")))
            {
                db.CreateTable<Bazkidea>(); // Asegurar que la tabla existe

                List<Bazkidea> bazkideak = new List<Bazkidea>
                {
                    new Bazkidea { Id = 8, Izena = "Francisco Franco Bahamonde", Email = "jon.goni@example.com", Telefonoa = "123456789", Helbidea = "Kalea 1, Donostia", BazkideMota = "Premium", KomerzialaId = 1 },
                    new Bazkidea { Id = 6, Izena = "Ane Lasa", Email = "ane.lasa@example.com", Telefonoa = "987654321", Helbidea = "Kalea 2, Bilbo", BazkideMota = "Estandar", KomerzialaId = 2 },
                    new Bazkidea { Id = 4, Izena = "Mikel Etxeberria", Email = "mikel.etxeberria@example.com", Telefonoa = "654321987", Helbidea = "Kalea 3, Gasteiz", BazkideMota = "Estandar", KomerzialaId = 3 },
                    new Bazkidea { Id = 7, Izena = "Maite Arrieta", Email = "maite.arrieta@example.com", Telefonoa = "321987654", Helbidea = "Kalea 4, Iruñea", BazkideMota = "Premium", KomerzialaId = 4 },
                    new Bazkidea { Id = 5, Izena = "Ander Olaizola", Email = "ander.olaizola@example.com", Telefonoa = "159753468", Helbidea = "Kalea 5, Baiona", BazkideMota = "Estandar", KomerzialaId = 5 }
                };

                foreach (var bazkidea in bazkideak)
                {
                    db.InsertOrReplace(bazkidea);
                }
            }
        }

        /// <summary>
        ///    Egoitza nagusitik artikulo berriak badatozte eguneratzeko metodoa, baita ere
        ///    jadanik datu basean dauden artikuluoak eguneratzeko bere Stock-a edo Prezioa
        /// </summary>
        /// <returns></returns>
        public async Task XMLArtikuloakKargatu()
        {
            var xmlFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".xml" } }
            });

            var pickResult = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = xmlFileType,
                PickerTitle = "XML Fitxategi bat aukeratu mesedez."
            });

            if (pickResult == null)
            {
                Debug.WriteLine("Ez da aukeratu fitxategirik");
                return;
            }

            string fitxHelbidea = pickResult.FullPath;

            try
            {
                // XML Artikuloak kargatu aukeratutako fitxategitik
                var xmlArtikuloak = _xmlManager.LoadArtikuloakFromXml(fitxHelbidea);

                if (xmlArtikuloak.Count == 0)
                {
                    Debug.WriteLine("Ez dira aurkitu daturik XML fitxategian");
                    return;
                }

                var existingArtikuloak = await _database.Table<Artikuloa>().ToListAsync();
                var existingArtikuloakDict = existingArtikuloak.ToDictionary(a => (a.Izena, a.Kategoria));

                foreach (var xmlArtikuloa in xmlArtikuloak)
                {
                    if (existingArtikuloakDict.TryGetValue((xmlArtikuloa.Izena, xmlArtikuloa.Kategoria), out var existingArtikuloa))
                    {
                        if (existingArtikuloa.Prezioa != xmlArtikuloa.Prezioa || existingArtikuloa.Stock != xmlArtikuloa.Stock)
                        {
                            existingArtikuloa.Prezioa = xmlArtikuloa.Prezioa;
                            existingArtikuloa.Stock = xmlArtikuloa.Stock;
                            await _database.UpdateAsync(existingArtikuloa);
                        }
                    }
                    else
                    {
                        await _database.InsertAsync(xmlArtikuloa);
                    }
                }
            }
            catch (SQLiteException ex)
            {
                Debug.WriteLine($"SQLiteException: {ex.Message}");
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erroreren bat gertatu da: {ex.Message}");
            }

        }
    }
}
