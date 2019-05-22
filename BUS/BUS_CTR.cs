using BUS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class BUS_CTR
    {
        private BUS_CTR()
        {
        }
        private static BUS_CTR instance;
        static object key = new object();
        public static BUS_CTR Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (key)
                    {
                        instance = new BUS_CTR();
                    }
                }
                return instance;
            }
        }
        public int Insert(TrickAvarage dto)
        {
            return ctx_library.Instance.Insert<TrickAvarage>(dto);
        }
        public TrickAvarage Get()
        {
            var lst = ctx_library.Instance.GetAll<TrickAvarage>().ToList();
            if (lst != null && lst.Any())
                return lst.FirstOrDefault();
            return null;
        }
        public int Update(TrickAvarage model)
        {
            ctx_library.Instance.Update<TrickAvarage>(model, model.TrickAvarageId);
            return 1;
        }
    }
}
