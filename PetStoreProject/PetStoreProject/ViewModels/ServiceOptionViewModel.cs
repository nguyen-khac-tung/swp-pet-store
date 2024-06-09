﻿namespace PetStoreProject.ViewModels
{
    public class ServiceOptionViewModel
    {
        public int ServiceOptionId { get; set; }

        public int ServiceId { get; set; }
        
        public string PetType { get; set; }

        public string Weight { get; set; } 

        public float Price { get; set; }

        public List<string> Weights { get; set; }
    }
}
