﻿using System;
using System.Globalization;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;

using Translation.Client.Web.Helpers;
using Translation.Client.Web.Helpers.ActionFilters;
using Translation.Client.Web.Helpers.Mappers;
using Translation.Client.Web.Models.Base;
using Translation.Client.Web.Models.Organization;
using Translation.Common.Contracts;
using Translation.Common.Helpers;
using Translation.Common.Models.Requests.Integration;
using Translation.Common.Models.Requests.Integration.Token;
using Translation.Common.Models.Requests.Journal;
using Translation.Common.Models.Requests.Organization;
using Translation.Common.Models.Requests.Project;
using Translation.Common.Models.Requests.User;
using Translation.Common.Models.Requests.User.LoginLog;
using Translation.Common.Models.Shared;

namespace Translation.Client.Web.Controllers
{
    public class OrganizationController : BaseController
    {
        private readonly IIntegrationService _integrationService;
        private readonly IProjectService _projectService;

        public OrganizationController(IIntegrationService integrationService,
                                      IProjectService projectService,
                                      IOrganizationService organizationService,
                                      IJournalService journalService) : base(organizationService, journalService)
        {
            _integrationService = integrationService;
            _projectService = projectService;
        }

        [HttpGet]
        public IActionResult Detail(Guid id)
        {
            var organizationUid = id;
            if (organizationUid.IsEmptyGuid())
            {
                organizationUid = CurrentUser.OrganizationUid;
            }

            var request = new OrganizationReadRequest(CurrentUser.Id, organizationUid);
            var response = OrganizationService.GetOrganization(request);
            if (response.Status.IsNotSuccess)
            {
                return RedirectToAccessDenied();
            }

            var model = OrganizationMapper.MapOrganizationDetailModel(response.Item);
            return View(model);
        }

        [HttpGet]
        public IActionResult Edit(Guid id)
        {
            var organizationUid = id;
            if (organizationUid.IsEmptyGuid())
            {
                organizationUid = CurrentUser.OrganizationUid;
            }

            var request = new OrganizationReadRequest(CurrentUser.Id, organizationUid);
            var response = OrganizationService.GetOrganization(request);
            if (response.Status.IsNotSuccess)
            {
                return RedirectToAccessDenied();
            }

            var model = OrganizationMapper.MapOrganizationEditModel(response.Item);
            return View(model);
        }

        [HttpPost,
         JournalFilter(Message = "journal_organization_edit")]
        public async Task<IActionResult> Edit(OrganizationEditModel model)
        {
            if (model.IsNotValid())
            {
                return View(model);
            }

            var request = new OrganizationEditRequest(CurrentUser.Id, model.OrganizationUid, model.Name, model.Description);

            var response = await OrganizationService.EditOrganization(request);
            if (response.Status.IsNotSuccess)
            {
                model.MapMessages(response);
                return View(model);
            }

            CurrentUser.IsActionSucceed = true;
            return Redirect($"/Organization/Detail/{model.OrganizationUid }");
        }

