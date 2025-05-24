using GicCinema.Models;

namespace GicCinema.Services;

public interface ICinemaService
{
    Cinema CreateCinema(string movie, int rows, int seatsPerRow);
    Cinema GetCinema();
    void AddBooking(Booking booking);
    Booking? TryGetBooking(string bookingId);
}