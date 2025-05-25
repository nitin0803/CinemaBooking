using GicCinema.Enums;
using GicCinema.Services.Screen;
using GicCinema.Services.SeatSelection;
using GicCinema.Utility;
using GicCinema.Validator;

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

        Console.WriteLine(CinemaUtility.AppMessage.NumberOfTickets + CinemaUtility.AppMessage.BlankMessage);
        
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
        ShowScreen(newBookingId);
        
        Console.WriteLine(CinemaUtility.AppMessage.AcceptOrNewSeatSelectionMessage);
        var newSeatPosition = Console.ReadLine();
        if (IsUserAcceptedSeatSelection(newSeatPosition))
        {
            ConfirmSeats(newBookingId);
            return;
        }

        while (!IsUserAcceptedSeatSelection(newSeatPosition))
        {
            while (!InputValidator.IsNewSeatPositionValid(cinema.HallLayout.RowLayOuts, newSeatPosition))
            {
                Console.WriteLine("New seating position is not valid! Please try again:");
                newSeatPosition = Console.ReadLine();
            }

            newBookingId = defaultSeatSelectionService.ReserveSeats(numberOfTickets, newSeatPosition);
            ShowScreen(newBookingId);
            Console.WriteLine(CinemaUtility.AppMessage.AcceptOrNewSeatSelectionMessage);
            newSeatPosition = Console.ReadLine();
        }
        ConfirmSeats(newBookingId);
    }

    private bool IsUserAcceptedSeatSelection(string newSeatPosition)
    {
        return string.IsNullOrWhiteSpace(newSeatPosition);
    }

    private void ConfirmSeats(string newBookingId)
    {
        defaultSeatSelectionService.ConfirmSeats(newBookingId);
        Console.WriteLine($"Booking id: {newBookingId} confirmed.");
    }

    private void ShowScreen(string newBookingId)
    {
        Console.WriteLine($"Booking id: {newBookingId}");
        Console.WriteLine("Selected seats: ");
        screenService.Show(newBookingId);
    }
}