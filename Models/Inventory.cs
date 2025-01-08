using System.ComponentModel.DataAnnotations;

namespace InventoryService.Models
{
    public class Inventory : BaseModel
    {
        [Required]
        public string ProductName { get; set; }
        
        [Required]
        public int Quantity { get; set; }
        
        [Required]
        public decimal PricePerUnit { get; set; }

    }
}