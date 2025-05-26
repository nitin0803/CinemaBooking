using System.Text.RegularExpressions;
using GicCinema.Models;

namespace GicCinema.Utility;

public static class CinemaUtility
{
    public static char GetNewSeatPositionRowLabel(string newSeatPosition) => newSeatPosition.Substring(0, 1)[0];
    public static int GetNewSeatPositionNumber(string newSeatPosition) => Convert.ToInt32(newSeatPosition.Substring(1));

    public struct AppMessage
    {
        public const string DefineCinema =
            "Please define movie title and seating map in [Title] [Row] [SeatsPerRow] format:";

        public const string Welcome = "Welcome to GIC Cinemas";
        public const string EnterSelection = "Please enter your selection:";
        
        public const string Blank = "or enter blank to go back to main menu:";
        public const string NumberOfTickets = "Enter number of tickets to book, ";
        public const string BookingId = "Enter booking id, ";
        public const string ThankYou = "Thank you for using GIC Cinames system. Bye!";

        public const string AcceptOrNewSeatSelection =
            "Enter blank to accept seat selection, or enter new seating position:";

        public const string TicketsReserved = "Successfully reserved {0} {1} tickets.";
        public const string BookingIdConfirmed = "Booking id: {0} confirmed.";
        public const string SeatsAvailabilityAlert = "Sorry, there are only {0} seats available.";
        public const string NoBookingFound = "Sorry, no booking Found for entered booking id: {0}";
    }
    
    public struct MenutItem
    {
        public const string BookTickets = "[1] Book tickets for {0} ({1} seats available)";
        public const string CheckBookings = "[2] Check bookings";
        public const string Exit = "[3] Exit";
    }

    public struct ValidationMessage
    {
        public const string InvalidSelection = "Input selection is not correct, please try again!";
        public const string InvalidMovieDetails = "Entered movie details are not in correct format!";
        public const string MovieNameExceed = "Please, enter movie name less than 50 characters";
        public const string InvalidRow = "Please, enter row as positive integer value";
        public const string RowRangeExceed = "Please, enter rows between 1 and 26, inclusive";
        public const string InvalidSeatsPerRow = "Please, enter seats per row as positive integer value";
        public const string SeatsPerRowRangeExceed = "Please, enter seats per row between 1 and 50, inclusive";
        public const string InvalidNumberOfTickets = "Please, enter number of tickets as positive integer value";
        public const string InvalidSeatingPosition = "New seating position is not valid! Please try again:";
        public const string InvalidBookingIdFormat = "Entered booking id is not in correct format, please try again!";
    }

    public struct ExceptionMessage
    {
        public const string DuplicateBookingsFound =
            "Exception occurred as duplicate booking entries found for booking id {0} !";
    }

    public static bool AreCinemaDetailsValid(string? cinemaDetails)
    {
        if (string.IsNullOrWhiteSpace(cinemaDetails)) return false;

        var cinemaValues = cinemaDetails.Split(" ");
        if (cinemaValues.Length != 3)
        {
            Console.WriteLine(ValidationMessage.InvalidMovieDetails);
            return false;
        }

        if (cinemaValues[0].Length > 50)
        {
            Console.WriteLine(ValidationMessage.MovieNameExceed);
            return false;
        }

        if (!AreRowsValid(cinemaValues)) return false;

        if (!AreSeatsPerRowValid(cinemaValues)) return false;

        return true;
    }

    public static bool IsNewSeatPositionValid(IReadOnlyList<RowLayOut> rowLayouts, string newSeatPosition)
    {
        const string startSeatPositionPattern = @"^[A-Za-z]\d+$";
        if (!Regex.IsMatch(newSeatPosition, startSeatPositionPattern)) return false;

        var newSeatPositionRowLabel = GetNewSeatPositionRowLabel(newSeatPosition);

        var allLabels = rowLayouts.Select(r => r.RowLabel).ToList();
        if (!allLabels.Contains(newSeatPositionRowLabel)) return false;

        var newSeatPositionNumber = GetNewSeatPositionNumber(newSeatPosition);
        var allSeatNumbersInRow = rowLayouts.First().Seats.Select(s => s.SeatNumber).ToList();
        if (!allSeatNumbersInRow.Contains(newSeatPositionNumber)) return false;
        return true;
    }

    public static bool IsNumberOfTicketsValid(string numberOfTicketsInput)
    {
        if (!int.TryParse(numberOfTicketsInput, out int numberOfTickets))
        {
            return false;
        }

        if (numberOfTickets < 1)
        {
            return false;
        }

        return true;
    }

    private static bool AreSeatsPerRowValid(string[] inputArray)
    {
        if (!int.TryParse(inputArray[2], out int seatsPerRow))
        {
            Console.WriteLine(ValidationMessage.InvalidSeatsPerRow);
            return false;
        }

        if (seatsPerRow is < 1 or > 50)
        {
            Console.WriteLine(ValidationMessage.SeatsPerRowRangeExceed);
            return false;
        }

        return true;
    }

    private static bool AreRowsValid(string[] inputArray)
    {
        if (!int.TryParse(inputArray[1], out int rows))
        {
            Console.WriteLine(ValidationMessage.InvalidRow);
            return false;
        }

        if (rows is < 1 or > 26)
        {
            Console.WriteLine(ValidationMessage.RowRangeExceed);
            return false;
        }

        return true;
    }
}