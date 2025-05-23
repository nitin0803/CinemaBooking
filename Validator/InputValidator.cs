namespace GicCinema.Validator;

public static class InputValidator
{
    private const string InvalidInput = "Input details are not in correct format!";
    private const string MovieNameExceed = "Please enter movie name less than 50 characters";
    private const string InvalidRowInput = "Please enter Row as integer value";
    private const string RowRangeExceed = "Please enter Rows between 1 and 26, inclusive";
    private const string InvalidSeatsPerRowInput = "Please enter SeatsPerRow as integer value";
    private const string SeatsPerRowRangeExceed = "Please enter SeatsPerRow between 1 and 50, inclusive";

    public static bool IsInputValid(string? inputString)
    {
        if (string.IsNullOrWhiteSpace(inputString)) return false;
        var inputArray = inputString.Split(" ");
        if (inputArray.Length != 3)
        {
            Console.WriteLine(InvalidInput);
            return false;
        }

        if (inputArray[0].Length > 50)
        {
            Console.WriteLine(MovieNameExceed);
            return false;
        }

        if (!AreRowsValid(inputArray)) return false;

        if (!AreSeatsPerRowValid(inputArray)) return false;

        return true;
    }

    private static bool AreSeatsPerRowValid(string[] inputArray)
    {
        if (!int.TryParse(inputArray[2], out int seatsPerRow))
        {
            Console.WriteLine(InvalidSeatsPerRowInput);
            return false;
        }

        if (seatsPerRow is < 1 or > 50)
        {
            Console.WriteLine(SeatsPerRowRangeExceed);
            return false;
        }

        return true;
    }

    private static bool AreRowsValid(string[] inputArray)
    {
        if (!int.TryParse(inputArray[1], out int rows))
        {
            Console.WriteLine(InvalidRowInput);
            return false;
        }

        if (rows is < 1 or > 26)
        {
            Console.WriteLine(RowRangeExceed);
            return false;
        }

        return true;
    }
}