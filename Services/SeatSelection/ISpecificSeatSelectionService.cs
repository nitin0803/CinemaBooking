namespace GicCinema.Services.SeatSelection;

public interface ISpecificSeatSelectionService
{
    string ReserveSeats(int numberOfTickets, string? startingPosition = null);
}