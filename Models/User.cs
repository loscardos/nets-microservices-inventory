using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryService.Models
{
    public class User : BaseModel
    {
        public string Name { get; set; }

        [Index(IsUnique = true)]
        public string Email { get; set; }

        public string Password { get; set; }

        public virtual List<UserRole> UserRoles { get; set; }
    }
}