namespace API.Helpers;

public class Authorization
{
    public enum Roles
    {
        Administrator,
        Manager,
        Employee
    }

    public const Roles role_default = Roles.Employee;
}
