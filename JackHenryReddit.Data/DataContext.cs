using JackHenryReddit.Data.Models;
using Microsoft.Extensions.Logging;
using System;
using System.Collections;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackHenryReddit.Data
{
    /// <summary>
    /// This needs to be a Singleton in the DI container
    /// </summary>
    public class InMemoryDataContext : IDataContext
    {
        static ConcurrentDictionary<string, IEnumerable<object>> _dataStore = new ConcurrentDictionary<string, IEnumerable<object>>();
        
        IEnumerable<T> getTableData<T>()
        {
            IEnumerable<object> data = new List<object>();
            if (!_dataStore.TryGetValue(typeof(T).Name, out data))
            {
                data = new List<object>();
                setTableData<T>(new List<T>());
            }
            return data.Cast<T>();
        }

        void setTableData<T>(IEnumerable<T> tableData)
        {
            _dataStore.AddOrUpdate(typeof(T).Name, tableData.Cast<object>(), (key, preValue) => tableData.Cast<object>());
        }

        public async Task Add<T>(IEnumerable<T> data)
            => await Task.Run(() => 
            { 
                // _dataStore is using a ConcurrentDictionary, but we need this entire process to be atomic
                lock (_dataStore) 
                {
                    var currentData = getTableData<T>();

                    var updatedData = currentData
                                      .Concat(data)
                                      .DistinctBy(c => (c as RedditResponseChildData).name);

                    setTableData(updatedData); 
                } 
            });

        public async Task Replace<T>(IEnumerable<T> data)
            => await Task.Run(() => setTableData<T>(data));

        public async Task Clear<T>()
            => await Task.Run(() => setTableData<T>(new List<T>()));

        public async Task Count<T>()
            => await Task.Run(() => getTableData<T>().Count());

        public async Task<IEnumerable<T>> Get<T>()
            => await Task.Run(() => getTableData<T>());
    }
}
