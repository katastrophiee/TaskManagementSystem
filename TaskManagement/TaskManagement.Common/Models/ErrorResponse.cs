namespace TaskManagement.Common.Models;

public sealed class ErrorResponse
{
    public string Title { get; set; }

    public string Description { get; set; }

    public int? StatusCode { get; set; }

    public string AdditionalDetails { get; set; }
}
