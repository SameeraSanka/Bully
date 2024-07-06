using Bulky.DataAccess.Repository.IRepository;
using Bulky.Models;
using Bulky.Models.ViewModels;
using Bulky.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace BulkyBooks.Areas.Customer.Controllers
{
	[Area("customer")]
	[Authorize]
	public class CartController : Controller
	{
		private readonly IUnitOfWork _unitOfWork;
		[BindProperty]
		public ShoppingCartVM ShoppingCartVM { get; set; } // methana BindProperty dammama POST action eke wenama parameter pass krnna one na

		public CartController(IUnitOfWork unitOfWork)
		{
			_unitOfWork = unitOfWork;
		}
		public IActionResult Index()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM = new()
			{
				shoppingCartsList = _unitOfWork.ShoppingCart.GetAll(
				shoppingItems => shoppingItems.ApplicationUserId == userId,
				includeProperties: "Product"),
				orderHeader=new()
			};
			foreach (var cart in ShoppingCartVM.shoppingCartsList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.orderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			return View(ShoppingCartVM);
		}

		public IActionResult Plus(int cartId)
		{
			var cartFromDB = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
			cartFromDB.Count += 1;
			_unitOfWork.ShoppingCart.Update(cartFromDB);
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Minus(int cartId)
		{
			var cartFromDB = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
			if (cartFromDB.Count <= 1)
			{
				//remove
				_unitOfWork.ShoppingCart.Remove(cartFromDB);
			}
			else
			{
				cartFromDB.Count -= 1;
				_unitOfWork.ShoppingCart.Update(cartFromDB);
			}
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Remove(int cartId)
		{
			var cartFromDB = _unitOfWork.ShoppingCart.Get(u => u.Id == cartId);
			_unitOfWork.ShoppingCart.Remove(cartFromDB);
			_unitOfWork.Save();
			return RedirectToAction(nameof(Index));
		}

		public IActionResult Summery()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM = new ShoppingCartVM()
			{
				shoppingCartsList = _unitOfWork.ShoppingCart.GetAll(
				shoppingItems => shoppingItems.ApplicationUserId == userId,
				includeProperties: "Product"),
				orderHeader = new()
			};

			ShoppingCartVM.orderHeader.ApplicationUser = _unitOfWork.ApplicationUser.Get(u => u.Id == userId);

			ShoppingCartVM.orderHeader.Name = ShoppingCartVM.orderHeader.ApplicationUser.FirstName;
			ShoppingCartVM.orderHeader.PhoneNumber = ShoppingCartVM.orderHeader.ApplicationUser.PhoneNumber;
			ShoppingCartVM.orderHeader.StreetAddress = ShoppingCartVM.orderHeader.ApplicationUser.StreetAddress;
			ShoppingCartVM.orderHeader.City = ShoppingCartVM.orderHeader.ApplicationUser.City;
			ShoppingCartVM.orderHeader.State = ShoppingCartVM.orderHeader.ApplicationUser.State;
			ShoppingCartVM.orderHeader.PostalCode = ShoppingCartVM.orderHeader.ApplicationUser.PostalCode;

			foreach (var cart in ShoppingCartVM.shoppingCartsList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.orderHeader.OrderTotal += (cart.Price * cart.Count);
			}
			return View(ShoppingCartVM);
		}

		[HttpPost]
		[ActionName("Summery")]
		public IActionResult SummeryPOST()
		{
			var claimsIdentity = (ClaimsIdentity)User.Identity;
			var userId = claimsIdentity.FindFirst(ClaimTypes.NameIdentifier).Value;

			ShoppingCartVM.shoppingCartsList = _unitOfWork.ShoppingCart.GetAll(
				shoppingItems => shoppingItems.ApplicationUserId == userId,
				includeProperties: "Product");

			ShoppingCartVM.orderHeader.OrderDate = System.DateTime.Now;
			ShoppingCartVM.orderHeader.ApplicationUserId = userId;

			foreach (var cart in ShoppingCartVM.shoppingCartsList)
			{
				cart.Price = GetPriceBasedOnQuantity(cart);
				ShoppingCartVM.orderHeader.OrderTotal += (cart.Price * cart.Count);
			}

			if (ShoppingCartVM.orderHeader.ApplicationUser.CompanyId.GetValueOrDefault() == 0)
			{
				//normal customer
				ShoppingCartVM.orderHeader.PaymentStatus = SD.PaymentStatusPending;
				ShoppingCartVM.orderHeader.OrderStatus = SD.StatusPending;
			}
			else
			{
				//gose to company User
				ShoppingCartVM.orderHeader.PaymentStatus = SD.PaymentStatusDelayedPayment;
				ShoppingCartVM.orderHeader.OrderStatus = SD.StatusApproved;
			}

			_unitOfWork.OrderHeader.Add(ShoppingCartVM.orderHeader);
			_unitOfWork.Save();

			//save order details
			foreach (var cart in ShoppingCartVM.shoppingCartsList)
			{
				OrderDetail orderDetail = new OrderDetail()
				{
					ProductId = cart.ProductId,
					OrderHeaderId = ShoppingCartVM.orderHeader.Id,
					Price = cart.Price,
					Count = cart.Count
				};
				_unitOfWork.OrderDetails.Add(orderDetail);
				_unitOfWork.Save();
			}

			return View(ShoppingCartVM);
		}

		private double GetPriceBasedOnQuantity(ShoppingCart shoppingCart)
		{
			if (shoppingCart.Count <= 50)
			{
				return shoppingCart.Product.Price;
			}
			else
			{
				if (shoppingCart.Count <= 100)
				{
					return shoppingCart.Product.Price50;
				}
				else
				{
					return shoppingCart.Product.Price100;
				}
			}
		}
	}
}
