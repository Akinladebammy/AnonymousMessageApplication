namespace AnonymousMessageApplication.Models.Entities
{
    public class Admin
    {
        public Guid Id { get; set; } = Guid.NewGuid();
        public string Username { get; set; }  // unique
        public string PasswordHash { get; set; }
        public string Role { get; set; } = "Admin"; // for JWT claims
    }
}
