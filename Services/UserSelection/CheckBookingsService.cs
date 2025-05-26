using System.Text.RegularExpressions;
using GicCinema.Enums;
using GicCinema.Services.Screen;
using GicCinema.Utility;

namespace GicCinema.Services.UserSelection;

public class CheckBookingsService(ICinemaService cinemaService, IScreenService screenService) : IUserSelectionService
{
    private const string BookingIdPattern = @"^GIC\d{4}$";

    public void Handle(MenuItemOption menuItemOption)
    {
        if (!IsResponsible(menuItemOption)) return;
        Console.WriteLine(CinemaUtility.AppMessage.BookingId + CinemaUtility.AppMessage.Blank);
        var bookingId = Console.ReadLine();
        Console.WriteLine();
        while (!string.IsNullOrWhiteSpace(bookingId))
        {
            ShowBooking(bookingId);
            Console.WriteLine();
            Console.WriteLine(CinemaUtility.AppMessage.BookingId + CinemaUtility.AppMessage.Blank);
            bookingId = Console.ReadLine();
            Console.WriteLine();
        }
    }

    private static bool IsResponsible(MenuItemOption menuItemOption) => menuItemOption == MenuItemOption.CheckBookings;

    private void ShowBooking(string bookingId)
    {
        while (string.IsNullOrWhiteSpace(bookingId) || !Regex.IsMatch(bookingId, BookingIdPattern))
        {
            Console.WriteLine(CinemaUtility.ValidationMessage.InvalidBookingIdFormat);
            bookingId = Console.ReadLine() ?? string.Empty;
            Console.WriteLine();
        }

        var booking = cinemaService.TryGetBooking(bookingId);
        if (booking == null)
        {
            Console.WriteLine(CinemaUtility.AppMessage.NoBookingFound, bookingId);
            return;
        }

        screenService.Show(bookingId);
    }
}