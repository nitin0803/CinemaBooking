using GicCinema.Models;

namespace GicCinema.Services;

public class CinemaService : ICinemaService
{
    public Cinema CreateCinema(string movie, int rows, int seatsPerRow)
    {
        return Cinema.Create(movie, rows, seatsPerRow);
    }

    public Cinema GetCinema()
    {
        return Cinema.GetCinema();
    }

    public void AddBooking(Booking booking)
    {
        GetCinema().Bookings.Add(booking);
    }

    public Booking? TryGetBooking(string bookingId)
    {
        try
        {
            return GetCinema().Bookings.SingleOrDefault(b => b.BookingId == bookingId); 
        }
        catch (Exception)
        {
            Console.WriteLine("Exception occurred as duplicate booking entries found in cinema");
            throw;
        }
    }
}