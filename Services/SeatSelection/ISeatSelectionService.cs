namespace GicCinema.Services.SeatSelection;

public interface ISeatSelectionService
{
    string ReserveSeats(int numberOfTickets, string? newSeatPosition = null);
    void ConfirmSeats(string bookingId);
    void FreeSeats(string newBookingId);
}