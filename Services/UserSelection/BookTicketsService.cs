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

        Console.WriteLine(CinemaUtility.AppMessage.NumberOfTickets + CinemaUtility.AppMessage.Blank);
        
        var numberOfTicketsInput = Console.ReadLine();
        Console.WriteLine();
        if (string.IsNullOrWhiteSpace(numberOfTicketsInput)) return;

        while (!CinemaUtility.IsNumberOfTicketsValid(numberOfTicketsInput!))
        {
            Console.WriteLine(CinemaUtility.ValidationMessage.InvalidNumberOfTickets);
            numberOfTicketsInput = Console.ReadLine();
            Console.WriteLine();
        }
        
        var numberOfTickets = Convert.ToInt32(numberOfTicketsInput);
        var cinema = cinemaService.GetCinema();
        var availableSeats = cinema.AvailableSeats;
        
        if (numberOfTickets > availableSeats)
        {
            Console.WriteLine(CinemaUtility.AppMessage.SeatsAvailabilityAlert, availableSeats);
            Console.WriteLine();
            return;
        }

        var newBookingId = seatSelectionService.ReserveSeats(numberOfTickets);
        Console.WriteLine(CinemaUtility.AppMessage.TicketsReserved, numberOfTickets, cinema.Movie);
        ShowScreen(newBookingId);
        
        Console.WriteLine(CinemaUtility.AppMessage.AcceptOrNewSeatSelection);
        
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
                Console.WriteLine(CinemaUtility.ValidationMessage.InvalidSeatingPosition);
                newSeatPosition = Console.ReadLine();
                Console.WriteLine();
            }

            newBookingId = seatSelectionService.ReserveSeats(numberOfTickets, newSeatPosition);
            ShowScreen(newBookingId);
            
            Console.WriteLine(CinemaUtility.AppMessage.AcceptOrNewSeatSelection);
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
        Console.WriteLine(CinemaUtility.AppMessage.BookingIdConfirmed, newBookingId);
        Console.WriteLine();
    }

    private void ShowScreen(string newBookingId)
    {
        screenService.Show(newBookingId);
        Console.WriteLine();
    }
}