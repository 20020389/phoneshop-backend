using System.Net.Mail;
using System.Text.RegularExpressions;
using PhoneShop.Interface;

namespace PhoneShop.Lib;

public class Validator
{
  private static bool isEmail(string email)
  {
    try
    {
      if (email == "" || email == null)
      {
        return false;
      }
      new MailAddress(email);
      return true;
    }
    catch (FormatException)
    {
      return false;
    }
  }

  private static bool isPassword(string password)
  {
    return Regex.IsMatch(password, @"^(?=.*?[a-z])(?=.*?[0-9]).{6,}$");
  }

  public static void validateRegister(RegisterBody body)
  {
    if (!isEmail(body.Email))
    {
      throw new HttpException("Email field is not an email");
    }

    if (!isPassword(body.Password))
    {
      throw new HttpException(
          "Password must be at least 6 characters, contain one number and one uppercase letter"
      );
    }
  }
}
