using System.Text;
using GicCinema.Enums;
using GicCinema.Models;

namespace GicCinema.Services.Screen;

public class ScreenService(ICinemaService cinemaService) : IScreenService
{
    public void Show(string currentBookingId)
    {
        var cinema = cinemaService.GetCinema();
        var totalRows = cinema.TotalRows;
        var seatsPerRow = cinema.SeatsPerRow;
        
        Console.WriteLine($"Booking id: {currentBookingId}");
        Console.WriteLine($"Selected seats: ");
        
        Console.WriteLine("         S C R E E N                  ");
        var separator = new StringBuilder();
        for (var i =0; i <= seatsPerRow*3 + 2; i++)
        {
            separator.Append('-');
        }
        Console.WriteLine(separator);
        var hallLayout = cinema.HallLayout;
        var rowLayOuts = hallLayout.RowLayOuts;
        foreach (var rowLayOut in rowLayOuts)
        {
            Console.Write($"{rowLayOut.RowLabel} ");
            foreach (var seat in rowLayOut.Seats)
            {
                var seatSymbol = GetSeatSymbol(seat, currentBookingId);
                Console.Write(seatSymbol);
            }
            Console.WriteLine(); // New line after each row
        }
        var seatNumber = new StringBuilder();
        var anyRowLayout = rowLayOuts.First();
        foreach (var seat in anyRowLayout.Seats)
        {
            seatNumber.Append(seat.SeatNumber + "  ");
        }
        Console.Write("   " + seatNumber);
        Console.WriteLine(); // New line after each row
    }

    private static string GetSeatSymbol(Seat seat, string currentBookingId)
    {
        return seat switch
        {
            { Status: SeatStatus.Empty } => " . ",
            { BookingId: var bookingId } when (bookingId == currentBookingId) => " o ",
            _ => " # "
        };
    }
}