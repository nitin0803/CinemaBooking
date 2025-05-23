using GicCinema.Enums;
using GicCinema.Models;

namespace GicCinema.Services.SeatSelection;

public class DefaultSeatSelectionService(ICinemaService cinemaService) : IDefaultSeatSelectionService
{
    public string ReserveSeats(int numberOfTickets)
    {
        var cinema = cinemaService.GetCinema();
        var hallLayOut = cinema.HallLayout;
        var bookings = cinema.Bookings;
        var seatsPerRow = cinema.SeatsPerRow;

        var newBookingId = "GIC" + (bookings.Last().Number + 1).ToString("D4");
        var filledSeatsCounter = 0;
        foreach (var rowLayOut in hallLayOut.RowLayOuts.Reverse())
        {
            var rowSeats = rowLayOut.Seats;
            var middleSeatNumber = GetMiddleSeatNumber(rowSeats.Count);
            var middleSeat = rowSeats.Single(s => s.SeatNumber == middleSeatNumber);
            for (int i = 1; i <= numberOfTickets; i++)
            {
                if (filledSeatsCounter == numberOfTickets) break;
                filledSeatsCounter = ReserveSeat(middleSeat, newBookingId, filledSeatsCounter);

                if (filledSeatsCounter == numberOfTickets) break;
                var rightSideSeat = rowSeats.Single(s => s.SeatNumber == middleSeat.SeatNumber + i);
                filledSeatsCounter = ReserveSeat(rightSideSeat, newBookingId, filledSeatsCounter);

                if (filledSeatsCounter == numberOfTickets) break;
                var leftSideSeat = rowSeats.Single(s => s.SeatNumber == middleSeat.SeatNumber - i);
                filledSeatsCounter = ReserveSeat(leftSideSeat, newBookingId, filledSeatsCounter);
            }
        }

        return newBookingId;
    }

    private int GetMiddleSeatNumber(int rowSeats)
    {
        if (rowSeats % 2 == 0) return rowSeats / 2;
        return rowSeats / 2;
    }

    private int ReserveSeat(Seat seat, string newBookingId, int filledSeatsCounter)
    {
        if (seat.Status == SeatStatus.Empty)
        {
            seat.Update(SeatStatus.Reserved, newBookingId);
            filledSeatsCounter++;
        }

        return filledSeatsCounter;
    }
}