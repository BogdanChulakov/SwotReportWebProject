﻿namespace OnlineSlotReports.Services.Data.GamingHallServices
{
    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Linq;
    using System.Threading.Tasks;

    using OnlineSlotReports.Common;
    using OnlineSlotReports.Data.Common.Repositories;
    using OnlineSlotReports.Data.Models;
    using OnlineSlotReports.Services.Mapping;

    public class GamingHallService : IGamingHallService
    {
        private readonly IDeletableEntityRepository<GamingHall> repository;

        public GamingHallService(IDeletableEntityRepository<GamingHall> repository)
        {
            this.repository = repository;
        }

        public async Task AddAsync(string hallName, string imageUrl, string description, string phoneNumber, string adress, string town, string userId)
        {
            var gamingHall = new GamingHall();
            if (imageUrl != null)
            {
                gamingHall.HallName = hallName;
                gamingHall.ImageUrl = imageUrl;
                gamingHall.Description = description;
                gamingHall.PhoneNumber = phoneNumber;
                gamingHall.Adress = adress;
                gamingHall.Town = town;
                gamingHall.UserId = userId;
            }
            else
            {
                gamingHall.HallName = hallName;
                gamingHall.ImageUrl = GlobalConstants.DefaultLogo;
                gamingHall.Description = description;
                gamingHall.PhoneNumber = phoneNumber;
                gamingHall.Adress = adress;
                gamingHall.Town = town;
                gamingHall.UserId = userId;
            }

            await this.repository.AddAsync(gamingHall);
            await this.repository.SaveChangesAsync();
        }

        public IEnumerable<T> All<T>()
        {
            IQueryable<GamingHall> halls = this.repository.All().OrderBy(x => x.HallName);

            return halls.To<T>().ToList();
        }

        public IEnumerable<T> AllHalls<T>(string userId)
        {
            IQueryable<GamingHall> halls = this.repository.All().Where(x => x.UserId == userId).OrderBy(x => x.CreatedOn);

            return halls.To<T>().ToList();
        }

        public IEnumerable<T> AllOfChain<T>(string hallName)
        {
            IQueryable<GamingHall> halls = this.repository.All().Where(x => x.HallName == hallName).OrderBy(x => x.CreatedOn);

            return halls.To<T>().ToList();
        }

        public async Task DeleteAsync(string id)
        {
            var hall = this.repository.All().Where(x => x.Id == id).FirstOrDefault();
            this.repository.Delete(hall);
            await this.repository.SaveChangesAsync();
        }

        public T GetT<T>(string id)
        {
            var hall = this.repository.All().Where(x => x.Id == id).To<T>().FirstOrDefault();

            return hall;
        }

        public async Task UpdateAsync(string id, string hallName, string imageUrl, string description, string phoneNumber, string adress, string town)
        {
            var gamingHall = this.repository.All().Where(x => x.Id == id).FirstOrDefault();

            gamingHall.HallName = hallName;
            gamingHall.ImageUrl = imageUrl;
            gamingHall.Description = description;
            gamingHall.PhoneNumber = phoneNumber;
            gamingHall.Adress = adress;
            gamingHall.Town = town;

            await this.repository.SaveChangesAsync();
        }
    }
}
