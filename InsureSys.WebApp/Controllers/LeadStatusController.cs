using AutoMapper;
using Insurancesys.web.Helper;
using Insurancesys.web.Models;
using Insurancesys.web.Models.Common;
using InsuranceSys.Application;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain;
using InsuranceSys.Domain.Entities;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Insurancesys.web.Controllers
{
    [Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)] // Use cookie authentication
    public class LeadStatusController : Controller
    {
        private readonly IMapper _mapper;
        private readonly ILeadStatusService _leadStatusService;
        public LeadStatusController(ILeadStatusService leadStatusService, IMapper mapper)
        {
            _leadStatusService = leadStatusService;
            _mapper = mapper;
        }
        [Route("LeadStatus")]
        public IActionResult LeadStatusList()
        {
            return View("../Masters/LeadStatus/LeadStatusList");
        }
        [HttpPost]
        public async Task<IActionResult> GetData()
        {
            var result = await DataTableHelper.BuildGridResponseAsync(Request, _leadStatusService.GetAllAsync);
            return Json(result);
        }
        [HttpGet("LeadStatus/Add")]
        [HttpGet("LeadStatus/Edit/{id}")]
        public async Task<IActionResult> AddEditLeadStatus(string id = "")
        {
            LeadStatusViewModel model = new();
            if (!string.IsNullOrEmpty(id))
            {
                if (int.TryParse(id, out int _id) && _id > 0)
                {
                    var leadStatusdto = await _leadStatusService.GetByIdAsync(Convert.ToInt16(_id));
                    if (leadStatusdto != null)
                    {
                        model = _mapper.Map<LeadStatusViewModel>(leadStatusdto);
                        HttpContext.Session.SetString("Original_LeadStatus", model.LeadStatus);
                        model.IsEditMode = true;
                    }
                    else
                    {
                        SetTempDataForNoRecord();
                        HttpContext.Session.SetString("Original_LeadStatus", "");
                        return RedirectToAction("LeadStatusList");
                    }
                }
                else
                {
                    SetTempDataForNoRecord();
                    HttpContext.Session.SetString("Original_LeadStatus", "");
                    return RedirectToAction("LeadStatusList");
                }
            }
            else
            {
                model.IsEditMode = false;
                model.IsActive = true;
                HttpContext.Session.SetString("Original_LeadStatus", "");
            }
            return View("../Masters/LeadStatus/AddEditLeadStatus", model);
        }

        [HttpPost]
        public async Task<IActionResult> SaveLeadStatus(LeadStatusViewModel model, bool saveAndExit = true)
        {
            if (!ModelState.IsValid)
            {
                // Extract all model state errors
                var errors = ModelState
                    .Where(x => x.Value?.Errors?.Count > 0)
                    .SelectMany(x => x.Value!.Errors
                        .Select(error => new { Key = x.Key, ErrorMessage = error.ErrorMessage }))
                    .ToList();
                foreach (var error in errors)
                {
                    ModelState.AddModelError(error.Key, error.ErrorMessage);
                }
                return View("../Masters/LeadStatus/AddEditLeadStatus", model);
            }
            var leadStatus = _mapper.Map<LeadStatusEntity>(model);

            if (model.IsEditMode)
            {
                #region Update
                var rowsaffected = await _leadStatusService.UpdateAsync(leadStatus);
                TempData["RowsAffected"] = rowsaffected;
                if (rowsaffected > 0)
                    TempData["Message"] = Constants.SuccessMessages.MsgUpdateSuccess;
                else
                    TempData["Message"] = Constants.ErrorMessages.MsgUpdateFailure;
                #endregion
            }
            else
            {
                #region Insert
                var rowsaffected = await _leadStatusService.AddAsync(leadStatus);
                TempData["RowsAffected"] = rowsaffected;
                if (rowsaffected > 0)
                    TempData["Message"] = Constants.SuccessMessages.MsgInsertSuccess;
                else
                    TempData["Message"] = Constants.ErrorMessages.MsgInsertFailure;
                #endregion
            }
            if (saveAndExit)
            {
                return RedirectToAction("LeadStatusList");
            }
            else if (model.IsEditMode)
            {
                return RedirectToAction("AddEditLeadStatus", new RouteValueDictionary(new { id = model.LeadStatusID }));
            }
            else
            {
                return RedirectToAction("AddEditLeadStatus");
            }
        }

        [HttpDelete]
        public async Task<IActionResult> Delete(int id)
        {
            bool status = Convert.ToBoolean(await _leadStatusService.DeleteAsync(id));
            return new JsonResult(status);
        }
        [AcceptVerbs("Get", "Post")]
        public async Task<IActionResult> IsLeadStatusExist(string LeadStatus = "")
        {
            string Original_LeadStatus = HttpContext.Session.GetString("Original_LeadStatus") ?? "";
            bool IsEditMode = !string.IsNullOrEmpty(Original_LeadStatus);
            bool IsExist = await _leadStatusService.FindByNameAsync(LeadStatus);
            if (IsEditMode && !string.Equals(Original_LeadStatus, LeadStatus) && IsExist)
                return Json($"Lead Status '{LeadStatus}' is already in use.");
            else if (!IsEditMode && IsExist)
                return Json($"Lead Status '{LeadStatus}' is already in use.");
            return Json(true);
        }
        #region Helper methods
        private void SetTempDataForNoRecord()
        {
            TempData["RowsAffected"] = 0;
            TempData["Message"] = Constants.AlertMessages.MsgNoRecords;
        }
        #endregion
    }
}
