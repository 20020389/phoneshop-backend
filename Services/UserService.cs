using System.Net;
using Microsoft.EntityFrameworkCore;
using PhoneShop.Lib;
using PhoneShop.Lib.Extension;
using PhoneShop.Model;
using PhoneShop.Prisma;
using BC = BCrypt.Net.BCrypt;

namespace PhoneShop.Service;

public class UserService
{
  public UserService()
  {
  }

  public async Task<User> login(LoginBody body)
  {
    User newUser = await DBExtension.runTransaction(async db =>
    {
      return await db.Users.Where(u => u.Email == body.Email).FirstAsync();
    });
    if (newUser == null)
    {
      throw new HttpException("User not found", HttpStatusCode.NotFound);
    }

    if (!BC.Verify(body.Password, newUser.Password))
    {
      throw new HttpException("Wrong password");
    }

    newUser.Password = "";

    return newUser;
  }

  public async Task<User> getUser(string uid)
  {
    User newUser = await DBExtension.runTransaction(async db =>
    {
      return await db.Users.Where(u => u.Uid == uid).FirstAsync();
    });
    if (newUser == null)
    {
      throw new HttpException("User not found", HttpStatusCode.NotFound);
    }

    newUser.Password = "";

    return newUser;
  }

  public async Task<User> register(RegisterBody body)
  {
    Validator.validateRegister(body);
    var newUser = createUser(body);

    await DBExtension.runTransaction(async db =>
    {
      await db.Users.AddAsync(newUser);
    });

    newUser.Password = "";

    return newUser;
  }

  private User createUser(RegisterBody body)
  {
    return new User
    {
      Email = body.Email,
      Password = BC.HashPassword(body.Password),
      Name = body.Name ?? body.Email,
      PhoneNumber = body.PhoneNumber,
      Profile = body.Profile
    };
  }
}