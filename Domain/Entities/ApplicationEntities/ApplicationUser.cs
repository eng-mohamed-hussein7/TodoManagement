using Microsoft.AspNetCore.Identity;

namespace Domain.Entities.ApplicationEntities;

public class ApplicationUser : IdentityUser<Guid>
{
    public string? FullName { get; set; }
    public bool IsDeleted { get; set; } = false;
    public DateTime CreatedDate { get; set; } = DateTime.Now;
}
