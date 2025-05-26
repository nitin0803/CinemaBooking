using GicCinema.Enums;
using GicCinema.Services;
using GicCinema.Services.Screen;
using GicCinema.Services.SeatSelection;
using GicCinema.Services.UserSelection;
using GicCinema.Utility;
using Microsoft.Extensions.DependencyInjection;

var serviceProvider = RegisterDependencies();

Console.WriteLine(CinemaUtility.AppMessage.DefineCinema);

var inputString = Console.ReadLine();
Console.WriteLine();

while (!CinemaUtility.AreCinemaDetailsValid(inputString))
{
    Console.WriteLine(CinemaUtility.AppMessage.DefineCinema);
    inputString = Console.ReadLine();
    Console.WriteLine();
}

var inputArray = inputString!.Split(" ");
var rows = int.Parse(inputArray[1]);
var seatsPerRow = int.Parse(inputArray[2]);

var cinemaService = serviceProvider.GetRequiredService<ICinemaService>();
var cinema = cinemaService.CreateCinema(inputArray[0], rows, seatsPerRow);

MenuItemOption menuItemOption = MenuItemOption.None;
while (menuItemOption != MenuItemOption.Exit)
{
    Console.WriteLine(CinemaUtility.AppMessage.Welcome);
    Console.WriteLine(CinemaUtility.MenutItem.BookTickets, cinema.Movie,cinema.AvailableSeats);
    Console.WriteLine(CinemaUtility.MenutItem.CheckBookings);
    Console.WriteLine(CinemaUtility.MenutItem.Exit);
    Console.WriteLine(CinemaUtility.AppMessage.EnterSelection);

    var menuItemSelection = Console.ReadLine();
    Console.WriteLine();
    
    if (!Enum.TryParse(menuItemSelection, out menuItemOption)
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