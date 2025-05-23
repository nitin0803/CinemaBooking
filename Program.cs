// See https://aka.ms/new-console-template for more information

using GicCinema.Enums;
using GicCinema.Services;
using GicCinema.Services.Screen;
using GicCinema.Services.SeatSelection;
using GicCinema.Services.UserSelection;
using GicCinema.Utility;
using GicCinema.Validator;
using Microsoft.Extensions.DependencyInjection;

Console.WriteLine(AppMessages.DefineCinema);
var inputString = Console.ReadLine();
while (!InputValidator.IsInputValid(inputString))
{
    Console.WriteLine(AppMessages.DefineCinema);
    inputString = Console.ReadLine();
}

var inputArray = inputString.Split(" ");
var movie = inputArray[0];
var rows = int.Parse(inputArray[1]);
var seatsPerRow = int.Parse(inputArray[2]);

var serviceProvider = RegisterDependencies();

var cinemaService = serviceProvider.GetRequiredService<ICinemaService>();
var cinema = cinemaService.CreateCinema(inputArray[0],rows, seatsPerRow);

var menuItemOption = MenuItemOption.None;
while (menuItemOption != MenuItemOption.Exit)
{
    Console.WriteLine(AppMessages.WelcomeMessage);
    Console.WriteLine($"[1] Book tickets for {cinema.Movie} ({cinemaService.GetCinema().AvailableSeats} seats available)");
    Console.WriteLine("[2] Check bookings");
    Console.WriteLine("[3] Exit");
    Console.WriteLine("Please enter your selection:");
    var isMenuItemOptionValid = Enum.TryParse(Console.ReadLine(), out menuItemOption); //TODO
    menuItemOption = isMenuItemOptionValid ? menuItemOption : MenuItemOption.None;
    
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
        .AddTransient<IDefaultSeatSelectionService, DefaultSeatSelectionService>()
        .AddTransient<ISpecificSeatSelectionService, SpecificSpecificSeatSelectionService>()
        .AddTransient<IUserSelectionService, BookTicketsService>()
        .AddTransient<IUserSelectionService, CheckBookingsService>()
        .AddTransient<IUserSelectionService, ExitService>()
        .AddTransient<IScreenService, ScreenService>()
        .BuildServiceProvider();
    return buildServiceProvider;
}