namespace Domain.Entities
{
    public class Contact
    {
        public Guid Id { get; set; }
        public Guid UserId { get; set; }

        public required string Email { get; set; }
        public required string Phone { get; set; }
    }
}
