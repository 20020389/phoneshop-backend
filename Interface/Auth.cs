

using PhoneShop.Model;

namespace PhoneShop.Interface;

public class UserRole
{
  public static String STORE = "STORE";
  public static String DEFAULT = "DEFAULT";
}

public class RegisterBody
{
  public string Email { get; set; } = null!;

  public string Password { get; set; }

  public string? Name { get; set; }

  public string? PhoneNumber { get; set; }

  public string? Role { get; set; }

  public string? Profile { get; set; }
}

public class LoginBody
{
  public string Email { get; set; } = null!;

  public string Password { get; set; }

}

public class UpdateUserBody
{
  public string? Name { get; set; }

  public string? PhoneNumber { get; set; }

  public string? Profile { get; set; }

  public string? Image { get; set; }
}

public class UserWithoutPassword : User
{

  public String? Password { get; set; }
}
