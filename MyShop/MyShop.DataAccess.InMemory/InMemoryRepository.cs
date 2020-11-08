using MyShop.Core.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Caching;
using System.Text;
using System.Threading.Tasks;

namespace MyShop.DataAccess.InMemory
{
    public class InMemoryRepository<T> where T : BaseEntity
    {
        ObjectCache cache = MemoryCache.Default;
        List<T> items;
        string className;

        public InMemoryRepository()
        {
            className = typeof(T).Name;
            items = cache[className] as List<T>;

            if (items == null)
            {
                items = new List<T>();
            }
        }

        public T Find(string id)
        {
            var t = items.FirstOrDefault(i => i.Id == id);

            if (t != null)
            {
                return t;
            }
            else
            {
                throw new Exception(className + "not found");
            }
        }

        public void Insert(T t)
        {
            items.Add(t);
        }

        public void Update(T t)
        {
            var tToUpdate = items.FirstOrDefault(i => i.Id == t.Id);

            if (tToUpdate != null)
            {
                tToUpdate = t;
            }
            else
            {
                throw new Exception(className + "not found");
            }
        }

        public void Delete(string id)
        {
            var tToDelete = items.FirstOrDefault(i => i.Id == id);

            if (tToDelete != null)
            {
                items.Remove(tToDelete);
            }
            else
            {
                throw new Exception(className + "not found");
            }
        }

        public void Commit()
        {
            cache[className] = items;
        }

        public IQueryable<T> Collection()
        {
            return items.AsQueryable();
        }
    }
}
