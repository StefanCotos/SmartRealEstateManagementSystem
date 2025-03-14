using Microsoft.ML;

namespace Application.AIML
{
    public static class EstateDataParser
    {
        private static MLContext mlContext = new MLContext();
        public static IDataView GetEstates()
        {
            string path = Path.GetFullPath("../Application/Data/data.csv");
            var dataView = mlContext.Data.LoadFromTextFile<EstateData>(path, hasHeader: true, separatorChar: ',');

            return dataView;
        }
    }
}
