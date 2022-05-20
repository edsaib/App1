using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace App1.Services
{
    public interface IDataStore<T>
    {
        Task<int> AddContactAsync(T contact);
        Task<int> UpdateContactAsync(T contact);
        Task<int> DeleteContactAsync(int id);
        Task<T> GetContactAsync(int id);
        Task<List<T>> GetContactsAsync(bool forceRefresh = false);
    }
}
