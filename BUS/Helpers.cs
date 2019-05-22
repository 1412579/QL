using BUS;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;

namespace BUS
{
    public class Helpers
    {
        private Helpers()
        {
        }
        private static Helpers instance;
        static object key = new object();
        public static Helpers Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (key)
                    {
                        instance = new Helpers();
                    }
                }
                return instance;
            }
        }

        /// <summary>
        /// Lấy nội dung JSON từ http url
        /// </summary>
        /// <param name="strUrl"></param>
        /// <returns></returns>
        public dynamic GetJSON(String strUrl)
        {
            try
            {
                WebClient webClient = new WebClient();
                Stream stream = webClient.OpenRead(strUrl);
                string b;
                using (StreamReader br = new StreamReader(stream))
                {
                    b = br.ReadToEnd();
                }
                JavaScriptSerializer serializer = new JavaScriptSerializer();
                dynamic result = serializer.Deserialize<object>(b);
                return result;
            }
            catch (Exception ex)
            {
            }
            return null;
        }

    }
}
