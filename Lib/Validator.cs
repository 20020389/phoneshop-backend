using System.Text.RegularExpressions;
using PhoneShop.Model;

namespace PhoneShop.Lib;

public class Validator
{
    private static bool isEmail(string email)
    {
        return Regex.IsMatch(
            email,
            @"\A(?:[a-z0-9!#$%&'*+/=?^_`{|}~-]+(?:\.[a-z0-9!#$%&'*+/=?^_`{|}~-]+)*@(?:[a-z0-9](?:[a-z0-9-]*[a-z0-9])?\.)+[a-z0-9](?:[a-z0-9-]*[a-z0-9])?)\Z",
            RegexOptions.IgnoreCase
        );
    }

    private static bool isPassword(string password)
    {
        return Regex.IsMatch(password, @"^(?=.*?[a-z])(?=.*?[0-9]).{6,}$");
    }

    public static void validateRegister(RegisterBody body)
    {
        if (body.Email is null)
        {
            throw new HttpException("Missing email field");
        }

        if (body.Password is null)
        {
            throw new HttpException("Missing password field");
        }

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
