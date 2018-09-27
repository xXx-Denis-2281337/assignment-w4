using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;

namespace KmaOoad18.Assignments.Week4.Data
{
    public class Product
    {
        public decimal Price { get; set; }
        public string Sku { get; set; }
        public string Name { get; set; }
    }
}
