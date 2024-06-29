using BullyWeb.Data;
using BullyWeb.Models;
using Microsoft.AspNetCore.Mvc;

namespace BullyWeb.Controllers
{
    public class CategoryController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CategoryController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            List<Category> objCategory = _context.Categories.ToList();

            return View(objCategory);
        }

        public IActionResult Create()
        {
            return View();
        }

        [HttpPost]
        public IActionResult Create(Category category)
        {
            if (category.Name == category.DisplayOrder.ToString())
            {
                ModelState.AddModelError("Name", "The Display Order connot exaclty match the Name");
            }

            if (ModelState.IsValid)
			{
				_context.Categories.Add(category);
				_context.SaveChanges();
				TempData["success"] = "Category created successfully";
				return RedirectToAction("Index");

			}

            return View();
           
        }

        [HttpGet]
        public IActionResult Edit(int? id)
        {
            if (id == null || id == 0) 
            {
                return NotFound();
            }
            Category? categoryFromDb = _context.Categories.Find(id);
            //Category categoryFromDb1 = _context.Categories.FirstOrDefault(category => category.Id == id);
            //Category categoryFromDb2 = _context.Categories.Where(category => category.Id == id).FirstOrDefault();

            if (categoryFromDb == null)
            {
                return NotFound();
            }
			 return View(categoryFromDb);
		}

		[HttpPost]
		public IActionResult Edit(Category category)
		{
			if (ModelState.IsValid)
			{
				_context.Categories.Update(category);
				_context.SaveChanges();
				TempData["success"] = "Category updated successfully";
				return RedirectToAction("Index");
			}

			return View();

		}

		[HttpGet]
		public IActionResult Delete(int? id)
		{
			if (id == null || id == 0)
			{
				return NotFound();
			}
			Category? categoryFromDb = _context.Categories.Find(id);
		
			if (categoryFromDb == null)
			{
				return NotFound();
			}
			return View(categoryFromDb);
		}

		[HttpPost, ActionName("Delete")]
		public IActionResult DeletePOST(int? id )
		{
			Category? obj = _context.Categories.Find(id);
			if (obj == null) 
			{
				return NotFound();
			}
			_context.Categories.Remove(obj);
			_context.SaveChanges();
			TempData["success"] = "Category deleted successfully";
			return RedirectToAction("Index");

		}

	}
}
