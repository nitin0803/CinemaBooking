namespace GicCinema.Services.SeatSelection;

public interface IDefaultSeatSelectionService
{
    string ReserveSeats(int numberOfTickets);
}