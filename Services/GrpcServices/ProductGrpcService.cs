using Grpc.Core;
using GrpcService1;
using GrpcService1.Services;

public class ProductGrpcService : ProductGrpc.ProductGrpcBase
{
    private readonly IProductService _productService;
    private readonly ILogger<ProductGrpcService> _logger;

    public ProductGrpcService(IProductService productService, ILogger<ProductGrpcService> logger)
    {
        _productService = productService;
        _logger = logger;
    }

    public override async Task<GetUserProductsResponse> GetUserProducts(
        GetUserProductsRequest request, ServerCallContext context)
    {
        var products = await _productService.GetUserProductsAsync(request.UserId);
        
        var response = new GetUserProductsResponse();
        response.Products.AddRange(products.Select(up => new UserProductDto
        {
            Id = up.Id,
            Product = new ProductDto
            {
                Id = up.Product.Id,
                Name = up.Product.Name,
                Value = (double)up.Product.Value
            }
        }));

        return response;
    }

    public override async Task<SellProductResponse> SellProduct(
        SellProductRequest request, ServerCallContext context)
    {
        var (success, newBalance) = await _productService.SellProductAsync(
            request.UserId, request.UserProductId);

        return new SellProductResponse
        {
            Success = success,
            NewBalance = (double)newBalance
        };
    }
}