using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
		private readonly IWebHostEnvironment _webHostEnvironment; //image save krnna ganne meka. meka nisa  wwwroot folder eka access krnna puluwan
        public ProductController(IUnitOfWork unitOfWork, IWebHostEnvironment webHostEnvironment)
        {
            _unitOfWork = unitOfWork;
			_webHostEnvironment = webHostEnvironment;
        }

        public IActionResult Index()
        {
            List<Product> objProduct = _unitOfWork.Product.GetAll(includeProperties:"Category").ToList();

            return View(objProduct);
        }

		//view eka pennanaa eka. 
		// dan mekedi  wenne update viwe ekai create view ekai dekama ekak krna eka. parameter ekata "int?"
		//ywanne id ekak thiynnth puluwan nthiwennth puluwan kiynna
        public IActionResult Upsert(int? id)
        {
			IEnumerable<SelectListItem> CategoryList = _unitOfWork.Category.GetAll().Select(category => new SelectListItem
			{
				Text = category.Name,
				Value = category.Id.ToString()
			});

			ProductVM productVM = new()
			{
				Product = new Product(),
				CategoryList = CategoryList
			};
			// meka create ekata view eka open wenwa id ekak enne naththan
			if (id == null || id == 0)
			{
				return View(productVM);
			}
			else //id ekak awoth meka wda krnwa update ekata
			{
				productVM.Product = _unitOfWork.Product.Get(product => product.Id == id);
				return View(productVM);
			}
			

			// uda liyala thiyna ela mehema liynnath puluwan
			//ProductVM productVM = new()
			//{
			//	Product = new Product(),
			//	CategoryList = _unitOfWork.Category.GetAll().Select(category => new SelectListItem
			//	{
			//		Text = category.Name,
			//		Value = category.Id.ToString()
			//	})
			//};
			//return View(productVM);

		}

		//function eka. meke IFormFile kiyla dnne image ekak uplade krna nisa eka gnne meken
		[HttpPost]
		public IActionResult Upsert(ProductVM productVMData, IFormFile? file)
		{
			if (ModelState.IsValid)
			{
				try
				{
					string wwwRootPath = _webHostEnvironment.WebRootPath;
					if(file != null)
					{						//image name eka hadanwa   // methana file extenton eka add krnwa
						string fileName = Guid.NewGuid().ToString() + Path.GetExtension(file.FileName);	
						string productPath = Path.Combine(wwwRootPath, @"Images\Product"); // save wenna one path eka hadanwa

						//update kranakota meka balanne
						if(!string.IsNullOrEmpty(productVMData.Product.ImageUrl))
						{
							//delete the old image
							var oldImagePath = Path.Combine(wwwRootPath,productVMData.Product.ImageUrl.TrimStart('\\'));

							if (System.IO.File.Exists(oldImagePath))
							{
								System.IO.File.Delete(oldImagePath);
							}
						}

						using (var fileStream = new FileStream(Path.Combine(productPath, fileName),FileMode.Create))
						{
							file.CopyTo(fileStream);
						}
						productVMData.Product.ImageUrl = @"\Images\Product\" + fileName;
					}

					if(productVMData.Product.Id == 0)
					{
						_unitOfWork.Product.Add(productVMData.Product);
					}
					else
					{
						_unitOfWork.Product.Update(productVMData.Product);	
					}

					_unitOfWork.Save();
					TempData["success"] = "Product created successfully";
					return RedirectToAction("Index");
				}
				catch (Exception ex)
				{
					// Log the exception (ex) as needed
					TempData["error"] = "An error occurred while creating the product. Please try again.";
				}
			}

			productVMData.CategoryList = _unitOfWork.Category.GetAll().Select(category => new SelectListItem
			{
				Text = category.Name,
				Value = category.Id.ToString()
			});
			return View(productVMData);
		}


		//[HttpGet]
		//public IActionResult Delete(int? id)
		//{
		//	if (id == null)
		//	{
		//		return NotFound();
		//	}
		//	Product? productFromDb = _unitOfWork.Product.Get(dataFromDb => dataFromDb.Id == id);
		//	if (productFromDb == null)
		//	{
		//		return NotFound();
		//	}
		//	return View(productFromDb);
		//}
		//[HttpPost, ActionName("Delete")]
		//public IActionResult DeletePost(int? id)
		//{
		//	Product? obj = _unitOfWork.Product.Get(dataFromDb => dataFromDb.Id == id);
		//	if (obj != null)
		//	{
		//		_unitOfWork.Product.Remove(obj);
		//		_unitOfWork.Save();
		//		TempData["success"] = "Product deleted successfully";
		//		return RedirectToAction("Index");
		//	}

		//	return NotFound();
		//}

		#region API CALLS

		[HttpGet]
		public IActionResult GetAll()
		{
            List<Product> objProduct = _unitOfWork.Product.GetAll(includeProperties: "Category").ToList();
			return Json(new { data = objProduct });
		}


		[HttpDelete]
        public IActionResult Delete(int? id)
        {
			var productToBeDeleted = _unitOfWork.Product.Get(product => product.Id == id);
			if (productToBeDeleted == null)
			{
				return Json(new {success = false, message = "Error while deleting"});
			}
            var oldImagePath = Path.Combine(_webHostEnvironment.WebRootPath, productToBeDeleted.ImageUrl.TrimStart('\\'));

            if (System.IO.File.Exists(oldImagePath))
            {
                System.IO.File.Delete(oldImagePath);
            }

			_unitOfWork.Product.Remove(productToBeDeleted);
			_unitOfWork.Save();

            return Json(new { success = true, message = "Deleted Successfully" });
        }

        #endregion

    }
}
