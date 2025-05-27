using AutoMapper;
using Insurancesys.web.Models;
using Insurancesys.web.Models.Common;
using InsuranceSys.Application;
using InsuranceSys.Application.Interface;
using InsuranceSys.Domain;
using InsuranceSys.Domain.DTO;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Primitives;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Data;
using System.Diagnostics.Metrics;

namespace Insurancesys.web.Controllers
{
	//[Authorize(AuthenticationSchemes = CookieAuthenticationDefaults.AuthenticationScheme)] // Use cookie authentication
	public class LeadController : Controller
	{
		private readonly ILeadService _leadService;
		private readonly IMapper _mapper;
		public LeadController(
			 ILeadService leadService,
			 IMapper mapper)
		{
			_leadService = leadService;
			_mapper = mapper;
		}
		[Route("Leads")]
		public IActionResult ListOfLeads()
		{
			return View("../Customer/ListOfLeads");
		}

		[HttpPost]
		public async Task<IActionResult> GetData(DataTableRequest param)
		{			
			int page = param.iDisplayStart;
			int pagesize = param.iDisplayLength;

			#region sorting

			StringValues sortDirValues = Request.Form["sSortDir_0"];
			string sortDirection = sortDirValues.Count > 0 ? sortDirValues[0].ToLower() : "desc";

			string sortingField = Request.Form["SortingField"];
			string SortExp = !string.IsNullOrWhiteSpace(sortingField) ? sortingField : "1";
			SortExp += (sortDirection == "asc") ? " asc" : " desc";
			#endregion
			string searchText = Request.Form["searchText"];
			string searchTerm = !string.IsNullOrWhiteSpace(searchText)? searchText:"";
			

			var @params = ImmutableDictionary<string, object>.Empty
			.Add("@PageNumber", page)
			.Add("@PageSize", pagesize)
			.Add("@SearchTerm", searchTerm)
			.Add("@SortExp", SortExp);

			try
			{
				using (DataSet ds = await _leadService.GetAllLeads(@params))
				{
					if (ds != null && ds.Tables.Count > 0)
					{
						int totalRecords = 1;
						int.TryParse(Convert.ToString(ds.Tables[0].Rows[0]["TotalRecords"]), out totalRecords);
						using (DataTable dtContent = ds.Tables[1])
						{
							if (dtContent != null && dtContent.Rows.Count > 0)
							{
								var response = dtContent.AsEnumerable()
								.Select(row => dtContent.Columns.Cast<DataColumn>()
								.ToDictionary(col => col.ColumnName, col => row[col])).ToList();

								return Json(new
								{
									iTotalRecords = totalRecords,
									iTotalDisplayRecords = totalRecords,
									data = response
								});
							}
							else
							{
								// Handle the case when no rows are returned
								return Json(new
								{
									iTotalRecords = 0,
									iTotalDisplayRecords = 0,
									data = new List<object>() // Empty list for no data
								});
							}
						}
					}
					else
					{
						// Handle the case when no rows are returned
						return Json(new
						{
							iTotalRecords = 0,
							iTotalDisplayRecords = 0,
							data = new List<object>() // Empty list for no data
						});
					}
				}
			}
			catch (Exception ex)
			{
				throw;
			}
		}

		[HttpGet("Leads/Edit/{id}")]
		[HttpGet("Leads/Add")]
		public async Task<ActionResult> AddEditLeads(string id = "")
		{
			LeadViewModel model = new();
			model = BindDropdowns(model);
			if (!string.IsNullOrEmpty(id))
			{
				if (int.TryParse(id, out int _id) && _id > 0)
				{
					var leaddto = await _leadService.GetLeadByIdAsync(Convert.ToInt16(_id));
					if (leaddto != null)
					{
						model = _mapper.Map<LeadViewModel>(leaddto);
						//HttpContext.Session.SetString("Original_CountryName", model.CountryName);
						model.IsEditMode = true;
					}
					else
					{
						//SetTempDataForNoRecord();
						//HttpContext.Session.SetString("Original_CountryName", "");
						return RedirectToAction("ListOfLeads");
					}
				}
				else
				{
					//SetTempDataForNoRecord();
					//HttpContext.Session.SetString("Original_CountryName", "");
					return RedirectToAction("ListOfLeads");
				}
			}
			else
			{
				model.IsEditMode = false;
				model.IsActive = true;
				//HttpContext.Session.SetString("Original_CountryName", "");
			}
			return View("../Customer/AddEditLeads", model);
		}

		[HttpPost]
		public async Task<IActionResult> SaveLead(LeadViewModel model, bool saveAndExit = true)
		{
			if (!ModelState.IsValid)
			{
				model = BindDropdowns(model);
				var errors = ModelState.Where(x => x.Value.Errors.Count > 0).Select(x => new { x.Key, x.Value.Errors }).ToArray();
				foreach (var item in errors)
				{
					ModelState.AddModelError(item.Key, item.Errors.Select(s => s.ErrorMessage).ToString());
				}
				return View("../Customer/AddEditLeads", model);
			}
			var lead = _mapper.Map<LeadDto>(model);
			if (model.IsEditMode)
			{
				#region Update
				int rowsaffected = await _leadService.UpdateLead(lead);
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
				int rowsaffected = await _leadService.AddLead(lead);
				TempData["RowsAffected"] = rowsaffected;
				if (rowsaffected > 0)
					TempData["Message"] = Constants.SuccessMessages.MsgInsertSuccess;
				else
					TempData["Message"] = Constants.ErrorMessages.MsgInsertFailure;
				#endregion
			}
			if (saveAndExit)
			{
				return RedirectToAction("ListOfLeads");
			}
			else if (model.IsEditMode)
			{
				return RedirectToAction("AddEditLead", new RouteValueDictionary(new { id = model.LeadID }));
			}
			else
			{
				return RedirectToAction("AddEditLead");
			}
		}

		#region Helper methods
		private LeadViewModel BindDropdowns(LeadViewModel model)
		{
			model.lstCompanies = GetCompanyDropdown();
			model.lstInsuranceType = GetInsuranceTypeDropdown();
			model.lstUsers = GetUserDropdown();
			model.lstLeadStatus = GetLeadStatusDropdown();
			return model;
		}
		private List<SelectListItem> GetCompanyDropdown()
		{
			return new List<SelectListItem>
			{
				new() { Text = "Bajaj Allienz", Value = "1" },
				new() { Text = "Care Insurance", Value = "2" },
				new() { Text = "Edelwiess", Value = "3" }
			};
		}

		private List<SelectListItem> GetInsuranceTypeDropdown()
		{
			return new List<SelectListItem>
			{
				new() { Text = "Health Insurance", Value = "1" },
				new() { Text = "Motor Insurance", Value = "2" },
				new() { Text = "General Insurance", Value = "3" }
			};
		}

		private List<SelectListItem> GetUserDropdown()
		{
			return new List<SelectListItem>
			{
				new() { Text = "Default User", Value = "1", Selected = true },
				new() { Text = "User 2", Value = "2" }
			};
		}

		private List<SelectListItem> GetLeadStatusDropdown()
		{
			return new List<SelectListItem>
			{
				new() { Text = "Contacted", Value = "1", Selected = true },
				new() { Text = "Qualified", Value = "2" },
				new() { Text = "Proposal Sent", Value = "3" },
				new() { Text = "Negotiation", Value = "4" },
				new() { Text = "Closed/Won", Value = "5" },
				new() { Text = "Closed/Lost", Value = "6" }
			};
		}
		#endregion
	}
}
