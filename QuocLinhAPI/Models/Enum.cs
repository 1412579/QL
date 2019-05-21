using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace QuocLinhAPI.Models
{
    [Serializable]
    public class Enum
    {
        
    }
    public enum Role
    {
        Admin = 1,
        [Description("Người dùng")]
        User = 0
    }
    public enum API
    {
        Adincube = 1,
        Appodeal = 0
    }
}