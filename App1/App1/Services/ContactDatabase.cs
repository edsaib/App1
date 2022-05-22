using App1.Models;
using SQLite;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App1.Services
{
    public class ContactDatabase : IDataStore<Contact>
    {

        private readonly SQLiteAsyncConnection _connection;

        public ContactDatabase()
        { }
        public ContactDatabase(string dbPath, SQLite.SQLiteOpenFlags flags)
        {
            _connection = new SQLiteAsyncConnection(dbPath, flags);
            _connection.CreateTableAsync<Contact>();        // only creates a table if it doesn't exist
        }

        public Task<int> AddContactAsync(Contact contact)
        {
            return _connection.InsertAsync(contact);
        }

        public Task<int> DeleteContactAsync(int id)
        {
            return (_connection.DeleteAsync<Contact>(id));
        }

        public Task<Contact> GetContactAsync(int id)
        {
            return _connection.GetAsync<Contact>(id);
        }

        public Task<List<Contact>> GetContactsAsync(bool forceRefresh = false)
        {
            return _connection.Table<Contact>().ToListAsync();
        }

        public Task<int> UpdateContactAsync(Contact contact)
        {
            return _connection.UpdateAsync(contact);
        }
    }
}
