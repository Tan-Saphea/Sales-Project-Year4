using SalesPurchaseManagement.Models;

namespace SalesPurchaseManagement.Security;

/// <summary>
/// Holds the currently authenticated employee session state for the application.
/// </summary>
public static class EmployeeSession
{
    public static int CurrentEmployeeID { get; set; } = 1;
    public static string CurrentEmployeeName { get; set; } = "Dara Sok";
    public static string CurrentUserName { get; set; } = "dara";
    public static string CurrentRole { get; set; } = "Administrator";
    public static string CurrentUserType { get; set; } = "Admin";
    public static bool IsLoggedIn { get; set; } = true;

    public static void SetSession(Employee employee)
    {
        CurrentEmployeeID = employee.EmployeeID;
        CurrentEmployeeName = employee.EmployeeName;
        CurrentUserName = employee.UserName;
        CurrentRole = employee.Role ?? "Staff";
        CurrentUserType = employee.UserType ?? "Staff";
        IsLoggedIn = true;
    }

    public static void ClearSession()
    {
        CurrentEmployeeID = 0;
        CurrentEmployeeName = string.Empty;
        CurrentUserName = string.Empty;
        CurrentRole = string.Empty;
        CurrentUserType = string.Empty;
        IsLoggedIn = false;
    }
}
