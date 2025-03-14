namespace Domain.Entities
{
    public class Image
    {
        public Guid Id { get; set; }
        public Guid EstateId { get; set; }
        public required string Extension { get; set; }
    }
}
