using Domain.Common;
using Microsoft.ML;
using Microsoft.ML.Data;

namespace Application.AIML
{
    public class EstatePricePredictionModel : IDisposable
    {
        private readonly MLContext mlContext;
        private ITransformer? model;
        private readonly int numberOfLeaves = 1500;
        private bool disposedValue;

        public EstatePricePredictionModel()
        {
            mlContext = new MLContext(seed: 0);

            if (model == null)
            {
                string modelPath = Path.Combine(Directory.GetCurrentDirectory(), $"model_{numberOfLeaves}.zip");
                if (File.Exists(modelPath))
                {
                    model = mlContext.Model.Load(modelPath, out DataViewSchema schema);
                }
            }
        }

        public Result<bool> Train(IDataView dataView)
        {
            try
            {
                // Split the data into training and testing sets
                var splitData = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
                var trainingData = splitData.TrainSet;
                var pipeline = mlContext.Transforms.CopyColumns(outputColumnName: "Label", inputColumnName: nameof(EstateData.Price))
                                    .Append(mlContext.Transforms.Text.FeaturizeText("StreetFeaturized", nameof(EstateData.Street)))
                                    .Append(mlContext.Transforms.Categorical.OneHotEncoding("StateEncoded", nameof(EstateData.State)))
                                    .Append(mlContext.Transforms.Categorical.OneHotEncoding("ZipCodeEncoded", nameof(EstateData.ZipCode)))
                                    .Append(mlContext.Transforms.Conversion.ConvertType(nameof(EstateData.Bedrooms), outputKind: DataKind.Single))
                                    .Append(mlContext.Transforms.Conversion.ConvertType(nameof(EstateData.Bathrooms), outputKind: DataKind.Single))
                                    .Append(mlContext.Transforms.Conversion.ConvertType(nameof(EstateData.LandSize), outputKind: DataKind.Single))
                                    .Append(mlContext.Transforms.Conversion.ConvertType(nameof(EstateData.HouseSize), outputKind: DataKind.Single))
                                    .Append(mlContext.Transforms.NormalizeMeanVariance(nameof(EstateData.Bedrooms)))
                                    .Append(mlContext.Transforms.NormalizeMeanVariance(nameof(EstateData.Bathrooms)))
                                    .Append(mlContext.Transforms.NormalizeMeanVariance(nameof(EstateData.LandSize)))
                                    .Append(mlContext.Transforms.NormalizeMeanVariance(nameof(EstateData.HouseSize)))
                                    .Append(mlContext.Transforms.Concatenate("Features",
                                        "StreetFeaturized", "StateEncoded", "ZipCodeEncoded",
                                        nameof(EstateData.Bedrooms), nameof(EstateData.Bathrooms),
                                        nameof(EstateData.LandSize), nameof(EstateData.HouseSize)))
                                    .Append(mlContext.Regression.Trainers.FastTree(
                                        labelColumnName: nameof(EstateData.Price),
                                        featureColumnName: "Features",
                                        numberOfLeaves: numberOfLeaves,
                                        minimumExampleCountPerLeaf: 50,
                                        numberOfTrees: 800,
                                        learningRate: 0.1));


                // Train the model
                model = pipeline.Fit(trainingData);

                mlContext.Model.Save(model, trainingData.Schema, "model_" + numberOfLeaves.ToString() + ".zip");
                return Result<bool>.Success(true);
            }
            catch (Exception ex)
            {
                return Result<bool>.Failure(ex.Message);
            }
        }
        public Result<RegressionMetrics> Evaluate(IDataView dataView)
        {
            try
            {
                if (model == null)
                {
                    string modelPath = Path.Combine(Directory.GetCurrentDirectory(), $"model_{numberOfLeaves}.zip");
                    model = mlContext.Model.Load(modelPath, out DataViewSchema schema);
                }
                var splitData = mlContext.Data.TrainTestSplit(dataView, testFraction: 0.2);
                var testData = splitData.TestSet;

                var predictions = model.Transform(testData);
                var metrics = mlContext.Regression.Evaluate(predictions, "Label", "Score");
                return Result<RegressionMetrics>.Success(metrics);
            }
            catch (Exception ex)
            {
                return Result<RegressionMetrics>.Failure(ex.Message);
            }
        }

        public Result<float> Predict(EstateData estateData)
        {
            try
            {
                if (model == null)
                {
                    throw new InvalidOperationException("Modelul nu a fost încărcat corespunzător.");
                }

                var predictionEngine = mlContext.Model.CreatePredictionEngine<EstateData, EstatePricePrediction>(model);
                var prediction = predictionEngine.Predict(estateData);
                return Result<float>.Success(prediction.Price);
            }
            catch (Exception ex)
            {
                return Result<float>.Failure(ex.Message);
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing && model != null)
                { model = null; }

                disposedValue = true;
            }
        }

        ~EstatePricePredictionModel()
        {
            Dispose(disposing: false);
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
