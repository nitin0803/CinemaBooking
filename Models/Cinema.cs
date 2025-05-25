namespace GicCinema.Models;

public class Cinema
{
    private static readonly object LockObject = new();
    private static Cinema? _instance;

    private Cinema(string movie, int totalRows, int seatsPerRow)
    {
        Movie = movie;
        TotalRows = totalRows;
        SeatsPerRow = seatsPerRow;
        Bookings = new List<Booking>();
        HallLayout = CreateHallLayout();
    }

    public string Movie { get; }
    private int TotalRows { get; }
    public int SeatsPerRow { get; }
    public HallLayout HallLayout { get; }
    public List<Booking> Bookings { get; }
    public int AvailableSeats => TotalHallSeats - TotalBookedSeats;
    
    private int TotalHallSeats => TotalRows * SeatsPerRow;
    private int TotalBookedSeats => Bookings.Aggregate(0, (total, booking) => total + booking.NumberOfBookedSeats);

    public static Cinema Create(string movie, int rows, int seatsPerRow)
    {
        lock (LockObject)
        {
            if (_instance != null) return _instance;
            _instance = new Cinema(movie, rows, seatsPerRow);
            return _instance;
        }
    }

    public static Cinema GetCinema()
    {
        if (_instance != null) return _instance;
        Console.WriteLine("Exception occurred as no Cinema available!");
        throw new Exception("No Cinema Found");
    }

    private HallLayout CreateHallLayout()
    {
        var rowLayouts = new List<RowLayOut>();
        for (var i = 0; i < TotalRows; i++)
        {
            var rowLabel = (char)('A' + (TotalRows - 1) - i); // Convert 0 -> 'A', 1 -> 'B', etc.

            var emptySeats = new List<Seat>();
            for (var seatNumber = 1; seatNumber <= SeatsPerRow; seatNumber++)
            {
                emptySeats.Add(new Seat(seatNumber));
            }

            rowLayouts.Add(new RowLayOut(rowLabel, emptySeats));
        }

        return new HallLayout(rowLayouts);
    }
}