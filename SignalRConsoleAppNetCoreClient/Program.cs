using Microsoft.AspNetCore.SignalR.Client;
using System;
using System.Threading.Tasks;

namespace SignalRConsoleAppNetCoreClient
{
    class Program
    {
        private static HubConnection _hubConnection;
        private static string _baseHubUrl = "http://localhost:5000"; // the url from project SignalRServer

        static async Task Main(string[] args)
        {
            Console.WriteLine("Hello World!");

            // TO DO: put this in a try catch in a separate method
            // create the hub connection
            _hubConnection = new HubConnectionBuilder()
                .WithUrl($"{_baseHubUrl}/hubs/backgroundColor") //the url from MapHub in the project SignalRServer
                .Build();

            _hubConnection.On<string>("ChangeBackground", (color) =>
            {
                switch (color.ToUpper())
                {
                    case "RED": Console.BackgroundColor = ConsoleColor.Red; break;
                    case "GREEN": Console.BackgroundColor = ConsoleColor.Green; break;
                    case "BLUE": Console.BackgroundColor = ConsoleColor.Blue; break;
                    default: Console.BackgroundColor = ConsoleColor.Black;break;
                }

                Console.ForegroundColor = ConsoleColor.White;
                Console.WriteLine($"Changed colour to {color}");
            });

            await _hubConnection.StartAsync();

            var keepGoing = true;
            do
            {
                var key = Console.ReadKey();
                Console.WriteLine();
                switch (key.Key)
                {
                    case ConsoleKey.R:
                        await _hubConnection.SendAsync("ChangeBackground", "red");
                        break;
                    case ConsoleKey.G:
                        await _hubConnection.SendAsync("ChangeBackground", "green");
                        break;
                    case ConsoleKey.B:
                        await _hubConnection.SendAsync("ChangeBackground", "blue");
                        break;
                    case ConsoleKey.X:
                        keepGoing = false;
                        break;
                    default:
                        Console.WriteLine("Not a valid color");
                        break;
                }
            } while (keepGoing);

            //stop the connection
            await _hubConnection.StopAsync();
        }
    }
}
