﻿using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Translation.Client.Web.Helpers;
using Translation.Client.Web.Models;
using Translation.Client.Web.Models.InputModels;
using Translation.Common.Helpers;

namespace Translation.Client.Web.Controllers
{
    public class ProjectController : BaseController
    {
        [HttpGet]
        public IActionResult Create()
        {
            var model = new ProjectCreateModel();
            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> Create(ProjectCreateModel model)
        {
            if (!model.Validate())
            {
                return View(model);
            }

            //todo: map request and post

            var uid = "";
            return RedirectToAction("Detail", "Project", uid);

            return View();
        }

        [HttpGet]
        public IActionResult Detail(string id)
        {
            if (id.IsNotUid())
            {
                return RedirectToAccessDenied();
            }

            return View();
        }

        [HttpGet]
        public JsonResult Items()
        {
            return Json(null);
        }
    }
}
