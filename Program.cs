using IS_Kactus_Expenses.Data;
using IS_Kactus_Expenses.Service;
using IS_Kactus_Expenses.Service.Interface;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Polly;

var builder = Host.CreateDefaultBuilder(args)
    .ConfigureAppConfiguration((context, config) =>
    {
        config.AddJsonFile("appsettings.json", optional: false, reloadOnChange: true);
    })
    .ConfigureServices((context, services) =>
    {
        services.AddDbContext<DataContext>(options =>
            options.UseSqlServer(context.Configuration.GetConnectionString("DefaultConnection")));

        services.AddHttpClient<IApiClient, ApiClient>(client =>
            client.BaseAddress = new Uri(context.Configuration["ApiSettings:BaseUrl"]!))
            .AddPolicyHandler(Policy<HttpResponseMessage>.Handle<HttpRequestException>()
                .WaitAndRetryAsync(3, retryAttempt => TimeSpan.FromSeconds(Math.Pow(2, retryAttempt))));

        services.AddScoped<IEmailService, EmailService>();

        services.AddScoped<IUserRepository, UserRepository>();
        services.AddScoped<IUserService, UserService>();

        services.AddScoped<ApplicationRunner>();
    });

var app = builder.Build();



//------- Ejecuta la lógica principal de la aplicación ----------//

using var scope = app.Services.CreateScope();
var applicationRunner = scope.ServiceProvider.GetRequiredService<ApplicationRunner>();

if (args.Length == 0)
{
    Console.WriteLine("No se proporcionaron parámetros. Use 'update' para la opción 1 o 'create' para la opción 2.");
    return;
}


switch (args[0].ToLower())
{
    // dotnet run -- update
    case "update":
        await applicationRunner.UpdateUsersAsync();
        break;

    //dotnet run -- create
    //case "create":
    //    var codEmpList = new List<string> { "10198901245", "1010178996", "1033754995", "80219752", "1001347043", "1014267333", "1077148146" };
    //    await applicationRunner.CreateUsersAsync(codEmpList);
    //    break;

    case "create":
        if (args.Length < 2)
        {
            Console.WriteLine("Debe proporcionar la ruta del archivo Excel. Uso: dotnet run -- create <ruta_al_archivo>");
            return;
        }

        string filePath = args[1];
        if (!File.Exists(filePath))
        {
            Console.WriteLine($"El archivo especificado no existe: {filePath}");
            return;
        }

        await applicationRunner.CreateUsersAsync(filePath);
        break;

    default:
        Console.WriteLine($"Comando no reconocido: {args[0]}");
        Console.WriteLine("Use 'update' para la opción 1 o 'create' para la opción 2.");
        break;
}