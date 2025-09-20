using Microsoft.AspNetCore.Identity;

namespace TaskManager.Domain.Entities.Models.Identity
{
    public class ApplicationUser : IdentityUser<Guid>
    {
        public string FullName { get; set; }
    }
}
