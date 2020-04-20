﻿namespace OnlineSlotReports.Web.Controllers
{
    using System;
    using System.Security.Claims;
    using System.Threading.Tasks;

    using CloudinaryDotNet;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Http;
    using Microsoft.AspNetCore.Identity;
    using Microsoft.AspNetCore.Mvc;
    using OnlineSlotReports.Common;
    using OnlineSlotReports.Data.Models;
    using OnlineSlotReports.Services.Data.GamingHallServices;
    using OnlineSlotReports.Web.CloudinaryHelper;
    using OnlineSlotReports.Web.ViewModels.GamingHallViewModels;

    [Authorize]
    public class GamingHallController : Controller
    {
        private const int ItemPerPage = GlobalConstants.ItemPerPage;

        private readonly IGamingHallService service;
        private readonly UserManager<ApplicationUser> userManager;
        private readonly Cloudinary cloudinary;

        public GamingHallController(IGamingHallService service, UserManager<ApplicationUser> userManager, Cloudinary cloudinary)
        {
            this.service = service;
            this.userManager = userManager;
            this.cloudinary = cloudinary;
        }

        public IActionResult Add()
        {
            return this.View();
        }

        [HttpPost]
        public async Task<IActionResult> Add(InputGamingHallViewModel input, IFormFile file)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            if (file != null)
            {
                input.ImageUrl = await CloudinaryExtension.UploadAsync(this.cloudinary, file);
            }

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            await this.service.AddAsync(input.HallName, input.ImageUrl, input.Description, input.PhoneNumber, input.Adress, input.Town, userId);
            this.TempData["Message"] = "Gaming hall was aded successfully!";
            return this.Redirect("/GamingHall/Halls");
        }

        public async Task<IActionResult> Halls()
        {
            var user = await this.userManager.GetUserAsync(this.User);

            AllHallsViewModel allHallsViewModel = new AllHallsViewModel
            {
                GamingHalls = this.service.AllHalls<GamingHallViewModel>(user.Id),
            };

            return this.View(allHallsViewModel);
        }

        public async Task<IActionResult> Delete(string id)
        {
            await this.service.DeleteAsync(id);

            this.TempData["Message"] = "Gaming hall was deleted successfully!";
            return this.Redirect("/GamingHall/Halls");
        }

        public IActionResult Update(string id)
        {
            var hall = this.service.GetT<UserIdHallViewModel>(id);
            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);
            if (hall == null || userId != hall.UserId)
            {
                return this.NotFound();
            }

            var datailsViewModel = this.service.GetT<DetailsViewModel>(id);
            return this.View(datailsViewModel);
        }

        [HttpPost]
        public async Task<IActionResult> Update(DetailsViewModel input, IFormFile file)
        {
            if (!this.ModelState.IsValid)
            {
                return this.View(input);
            }

            if (file != null)
            {
                input.ImageUrl = await CloudinaryExtension.UploadAsync(this.cloudinary, file);
            }

            await this.service.UpdateAsync(input.Id, input.HallName, input.ImageUrl, input.Description, input.PhoneNumber, input.Adress, input.Town);
            this.TempData["Message"] = "Gaming hall was edited successfully!";
            return this.Redirect("/GamingHall/Halls");
        }

        public IActionResult AddElements([FromRoute] string id)
        {
            var hallView = new AddElementsViewModel
            {
                Id = id,
            };

            return this.View(hallView);
        }

        [AllowAnonymous]
        [HttpGet("/GamingHall/Search/")]
        public IActionResult Search(string name)
        {
            var searchModel = new SearchHallsViewModel();
            var halls = this.service.Search<GamingHallsIndexViewModel>(name);
            searchModel.GamingHalls = halls;
            if (name != null)
            {
               this.TempData["message"] = "No result for this serach!";
            }

            return this.View(searchModel);
        }

        [AllowAnonymous]
        public IActionResult All(int page = 1)
        {
            AllIndexHallViewModel allHallsViewModel = new AllIndexHallViewModel
            {
                GamingHalls = this.service.All<GamingHallsIndexViewModel>(ItemPerPage, (page - 1) * ItemPerPage),
            };

            var count = this.service.GetHallsCount();
            allHallsViewModel.PagesCount = (int)Math.Ceiling((double)count / ItemPerPage);
            allHallsViewModel.CurentPage = page;
            return this.View(allHallsViewModel);
        }

        [AllowAnonymous]
        public IActionResult Index([FromRoute] string id)
        {
            var model = this.service.GetT<IndexGamingHallViewModel>(id);
            if (model == null)
            {
                return this.NotFound();
            }

            return this.View(model);
        }

        [AllowAnonymous]
        public IActionResult Details([FromRoute]string id)
        {
            var datailsViewModel = this.service.GetT<DetailsViewModel>(id);
            if (datailsViewModel == null)
            {
                return this.NotFound();
            }

            return this.View(datailsViewModel);
        }
    }
}
