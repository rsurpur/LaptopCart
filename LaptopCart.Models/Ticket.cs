using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LaptopCart.Models
{
    public class Ticket
    {
        public int Id { get; set; }
        [Required]
        public string Subject { get; set; } = string.Empty;
        [Required]
        public string Description { get; set; } = string.Empty;
        public string Status { get; set; } = "Open";
        public string? UserId { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
    }
}
