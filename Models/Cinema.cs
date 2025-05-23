namespace GicCinema.Models;

public class Cinema
{
    private static readonly object _lockObject = new();
    private static Cinema _instance;

    private Cinema(string movie, int totalRows, int seatsPerRow)
    {
        Movie = movie;
        TotalRows = totalRows;
        SeatsPerRow = seatsPerRow;
        Bookings = new List<Booking>();
        HallLayout = CreateHallLayout();
    }

    public string Movie { get; }
    public int TotalRows { get; }
    public int SeatsPerRow { get; }
    public HallLayout HallLayout { get; }
    public List<Booking> Bookings { get; }

    public int TotalHallSeats => TotalRows * SeatsPerRow;

    public int TotalBookedSeats => Bookings.SelectMany(b => b.Seats).Count();
    public int AvailableSeats => TotalHallSeats - TotalBookedSeats;

    public static Cinema Create(string movie, int rows, int seatsPerRow)
    {
        if (_instance == null)
        {
            lock (_lockObject)
            {
                if (_instance == null)
                {
                    _instance = new Cinema(movie, rows, seatsPerRow);
                    return _instance;
                }
            }
        }

        return _instance;
    }

    public static Cinema GetCinema()
    {
        return _instance;
    }

    private HallLayout CreateHallLayout()
    {
        var rowLayouts = new List<RowLayOut>();
        for (var currentRow = 0; currentRow < TotalRows; currentRow++)
        {
            var rowLabel = (char)('A' + (TotalRows-1) - currentRow); // Convert 0 -> 'A', 1 -> 'B', etc.

            var emptySeats = new List<Seat>();
            for (var seatNumber = 1; seatNumber <= SeatsPerRow; seatNumber++)
            {
                emptySeats.Add(new Seat(rowLabel, seatNumber));
            }
            rowLayouts.Add(new RowLayOut(rowLabel, emptySeats));
        }

        return new HallLayout(rowLayouts);
    }
}