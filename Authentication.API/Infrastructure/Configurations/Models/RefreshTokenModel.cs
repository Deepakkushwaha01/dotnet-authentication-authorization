namespace Authentication.API.Infrastructure.Configurations.Models
{
    using System.ComponentModel.DataAnnotations.Schema;
    using Authentication.API.Infrastructure.Configurations.Entity;
    public class RefreshToken : TrackedEntity
    {
        required public string Token { get; set; }
        public string UserId { get; set; } = string.Empty;
        required public DateTime Expires { get; set; }

        [NotMapped]
        public bool IsExpired => DateTime.UtcNow >= Expires;

    }
}