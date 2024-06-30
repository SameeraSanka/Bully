using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Microsoft.AspNetCore.Mvc;

namespace BulkyWeb.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class ProductController : Controller
    {
        private readonly IUnitOfWork _unitOfWork;
        public ProductController(IUnitOfWork unitOfWork)
        {
            _unitOfWork = unitOfWork;
        }

        public IActionResult Index()
        {
            List<Product> objProduct = _unitOfWork.Product.GetAll().ToList(); 
            
            return View(objProduct);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Product product)
        {
            if (ModelState.IsValid) 
            {
                _unitOfWork.Product.Add(product);
                _unitOfWork.Save();
				TempData["success"] = "Product created successfully";
                return RedirectToAction("Index");
			}

            return View();
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null) 
            { 
                return NotFound();  
            }
            Product? productFromDb = _unitOfWork.Product.Get(dataFromDb => dataFromDb.Id == id);
            if (productFromDb == null) 
            {
                return NotFound();
            }
            return View(productFromDb);
        }
		[HttpPost]
		public IActionResult Edit(Product product)
		{
			if (ModelState.IsValid)
			{
				_unitOfWork.Product.Update(product);
				_unitOfWork.Save();
				TempData["success"] = "Product Updated successfully";
				return RedirectToAction("Index");
			}

			return View();
		}

		[HttpGet]
		public IActionResult Delete(int? id)
		{
			if (id == null)
			{
				return NotFound();
			}
			Product? productFromDb = _unitOfWork.Product.Get(dataFromDb => dataFromDb.Id == id);
			if (productFromDb == null)
			{
				return NotFound();
			}
			return View(productFromDb);
		}
		[HttpPost, ActionName("Delete")]
		public IActionResult DeletePost(int? id)
		{
			Product? obj = _unitOfWork.Product.Get(dataFromDb => dataFromDb.Id == id);
			if (obj != null)
			{
				_unitOfWork.Product.Remove(obj);
				_unitOfWork.Save();
				TempData["success"] = "Product deleted successfully";
				return RedirectToAction("Index");
			}

			return NotFound();
		}


	}
}
