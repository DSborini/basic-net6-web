var builder = WebApplication.CreateBuilder(args);
var app = builder.Build();
var configuration = app.Configuration;
ProductRepository.Init(configuration);

app.MapPost("/products", (Product product) =>
{
    ProductRepository.Add(product);
    return Results.Created($"/products/{product.Code}", product);
});

app.MapGet("/products/{code}", (string code) =>
{
    var product = ProductRepository.GetBy(code);
    if (product == null)
        return Results.NotFound();

    return Results.Ok(product);
});

app.MapPut("/products/{code}", (string code, Product product) =>
{
    var existingProduct = ProductRepository.GetBy(code);
    if (existingProduct == null)
        return Results.NotFound();

    existingProduct.Name = product.Name;
    existingProduct.Price = product.Price;

    return Results.Ok(existingProduct);
});

app.MapDelete("/products/{code}", (string code) =>
{
    ProductRepository.Remove(code);
    return Results.NoContent();
});

app.Run();

public class Product
{
    public string Code { get; set; }
    public string Name { get; set; }
    public decimal Price { get; set; }
}

public static class ProductRepository
{
    public static List<Product> Products { get; set; } = Products = new List<Product>();

    public static void Init(IConfiguration configuration)
    {
        var products = configuration.GetSection("Products").Get<List<Product>>();
        Products = products;
    }

    public static void Add(Product product)
    {
        Products.Add(product);
    }

    public static Product GetBy(string code)
    {
        return Products.FirstOrDefault(p => p.Code == code);
    }

    public static void Remove(string code)
    {
        var product = GetBy(code);
        if (product != null)
            Products.Remove(product);
    }
}