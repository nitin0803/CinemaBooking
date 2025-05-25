namespace GicCinema.Services.SeatSelection;

public interface IDefaultSeatSelectionService
{
    string ReserveSeats(int numberOfTickets, string? newSeatPosition = null);
    void ConfirmSeats(string bookingId);
}