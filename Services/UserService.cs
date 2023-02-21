
using Microsoft.EntityFrameworkCore;
using PhoneShop.Lib;
using PhoneShop.Model;
using PhoneShop.Prisma;
using BC = BCrypt.Net.BCrypt;

namespace PhoneShop.Service;

public class UserService
{
  private readonly PrismaClient _db;
  public UserService(PrismaClient db)
  {
    _db = db;
  }

  public async Task<User> register(RegisterBody body)
  {
    var newUser = new User
    {
      Email = body.Email,
      Name = body.Email,
      Password = BC.HashPassword(body.Password)
    };

    await _db.runTransaction(async (db) =>
    {
      await db.Users.AddAsync(newUser);
    });

    return newUser;
  }
}