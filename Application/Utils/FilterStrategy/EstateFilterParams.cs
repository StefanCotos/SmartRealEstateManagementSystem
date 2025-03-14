namespace Application.Utils.FilterStrategy
{
    public class EstateFilterParams
    {
        public string? Name { get; set; }
        public decimal Price { get; set; }
        public int Bedrooms { get; set; }
        public int Bathrooms { get; set; }
        public decimal LandSize { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public decimal HouseSize { get; set; }
        public int Page { get; set; }
        public int PageSize { get; set; }
    }
}
