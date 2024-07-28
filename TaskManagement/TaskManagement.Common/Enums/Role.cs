using System.ComponentModel.DataAnnotations;

namespace TaskManagement.Common.Enums;

public enum Role
{
    [Display(Name = "Task User")]
    TaskUser = 0,

    [Display(Name = "Admin User")]
    Admin = 1,
}
