using CITechnicalTest.Models;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;

namespace CITechnicalTest.Components
{
    public class CategoryViewComponent: ViewComponent
    {
        public async Task<IViewComponentResult> InvokeAsync()
        {
      

            var strCategory = HttpContext.Session.GetString("countrycategorylist");
            var countryCategory = JsonConvert.DeserializeObject<List<CountryCategory>>(strCategory);
            var countryList = (from c in countryCategory
                              group c by c.category into g
                              select new CountryCategory { 
                                  category = g.Key,
                                  name = ""
                              }).ToList();

            return View(countryList);
        }

    }
}
