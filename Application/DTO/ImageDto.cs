

namespace Application.DTO
{
    public class ImageDto
    {
        public Guid Id { get; set; }
        public Guid EstateId { get; set; }
        public required string Extension { get; set; }
    }
}
