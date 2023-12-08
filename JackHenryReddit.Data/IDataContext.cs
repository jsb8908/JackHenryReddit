using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JackHenryReddit.Data
{
    public interface IDataContext
    {
        Task Add<T>(IEnumerable<T> data);
        Task<IEnumerable<T>> Get<T>();
        Task Replace<T>(IEnumerable<T> data);
        Task Clear<T>();
        Task Count<T>();
    }
}
