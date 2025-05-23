namespace GicCinema.Models;

public record Booking(int Number, IReadOnlyList<Seat> Seats);