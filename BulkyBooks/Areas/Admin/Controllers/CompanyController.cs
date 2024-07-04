using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace BulkyBooks.Areas.Admin.Controllers
{
	[Area("Admin")]
	[Authorize(Roles = SD.Role_Admin)]
	public class CompanyController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public CompanyController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
			List<Company> Companies = _unitOfWork.Company.GetAll().ToList();
			return View(Companies);
        }

        //meka data table walata ywana nisa jason wlain ywnne
        [HttpGet]
        public IActionResult GetAll()
        {
            List<Company> Companies = _unitOfWork.Company.GetAll().ToList();
            return Json(new {data= Companies});
        }

        public IActionResult Upsert(int? id)
        {
            CompanyVM companyVM = new CompanyVM()
            {
                Company = new Company()
            };
            if (id == null)
            {
                return View(companyVM);
            }
            else
            {
                companyVM.Company = _unitOfWork.Company.Get(company =>company.Id == id);
                return View(companyVM);
            }

        }
        [HttpPost]
        public IActionResult Upsert(CompanyVM companyVM)
        {
            if(ModelState.IsValid)
            {
                try
                {
                    if(companyVM.Company.Id == 0)
                    {
                        _unitOfWork.Company.Add(companyVM.Company);
                    }
                    else
                    {
                        _unitOfWork.Company.update(companyVM.Company);
                    }

                    _unitOfWork.Save();
					TempData["success"] = "Company created successfully";
                    return RedirectToAction("Index");
				}catch (Exception ex)
                {
					TempData["error"] = "An error occurred while creating the Company. Please try again.";
				}
            }
            return View(companyVM);
        }

        [HttpDelete]
        public IActionResult Delete(int? id)
        {
            var companyDataDB = _unitOfWork.Company.Get(company =>  company.Id == id);
            if (companyDataDB == null)
            {
				return Json(new { success = false, message = "Error while deleting" });
			}
            _unitOfWork.Company.Remove(companyDataDB);
            _unitOfWork.Save();

			return Json(new { success = true, message = "Deleted Successfully" });
		}
    }

}
