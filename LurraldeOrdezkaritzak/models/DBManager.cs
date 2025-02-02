using SQLite;
using System;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace lurraldeOrdezkaritzak
{
    public class DBManager
    {
        private readonly SQLiteAsyncConnection _database;

        public DBManager(string dbPath)
        {
            _database = new SQLiteAsyncConnection(dbPath);
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
                await GenerateSampleArtikuloak();
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


        public async Task GenerateSampleArtikuloak()
        {
            var count = await _database.Table<Artikuloa>().CountAsync();
            if (count > 0) return;

            var sampleArtikuloak = new List<Artikuloa>
                {
                    new Artikuloa("Tornillo", "Burdindegia", 0.10, 500),
                    new Artikuloa("Clavo", "Burdindegia", 0.05, 1000),
                    new Artikuloa("Bisagra", "Burdindegia", 2.50, 100),
                    new Artikuloa("Llave Inglesa", "Eskuzko Tresnak", 15.99, 20),
                    new Artikuloa("Destornillador", "Eskuzko Tresnak", 7.99, 50),
                    new Artikuloa("Martillo", "Eskuzko Tresnak", 12.99, 30),
                    new Artikuloa("Taladro", "Tresna Elektrikoak", 89.99, 15),
                    new Artikuloa("Sierra Circular", "Tresna Elektrikoak", 149.99, 10),
                    new Artikuloa("Lijadora", "Tresna Elektrikoak", 59.99, 25),
                    new Artikuloa("Compresor de Aire", "Tresna Elektrikoak", 199.99, 5)
                };

            await _database.InsertAllAsync(sampleArtikuloak);
        }

    }
}
