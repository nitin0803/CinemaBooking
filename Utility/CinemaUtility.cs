using System.Text.RegularExpressions;
using GicCinema.Models;

namespace GicCinema.Utility;

public static class CinemaUtility
{
    public struct AppMessage
    {
        public const string DefineCinema = "Please define movie title and seating map in [Title] [Row] [SeatsPerRow] format:";
        public const string WelcomeMessage = "Welcome to GIC Cinemas";
        public const string BlankMessage = "or enter blank to go back to main menu:";
        public const string NumberOfTickets = "Enter number of tickets to book, ";
        public const string BookingIdMessage = "Enter booking id, ";
        public const string ThankYouMessage = "Thank you for using GIC Cinames system. Bye!";
        public const string AcceptOrNewSeatSelectionMessage = "Enter blank to accept seat selection, or enter new seating position:";
    }
    
    public struct ValidationMessage
    {
        public const string InvalidSelection = "Input selection is not correct, please try again!";
        public const string InvalidInput = "Input details are not in correct format!";
        public  const string MovieNameExceed = "Please enter movie name less than 50 characters";
        public  const string InvalidRowInput = "Please enter Row as integer value";
        public  const string RowRangeExceed = "Please enter Rows between 1 and 26, inclusive";
        public  const string InvalidSeatsPerRowInput = "Please enter SeatsPerRow as integer value";
        public  const string SeatsPerRowRangeExceed = "Please enter SeatsPerRow between 1 and 50, inclusive";
    }
    
    public static bool IsInputValid(string? inputString)
    {
        if (string.IsNullOrWhiteSpace(inputString)) return false;
        var inputArray = inputString.Split(" ");
        if (inputArray.Length != 3)
        {
            Console.WriteLine(ValidationMessage.InvalidInput);
            return false;
        }

        if (inputArray[0].Length > 50)
        {
            Console.WriteLine(ValidationMessage.MovieNameExceed);
            return false;
        }

        if (!AreRowsValid(inputArray)) return false;

        if (!AreSeatsPerRowValid(inputArray)) return false;

        return true;
    }

    public static bool IsNewSeatPositionValid(IReadOnlyList<RowLayOut> rowLayouts, string newSeatPosition)
    {
        const string startSeatPositionPattern = @"^[A-Za-z]\d+$";
        if (!Regex.IsMatch(newSeatPosition, newSeatPosition)) return false;
        
        var newSeatPositionRowLabel = CinemaUtility.GetNewSeatPositionRowLabel(newSeatPosition);
        
        var allLabels = rowLayouts.Select(r => r.RowLabel).ToList();
        if (!allLabels.Contains(newSeatPositionRowLabel)) return false;
        
        var newSeatPositionNumber = CinemaUtility.GetNewSeatPositionNumber(newSeatPosition);
        var allSeatNumbersInRow = rowLayouts.First().Seats.Select(s => s.SeatNumber).ToList();
        if (!allSeatNumbersInRow.Contains(newSeatPositionNumber)) return false;
        return true;
    }

    private static bool AreSeatsPerRowValid(string[] inputArray)
    {
        if (!int.TryParse(inputArray[2], out int seatsPerRow))
        {
            Console.WriteLine(ValidationMessage.InvalidSeatsPerRowInput);
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
            Console.WriteLine(ValidationMessage.InvalidRowInput);
            return false;
        }

        if (rows is < 1 or > 26)
        {
            Console.WriteLine(ValidationMessage.RowRangeExceed);
            return false;
        }

        return true;
    }
    
    
    
    public static char GetNewSeatPositionRowLabel(string newSeatPosition) =>  newSeatPosition.Substring(0, 1)[0];
    public static int GetNewSeatPositionNumber(string newSeatPosition) =>  Convert.ToInt32(newSeatPosition.Substring(1));
    
}