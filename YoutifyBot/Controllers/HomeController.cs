﻿using Microsoft.AspNetCore.Mvc;
using YoutifyBot.Models.Repository;

namespace YoutifyBot.Controllers
{
    public class HomeController : Controller
    {
        IUnitOfWork unitOfWork;

        public HomeController(IUnitOfWork unitOfWork)
        {
            this.unitOfWork = unitOfWork;
        }

        public async Task<IActionResult> Index()
        {
            return View();
        }
    }
}