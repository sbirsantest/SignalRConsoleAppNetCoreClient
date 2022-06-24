using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace SignalRConsoleAppNetCoreClient
{
    public enum OrderState
    {
        OVERDUE = 0,
        READYFORINPUT = 1,
        READYFORPICKUP = 2,
        PICKEDUP = 3,
        CANCELED = 4,
        SCHEDULED = 9
    }

    class Program
    {
        private static HubConnection _hubConnection;
        //private static string _baseHubUrl = "http://localhost:5000"; // the url from project SignalRServer
        private static string _webApiHubUrl = "http://localhost:5081"; // the url from BD WebApi

        //static async Task Main(string[] args)
        //{
        //    Console.WriteLine("Hello World!");

        //    // TO DO: put this in a try catch in a separate method
        //    // create the hub connection
        //    _hubConnection = new HubConnectionBuilder()
        //        .WithUrl($"{_baseHubUrl}/hubs/backgroundColor", Microsoft.AspNetCore.Http.Connections.HttpTransportType.LongPolling) //the url from MapHub in the project SignalRServer
        //        .Build();

        //    _hubConnection.On<string>("ChangeBackground", (color) =>
        //    {
        //        switch (color.ToUpper())
        //        {
        //            case "RED": Console.BackgroundColor = ConsoleColor.Red; break;
        //            case "GREEN": Console.BackgroundColor = ConsoleColor.Green; break;
        //            case "BLUE": Console.BackgroundColor = ConsoleColor.Blue; break;
        //            default: Console.BackgroundColor = ConsoleColor.Black;break;
        //        }

        //        Console.ForegroundColor = ConsoleColor.White;
        //        Console.WriteLine($"Changed colour to {color}");
        //    });

        //    await _hubConnection.StartAsync();

        //    var keepGoing = true;
        //    do
        //    {
        //        var key = Console.ReadKey();
        //        Console.WriteLine();
        //        switch (key.Key)
        //        {
        //            case ConsoleKey.R:
        //                await _hubConnection.SendAsync("ChangeBackground", "red");
        //                break;
        //            case ConsoleKey.G:
        //                await _hubConnection.SendAsync("ChangeBackground", "green");
        //                break;
        //            case ConsoleKey.B:
        //                await _hubConnection.SendAsync("ChangeBackground", "blue");
        //                break;
        //            case ConsoleKey.X:
        //                keepGoing = false;
        //                break;
        //            default:
        //                Console.WriteLine("Not a valid color");
        //                break;
        //        }
        //    } while (keepGoing);

        //    //stop the connection
        //    await _hubConnection.StopAsync();
        //}

        static async Task Main(string[] args)
        {
            //Console.WriteLine("Hello World!");

            try
            {
                // TO DO: put this in a try catch in a separate method
                // create the hub connection
                //_hubConnection = new HubConnectionBuilder()
                //    .WithUrl($"{_webApiHubUrl}/rt-api/orderstatechanged", config => config.Headers.Add("x-api-key", "0000000000-000000qm00-0000040000-0000000000-th1s01s0de-v0"))
                //    .Build();
                if (args.Length != 1)
                {
                    Console.WriteLine("Plese give an ApiKey as argument in order to can connect to WebAPi SignalR!");
                    return;
                }

                if (args[0] == "0000000000-000000qm00-0000040000-0000000000-th1s01s0de-v0")
                    Console.WriteLine("Developer - localdev is connected");
                else
                    if (args[0] == "0000000000-000000qm00-0000080000-0000000000-th1s01s0de-v0")
                        Console.WriteLine("Synctool is connected");
                    else
                        Console.WriteLine("A new client is connected");

                _hubConnection = new HubConnectionBuilder()
                    .WithUrl($"{_webApiHubUrl}/rt-api/orderstatechanged?access_token=apikey-{args[0]}")
                    .Build();

#if DEBUG
                _hubConnection.On<string, string, string>("OrderStateChanged", (orderNumber, orderState, extraInfo) =>
                {
                    var changedAt = String.Empty;
                    var changedFrom = String.Empty;
                    (changedAt, changedFrom) = ParseExtraInfo(extraInfo);
                    Console.WriteLine($"Order with orderNumber = '{orderNumber}' was changed at '{changedAt}' from '{changedFrom}' and its state is '{orderState}'");
                });
#else
            _hubConnection.On<string, string>("OrderStateChanged", (orderNumber, orderState) =>
            {
                Console.WriteLine($"Order with orderNumber = '{orderNumber}'  has state = '{orderState}'");
            });

#endif
                await _hubConnection.StartAsync();

                var keepGoing = true;
                do
                {
                    //var key = Console.ReadKey();
                    //Console.WriteLine();
                    //if (key.Key == ConsoleKey.X)
                    //{
                    //    keepGoing = false;
                    //}
                } while (keepGoing);

                //stop the connection
                await _hubConnection.StopAsync();

            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
        }

        private static (string, string) ParseExtraInfo(string extraInfo)
        {
            var tmp = extraInfo?.Split('$', StringSplitOptions.RemoveEmptyEntries);
            if (tmp.Length == 2)
            {
                return (tmp[0], tmp[1]);
            }
            else
            {
                var msg = "bad extra info!";
                //_logger.LogError(msg);

                throw new InvalidOperationException(msg);
            }
        }

    }
}
