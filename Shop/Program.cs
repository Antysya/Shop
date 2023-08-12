using Microsoft.AspNetCore.Http.Json;
using Shop;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<JsonOptions>(
    options =>
    {
        options.SerializerOptions.WriteIndented = true;
    }
);

var app = builder.Build();

var catalog = new Catalog();

app.MapGet("/", () => "Shop");



app.MapGet("/get_products",()=> catalog.GetProducts());
app.MapPost("/add_products", (Product product) => catalog.AddProducts(product));
app.MapDelete("/del_products", (Product product) => catalog.DelProducts(product));
app.MapPut("/put_products", (string productName, Product product) => catalog.PutProducts(productName, product));

app.Run();