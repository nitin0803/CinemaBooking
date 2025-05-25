// See https://aka.ms/new-console-template for more information

using GicCinema.Enums;
using GicCinema.Services;
using GicCinema.Services.Screen;
using GicCinema.Services.SeatSelection;
using GicCinema.Services.UserSelection;
using GicCinema.Utility;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine(CinemaUtility.AppMessage.DefineCinema);
var inputString = Console.ReadLine();
while (!CinemaUtility.IsInputValid(inputString))
{
    Console.WriteLine(CinemaUtility.AppMessage.DefineCinema);
    inputString = Console.ReadLine();
}

var inputArray = inputString.Split(" ");
var movie = inputArray[0];
var rows = int.Parse(inputArray[1]);
var seatsPerRow = int.Parse(inputArray[2]);

var serviceProvider = RegisterDependencies();

var cinemaService = serviceProvider.GetRequiredService<ICinemaService>();
var cinema = cinemaService.CreateCinema(inputArray[0],rows, seatsPerRow);

MenuItemOption menuItemOption = MenuItemOption.None;
while (menuItemOption != MenuItemOption.Exit)
{
    Console.WriteLine(CinemaUtility.AppMessage.WelcomeMessage);
    Console.WriteLine($"[1] Book tickets for {cinema.Movie} ({cinema.AvailableSeats} seats available)");
    Console.WriteLine("[2] Check bookings");
    Console.WriteLine("[3] Exit");
    Console.WriteLine("Please enter your selection:");

    var menuItemSelection = Console.ReadLine();
    if(!Enum.TryParse(menuItemSelection, out menuItemOption)
       || !Enum.GetValues<MenuItemOption>().Contains(menuItemOption))
    {
        Console.WriteLine(CinemaUtility.ValidationMessage.InvalidSelection);
        Console.WriteLine();
        menuItemOption = MenuItemOption.None;
        continue;
    }
    
    var userSelectionServices = serviceProvider.GetServices<IUserSelectionService>();
    foreach (var userSelectionService in userSelectionServices)
    {
        userSelectionService.Handle(menuItemOption);
    }
}

ServiceProvider RegisterDependencies()
{
    var buildServiceProvider = new ServiceCollection()
        .AddSingleton<ICinemaService, CinemaService>()
        .AddTransient<IUserSelectionService, BookTicketsService>()
        .AddTransient<IUserSelectionService, CheckBookingsService>()
        .AddTransient<IUserSelectionService, ExitService>()
        .AddTransient<ISeatSelectionService, SeatSelectionService>()
        .AddTransient<IScreenService, ScreenService>()
        .BuildServiceProvider();
    return buildServiceProvider;
}