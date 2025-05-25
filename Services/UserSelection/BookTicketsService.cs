using GicCinema.Enums;
using GicCinema.Services.Screen;
using GicCinema.Services.SeatSelection;
using GicCinema.Utility;

namespace GicCinema.Services.UserSelection;

public class BookTicketsService(
    ICinemaService cinemaService,
    ISeatSelectionService seatSelectionService,
    IScreenService screenService) : IUserSelectionService
{
    private static bool IsResponsible(MenuItemOption menuItemOption) => menuItemOption == MenuItemOption.BookTickets;

    public void Handle(MenuItemOption menuItemOption)
    {
        if (!IsResponsible(menuItemOption)) return;

        Console.WriteLine(CinemaUtility.AppMessage.NumberOfTickets + CinemaUtility.AppMessage.BlankMessage);
        
        var inputString = Console.ReadLine();
        Console.WriteLine();
        if (string.IsNullOrWhiteSpace(inputString)) return;
        
        var numberOfTickets = Convert.ToInt32(inputString);
        var cinema = cinemaService.GetCinema();
        var availableSeats = cinema.AvailableSeats;
        
        if (numberOfTickets > availableSeats)
        {
            Console.WriteLine($"Sorry, there are only {availableSeats} seats available");
            Console.WriteLine();
            return;
        }

        var newBookingId = seatSelectionService.ReserveSeats(numberOfTickets);
        Console.WriteLine($"Successfully reserved {numberOfTickets} {cinema.Movie} tickets.");
        ShowScreen(newBookingId);
        
        Console.WriteLine(CinemaUtility.AppMessage.AcceptOrNewSeatSelectionMessage);
        
        var newSeatPosition = Console.ReadLine();
        Console.WriteLine();
        if (HasUserAcceptedSeatSelection(newSeatPosition))
        {
            ConfirmSeats(newBookingId);
            return;
        }
        
        while (!HasUserAcceptedSeatSelection(newSeatPosition))
        {
            seatSelectionService.FreeSeats(newBookingId);
            while (!CinemaUtility.IsNewSeatPositionValid(cinema.HallLayout.RowLayOuts, newSeatPosition!))
            {
                Console.WriteLine("New seating position is not valid! Please try again:");
                newSeatPosition = Console.ReadLine();
                Console.WriteLine();
            }

            newBookingId = seatSelectionService.ReserveSeats(numberOfTickets, newSeatPosition);
            ShowScreen(newBookingId);
            
            Console.WriteLine(CinemaUtility.AppMessage.AcceptOrNewSeatSelectionMessage);
            newSeatPosition = Console.ReadLine();
            Console.WriteLine();
        }
        
        ConfirmSeats(newBookingId);
    }

    private bool HasUserAcceptedSeatSelection(string? newSeatPosition)
    {
        return string.IsNullOrWhiteSpace(newSeatPosition);
    }

    private void ConfirmSeats(string newBookingId)
    {
        seatSelectionService.ConfirmSeats(newBookingId);
        Console.WriteLine($"Booking id: {newBookingId} confirmed.");
        Console.WriteLine();
    }

    private void ShowScreen(string newBookingId)
    {
        screenService.Show(newBookingId);
        Console.WriteLine();
    }
}