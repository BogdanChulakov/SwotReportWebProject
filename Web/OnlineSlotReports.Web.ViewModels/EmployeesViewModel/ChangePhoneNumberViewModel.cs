﻿namespace OnlineSlotReports.Web.ViewModels.EmployeesViewModel
{
    using System.ComponentModel.DataAnnotations;

    using OnlineSlotReports.Data.Models;
    using OnlineSlotReports.Services.Mapping;

    public class ChangePhoneNumberViewModel : IMapFrom<Employee>
    {
        [Required]
        [RegularExpression("^[0-9]+$")]
        public string PhoneNumber { get; set; }
    }
}
