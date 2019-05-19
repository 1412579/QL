using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Data.Entity;

namespace BUS
{
    public class ctx_library
    {
        private ctx_library()
        {
        }
        QLEntities ctx;
        private static ctx_library instance;
        static object key = new object();
        public static ctx_library Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (key)
                    {
                        instance = new ctx_library();
                    }
                }
                return instance;
            }
        }

        public IEnumerable<T> GetAll<T>() where T : class
        {
            ctx = new QLEntities();
            return ctx.Set<T>();//.OrderByDescending("ID");
        }

        //public IEnumerable<T> GetAll_with_Sort<T>(bool IsOrder, string keyword) where T : class
        //{
        //    ctx = new QLEntities();
        //    if (IsOrder)
        //        return ctx.Set<T>().OrderBy(keyword).ToList();
        //    else
        //        return ctx.Set<T>().OrderByDescending(keyword);
        //}

        public T GetByID<T>(int id) where T : class
        {
            ctx = new QLEntities();
            return ctx.Set<T>().Find(id);
        }

        public int Insert<T>(T dto) where T : class
        {
            ctx = new QLEntities();
            ctx.Set<T>().Add(dto);
            ctx.SaveChanges();
            return ctx.SaveChanges(); 
        }

        public void Update<T>(T dto, int id) where T : class
        {
            ctx = new QLEntities();
            var dUp = ctx.Set<T>().Find(id);
            if (dUp != null)
            {
                ctx.Entry(dUp).CurrentValues.SetValues(dto);
            }
            ctx.SaveChanges();
        }

        public int Delete<T>(int id) where T : class
        {
            ctx = new QLEntities();
            T entity = ctx.Set<T>().Find(id);
            if (ctx.Entry<T>(entity).State == EntityState.Detached)
            {
                ctx.Set<T>().Attach(entity);
            }
            ctx.Set<T>().Remove(entity);
            return ctx.SaveChanges();
        }
    }
}
