using LaptopCart.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Security.Claims;

namespace LaptopCart.Controllers
{
    public class CartController : Controller
    {
        private readonly ApplicationDbContext _context;
        public CartController(ApplicationDbContext context)
        {
            _context = context;
        }
        public IActionResult Index()
        {
            var userId = User.FindFirstValue(System.Security.Claims.ClaimTypes.NameIdentifier);
            var cartItems = _context.CartItems.Where(c => c.UserId == userId).Include(c => c.Product).ToList();
            return View(cartItems);
        }
        public async Task<IActionResult> Plus(int cartId)
        {
            var cartFromDb = _context.CartItems.FirstOrDefault(x => x.Id == cartId);
            if(cartFromDb == null)
            
                return NotFound();
                cartFromDb.Quantity += 1;
                _context.CartItems.Update(cartFromDb);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            
        }
        public async Task<IActionResult> Minus(int cartId)
        {
            var cartFromDb = _context.CartItems.FirstOrDefault(x => x.Id == cartId);
            if (cartFromDb == null)

                return NotFound();
            cartFromDb.Quantity -= 1;
            _context.CartItems.Update(cartFromDb);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));

        }
        
        public IActionResult Remove(int cartId)
        {
            var cartFromDb = _context.CartItems.FirstOrDefault(x => x.Id == cartId);
            var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
            _context.CartItems.Remove(cartFromDb);
            HttpContext.Session.SetInt32(
                   Constants.SessionCart,
                   _context.CartItems.Count(x => x.UserId == userId) - 1
               );
            _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }
    }
}
