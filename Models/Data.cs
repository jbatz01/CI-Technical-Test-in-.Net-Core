using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace CITechnicalTest.Models
{
        public class RateData
    { 
        public DateTime date { get; set; }
        public string Base { get; set; }
        public string code { get; set; }
        public string rate { get; set; }
    }

    public class CSVData
    {
        public DateTime Date { get; set; }
        public string country { get; set; }

        public string currency { get; set; }

        public decimal amount { get; set; }

    }

    //public class RatesDataset
    //{
    //    public DateTime Date { get; set; }
    //    public string country { get; set; }
    //    public string currency { get; set; }
    //    public decimal amount { get; set; }
    //    public string code { get; set; }
    //    public string rate { get; set; }
    //    public decimal amount_eur { get; set; }
    //}


    public class Country
    {
        public string name { get; set; }
        public decimal amount { get; set; }
    }

    public class CountryCategory
    {
        public string name { get; set; }
        public string category { get; set; }
    }

    public class DataList
    { 
        public List<Country> countryList { get; set; }

        public List<CountryCategory> categoryList { get; set; }
    }
}

