using CITechnicalTest.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading.Tasks;
using CITechnicalTest.RestAPI;
using CITechnicalTest.Models;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using CITechnicalTest.Helpers;
using System.Dynamic;
using Microsoft.AspNetCore.Http;


namespace CITechnicalTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {

            //Task 1 Extract rates from 1st October and 31st October 2019
            var dtFrom = new DateTime(2019, 10, 01);
            var dtTo = new DateTime(2019, 10, 31);
            var rateList = this.GetRates(dtFrom, dtTo);

            //Task 2 Load data files and merge into one data
            var csvData = ReadCSVFile.getCSVData();


            //Task 3 Merge the data with the rates dataset.
            var rateData = (from csv in csvData
                            join rate in rateList
                            on new { csv.Date, csv.currency } equals new { Date = rate.date, currency = rate.code }
                            select new
                            {
                                country = csv.country,
                                amount = csv.amount,
                                currency = csv.currency,
                                Date = csv.Date,
                                code = rate.code,
                                rate = rate.rate,
                            }).ToList();


            //Task 4 Add a new column or property called amount_eur, with the values in the column amount converted to Euros

            List<dynamic> dmRateList = new List<dynamic>();
            foreach (var rd in rateData) {
                dynamic dmRate = new ExpandoObject();
                dmRate.country = rd.country;
                dmRate.amount = rd.amount;
                dmRate.currency = rd.currency;
                dmRate.date = rd.Date;
                dmRate.code = rd.code;
                dmRate.rate = rd.rate;
                dmRate.amount_eur = this.GetAmountEuro(rd.amount,rd.rate);
                dmRateList.Add(dmRate);
            }

            //Task 5 Group Countries into the specified Categories.
            var countryList = from r in dmRateList.ToList()
                                group r by r.country;

            List<CountryCategory> countryCategoryList = new List<CountryCategory>();
            foreach (var country in countryList.ToList())
            {
                CountryCategory countryCategory = new CountryCategory()
                {
                    name = country.Key,
                    category = this.GetCategory(country.Key)
                };
                countryCategoryList.Add(countryCategory);
            }

            HttpContext.Session.SetString("countrycategorylist", JsonConvert.SerializeObject(countryCategoryList));

            //Task 6 Calculate the total amount in Euro for each country group.
            List <Country> countryGroup = new List<Country>();
            foreach (var country in countryCategoryList.ToList())
            {
                Country amtByCountry = new Country() {
                    name = country.name,
                    amount = this.GetAmountByCountry(country.name, dmRateList)
                };

                countryGroup.Add(amtByCountry);
            }

            var amountByCountryList = countryGroup.OrderByDescending(x => x.amount).ToList();



            //Task 7 Display on web view as a formatted HTML table.
            return View(amountByCountryList);

        }
        [HttpGet]
        public IActionResult CountryList(string category)
        {

            var strCountry = HttpContext.Session.GetString("countrycategorylist");
            var objCountry = JsonConvert.DeserializeObject<List<CountryCategory>>(strCountry);
            var countryList = (from c in objCountry
                               where c.category.Equals(category)
                               group c by c.name into g
                               select new CountryCategory
                               {
                                   category = "",
                                   name = g.Key
                               }).ToList();

            
            return PartialView("CountryList", countryList);
        }
       


        private List<RateData> GetRates(DateTime dtFrom, DateTime dtTo) {

            List<RateData> rtDataList = new List<RateData>();
            
            DateTime curDate = dtFrom;
            while (curDate <= dtTo)
            {
                var url = string.Format("http://data.fixer.io/api/{0}?access_key=5dc6fbe85ad5b160c65efd6e520c7b9a&base=EUR ", curDate.ToString("yyyy-MM-dd"));

                var objAPI = new RequestObject(url);
                //var json = objAPI.ExecAPIRequest(); 

                var json = JsonData.ReadJsonData(curDate.Day - 1);

                var dtaObj = JObject.Parse(json);

                foreach (var item in dtaObj["rates"].ToList())
                {
                    if (item != null)
                    {
                        var key = item.ToString().Split(":")[0].ToString().Replace("\"", "");
                        var val = item.ToString().Split(":")[1];
                        var bs = dtaObj["base"].ToString();

                        RateData rateData = new RateData() {
                            date = curDate,
                            code = key,
                            rate = val,
                            Base = bs
                        };
                        rtDataList.Add(rateData);
                    }
                }
                curDate = curDate.AddDays(1);
            }

            return rtDataList;
        }

        private decimal GetAmountEuro(decimal amt, string rate)
        {
            decimal amtEur = 0;
            if (!string.IsNullOrEmpty(rate))
            {
                amtEur = amt / Convert.ToDecimal(rate);
            }

            return amtEur;
        }

        private string GetCategory(string country)
        {
            string category = "";

            switch (country)
            {
                case "Austria": case "Italy": case "Belgium": case "Latvia":
                    category = "EU";
                    break;
                case "Chile": case "Qatar": case "United Arab Emirates": case "United States of America":
                    category = "ROW";
                    break;
                case "United Kingdom":
                    category = "United Kingdom";
                    break;
                case "Australia":
                    category = "Australia";
                    break;
                case "South Africa":
                    category = "South Africa";
                    break;
            }
             
            return category;
        }

        private decimal GetAmountByCountry(string country, List<dynamic> countryList)
        {
            decimal amtEuro = 0;

            if (countryList.Any())
            {
                var amtCountries = countryList.Where(x => x.country == country && x.currency == "EUR").ToList();
                if (amtCountries.Any())
                {
                    foreach (dynamic cnt in amtCountries)
                    {
                        foreach (var amt in cnt)
                        {
                            if (amt.Key == "amount_eur")
                            {
                                amtEuro = amtEuro + Convert.ToDecimal(amt.Value);
                            }

                        }
                    }
                }
            }
            
            return amtEuro;
        }








    }
}
