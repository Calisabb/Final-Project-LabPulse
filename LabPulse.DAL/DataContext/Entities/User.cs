using Microsoft.AspNetCore.Identity;

namespace LabPulse.DAL.DataContext.Entities
{
    public class User : IdentityUser<Guid>
    {
        public DateTime CreatedAt { get; set; }
    }
}
