using GicCinema.Enums;
using GicCinema.Services.Screen;
using GicCinema.Services.SeatSelection;
using GicCinema.Utility;

namespace GicCinema.Services.UserSelection;

public class BookTicketsService(
    ICinemaService cinemaService,
    IDefaultSeatSelectionService defaultSeatSelectionService,
    ISpecificSeatSelectionService specificSeatSelectionService,
    IScreenService screenService) : IUserSelectionService
{
    private static bool IsResponsible(MenuItemOption menuItemOption) => menuItemOption == MenuItemOption.BookTickets;

    public void Handle(MenuItemOption menuItemOption)
    {
        if (!IsResponsible(menuItemOption)) return;

        Console.WriteLine(AppMessages.NumberOfTickets + AppMessages.BlankMessage);
        
        var inputString = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(inputString)) return;
        
        var numberOfTickets = Convert.ToInt32(inputString);
        var cinema = cinemaService.GetCinema();
        var availableSeats = cinema.AvailableSeats;
        
        if (numberOfTickets > availableSeats)
        {
            Console.WriteLine($"Sorry, there are only {availableSeats} seats available");
            return;
        }

        var newBookingId = defaultSeatSelectionService.ReserveSeats(numberOfTickets);

        Console.WriteLine($"Successfully reserved {numberOfTickets} {cinema.Movie} tickets.");
        Console.WriteLine($"Booking id: {newBookingId}");
        Console.WriteLine("Selected seats: ");
        screenService.Show(newBookingId);
        
        Console.WriteLine(AppMessages.AcceptOrNewSeatSelectionMessage);
        var acceptOrNewSelection = Console.ReadLine();
        if (string.IsNullOrWhiteSpace(acceptOrNewSelection))
        {
            newBookingId = specificSeatSelectionService.ReserveSeats(numberOfTickets, acceptOrNewSelection);
        } 
        newBookingId = defaultSeatSelectionService.ReserveSeats(numberOfTickets);
        screenService.Show(newBookingId);
    }
}