using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Common.Enums;

public enum Role
{
    [Display(Name = "TaskUser")]
    TaskUser = 0,

    [Display(Name = "Admin")]
    Admin = 1,
}
