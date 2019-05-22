using BUS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class BUS_API
    {
        private BUS_API()
        {
        }
        private static BUS_API instance;
        static object key = new object();
        public static BUS_API Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (key)
                    {
                        instance = new BUS_API();
                    }
                }
                return instance;
            }
        }

        public int Insert(InfoApi dto)
        {
            return ctx_library.Instance.Insert<InfoApi>(dto);
        }

        public int Delete(int InfoApiId)
        {
            return ctx_library.Instance.Delete<InfoApi>(InfoApiId);
        }

        public List<InfoApi> GetAll()
        {
            return ctx_library.Instance.GetAll<InfoApi>().ToList();
        }

        public List<InfoApi> GetAllByUserId(int userApiId)
        {
            return ctx_library.Instance.GetAll<InfoApi>().Where(c => c.UserId == userApiId).ToList();
        }

        public InfoApi GetById(int InfoApiId)
        {
            var dto = ctx_library.Instance.GetAll<InfoApi>().Where(c => c.InfoApiId == InfoApiId).FirstOrDefault();
            if (dto != null)
            {
                return dto;
            }
            return null;
        }

        public int Update(InfoApi model)
        {
            ctx_library.Instance.Update<InfoApi>(model, model.InfoApiId);
            return 1;
        }

    }
}
