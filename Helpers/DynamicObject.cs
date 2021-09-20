using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Dynamic;
using CITechnicalTest.Models;

namespace CITechnicalTest.Helpers
{
    public static class DynamicObject
    {
        public static ExpandoObject CreateExpandoFromObject(Object source)
        {
            dynamic result = new ExpandoObject();
            result.

            result.amount_eur = 123;
           
            return result;
        }


    }
}
