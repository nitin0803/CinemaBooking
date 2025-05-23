using System.Data;
using GicCinema.Enums;

namespace GicCinema.Models;

public class Seat
{
    public Seat(
        char rowLabel,
        int seatNumber)
    {
        RowLabel = rowLabel;
        SeatNumber = seatNumber;
        Status = SeatStatus.Empty;
        BookingId = null;
    }

    public char RowLabel { get; }
    public int SeatNumber { get; }
    public SeatStatus Status { get; private set; }
    public string? BookingId { get; private set; }

    public void Update(SeatStatus seatStatus, string bookingId)
    {
        Status = seatStatus;
        BookingId = bookingId;
    }
}