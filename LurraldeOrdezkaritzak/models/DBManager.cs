using Ikaslea.KomertzialakApp.Models.Enums;
using LurraldeOrdezkaritzak.ItemViews;
using SQLite;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Threading.Tasks;
using System.Xml.Linq;
using static LurraldeOrdezkaritzak.Estadistikak;

namespace lurraldeOrdezkaritzak
{
    /// <summary>
    /// SQLite datu-basearekin asinkronoki konektatzeko klasea.
    /// </summary>
    public class DBManager
    {
        private readonly SQLiteAsyncConnection _database;
        private readonly XmlManager _xmlManager;
        private static DBManager _instance;

        /// <summary>
        /// DBManager klasearen instantzia bakarra eskuratzen du (Singleton eredua).
        /// </summary>
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

        /// <summary>
        /// DBManager objektua sortzen du, SQLite datu-basearekin konexioa ezarriz.
        /// </summary>
        /// <param name="dbPath">Datu-basearen bidea.</param>
        public DBManager(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
            _xmlManager = new XmlManager();
            InitializeDatabase();
        }

        /// <summary>
        /// Datu-basearen taulak sortzen ditu hasierako exekuzioan.
        /// </summary>
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

        public Task<List<Bazkidea>> GetBazkideakAsync()
        {
            return _database.Table<Bazkidea>().ToListAsync();
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

        public Task<List<EskaeraArtikuloa>> GetEskaeraArtikuloakAsync()
        {
            return _database.Table<EskaeraArtikuloa>().ToListAsync();
        }

        public async Task<List<string>> GetKategoriakAsync()
        {
            return await _database.QueryScalarsAsync<string>("SELECT DISTINCT Kategoria FROM Artikuloa");
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


        /// <summary>
        /// EskaeraEgoitza taulako eskaera guztiak itzultzen ditu.
        /// </summary>
        /// <returns>EskaeraEgoitza objektuen zerrenda.</returns>
        public Task<List<EskaeraEgoitza>> GetEskaeraEgoitzaAsync()
        {
            return _database.Table<EskaeraEgoitza>().ToListAsync();
        }

        /// <summary>
        /// Emandako IDarekin lotutako artikuluaren izena itzultzen du.
        /// </summary>
        /// <param name="id">Artikuluaren IDa.</param>
        /// <returns>Artikuluaren izena edo "Ezezaguna" baldin eta ez bada aurkitzen.</returns>
        public async Task<string> GetArtikuloaById(int id)
        {
            var query = "SELECT izena FROM Artikuloa WHERE id = ?";
            var result = await _database.ExecuteScalarAsync<string>(query, id);
            return result ?? "Ezezaguna";
        }

        /// <summary>
        /// Emandako artikuluaren stock-a eguneratzen du, kantitatea kenduz.
        /// </summary>
        /// <param name="artikuloaId">Artikuluaren IDa.</param>
        /// <param name="kantitatea">Stock-etik kendu beharreko kantitatea.</param>
        public async Task UpdateArtikuloaStockAsync(int artikuloaId, int kantitatea)
        {
            var artikuloa = await _database.Table<Artikuloa>().FirstOrDefaultAsync(a => a.Id == artikuloaId);
            if (artikuloa != null)
            {
                artikuloa.Stock -= kantitatea;
                await _database.UpdateAsync(artikuloa);
            }
        }

        /// <summary>
        /// Emandako IDa duen EskaeraEgoitza "entregatuta" bezala markatzen du.
        /// </summary>
        /// <param name="id">EskaeraEgoitzaren IDa.</param>
        public async Task MarkEskaeraEgoitzaAsEntregatutaAsync(int id)
        {
            var eskaeraEgoitza = await _database.Table<EskaeraEgoitza>().FirstOrDefaultAsync(e => e.Id == id);
            if (eskaeraEgoitza != null)
            {
                eskaeraEgoitza.Entregatuta = true;
                await _database.UpdateAsync(eskaeraEgoitza);
            }
        }


        /// <summary>
        /// Emandako artikuloaren IDarekin lotutako eskaera guztiak itzultzen ditu.
        /// </summary>
        /// <param name="artikuloaId">Artikuluaren IDa.</param>
        /// <returns>Artikulu horrekin lotutako eskaeren zerrenda.</returns>
        public async Task<List<Eskaera>> GetEskaerakByArtikuloaIdAsync(int artikuloaId)
        {
            var query = @"
                        SELECT e.*
                        FROM Eskaera e
                        INNER JOIN EskaeraArtikuloa ea ON e.id = ea.eskaera_id
                        WHERE ea.artikuloa_id = ?";
            return await _database.QueryAsync<Eskaera>(query, artikuloaId);
        }

        /// <summary>
        /// Emandako IDa duen EskaeraEgoitza bilatzen du eta horri lotutako artikuluaren izena gehitzen du.
        /// </summary>
        /// <param name="id">EskaeraEgoitzaren IDa.</param>
        /// <returns>EskaeraEgoitza objektua artikuluaren izenarekin.</returns>
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

        /// <summary>
        /// EskaeraEgoitza taulako eskaera guztiak lortzen ditu, eta bakoitzari lotutako artikuluaren izena gehitzen dio.
        /// </summary>
        /// <returns>EskaeraEgoitza objektuen zerrenda artikuluaren izenarekin.</returns>
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
        /// <summary>
        /// XML fitxategi bat aukeratu eta bertako datuak SQLite datu-basean txertatzen ditu.
        /// Komertzialen eskaerak kargatzeko pentsatuta dagoen metodoa da
        /// </summary>
        public async Task XMLDatuakKargatu()
        {
            // Fitxategi mota onartua zehazten da (XML).
            var xmlFileType = new FilePickerFileType(new Dictionary<DevicePlatform, IEnumerable<string>>
            {
                { DevicePlatform.WinUI, new[] { ".xml" } }
            });

            // Fitxategi hautatzailea irekitzen da.
            var pickResult = await FilePicker.PickAsync(new PickOptions
            {
                FileTypes = xmlFileType,
                PickerTitle = "XML Fitxategi bat aukeratu mesedez." // "Por favor, seleccione un archivo XML."
            });

            if (pickResult == null)
            {
                Debug.WriteLine("Ez da aukeratu fitxategirik"); // "No se ha seleccionado ningún archivo."
                return;
            }

            string fitxHelbidea = pickResult.FullPath;

            try
            {
                // XML fitxategia kargatzen da.
                var xmlDoc = XDocument.Load(fitxHelbidea);

                // "Eskaerak" elementutik datuak irakurtzen dira.
                var xmlEskaerak = xmlDoc.Root.Element("Eskaerak").Descendants("Eskaera").Select(e => new Eskaera
                {
                    Id = (int)e.Element("id"),
                    EskaeraData = DateTime.Parse((string)e.Element("eskaeraData")),
                    BazkideaId = (int)e.Element("bazkideaId"),
                    Kontzeptua = (string)e.Element("kontzeptua"),
                    Egoera = Enum.Parse<Egoera>((string)e.Element("egoera")),
                    Guztira = (double)e.Element("guztira")
                }).ToList();

                // "EskaeraArtikuloak" elementutik datuak irakurtzen dira.
                var xmlEskaeraArtikuloak = xmlDoc.Root.Element("EskaeraArtikuloak").Descendants("EskaeraArtikuloa").Select(ea => new EskaeraArtikuloa
                {
                    Id = (int)ea.Element("id"),
                    Guztira = (double)ea.Element("guztira"),
                    Kantitatea = (int)ea.Element("kantitatea"),
                    Deskontua = (int)ea.Element("deskontua"),
                    ArtikuloaId = (int)ea.Element("artikuloa_id"),
                    EskaeraId = (int)ea.Element("eskaera_id")
                }).ToList();

                // XML fitxategiak daturik ez badu, mezu bat erakusten da.
                if (xmlEskaerak.Count == 0 && xmlEskaeraArtikuloak.Count == 0)
                {
                    Debug.WriteLine("Ez dira aurkitu daturik XML fitxategian"); // "No se han encontrado datos en el archivo XML."
                    return;
                }

                Debug.WriteLine($"Eskaerak encontrados: {xmlEskaerak.Count}"); // "Órdenes encontradas: {xmlEskaerak.Count}"
                Debug.WriteLine($"EskaeraArtikuloak encontrados: {xmlEskaeraArtikuloak.Count}"); // "Artículos de orden encontrados: {xmlEskaeraArtikuloak.Count}"

                // Eskaerak datu-basean txertatzen dira.
                foreach (var xmlEskaera in xmlEskaerak)
                {
                    try
                    {
                        Debug.WriteLine($"Insertando Eskaera ID: {xmlEskaera.Id}"); // "Insertando orden ID: {xmlEskaera.Id}"
                        await _database.InsertAsync(xmlEskaera);
                    }
                    catch (SQLiteException ex)
                    {
                        Debug.WriteLine($"Error al insertar Eskaera ID {xmlEskaera.Id}: {ex.Message}");
                        // "Error al insertar orden ID {xmlEskaera.Id}: {ex.Message}"
                    }
                }

                // EskaeraArtikuloak datu-basean txertatzen dira.
                foreach (var xmlEskaeraArtikuloa in xmlEskaeraArtikuloak)
                {
                    try
                    {
                        Debug.WriteLine($"Insertando EskaeraArtikuloa ID: {xmlEskaeraArtikuloa.Id}");
                        // "Insertando artículo de orden ID: {xmlEskaeraArtikuloa.Id}"
                        await _database.InsertAsync(xmlEskaeraArtikuloa);
                    }
                    catch (SQLiteException ex)
                    {
                        Debug.WriteLine($"Error al insertar EskaeraArtikuloa ID {xmlEskaeraArtikuloa.Id}: {ex.Message}");
                        // "Error al insertar artículo de orden ID {xmlEskaeraArtikuloa.Id}: {ex.Message}"
                    }
                }
            }
            catch (SQLiteException ex)
            {
                Debug.WriteLine($"SQLiteException: {ex.Message}"); // "Excepción de SQLite: {ex.Message}"
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Erroreren bat gertatu da: {ex.Message}"); // "Ha ocurrido un error: {ex.Message}"
            }
        }


        /// <summary>
        ///     Estadistikak ateratzeko metodo bat, zein izan den gehien eskaturiko artikuluak
        ///     egiten duena.
        /// </summary>
        /// <returns></returns>
        public async Task<List<ArtikuloaEstadistika>> GetArtikuloaEstadistikakAsync()
        {
            try
            {
                string query = @"
            SELECT 
                a.id AS ArtikuloId,
                a.izena AS ArtikuloIzena,
                SUM(ea.kantitatea) AS TotalSalduta
            FROM EskaeraArtikuloa ea
            JOIN Artikuloa a ON ea.artikuloa_id = a.id
            GROUP BY a.id, a.izena
            ORDER BY TotalSalduta DESC;";

                var estadistikak = await _database.QueryAsync<ArtikuloaEstadistika>(query);

                return estadistikak;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errorea estadistikak lortzerakoan: {ex.Message}");
                return new List<ArtikuloaEstadistika>();
            }
        }

        /// <summary>
        ///     Estadistikak ateratzeko metodo bat, zein bazkide egin dituen eskaera gehien ateratzen du,
        ///     eta ordenatu egiten dira.
        /// </summary>
        /// <returns></returns>
        public async Task<List<BazkideaEskaeraEstadistika>> GetBazkideakEskaeraEstadistikakAsync()
        {
            try
            {
                string query = @"
                SELECT 
                    b.id AS BazkideaId,
                    b.izena AS BazkideaIzena,
                    COUNT(e.id) AS EskaeraTotalak
                FROM Bazkidea b
                JOIN Eskaera e ON b.id = e.bazkideaId
                GROUP BY b.id, b.izena
                ORDER BY EskaeraTotalak DESC;";

                var estadistikak2 = await _database.QueryAsync<BazkideaEskaeraEstadistika>(query);

                return estadistikak2;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errorea bazkideen eskaera estadistikak lortzerakoan: {ex.Message}");
                return new List<BazkideaEskaeraEstadistika>();
            }
        }

        /// <summary>
        ///     Estadistikak ateratzeko metodo bat, bazkideen saldaera guztia ateratzen du,
        ///     hau dam zein bazkide gastatu duen gehiagoren arabera
        /// </summary>
        /// <returns></returns>
        public async Task<List<BazkideaArtikuloaSaldaera>> GetBazkideakArtikuloaSaldaeraAsync()
        {
            try
            {
                string query = @"
                SELECT 
                    b.id AS BazkideaId,
                    b.izena AS BazkideaIzena,
                    a.id AS ArtikuloaId,
                    a.izena AS ArtikuloaIzena,
                    SUM(ea.kantitatea) AS Unitateak,
                    SUM(ea.guztira) AS TotalSaldaera
                FROM Bazkidea b
                JOIN Eskaera e ON b.id = e.bazkideaId
                JOIN EskaeraArtikuloa ea ON e.id = ea.eskaera_id
                JOIN Artikuloa a ON ea.artikuloa_id = a.id
                GROUP BY b.id, b.izena, a.id, a.izena
                ORDER BY TotalSaldaera DESC;";

                var saldaerak = await _database.QueryAsync<BazkideaArtikuloaSaldaera>(query);

                return saldaerak;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Errorea bazkideen artikuluen saldaerak lortzerakoan: {ex.Message}");
                return new List<BazkideaArtikuloaSaldaera>();
            }
        }



    }
}
