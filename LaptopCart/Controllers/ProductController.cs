using LaptopCart.Data.Data;
using LaptopCart.Models;
using LaptopCart.Utility;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace LaptopCart.Controllers
{
    public class ProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        public ProductController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            //display the cart count in the session for the logged-in user
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            if(userId != null)
            {
                HttpContext.Session.SetInt32(
                    Constants.SessionCart,
                    _context.CartItems.Where(x => x.UserId == userId).Count()
                );
            }
            //var products = _context.Products.ToList();
            return View(_context.Products.ToList());
        }
        public IActionResult Details(int id)
        {
            var product = _context.Products.FirstOrDefault(p => p.Id == id);
            return View(product);
        }
        [Authorize]
        [HttpPost]
        public async Task<IActionResult> Details(CartItem cartItem)
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            var cartProduct = _context.CartItems.FirstOrDefault(p => p.ProductId == cartItem.ProductId && p.UserId == userId);
            if (cartProduct != null)
            {
                cartProduct.Quantity += cartItem.Quantity;
                _context.CartItems.Update(cartProduct);
                await _context.SaveChangesAsync();
            }
            else
            {
                cartItem.Id = 0;
                cartItem.UserId = userId;
                _context.CartItems.Add(cartItem);
                await _context.SaveChangesAsync();
            }
                return RedirectToAction("Index");
        }
    }
}
