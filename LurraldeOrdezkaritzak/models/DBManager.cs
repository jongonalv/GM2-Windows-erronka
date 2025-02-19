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

        public async Task<string> GetArtikuloaById(int id)
        {
            var query = "SELECT izena FROM Artikuloa WHERE id = ?";
            var result = await _database.ExecuteScalarAsync<string>(query, id);
            return result ?? "Ezezaguna"; 
        }

        public async Task UpdateArtikuloaStockAsync(int artikuloaId, int kantitatea)
        {
            var artikuloa = await _database.Table<Artikuloa>().FirstOrDefaultAsync(a => a.Id == artikuloaId);
            if (artikuloa != null)
            {
                artikuloa.Stock -= kantitatea;
                await _database.UpdateAsync(artikuloa);
            }
        }
        public async Task MarkEskaeraEgoitzaAsEntregatutaAsync(int id)
        {
            var eskaeraEgoitza = await _database.Table<EskaeraEgoitza>().FirstOrDefaultAsync(e => e.Id == id);
            if (eskaeraEgoitza != null)
            {
                eskaeraEgoitza.Entregatuta = true;
                await _database.UpdateAsync(eskaeraEgoitza);
            }
        }

        public async Task<List<Eskaera>> GetEskaerakByArtikuloaIdAsync(int artikuloaId)
        {
            var query = @"
        SELECT e.*
        FROM Eskaera e
        INNER JOIN EskaeraArtikuloa ea ON e.id = ea.eskaera_id
        WHERE ea.artikuloa_id = ?";
            return await _database.QueryAsync<Eskaera>(query, artikuloaId);
        }

        public async Task<EskaeraEgoitza> GetEskaeraEgoitzaByIdAsync(int id)
        {
            var query = @"
                SELECT ee.id, ee.Kantitatea, ee.Iritsiera_data AS IritsieraDataUnix, ee.Entregatuta,
                       ee.artikuloa_id AS ArtikuloaId, a.izena AS ArtikuluaIzena
                FROM EskaeraEgoitza ee
                INNER JOIN Artikuloa a ON ee.artikuloa_id = a.id
                WHERE ee.id = ?";

            var eskaeraEgoitza = await _database.QueryAsync<EskaeraEgoitza>(query, id);
            return eskaeraEgoitza.FirstOrDefault();
        }

        public async Task<List<EskaeraEgoitza>> GetEskaeraEgoitzaWithArtikuluaAsync()
        {
            var query = @"
                        SELECT ee.id, ee.Kantitatea, ee.Iritsiera_data AS IritsieraDataUnix, ee.Entregatuta,
                               ee.artikuloa_id AS ArtikuloaId, a.izena AS ArtikuluaIzena
                        FROM EskaeraEgoitza ee
                        INNER JOIN Artikuloa a ON ee.artikuloa_id = a.id";

            var eskaeraEgoitzaList = await _database.QueryAsync<EskaeraEgoitza>(query);
            return eskaeraEgoitzaList.ToList();
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
