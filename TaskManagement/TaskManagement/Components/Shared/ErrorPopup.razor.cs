using Microsoft.AspNetCore.Components;
using TaskManagement.Common.Models;

namespace TaskManagement.Components.Shared;

public partial class ErrorPopup
{
    [Parameter]
    public ErrorResponse Error { get; set; }
}
