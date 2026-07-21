using LaptopCart.Data;
using LaptopCart.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace LaptopCart.Controllers
{
    [Authorize(Roles ="Admin")]
    public class AdminProductController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IWebHostEnvironment _webHostEnvironment;
        public AdminProductController(ApplicationDbContext context, IWebHostEnvironment webHostEnvironment  )
        {
            _context = context;
            _webHostEnvironment = webHostEnvironment;
        }
        public IActionResult Index()
        {
            //List<Product> products = _context.Products.ToList();
            return View(_context.Products.ToList());
        }
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create( Product product)
        {
            //Custom Validation
            if(product.Name?.Trim().ToLower() == "test")
            {
                ModelState.AddModelError("Name","Product name can't be 'test'");
            }
            
            if(product.ImageFile != null && product.ImageFile.Length> 0)
            {
                string wwwRootPath = _webHostEnvironment.WebRootPath;
                string originalFileName = Path.GetFileNameWithoutExtension(product.ImageFile.FileName);
                string extension = Path.GetExtension(product.ImageFile.FileName);
                string uniqueFileName = $"{originalFileName}_{Guid.NewGuid():N}{extension}";
                string imageFolder = Path.Combine(wwwRootPath,"images");
                if(!Directory.Exists(imageFolder))
                {
                    Directory.CreateDirectory(imageFolder);
                }
                string filePath = Path.Combine(imageFolder,uniqueFileName);
                using (var stream = new FileStream(filePath,FileMode.Create))
                {
                    await product.ImageFile.CopyToAsync(stream);
                }
                product.ImagePath = "/images/" + uniqueFileName;
                string confirmPath = Path.Combine(wwwRootPath,product.ImagePath.TrimStart('/'));
                if(!System.IO.File.Exists(confirmPath))
                {
                    throw new FileNotFoundException("Image was not saved correctly",confirmPath);
                }

            }
            //Server side validation
            if(ModelState.IsValid)
            {
                _context.Products.Add(product);
                await _context.SaveChangesAsync();
                TempData["success"] = "Product Created successfully!";
                return RedirectToAction("Index");
            }
            else
            {
                return View(product);
            }
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _context.Products.FindAsync(id);
            if (id == null || product == null)
            {
                return View();
            }
            return View(product);
        }
        public async Task<IActionResult> Delete(int id)
        {
            Product product = await _context.Products.FindAsync(id);
            if (id == null || product == null)
            { 
                return View(); 
            }           
            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(Product product)
        {
            if (ModelState.IsValid)
            {
                if (product.ImageFile != null && product.ImageFile.Length > 0)
                {
                    var fileName = Guid.NewGuid().ToString() + Path.GetExtension(product.ImageFile.FileName);
                    var savePath = Path.Combine(_webHostEnvironment.WebRootPath, "images", fileName);

                    var imagesDir = Path.Combine(_webHostEnvironment.WebRootPath, "images");
                    if (!Directory.Exists(imagesDir))
                    {
                        Directory.CreateDirectory(imagesDir);
                    }

                    using (var stream = new FileStream(savePath, FileMode.Create))
                    {
                        await product.ImageFile.CopyToAsync(stream);
                    }
                    product.ImagePath = "/images/" + fileName;
                }
                else
                {
                    var existingProduct = await _context.Products.AsNoTracking()
                        .FirstOrDefaultAsync(p => p.Id == product.Id);

                    if (existingProduct != null)
                    {
                        product.ImagePath = existingProduct.ImagePath;
                    }
                }


                _context.Products.Update(product);
                await _context.SaveChangesAsync();
                TempData["success"] = "Product updated successfully!";
                return RedirectToAction("Index");
            }

            return View(product);
        }

        [HttpPost]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Product product = await _context.Products.FindAsync(id);
            if (product != null)
            {
                //To remove the image from images folder(wwwroot -> Images) 
                if(!string.IsNullOrEmpty(product.ImagePath))
                {
                    var imagePath = Path.Combine(_webHostEnvironment.WebRootPath, product.ImagePath.TrimStart('/').Replace("/", "\\"));
                    if (System.IO.File.Exists(imagePath))
                        System.IO.File.Delete(imagePath);
                   
                }
                _context.Products.Remove(product);
                await _context.SaveChangesAsync();
            }
            TempData["success"] = "Record deleted successfully!";
            return RedirectToAction(nameof(Index));
        }

     }
}
