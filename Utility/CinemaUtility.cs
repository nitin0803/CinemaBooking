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
    
    public static IReadOnlyList<RowLayOut> GetRowLayOutsSequence(IReadOnlyList<RowLayOut> rowLayouts, string startSeatPosition)
    {
        var newSeatPositionRowLabel = GetNewSeatPositionRowLabel(startSeatPosition);
        var firstSequence = new List<RowLayOut>();
        var secondSequence = new List<RowLayOut>();
        foreach (var rowLayOut in rowLayouts.Reverse())
        {
            var currentRowLabel = rowLayOut.RowLabel;
            if (currentRowLabel.Equals(newSeatPositionRowLabel) || currentRowLabel.CompareTo(newSeatPositionRowLabel) > -1)
            {
                firstSequence.Add(rowLayOut);
                continue;
            }
            secondSequence.Add(rowLayOut);
        }

        return firstSequence.Concat(secondSequence).ToList();
    }
    
    public static char GetNewSeatPositionRowLabel(string newSeatPosition) =>  newSeatPosition.Substring(0, 1)[0];
    public static int GetNewSeatPositionNumber(string newSeatPosition) =>  Convert.ToInt32(newSeatPosition.Substring(1));
    
}