        [HttpGet]
        public IActionResult UserLoginLogList(Guid id)
        {
            var organizationUid = id;
            if (organizationUid.IsEmptyGuid())
            {
                organizationUid = CurrentUser.OrganizationUid;
            }

            var model = new OrganizationUserLoginLogListModel();
            model.OrganizationUid = organizationUid;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> UserLoginLogListData(Guid id, int skip, int take)
        {
            var organizationUid = id;
            if (organizationUid.IsEmptyGuid())
            {
                return Forbid();
            }

            var request = new OrganizationLoginLogReadListRequest(CurrentUser.Id, organizationUid);
            SetPaging(skip, take, request);

            var response = await OrganizationService.GetUserLoginLogsOfOrganization(request);
            if (response.Status.IsNotSuccess)
            {
                return NotFound();
            }

            var result = new DataResult();
            result.AddHeaders("user", "ip", "country", "city", "browser", "browser_version", "platform", "platform_version", "created_at");

            for (var i = 0; i < response.Items.Count; i++)
            {
                var item = response.Items[i];
                var stringBuilder = new StringBuilder();
                stringBuilder.Append($"{item.Uid}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{result.PrepareLink($"/User/Detail/{item.Uid}", item.UserName)}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{item.Ip}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{item.Country}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{item.City}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{item.Browser}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{item.BrowserVersion}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{item.Platform}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{item.PlatformVersion}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{GetDateTimeAsString(item.CreatedAt)}{DataResult.SEPARATOR}");

                result.Data.Add(stringBuilder.ToString());
            }

            result.PagingInfo = response.PagingInfo;
            result.PagingInfo.Type = PagingInfo.PAGE_NUMBERS;

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> UserListData(Guid id, int skip, int take)
        {
            var organizationUid = id;
            if (organizationUid.IsEmptyGuid())
            {
                return Forbid();
            }

            var request = new UserReadListRequest(CurrentUser.Id, organizationUid);
            SetPaging(skip, take, request);
            var response = await OrganizationService.GetUsers(request);
            if (response.Status.IsNotSuccess)
            {
                return NotFound();
            }

            var result = new DataResult();
            result.AddHeaders("user_name", "email", "invited_at", "last_logged_in_at", "is_active", "created_at");

            for (var i = 0; i < response.Items.Count; i++)
            {
                var item = response.Items[i];
                var stringBuilder = new StringBuilder();
                stringBuilder.Append($"{item.Uid}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{result.PrepareLink($"/User/Detail/{item.Uid}", item.Name)}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{item.Email}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{GetDateTimeAsString(item.InvitedAt)}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{GetDateTimeAsString(item.LastLoggedInAt)}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{item.IsActive}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{GetDateTimeAsString(item.CreatedAt)}{DataResult.SEPARATOR}");

                result.Data.Add(stringBuilder.ToString());
            }

            result.PagingInfo = response.PagingInfo;
            result.PagingInfo.Type = PagingInfo.PAGE_NUMBERS;

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> IntegrationListData(Guid id, int skip, int take)
        {
            var organizationUid = id;
            if (organizationUid.IsEmptyGuid())
            {
                return Forbid();
            }

            var request = new IntegrationReadListRequest(CurrentUser.Id, organizationUid);
            SetPaging(skip, take, request);

            var response = await _integrationService.GetIntegrations(request);
            if (response.Status.IsNotSuccess)
            {
                return NotFound();
            }

            var result = new DataResult();
            result.AddHeaders("integration_name", "is_active", "created_at");

            for (var i = 0; i < response.Items.Count; i++)
            {
                var item = response.Items[i];
                var stringBuilder = new StringBuilder();
                stringBuilder.Append($"{item.Uid}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{result.PrepareLink($"/Integration/Detail/{item.Uid}", item.Name, false)}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{item.IsActive}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{GetDateTimeAsString(item.CreatedAt)}{DataResult.SEPARATOR}");
                result.Data.Add(stringBuilder.ToString());
            }

            result.PagingInfo = response.PagingInfo;
            result.PagingInfo.Type = PagingInfo.PAGE_NUMBERS;

            return Json(result);
        }

        [HttpGet]
        public async Task<IActionResult> ProjectListData(Guid id, int skip, int take)
        {
            var organizationUid = id;
            if (organizationUid.IsEmptyGuid())
            {
                return Forbid();
            }

            var request = new ProjectReadListRequest(CurrentUser.Id, organizationUid);
            SetPaging(skip, take, request);

            var response = await _projectService.GetProjects(request);
            if (response.Status.IsNotSuccess)
            {
                return NotFound();
            }

            var result = new DataResult();
            result.AddHeaders("project_name", "url", "label_count", "is_active", "created_at");

            for (var i = 0; i < response.Items.Count; i++)
            {
                var item = response.Items[i];
                var stringBuilder = new StringBuilder();
                stringBuilder.Append($"{item.Uid}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{result.PrepareLink($"/Project/Detail/{item.Uid}", item.Name)}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{result.PrepareLink(item.Url)}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{item.LabelCount}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{item.IsActive}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{GetDateTimeAsString(item.CreatedAt)}{DataResult.SEPARATOR}");

                result.Data.Add(stringBuilder.ToString());
            }

            result.PagingInfo = response.PagingInfo;
            result.PagingInfo.Type = PagingInfo.PAGE_NUMBERS;

            return Json(result);
        }

        [HttpGet]
        public IActionResult TokenRequestLogList(Guid id)
        {
            var organizationUid = id;
            if (organizationUid.IsEmptyGuid())
            {
                organizationUid = CurrentUser.OrganizationUid;
            }

            var model = new OrganizationTokenRequestLogListModel();
            model.OrganizationUid = organizationUid;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> TokenRequestLogListData(Guid id, int skip, int take)
        {
            var organizationUid = id;
            if (organizationUid.IsEmptyGuid())
            {
                return Forbid();
            }

            var request = new OrganizationTokenRequestLogReadListRequest(CurrentUser.Id, organizationUid);
            SetPaging(skip, take, request);

            var response = await _integrationService.GetTokenRequestLogsOfOrganization(request);
            if (response.Status.IsNotSuccess)
            {
                return NotFound();
            }

            var result = new DataResult();
            result.AddHeaders("integration", "integration_client", "ip", "country", "city", "http_method", "response_code", "created_at");

            for (var i = 0; i < response.Items.Count; i++)
            {
                var item = response.Items[i];
                var stringBuilder = new StringBuilder();
                stringBuilder.Append($"{item.Uid}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{result.PrepareLink($"/Integration/Detail/{item.IntegrationUid}", item.IntegrationName)}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{item.IntegrationClientUid}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{item.Ip}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{item.Country}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{item.City}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{item.HttpMethod}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{item.ResponseCode}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{GetDateTimeAsString(item.CreatedAt)}{DataResult.SEPARATOR}");

                result.Data.Add(stringBuilder.ToString());
            }

            result.PagingInfo = response.PagingInfo;
            result.PagingInfo.Type = PagingInfo.PAGE_NUMBERS;

            return Json(result);
        }

        [HttpGet]
        public IActionResult JournalList(Guid id)
        {
            var organizationUid = id;
            if (organizationUid.IsEmptyGuid())
            {
                organizationUid = CurrentUser.OrganizationUid;
            }

            var model = new OrganizationJournalListModel();
            model.OrganizationUid = organizationUid;
            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> JournalListData(Guid id, int skip, int take)
        {
            var organizationUid = id;
            if (organizationUid.IsEmptyGuid())
            {
                organizationUid = CurrentUser.OrganizationUid;
            }

            var request = new OrganizationJournalReadListRequest(CurrentUser.Id, organizationUid);
            SetPaging(skip, take, request);

            var response = await JournalService.GetJournalsOfOrganization(request);
            if (response.Status.IsNotSuccess)
            {
                return NotFound();
            }

            var result = new DataResult();
            result.AddHeaders("user_name", "integration_name", "message", "created_at");

            for (var i = 0; i < response.Items.Count; i++)
            {
                var item = response.Items[i];
                var stringBuilder = new StringBuilder();
                stringBuilder.Append($"{item.Uid}{DataResult.SEPARATOR}");

                if (item.UserUid.IsNotEmptyGuid())
                {
                    stringBuilder.Append($"{result.PrepareLink($"/User/Detail/{item.UserUid}", item.UserName)}{DataResult.SEPARATOR}");
                }
                else
                {
                    stringBuilder.Append($"-{DataResult.SEPARATOR}");
                }

                if (item.IntegrationUid.IsNotEmptyGuid())
                {
                    stringBuilder.Append($"{result.PrepareLink($"/Integration/Detail/{item.IntegrationUid}", item.IntegrationName)}{DataResult.SEPARATOR}");
                }
                else
                {
                    stringBuilder.Append($"-{DataResult.SEPARATOR}");
                }

                stringBuilder.Append($"{item.Message}{DataResult.SEPARATOR}");
                stringBuilder.Append($"{GetDateTimeAsString(item.CreatedAt)}{DataResult.SEPARATOR}");

                result.Data.Add(stringBuilder.ToString());
            }

            result.PagingInfo = response.PagingInfo;
            result.PagingInfo.Type = PagingInfo.PAGE_NUMBERS;

            return Json(result);
        }
    }
}
