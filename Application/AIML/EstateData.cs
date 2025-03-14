using Microsoft.ML.Data;

namespace Application.AIML
{
    public class EstateData
    {
        [LoadColumn(2)]
        public float Price { get; set; }
        [LoadColumn(3)]
        public int Bedrooms { get; set; }
        [LoadColumn(4)]
        public int Bathrooms { get; set; }
        [LoadColumn(5)]
        public float LandSize { get; set; }
        [LoadColumn(6)]
        public required string Street { get; set; }
        [LoadColumn(7)]
        public required string City { get; set; }
        [LoadColumn(8)]
        public required string State { get; set; }
        [LoadColumn(9)]
        public required string ZipCode { get; set; }
        [LoadColumn(10)]
        public float HouseSize { get; set; }
    }
}
