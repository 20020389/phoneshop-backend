using System.Net;
using Microsoft.EntityFrameworkCore;
using PhoneShop.Lib;
using PhoneShop.Lib.Extension;
using PhoneShop.Model;
using PhoneShop.Interface;
using BC = BCrypt.Net.BCrypt;

namespace PhoneShop.Service;

public class UserService
{
  public UserService()
  {
  }

  public async Task<User> login(LoginBody body)
  {
    User newUser = await PrismaExtension.runTransaction(async db =>
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
    User user = await PrismaExtension.runTransaction(async db =>
    {
      return await db.Users.Where(u => u.Uid == uid).FirstAsync();
    });
    if (user == null)
    {
      throw new HttpException("User not found", HttpStatusCode.NotFound);
    }

    user.Password = "";
    user.Id = 0;

    return user;
  }

  public async Task<User> register(RegisterBody body)
  {
    Validator.validateRegister(body);
    var newUser = createUser(body);

    await PrismaExtension.runTransaction(async db =>
    {
      await db.Users.AddAsync(newUser);
      db.SaveChanges();
    });

    newUser.Password = "";

    return newUser;
  }

  public async Task<User?> updateUser(string uid, UpdateUserBody body)
  {
    return await PrismaExtension.runTransaction(async db =>
    {
      var user = await db.Users.Where(user => user.Uid == uid).FirstOrDefaultAsync();

      if (user == null)
      {
        throw new HttpException("user not found with email", HttpStatusCode.NotFound);
      }

      if (body.Name != "" && body.Name != null)
      {
        user.Name = body.Name;
      }

      if (body.Image != "" && body.Image != null)
      {
        user.Image = body.Image;
      }

      if (body.PhoneNumber != "" && body.PhoneNumber != null)
      {
        user.PhoneNumber = body.PhoneNumber;
      }

      if (body.Profile != "" && body.Profile != null)
      {
        user.Profile = body.Profile;
      }

      db.SaveChanges();

      return user;
    });
  }

  private User createUser(RegisterBody body)
  {
    return new User
    {
      Email = body.Email,
      Password = BC.HashPassword(body.Password),
      Name = body.Name ?? body.Email.Split("@")[0],
      PhoneNumber = body.PhoneNumber,
      Profile = body.Profile,
      Role = body.Role ?? UserRole.DEFAULT
    };
  }

}