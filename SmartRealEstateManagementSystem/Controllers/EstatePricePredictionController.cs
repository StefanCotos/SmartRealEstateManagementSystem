using Application.AIML;
using Microsoft.AspNetCore.Mvc;
using Microsoft.ML.Data;

namespace SmartRealEstateManagementSystem.Controllers
{
    [Route("api/estate-price-prediction")]
    [ApiController]
    public class EstatePricePredictionController : ControllerBase
    {
        private readonly EstatePricePredictionModel estatePricePredictionModel;
        public EstatePricePredictionController()
        {
            estatePricePredictionModel = new EstatePricePredictionModel();
        }
        [HttpPost("train")]
        public ActionResult<bool> TrainModel()
        {
            var data = EstateDataParser.GetEstates();
            var result = estatePricePredictionModel.Train(data);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok();
        }
        [HttpGet("evaluate")]
        public ActionResult<RegressionMetrics> EvaluateModel()
        {
            var data = EstateDataParser.GetEstates();
            var result = estatePricePredictionModel.Evaluate(data);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }

        [HttpPost("predict")]
        public ActionResult<float> PredictPrice(EstateData estateData)
        {
            var result = estatePricePredictionModel.Predict(estateData);
            if (!result.IsSuccess)
            {
                return BadRequest(result.ErrorMessage);
            }
            return Ok(result.Data);
        }
    }
}
