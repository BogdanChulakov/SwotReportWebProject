﻿namespace OnlineSlotReports.Services.Data.EmployeesServices
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Text;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Identity;
    using OnlineSlotReports.Data.Common.Repositories;
    using OnlineSlotReports.Data.Models;
    using OnlineSlotReports.Services.Mapping;

    public class EmployeesService : IEmployeesService
    {
        private readonly IDeletableEntityRepository<Employee> repository;

        public EmployeesService(IDeletableEntityRepository<Employee> repository)
        {
            this.repository = repository;
        }

        public async Task AddAsync(string fullName, string email, string phonenumber, DateTime startWorkDate, string gamingHallId)
        {
            var employee = new Employee
            {
                FullName = fullName,
                Email = email,
                PhoneNumber = phonenumber,
                StartWorkDate = startWorkDate,
                GamingHallId = gamingHallId,
            };

            await this.repository.AddAsync(employee);
            await this.repository.SaveChangesAsync();
        }

        public IEnumerable<T> All<T>(string id)
        {
            IQueryable<Employee> employees = this.repository.All().Where(x => x.GamingHallId == id).OrderBy(x => x.StartWorkDate);

            return employees.To<T>().ToList();
        }

        public async Task ChangeEmailAsync(string id, string email)
        {
            var employee = await this.repository.GetByIdWithDeletedAsync(id);

            employee.Email = email;

            await this.repository.SaveChangesAsync();
        }

        public async Task ChangePhoneNumberAsync(string id, string phoneNumber)
        {
            var employee = await this.repository.GetByIdWithDeletedAsync(id);

            employee.PhoneNumber = phoneNumber;

            await this.repository.SaveChangesAsync();;
        }

        public async Task DeleteAsync(string id)
        {
            var employee = await this.repository.GetByIdWithDeletedAsync(id);

            this.repository.Delete(employee);

            await this.repository.SaveChangesAsync();
        }

        public T GetById<T>(string id)
        {
            var employee = this.repository.All().Where(x => x.Id == id).To<T>().FirstOrDefault();

            return employee;
        }

        public string GetHallId(string id)
        {
            var employee = this.repository.All().Where(x => x.Id == id).FirstOrDefault();

            return employee.GamingHallId;
        }
    }
}
