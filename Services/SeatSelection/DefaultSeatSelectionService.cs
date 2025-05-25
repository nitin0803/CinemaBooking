using GicCinema.Enums;
using GicCinema.Models;
using GicCinema.Utility;

namespace GicCinema.Services.SeatSelection;

public class DefaultSeatSelectionService(ICinemaService cinemaService)
    : IDefaultSeatSelectionService
{
    public string ReserveSeats(int numberOfTickets, string? newSeatPosition)
    {
        var cinema = cinemaService.GetCinema();
        var hallLayOut = cinema.HallLayout;
        var bookings = cinema.Bookings;
        var seatsPerRow = cinema.SeatsPerRow;

        var lastBookingNumber = GetLastBookingNumber();
        var newBookingId = "GIC" + (lastBookingNumber + 1).ToString("D4");
        var filledSeatsCounter = 0;
        var rowLayoutsSequence = string.IsNullOrWhiteSpace(newSeatPosition)
            ? hallLayOut.RowLayOuts.Reverse()
            : CinemaUtility.GetRowLayOutsSequence(hallLayOut.RowLayOuts, newSeatPosition);

        foreach (var rowLayOut in rowLayoutsSequence)
        {
            if (filledSeatsCounter == numberOfTickets) break;

            filledSeatsCounter = !string.IsNullOrWhiteSpace(newSeatPosition)
                                 && IsNewSeatPositionBelongsToCurrentRow(rowLayOut)
                ? ReserveSeatsFromNewSeatPosition(
                    newBookingId,
                    numberOfTickets,
                    rowLayOut.Seats,
                    filledSeatsCounter,
                    newSeatPosition)
                : ReserveSeatsFromMiddle(
                    newBookingId,
                    numberOfTickets,
                    rowLayOut.Seats,
                    filledSeatsCounter);
            ;
        }

        return newBookingId;

        int GetLastBookingNumber() => bookings.Count == 0 ? 0 : Convert.ToInt32(bookings.Last().bookingId.Substring(3));

        bool IsNewSeatPositionBelongsToCurrentRow(RowLayOut currentRow)
        {
            var newSeatPositionRowLabel = CinemaUtility.GetNewSeatPositionRowLabel(newSeatPosition);
            return currentRow.RowLabel.Equals(newSeatPositionRowLabel);
        }
    }

    public void ConfirmSeats(string newBookingId)
    {
        var cinema = cinemaService.GetCinema();
        var hallLayOut = cinema.HallLayout;
        var reserveSeats = hallLayOut.RowLayOuts
            .SelectMany(r => r.Seats)
            .Where(s => string.Equals(s.BookingId, newBookingId) && s.Status == SeatStatus.Reserved)
            .ToList();
        foreach (var reserveSeat in reserveSeats)
        {
            reserveSeat.Update(SeatStatus.Confirmed, newBookingId);
        }

        var numberOfConfirmedSeats = hallLayOut.RowLayOuts
            .SelectMany(r => r.Seats)
            .Count(s => string.Equals(s.BookingId, newBookingId) && s.Status == SeatStatus.Confirmed);
        cinemaService.AddBooking(new Booking(newBookingId, numberOfConfirmedSeats));
    }
    
    public void FreeSeats(string newBookingId)
    {
        var cinema = cinemaService.GetCinema();
        var hallLayOut = cinema.HallLayout;
        var reserveSeats = hallLayOut.RowLayOuts.SelectMany(r => r.Seats)
            .Where(s => s.Status == SeatStatus.Reserved)
            .ToList();
        foreach (var reserveSeat in reserveSeats)
        {
            reserveSeat.Update(SeatStatus.Empty, newBookingId);
        }
    }

    
    private int ReserveSeatsFromMiddle(
        string newBookingId,
        int numberOfTickets,
        IReadOnlyList<Seat> seatsToFill,
        int totalFilledSeats)
    {
        var seatsPerRow = seatsToFill.Count;
        var numberOfSeatsFilledInCurrentRow = 0;
        var middleSeatNumber = GetMiddleSeatNumber(seatsPerRow, numberOfTickets);
        var firstSeatToReserve = GetSeatToReserve(middleSeatNumber);

        if (firstSeatToReserve == null)
        {
            firstSeatToReserve = GetSeatToReserve(middleSeatNumber, DirectionSide.Left);
        }

        if (firstSeatToReserve == null) return totalFilledSeats;

        ReserveSeat(firstSeatToReserve);
        var rightSeatToReserve = firstSeatToReserve;
        var leftSeatToReserve = firstSeatToReserve;
        for (var i = 1; i <= seatsPerRow; i++)
        {
            if (totalFilledSeats == numberOfTickets || numberOfSeatsFilledInCurrentRow == seatsPerRow) break;

            if (rightSeatToReserve != null)
            {
                rightSeatToReserve = GetSeatToReserve(rightSeatToReserve.SeatNumber);
                if (rightSeatToReserve != null) ReserveSeat(rightSeatToReserve);
            }

            if (totalFilledSeats == numberOfTickets || numberOfSeatsFilledInCurrentRow == seatsPerRow) break;

            if (leftSeatToReserve == null) continue;
            leftSeatToReserve = GetSeatToReserve(leftSeatToReserve.SeatNumber, DirectionSide.Left);
            if (leftSeatToReserve != null) ReserveSeat(leftSeatToReserve);
        }

        return totalFilledSeats;

        Seat? GetSeatToReserve(int seatNumber, DirectionSide directionSide = DirectionSide.Right)
        {
            var seatToReserve = seatsToFill.Single(s => s.SeatNumber == seatNumber);
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

                seatToReserve = seatsToFill.Single(s => s.SeatNumber == nextSeatNumberToReserve);
            }

            return seatToReserve;

            bool HasRowLimitReached(int nextPossibleSeatNumberToReserve)
            {
                return nextPossibleSeatNumberToReserve > seatsPerRow || nextPossibleSeatNumberToReserve < 1;
            }
        }

        void ReserveSeat(Seat seatToReserve)
        {
            seatToReserve.Update(SeatStatus.Reserved, newBookingId);
            totalFilledSeats++;
            numberOfSeatsFilledInCurrentRow++;
        }
    }

    private int ReserveSeatsFromNewSeatPosition(
        string newBookingId,
        int numberOfTickets,
        IReadOnlyList<Seat> rowSeats,
        int totalFilledSeats,
        string newSeatPosition)
    {
        // consider loop of seats from new seat position to till last
        var newSeatPositionNumber = CinemaUtility.GetNewSeatPositionNumber(newSeatPosition);
        var seatsToFill = rowSeats
            .Where(s => s.SeatNumber >= newSeatPositionNumber)
            .ToList();
        // get first seat to reserve
        // keep checking for right side seats to reserve
        // if seatNumber exceeds seatsPerRow exit
        
        var seatsToFillCount = seatsToFill.Count;
        var numberOfSeatsFilledInCurrentRow = 0;
        
        var firstSeatToReserve = GetSeatToReserve(newSeatPositionNumber);

        if (firstSeatToReserve == null) return totalFilledSeats;

        ReserveSeat(firstSeatToReserve);
        
        var nextRightSeatToReserve = firstSeatToReserve;
        for (int i = 1; i <= seatsToFillCount; i++)
        {
            if (totalFilledSeats == numberOfTickets || numberOfSeatsFilledInCurrentRow == seatsToFillCount) break;

            var nextRightSeatNumber = nextRightSeatToReserve.SeatNumber + 1;
            nextRightSeatToReserve = GetSeatToReserve(nextRightSeatNumber);

            if (nextRightSeatToReserve == null) break;

            ReserveSeat(nextRightSeatToReserve);
        }

        return totalFilledSeats;

        Seat? GetSeatToReserve(int seatNumber)
        {
            var seatToReserve = seatsToFill.Single(s => s.SeatNumber == seatNumber);
            while (seatToReserve.Status != SeatStatus.Empty)
            {
                var nextSeatNumberToReserve = seatToReserve.SeatNumber + 1;
                if (HasRowLimitReached(nextSeatNumberToReserve))
                {
                    seatToReserve = null;
                    break;
                }

                seatToReserve = rowSeats.Single(s => s.SeatNumber == nextSeatNumberToReserve);
            }

            return seatToReserve;

            bool HasRowLimitReached(int nextPossibleSeatNumberToReserve)
            {
                return nextPossibleSeatNumberToReserve > seatsToFillCount || nextPossibleSeatNumberToReserve < 1;
            }
        }

        void ReserveSeat(Seat seatToReserve)
        {
            seatToReserve.Update(SeatStatus.Reserved, newBookingId);
            totalFilledSeats++;
            numberOfSeatsFilledInCurrentRow++;
        }
    }

    private int GetMiddleSeatNumber(int seatsPerRow, int numberOfTickets)
    {
        if (seatsPerRow % 2 != 0) return (seatsPerRow / 2) + 1;
        var medianSeatNumber = seatsPerRow / 2;
        return numberOfTickets % 2 == 0 ? medianSeatNumber : medianSeatNumber + 1;
    }
}