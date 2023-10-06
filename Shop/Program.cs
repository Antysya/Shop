using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Shop;
using Shop.Configurations;
using Serilog;
using Org.BouncyCastle.Cms;

var builder = WebApplication.CreateBuilder(args);
builder.Services.Configure<Microsoft.AspNetCore.Http.Json.JsonOptions>(
    options =>
    {
        options.SerializerOptions.WriteIndented = true;
    }
);
//добавление serilog 
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


builder.Services.Decorate<ISmtpEmailSender>((inner, provider) =>
    new RetrySendDecorator(inner, provider.GetService<ILogger<RetrySendDecorator>>()));

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Shop");

//ДЗ 5. Фоновые сервисы. Scoped 
app.MapGet("/sendmail", async (ISmtpEmailSender emailService) =>
{
    await emailService.SendEmailAsync("antysya@mail.ru", "Подключение", "Сервер успешно запущен");
});

app.MapGet("/sendmailwithresending", async (ISmtpEmailSender emailService, ILogger<Program> _logger) =>
{
    bool isSent = false;
    int maxRetryAttempts = 3;

    for (int i = 0; i < maxRetryAttempts; i++)
    {
        try
        {
            await emailService.SendEmailAsync("antysya@mail.ru", "Подключение", "Сервер успешно запущен");
            isSent = true;
            break;
        }
        catch (Exception ex)
        {
            _logger.LogWarning(ex, "Ошибка при отправки письма: {Recipient}, {Subject}", "antysya@mail.ru", "Подключение");
            await Task.Delay(TimeSpan.FromSeconds(30));
        }
    }

    if (isSent)
    {
        _logger.LogInformation("Письмо успешно отправлено: {Recipient}, {Subject}", "antysya@mail.ru", "Подключение");
    }
    else
    {
        _logger.LogError("Произошла ошибка. Письмо не отправлено: {Recipient}, {Subject}", "antysya@mail.ru", "Подключение");
    }
});

app.MapGet("/resendmaildecorator", async (ISmtpEmailSender emailService) =>
{
    await emailService.SendEmailAsync("antysya@mail.ru", "Подключение", "Сервер успешно запущен");
});

app.Run();

