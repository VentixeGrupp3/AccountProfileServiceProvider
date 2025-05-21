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
        Console.WriteLine("[AzureBusListener] Starting background service");

        try
        {
            var connectionString = _configuration.GetConnectionString("AzureServiceBus");
            Console.WriteLine("[AzureBusListener] Initializing ServiceBusClient");
            var client = new ServiceBusClient(connectionString);

            Console.WriteLine("[AzureBusListener] Creating processor for 'userprofile' queue");
            _processor = client.CreateProcessor("userprofile", new ServiceBusProcessorOptions());
            _processor.ProcessMessageAsync += ProcessMessageAsync;
            _processor.ProcessErrorAsync += ErrorHandler;

            Console.WriteLine("[AzureBusListener] Starting message processing");
            await _processor.StartProcessingAsync(stoppingToken);
            Console.WriteLine("[AzureBusListener] Message processing started successfully");

            while (!stoppingToken.IsCancellationRequested)
            {
                await Task.Delay(TimeSpan.FromMinutes(5), stoppingToken);
                Console.WriteLine("[AzureBusListener] Service heartbeat - still running");
            }
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AzureBusListener] Critical error in ExecuteAsync: {ex.Message}");
            Console.WriteLine($"[AzureBusListener] Stack trace: {ex.StackTrace}");
            throw;
        }
    }

    private async Task ErrorHandler(ProcessErrorEventArgs args)
    {
        Console.WriteLine($"[AzureBusListener] Error occurred: {args.Exception.Message}");
        Console.WriteLine($"[AzureBusListener] Error source: {args.ErrorSource}");
        Console.WriteLine($"[AzureBusListener] Entity path: {args.EntityPath}");
        Console.WriteLine($"[AzureBusListener] Full exception: {args.Exception}");

        // You might want to implement additional error handling logic here
        // For now, re-throwing to maintain the existing behavior
        throw args.Exception;
    }

    private async Task ProcessMessageAsync(ProcessMessageEventArgs args)
    {
        var messageId = args.Message.MessageId;
        Console.WriteLine($"[AzureBusListener] Processing message: {messageId}");

        try
        {
            var message = args.Message.Body.ToString();
            Console.WriteLine($"[AzureBusListener] Message content: {message}");

            using (var scope = _scopeFactory.CreateScope())
            {
                var userProfileServices = scope.ServiceProvider.GetRequiredService<UserProfileServices>();

                if (message.Contains("AppUserId"))
                {
                    Console.WriteLine("[AzureBusListener] Processing CREATE user profile request");
                    var request = JsonSerializer.Deserialize<createUserProfileRequest>(message);
                    Console.WriteLine($"[AzureBusListener] Creating profile for AppUserId: {request.AppUserId}");

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
                    Console.WriteLine($"[AzureBusListener] Profile update completed with result: Yes");
                }
                else if (!message.Contains("appUserId"))
                {
                    Console.WriteLine("[AzureBusListener] Processing UPDATE user profile request");
                    var request = JsonSerializer.Deserialize<updateUserProfileRequest>(message);
                    Console.WriteLine($"[AzureBusListener] Updating profile with ID: {request.Id}");

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
                    Console.WriteLine($"[AzureBusListener] Profile update completed with result: Yes");
                }
                else
                {
                    Console.WriteLine("[AzureBusListener] Message format not recognized");
                }
            }

            // Complete the message to remove it from the queue
            await args.CompleteMessageAsync(args.Message);
            Console.WriteLine($"[AzureBusListener] Message {messageId} processed and completed successfully");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"[AzureBusListener] Error processing message {messageId}: {ex.Message}");
            Console.WriteLine($"[AzureBusListener] Stack trace: {ex.StackTrace}");

            // You might want to implement additional error handling here
            // For example, you could choose to abandon the message instead of completing it
            // await args.AbandonMessageAsync(args.Message);
        }
    }

    public override async Task StopAsync(CancellationToken stoppingToken)
    {
        Console.WriteLine("[AzureBusListener] Stopping background service");

        if (_processor != null)
        {
            Console.WriteLine("[AzureBusListener] Stopping message processor");
            await _processor.StopProcessingAsync(stoppingToken);
            await _processor.DisposeAsync();
            Console.WriteLine("[AzureBusListener] Message processor stopped and disposed");
        }

        Console.WriteLine("[AzureBusListener] Background service stopped");
        await base.StopAsync(stoppingToken);
    }
}