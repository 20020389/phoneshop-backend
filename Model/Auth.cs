
namespace PhoneShop.Model;

public class UserRole {
  public static String STORE = "STORE";
  public static String DEFAULT = "DEFAULT";
}

public class RegisterBody {
  public string Email { get; set; } = null!;

  public string Password { get; set; }
}