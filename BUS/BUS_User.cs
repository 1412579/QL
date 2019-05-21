using BUS;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BUS
{
    public class BUS_User
    {
        private BUS_User()
        {
        }
        private static BUS_User instance;
        static object key = new object();
        public static BUS_User Instance
        {
            get
            {
                if (instance == null)
                {
                    lock (key)
                    {
                        instance = new BUS_User();
                    }
                }
                return instance;
            }
        }

        public User CheckLogin(string username, string password)
        {
            string md5MK = md5(password);
            var dto = ctx_library.Instance.GetAll<User>().Where(c => c.UserName.ToLower() == username.ToLower() && c.Password.ToLower() == md5MK.ToLower()).FirstOrDefault();
            if (dto != null)
            {
                return dto;
            }
            return null;
        }

        public int Insert(User dto)
        {
            dto.Password = md5(dto.Password);
            return ctx_library.Instance.Insert<User>(dto);
        }

        public List<User> GetAll()
        {
            return ctx_library.Instance.GetAll<User>().ToList();
        }

        public int CheckExistUsername(string username)
        {
            var dto = ctx_library.Instance.GetAll<User>().Where(c => c.UserName.ToLower() == username.ToLower()).FirstOrDefault();
            if (dto != null)
            {
                return dto.UserId;
            }
            return -1;
        }

        public User GetById(int userid)
        {
            var dto = ctx_library.Instance.GetAll<User>().Where(c => c.UserId == userid).FirstOrDefault();
            if (dto != null)
            {
                return dto;
            }
            return null;
        }

        public int UpdatePassword(int UserId, string OldPassword, string NewPassword)
        {
            var dto = ctx_library.Instance.GetByID<User>(UserId);
            if (CheckLogin(dto.UserName, md5(OldPassword)) != null)
            {
                dto.Password = md5(NewPassword);
                ctx_library.Instance.Update<User>(dto, UserId);
                return 1;
            }
            return 0;
        }

        public int UpdatePasswordAdmin(int UserId, string NewPassword, int Role)
        {
            try
            {
                var dto = ctx_library.Instance.GetByID<User>(UserId);
                dto.Password = md5(NewPassword);
                dto.Role = Role;
                ctx_library.Instance.Update<User>(dto, UserId);
                return 1;
            }
            catch { }
            return 0;
        }

        public static byte[] encryptData(string data)
        {
            System.Security.Cryptography.MD5CryptoServiceProvider md5Hasher = new System.Security.Cryptography.MD5CryptoServiceProvider();
            byte[] hashedBytes;
            System.Text.UTF8Encoding encoder = new System.Text.UTF8Encoding();
            hashedBytes = md5Hasher.ComputeHash(encoder.GetBytes(data));
            return hashedBytes;
        }
        public static string md5(string data)
        {
            return BitConverter.ToString(encryptData(data)).Replace("-", "").ToLower();
        }
    }
}
