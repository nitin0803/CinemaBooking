using System.Text.RegularExpressions;
using GicCinema.Enums;
using GicCinema.Services.Screen;
using GicCinema.Utility;

namespace GicCinema.Services.UserSelection;

public class CheckBookingsService(ICinemaService cinemaService, IScreenService screenService) : IUserSelectionService
{
    private const string BookingIdPattern = @"^GIC\d{4}$";
    public void Handle(Enums.MenuItemOption menuItemOption)
    {
        if (!IsResponsible(menuItemOption)) return;
        Console.WriteLine(AppMessages.BookingIdMessage + AppMessages.BlankMessage);
        var bookingId = Console.ReadLine();
        if(string.IsNullOrWhiteSpace(bookingId)) return;
        ShowBooking(bookingId);
    }

    private static bool IsResponsible(MenuItemOption menuItemOption) => menuItemOption == MenuItemOption.CheckBookings;

    private void ShowBooking(string bookingId)
    {
        while (!Regex.IsMatch(bookingId, BookingIdPattern))
        {
            Console.WriteLine("Entered bookingId is not in correct format, please try again!");
            bookingId = Console.ReadLine();
        }
        
        var booking = cinemaService.TryGetBooking(bookingId);
        if (booking == null)
        {
            Console.WriteLine($"No booking Found for entered booking id: {bookingId}");
        }
        screenService.Show(bookingId);
    }
}