using Application.Use_Cases.Commands.StripeC;
using Domain.Common;
using Microsoft.AspNetCore.Mvc;
using Stripe;
using Stripe.Checkout;

namespace SmartRealEstateManagementSystem.Controllers
{
    [ApiController]
    [Route("api/stripe")]
    public class StripeController : ControllerBase
    {
        private readonly StripeModel model;
        private readonly ProductService productService;

        public StripeController(StripeModel model, TokenService tokenService, CustomerService customerService, ChargeService chargeService, ProductService productService)
        {
            this.model = model;
            this.productService = productService;
        }

        [HttpGet("{id}")]
        public async Task<ActionResult<Result<string>>> GetProductById(string id)
        {
            StripeConfiguration.ApiKey = model.SecretKey;
            var options = new ProductGetOptions
            {
                Expand = new List<string> { "default_price" }
            };

            try
            {
                var product = await productService.GetAsync(id, options);

                if (product == null)
                {
                    return NotFound("Product not found");
                }

                return Ok(product);
            }
            catch (StripeException ex)
            {
                return BadRequest($"Stripe error: {ex.Message}");
            }
        }

        [HttpGet("getAllProducts")]
        public async Task<ActionResult<Result<string>>> GetAllProducts()
        {
            StripeConfiguration.ApiKey = model.SecretKey;
            var options = new ProductListOptions { Expand = new List<string>() { "data.default_price" } };

            try
            {
                var products = await productService.ListAsync(options);

                return Ok(products);
            }
            catch (StripeException ex)
            {
                return BadRequest($"Stripe error: {ex.Message}");
            }
        }

        [HttpPost("createProduct")]
        public async Task<ActionResult<Result<string>>> CreateProduct([FromBody] CreateProductRequestCommand request)
        {
            StripeConfiguration.ApiKey = model.SecretKey;

            try
            {
                // Creează produsul
                var productOptions = new ProductCreateOptions
                {
                    Name = request.Name,
                    Description = request.Description,
                    DefaultPriceData = new ProductDefaultPriceDataOptions
                    {
                        Currency = "eur",
                        UnitAmount = (long?)(request.UnitAmount * 100),
                    },
                };
                var product = await productService.CreateAsync(productOptions);

                if (product == null)
                {
                    return BadRequest("Failed to create product");
                }

                // Obține produsul cu toate caracteristicile sale
                var options = new ProductGetOptions
                {
                    Expand = new List<string> { "default_price" }
                };
                var fullProduct = await productService.GetAsync(product.Id, options);

                if (fullProduct == null)
                {
                    return BadRequest("Failed to retrieve product details");
                }

                var response = new
                {
                    ProductId = fullProduct.Id,
                    PriceId = fullProduct.DefaultPriceId,
                };

                return Ok(response);
            }
            catch (StripeException ex)
            {
                return BadRequest($"Stripe error: {ex.Message}");
            }
        }

        [HttpPut("updateProduct")]
        public async Task<ActionResult<Result<string>>> UpdateProduct([FromBody] UpdateProductRequestCommand request)
        {
            StripeConfiguration.ApiKey = model.SecretKey;

            try
            {
                // Update the product's name and description
                var productOptions = new ProductUpdateOptions
                {
                    Name = request.Name,
                    Description = request.Description,
                };
                var product = await productService.UpdateAsync(request.ProductId, productOptions);

                if (product == null)
                {
                    return BadRequest("Failed to update product");
                }

                // Create a new price for the updated product
                var priceOptions = new PriceCreateOptions
                {
                    UnitAmount = (long?)(request.UnitAmount * 100),
                    Currency = "eur",
                    Product = request.ProductId,
                };
                var priceService = new PriceService();
                var price = await priceService.CreateAsync(priceOptions);

                if (price == null)
                {
                    return BadRequest("Failed to create price");
                }

                // Update the product's default price
                var updateProductOptions = new ProductUpdateOptions
                {
                    DefaultPrice = price.Id
                };
                var updatedProduct = await productService.UpdateAsync(request.ProductId, updateProductOptions);

                if (updatedProduct == null)
                {
                    return BadRequest("Failed to update product's default price");
                }

                var response = new
                {
                    ProductId = updatedProduct.Id,
                    PriceId = price.Id,
                    UnitAmount = request.UnitAmount,
                };

                return Ok(response);
            }
            catch (StripeException ex)
            {
                return BadRequest($"Stripe error: {ex.Message}");
            }
        }

        [HttpPost("pay")]
        public async Task<ActionResult<string>> Pay([FromBody] CreatePayRequestCommand request)
        {
            StripeConfiguration.ApiKey = model.SecretKey;

            var options = new SessionCreateOptions
            {
                LineItems = new List<SessionLineItemOptions>
                {
                    new SessionLineItemOptions
                    {
                        Price = request.defaultPriceId,
                        Quantity = 1,
                    }
                },
                Mode = "payment",
                SuccessUrl = "https://smart-real-estate-frontend-405e53ac73f5.herokuapp.com/transaction-succes",
                CancelUrl = "https://smart-real-estate-frontend-405e53ac73f5.herokuapp.com",
            };

            try
            {
                var service = new SessionService();
                var session = await service.CreateAsync(options);

                if (session == null)
                {
                    return BadRequest("Failed to create payment session");
                }

                return Ok(session.Url);
            }
            catch (StripeException ex)
            {
                return BadRequest($"Stripe error: {ex.Message}");
            }
        }
    }
}
