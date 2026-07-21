using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http;

namespace LaptopCart.Models
{
    public class Product
    {
        [Key]
        public int Id { get; set; }
        [Required(ErrorMessage ="Product name is required")]
        public string Name { get; set; }
        [StringLength(500,ErrorMessage ="Description can't be more than 500 characters")]
        public string Description { get; set; }
        public decimal Price { get; set; }
        public string? ImagePath { get; set; }
        public DateTime CreatedAt { get; set; }

        [NotMapped]
        public IFormFile? ImageFile { get; set; }

        

    }
}
