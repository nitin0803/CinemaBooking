using GicCinema.Enums;
using GicCinema.Models;

namespace GicCinema.Services.SeatSelection;

public class DefaultSeatSelectionService(ICinemaService cinemaService)
    : IDefaultSeatSelectionService
{
    private Cinema cinema = cinemaService.GetCinema();

    public void ConfirmSeats(string newBookingId)
    {
        var hallLayOut = cinema.HallLayout;
        var reserveSeats = hallLayOut.RowLayOuts.SelectMany(r => r.Seats).Where(s => s.Status == SeatStatus.Reserved);
        foreach (var reserveSeat in reserveSeats)
        {
            reserveSeat.Update(SeatStatus.Confirmed, newBookingId);
        }
    }

    public string ReserveSeats(int numberOfTickets)
    {
        var hallLayOut = cinema.HallLayout;
        var bookings = cinema.Bookings;
        var seatsPerRow = cinema.SeatsPerRow;

        var lastBookingNumber = GetLastBookingNumber();
        var newBookingId = "GIC" + (lastBookingNumber + 1).ToString("D4");
        var filledSeatsCounter = 0;
        foreach (var rowLayOut in hallLayOut.RowLayOuts.Reverse())
        {
            if (filledSeatsCounter == numberOfTickets) break;
            filledSeatsCounter = ReserveSeatsFromMiddle(
                newBookingId,
                numberOfTickets,
                rowLayOut.Seats,
                filledSeatsCounter);
        }

        return newBookingId;

        int GetLastBookingNumber() => bookings.Count == 0 ? 0 : Convert.ToInt32(bookings.Last().bookingId.Substring(3));
    }

    private int ReserveSeatsFromMiddle(
        string newBookingId,
        int numberOfTickets,
        IReadOnlyList<Seat> rowSeats,
        int totalFilledSeats)
    {
        var seatsPerRow = rowSeats.Count;
        var numberOfSeatsFilledInCurrentRow = 0;
        var middleSeatNumber = GetMiddleSeatNumber(rowSeats.Count, numberOfTickets);
        var firstSeatToReserve = GetSeatToReserve(middleSeatNumber);
        ReserveSeat(firstSeatToReserve);
        var rightSeatToReserve = firstSeatToReserve;
        var leftSeatToReserve = firstSeatToReserve;
        for (int i = 1; i <= seatsPerRow; i++)
        {
            if (totalFilledSeats == numberOfTickets || numberOfSeatsFilledInCurrentRow == seatsPerRow) break;
            if (rightSeatToReserve != null)
            {
                rightSeatToReserve = GetSeatToReserve(rightSeatToReserve.SeatNumber);
                if (rightSeatToReserve != null) ReserveSeat(rightSeatToReserve);
            }

            if (totalFilledSeats == numberOfTickets || numberOfSeatsFilledInCurrentRow == seatsPerRow) break;
            if (leftSeatToReserve != null)
            {
                leftSeatToReserve = GetSeatToReserve(leftSeatToReserve.SeatNumber, DirectionSide.Left);
                if (leftSeatToReserve != null) ReserveSeat(leftSeatToReserve);
            }
        }

        Seat? GetSeatToReserve(int seatNumber, DirectionSide directionSide = DirectionSide.Right)
        {
            var seatToReserve = rowSeats.Single(s => s.SeatNumber == seatNumber);
            while (seatToReserve.Status != SeatStatus.Empty)
            {
                var nextSeatNumberToReserve = directionSide == DirectionSide.Right
                    ? seatToReserve.SeatNumber + 1
                    : seatToReserve.SeatNumber - 1;

                if (HasRowLimitReached(nextSeatNumberToReserve))
                {
                    seatToReserve = null;
                    break;
                }

                seatToReserve = rowSeats.Single(s => s.SeatNumber == nextSeatNumberToReserve);
            }

            return seatToReserve;
        }

        bool HasRowLimitReached(int nextPossibleSeatNumberToReserve)
        {
            return nextPossibleSeatNumberToReserve > seatsPerRow || nextPossibleSeatNumberToReserve < 1;
        }

        void ReserveSeat(Seat seatToReserve)
        {
            seatToReserve.Update(SeatStatus.Reserved, newBookingId);
            totalFilledSeats++;
            numberOfSeatsFilledInCurrentRow++;
        }

        return totalFilledSeats;
    }

    private int GetMiddleSeatNumber(int seatsPerRow, int numberOfTickets)
    {
        if (seatsPerRow % 2 != 0) return (seatsPerRow / 2) + 1;
        var medianSeatNumber = seatsPerRow / 2;
        return numberOfTickets % 2 == 0 ? medianSeatNumber : medianSeatNumber + 1;
    }
}