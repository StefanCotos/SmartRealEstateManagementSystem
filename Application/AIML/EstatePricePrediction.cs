using Microsoft.ML.Data;

namespace Application.AIML
{
    public class EstatePricePrediction
    {
        [ColumnName("Score")]
        public float Price { get; set; }
    }
}
