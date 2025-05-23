using GicCinema.Enums;
using GicCinema.Utility;

namespace GicCinema.Services.UserSelection;

public class ExitService : IUserSelectionService
{
    public void Handle(MenuItemOption menuItemOption)
    {
        if (!IsResponsible(menuItemOption)) return;
        Console.WriteLine(AppMessages.ThankYouMessage);
    }

    private static bool IsResponsible(MenuItemOption menuItemOption) => menuItemOption == MenuItemOption.Exit;
}