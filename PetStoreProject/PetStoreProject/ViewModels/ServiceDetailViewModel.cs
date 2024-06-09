﻿using PetStoreProject.Models;

namespace PetStoreProject.ViewModels
{
    public class ServiceDetailViewModel
    {
        public int ServiceId { get; set; }

        public string Name { get; set; }

        public string? Type { get; set; }

        public List<Image> Images {  get; set; } 

        public List<string> PetTypes { get; set; }

        public string? Description { get; set; }

        public bool IsDelete { get; set; }
    }
}
