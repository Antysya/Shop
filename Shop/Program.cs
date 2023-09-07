using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Shop;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(
    options =>
    {
        options.SerializerOptions.WriteIndented = true;
    }
);

builder.Services.AddSingleton<ICatalog, InMemoryCatalog>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<IEmailService, EmailService>();


var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();


app.MapGet("/", () => "Shop");


#region
//ДЗ2
//var catalog = new Catalog();
//app.MapGet("/get_products",()=> catalog.GetProducts());
//app.MapPost("/get_product", ([FromBody] string productName) =>
//{
//    catalog.GetProduct(productName);
//}
//);

//app.MapPost("/add_products", ([FromBody]Product product) =>
//{
//    catalog.AddProducts(product);
//    return Results.Created(uri: $"/get_product?name={product.Name}", product); //возвращает 201
//    }
//);
//app.MapDelete("/del_products", ([FromBody] Product product) => catalog.DelProducts(product));
//app.MapPut("/put_products", (string productName, Product product) => catalog.PutProducts(productName, product));

//ДЗ3
//Многопоточность

//app.MapGet("/get_products_async", async () => await catalog.GetProductsAsync());
//app.MapPost("/add_products_async",async ([FromBody] Product product) =>
//{
//    await catalog.AddProductsAsync(product);
//}
//);
//app.MapDelete("/del_products_async", async ([FromBody] Product product) =>
//{
//    await catalog.RemoveProductsAsync(product);
//});

//потокобезопасная коллекция
//var catalogBag = new CatalogBag();

//app.MapGet("/get_products_bag", () => catalogBag.GetProductsBag());
//app.MapPost("/get_product_bag", (string productName) =>
//{
//    catalogBag.GetProductBag(productName);
//});

//app.MapPost("/add_products_bag", ([FromBody] Product product) =>
//{
//    catalogBag.AddProductsBag(product);
//    return Results.Created(uri: $"/get_product_bag?name={product.Name}", product); //возвращает 201
//});

//app.MapDelete("/del_products_bag", ([FromBody] Product product) => catalogBag.RemoveProductBag(product));



////ДЗ-4
////Внедрение зависимости


//app.MapGet("/get_products", ([FromServices] ICatalog catalog) => catalog.GetProducts());
//app.MapPost("/get_product", ([FromBody] string productName, [FromServices] ICatalog catalog) =>
//{
//    catalog.GetProduct(productName);
//}
//);
//app.MapPost("/add_products", ([FromBody] Product product, [FromServices] ICatalog catalog) =>
//{
//    catalog.AddProducts(product);
//    return Results.Created(uri: $"/get_product?name={product.Name}", product); //возвращает 201
//}
//);
//app.MapDelete("/del_products", ([FromBody] Product product, [FromServices] ICatalog catalog) => catalog.DelProducts(product));
//app.MapPut("/put_products", (string productName, Product product, [FromServices] ICatalog catalog) => catalog.PutProducts(productName, product));

//app.MapGet("/get_product_async", async ([FromBody] Product product, [FromServices] ICatalog catalog) => await catalog.GetProductAsync(product.Name));
//app.MapGet("/get_products_async", async ([FromServices] ICatalog catalog) =>
//{
//    await catalog.GetProductsAsync();
//}
//);
//app.MapPost("/add_products_async", async ([FromBody] Product product, [FromServices] ICatalog catalog) =>
//{
//    await catalog.AddProductsAsync(product);
//}
//);
//app.MapDelete("/del_products_async", async ([FromBody] Product product, [FromServices] ICatalog catalog) =>
//{
//    await catalog.RemoveProductsAsync(product);
//});


//app.MapGet("/get_products_discounts_async", async ([FromServices] ICatalog catalog) =>
//{
//    await catalog.GetProductsDiscountsAsync();
//}
//);
#endregion

//ДЗ 5. Фоновые сервисы. Scoped 
app.MapGet("/sendmail", async (IEmailService emailService) =>
{
    await emailService.SendEmailAsync("antysya@mail.ru", "Подключение", "Сервер успешно запущен");
}

);

app.Run();
