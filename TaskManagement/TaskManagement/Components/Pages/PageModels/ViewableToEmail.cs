namespace TaskManagement.Components.Pages.PageModels;

public class ViewableToEmail
{
    public string Email { get; set; }

    public bool IsRemoveable { get; set; } = true;

    public ViewableToEmail(string email, bool isRemoveable)
    {
        Email = email;
        IsRemoveable = isRemoveable;
    }
}
