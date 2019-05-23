using BUS;
using System;
using System.Collections.Generic;
using System.Dynamic;
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

        public string GetApiLink(int TypeAPI,string Token, string Container, string Start,string End, int UserApiId = 0, TypeAppodeal whatapi = TypeAppodeal.Start, int taskId = 0)
        {
            var rsl = string.Empty;
            if(TypeAPI == 0)
            {
                if(whatapi == TypeAppodeal.Start)
                {
                    rsl = $"https://www.appodeal.com/api/v2/stats_api?api_key={Token}&user_id={UserApiId}&date_from={Start}&date_to={End}&detalisation[]=app&detalisation[]=country";
                }
                else if (whatapi == TypeAppodeal.Status)
                {
                    rsl = $"https://www.appodeal.com/api/v2/check_status?api_key={Token}&user_id={UserApiId}&task_id={taskId}";
                }
                else if (whatapi == TypeAppodeal.Output)
                {
                    rsl = $"https://www.appodeal.com/api/v2/output_result?api_key={Token}&user_id={UserApiId}&task_id={taskId}";
                }
            }
            else if(TypeAPI == 1)
            {
                rsl = $"https://api.adincube.com/api/1.3/public/reporting/inventory/{Container}?start={Start}&end={End}&auth_token={Token}";
            }
            return rsl;
        }

        public bool IsPropertyExist(dynamic d, string name)
        {
            Type type = d.GetType();
            return type.GetProperties().Any(p => p.Name.Equals(name));
        }

    }
}
