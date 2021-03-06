﻿namespace OnlineSlotReports.Web.Controllers
{
    using System.Security.Claims;
    using System.Threading.Tasks;
    using Microsoft.AspNetCore.Authorization;
    using Microsoft.AspNetCore.Mvc;
    using OnlineSlotReports.Services.Data.GamingHallServices;
    using OnlineSlotReports.Services.Data.MessageServices;
    using OnlineSlotReports.Services.Data.UsersHallsServices;
    using OnlineSlotReports.Web.ViewModels.GamingHallViewModels;
    using OnlineSlotReports.Web.ViewModels.MessageViewModels;

    public class MessageController : Controller
    {
        private readonly IMessageService massageService;
        private readonly IGamingHallService gamingHallService;
        private readonly IUsersHallsService usersHallsService;

        public MessageController(IMessageService massageService, IGamingHallService gamingHallService, IUsersHallsService usersHallsService)
        {
            this.massageService = massageService;
            this.gamingHallService = gamingHallService;
            this.usersHallsService = usersHallsService;
        }

        public IActionResult Create(string id)
        {
            var model = new InputMessageViewModel();
            model.GamingHallId = id;
            return this.View(model);
        }

        [HttpPost]
        public async Task<IActionResult> CreateAsync(string id, InputMessageViewModel input)
        {
            await this.massageService.AddAsync(input.Sender, input.Content, id);

            return this.Redirect("/GamingHall/Index/" + id);
        }

        [Authorize]
        public IActionResult All(string id)
        {
            var hall = this.gamingHallService.GetT<UserIdHallViewModel>(id);

            var userId = this.User.FindFirstValue(ClaimTypes.NameIdentifier);

            var exist = this.usersHallsService.IfExist(id, userId);

            if (hall == null || !exist)
            {
                return this.View("NotFound");
            }

            var newMessages = this.massageService.All<IndexMessageViewModel>(id);
            var readMessages = this.massageService.GetAllReadById<IndexMessageViewModel>(id);

            var allMessages = new AllMessageViewModel();
            allMessages.NewMessages = newMessages;
            allMessages.ReadMessages = readMessages;
            return this.View(allMessages);
        }

        [Authorize]
        public async Task<IActionResult> Index(string id)
        {
            var message = await this.massageService.GetByIdAsync<IndexMessageViewModel>(id);
            if (message == null)
            {
                return this.View("NotFound");
            }

            return this.View(message);
        }

        public async Task<IActionResult> Delete(string id)
        {
            var hallId = this.massageService.GetHallId(id);

            await this.massageService.DeleteAsync(id);

            return this.Redirect("/Message/All/" + hallId);
        }
    }
}
