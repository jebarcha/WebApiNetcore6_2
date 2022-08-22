namespace Core.Entities;

public class UsersRoles
{
    public int UserId { get; set; }
    public User User { get; set; }
    public int RolId { get; set; }
    public Role Role { get; set; }
}

