using Microsoft.AspNetCore.Identity;

namespace RealTimeMail.Model
{
    public class User
    {
        public Guid Id { get; set; }
        public string UserName { get; set; } = null!;
        public string ConnectionId { get; set; } = null!;
    }
}
