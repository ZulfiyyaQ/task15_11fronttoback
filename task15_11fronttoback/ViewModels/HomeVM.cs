﻿using task15_11fronttoback.Models;

namespace task15_11fronttoback.ViewModels
{
    public class HomeVM
    {
        public List<Product> Products { get; set; }
        public List<Slide> Slides { get; set; }
        public List<Product> LatestProducts { get; set; }
    }
}
