using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Web;

namespace BUS
{
    [Serializable]
    public class Enum
    {
        
    }

    [Serializable]
    public class DataApi
    {
        public string Name { get; set; }
        public string CountryCode { get; set; }
        public string Platform { get; set; }
        public int Requests { get; set; }
        public int Fills { get; set; }
        public int Impressions { get; set; }
        public double Fillrate { get; set; }
        public int Clicks { get; set; }
        public double CTR { get; set; }
        public int Views { get; set; }
        public double Revenue { get; set; }
        public double Ecpm { get; set; }
    }

    [Serializable]
    public class ListDataApi
    {
        public string Html { get; set; }
        public int TotalImpressions { get; set; }
        public int TotalClicks { get; set; }
        public double TotalRevenue { get; set; }
        public double TotalFillrate { get; set; }
        public int InfoApiId { get; set; }
    }

    [Serializable]
    public class CalledToAppodeal
    {
        public dynamic Data { get; set; }
        public int InfoApiId { get; set; }
    }

    [Serializable]
    public class NoData
    {
        public List<int> TimeOut { get; set; }
        public List<int> NoResult { get; set; }
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
    public enum TypeAppodeal
    {
        Start = 0,
        Status = 1,
        Output = 2
    }
}