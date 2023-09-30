using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Shop;
using Shop.Configurations;
using Serilog;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(
    options =>
    {
        options.SerializerOptions.WriteIndented = true;
    }
);
//���������� serilog 
builder.Host.UseSerilog((ctx, conf) =>
{
    conf.ReadFrom.Configuration(ctx.Configuration);
});

builder.Services.AddOptions<SmtpConfig>()
 .BindConfiguration("SmtpConfig")
 .ValidateDataAnnotations()
 .ValidateOnStart();

builder.Services.AddSingleton<ICatalog, InMemoryCatalog>();
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();
builder.Services.AddScoped<ISmtpEmailSender, SmtpEmailSender>();

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Shop");

//�� 5. ������� �������. Scoped 
app.MapGet("/sendmail", async (ISmtpEmailSender emailService) =>
{
    await emailService.SendEmailAsync("antysya@mail.ru", "�����������", "������ ������� �������");
    await emailService.SendEmailAsync("antysyamail.ru", "�����������", "������ ������� �������");
});


app.Run();

