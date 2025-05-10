using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.ApplicationEntities;

public class ApplicationRole : IdentityRole<Guid>
{
    public string? RoleTesting { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}