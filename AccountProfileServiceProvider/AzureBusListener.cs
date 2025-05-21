
using AccountProfileServiceProvider.Dto;
using AccountProfileServiceProvider.Protos;
using AccountProfileServiceProvider.Services;
using Azure.Messaging.ServiceBus;
using System.Text.Json;

namespace AccountProfileServiceProvider;

public class AzureBusListener(IConfiguration configuration, IServiceScopeFactory scopeFactory) : BackgroundService
{
    private readonly IConfiguration _configuration = configuration;
    private ServiceBusProcessor? _processor;
    private readonly IServiceScopeFactory _scopeFactory = scopeFactory;

    protected async override Task ExecuteAsync(CancellationToken stoppingToken)
    {

        var client = new ServiceBusClient(_configuration.GetConnectionString("AzureServiceBus"));
        _processor = client.CreateProcessor("userprofile", new ServiceBusProcessorOptions());
        _processor.ProcessMessageAsync += ProcessMessageAsync;
        _processor.ProcessErrorAsync += ErrorHandler;
        await _processor.StartProcessingAsync(stoppingToken);


        while (!stoppingToken.IsCancellationRequested)
        {
            await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
        }
    }

    private async Task ErrorHandler(ProcessErrorEventArgs args)
    {
        throw new NotImplementedException();
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        try
        {
            var message = args.Message.Body.ToString();
            using (var scope = _scopeFactory.CreateScope())
            {
                var userProfileServices = scope.ServiceProvider.GetRequiredService<UserProfileServices>();
                if (message.Contains("AppUserId"))
                {
                    var request = JsonSerializer.Deserialize<createUserProfileRequest>(message);
                    var dto = new AddAcountProfileForm
                    {
                        AppUserId = request.AppUserId,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        PhoneNumber = request.PhoneNumber,
                        StreetName = request.StreetName,
                        PostalCode = request.PostalCode,
                        City = request.City
                    };
                    var result = await userProfileServices.AddUserProfileAsync(dto, request.AppUserId);
                }

                else if (!message.Contains("appUserId"))
                {
                    var request = JsonSerializer.Deserialize<updateUserProfileRequest>(message);
                    var dto = new UpdateAcountProfileForm
                    {
                        Id = request.Id,
                        FirstName = request.FirstName,
                        LastName = request.LastName,
                        PhoneNumber = request.PhoneNumber,
                        StreetName = request.StreetName,
                        PostalCode = request.PostalCode,
                        City = request.City
                    };
                    var result = await userProfileServices.UpdateUserProfile(dto);
                }
            }
                

        }

        catch (Exception ex)
        {
            Console.WriteLine(ex.ToString());
        }
    }
}
