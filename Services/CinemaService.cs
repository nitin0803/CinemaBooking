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
        var cinema = Cinema.GetCinema();
        if (cinema == null)
        {
            Console.WriteLine("Cannot add booking booking as no cinama available!");
            throw new Exception("No Cinema Found"); // create exception classes.
        }
        
        cinema.Bookings.Add(booking);
    }

    public Booking? TryGetBooking(int bookingNumber)
    {
        var cinema = Cinema.GetCinema();
        try
        {
            if (cinema == null)
            {
                Console.WriteLine("Cannot find booking as no cinama available!");
                throw new Exception("No Cinema Found"); // create exception classes.
            }

            return cinema.Bookings.SingleOrDefault(b => b.Number == bookingNumber); 
        }
        catch (Exception e)
        {
            Console.WriteLine("Exception occurred as no or duplicate booking entries found in cinema");
            throw;
        }
    }
}