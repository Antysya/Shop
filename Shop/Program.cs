using Microsoft.AspNetCore.Http.Json;
using Microsoft.AspNetCore.Mvc;
using Shop;
using Shop.Configurations;
using Serilog;
using Org.BouncyCastle.Cms;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.AspNetCore.Connections;
using System.Threading;

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
builder.Services.AddScoped<IEmailSender, MailKitSmtpEmailSender>();

builder.Services.Decorate<IEmailSender, RetrySendDecorator>(); //перехват зависимости

var app = builder.Build();
app.UseSwagger();
app.UseSwaggerUI();

app.MapGet("/", () => "Shop");

//ДЗ 5. Фоновые сервисы. Scoped 
app.MapGet("/sendmail", async (IEmailSender emailService, CancellationToken cancellationToken) =>
{
    await emailService.SendEmailAsync("antysya@mail.ru", "Подключение", "Сервер успешно запущен", cancellationToken);
});

app.MapGet("/sendmailwithresending", async (IEmailSender emailService, ILogger<Program> _logger, CancellationToken cancellationToken) =>
{
    int attemptsLimit = 3;

    for (int currentAttept=1; currentAttept<=attemptsLimit; currentAttept++)
    {
        try
        {
            await emailService.SendEmailAsync("antysya@mail.ru", "Подключение", "Сервер успешно запущен", cancellationToken);
            break;
        }
        catch (ConnectionException ex) when (currentAttept<attemptsLimit)
        {
            _logger.LogWarning(ex, "Ошибка при отправки письма: {Recipient}, {Subject}", "antysya@mail.ru", "Подключение");
            await Task.Delay(TimeSpan.FromSeconds(30));
        }
        catch(Exception ex)
        {
            _logger.LogError(ex, "Произошла ошибка. Письмо не отправлено: {Recipient}, {Subject}", "antysya@mail.ru", "Подключение");
            break;
        }
    }
});

app.MapGet("/resendmaildecorator", async (IEmailSender emailService, CancellationToken cancellationToken) =>
{
    await emailService.SendEmailAsync("antysya@mail.ru", "Подключение", "Сервер успешно запущен", cancellationToken);
});

app.Run();

