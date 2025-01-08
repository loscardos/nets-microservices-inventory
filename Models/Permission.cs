using System.ComponentModel.DataAnnotations.Schema;

namespace InventoryService.Models
{
    public class Permission : BaseModel
    {
        public string Name { get; set; }

        [Index(IsUnique = true)]
        public string Key { get; set; }

        public virtual ICollection<RolePermission> RolePermissions { get; set; }
    }
}