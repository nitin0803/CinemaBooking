namespace GicCinema.Services.UserSelection;

public interface IUserSelectionService
{
    void Handle(Enums.MenuItemOption menuItemOption);
}