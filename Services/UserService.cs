using System.Net;
using Microsoft.EntityFrameworkCore;
using PhoneShop.Lib;
using PhoneShop.Lib.Extension;
using PhoneShop.Model;
using PhoneShop.Interface;
using BC = BCrypt.Net.BCrypt;
using System.Collections;

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

    await PrismaExtension.runTask(async db =>
    {
      await db.Users.AddAsync(newUser);
      db.SaveChanges();
    });

    newUser.Password = "";

    return newUser;
  }

  public async Task<User> updateUser(string uid, UpdateUserBody body)
  {
    return await PrismaExtension.runTransaction(async db =>
    {
      var user = await db.Users.Where(user => user.Uid == uid).FirstAsync();

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

  public async Task<object?> addProductToCart(String userId, RemoveProductFromCartBody body)
  {
    return await PrismaExtension.runTransaction(async db =>
    {
      var user = await Util.useMemo(async () =>
      {
        return await db.Users.Include(u => u.Cart).Where(u => u.Uid == userId).FirstAsync();
      });

      var phone = await Util.useMemo(async () =>
      {
        return await db.Phones.Where(u => u.Uid == body.PhoneId).FirstAsync();
      });

      if (user == null)
      {
        throw new HttpException("Unauthorized", HttpStatusCode.Unauthorized);
      }

      if (phone == null)
      {
        throw new HttpException("Phone not found", HttpStatusCode.NotFound);
      }

      if (user.Role == UserRole.STORE)
      {
        throw new HttpException("You are not a buyer", HttpStatusCode.UnprocessableEntity);
      }

      if (user.Cart == null)
      {
        var cart = new Cart();
        cart.Stringtemplates = new[] { new Stringtemplate() { Value = body.PhoneId } };
        user.Cart = cart;
      }
      else
      {
        for (var i = 0; i < body.Count; i++)
        {
          user.Cart.Stringtemplates.Add(new Stringtemplate() { Value = body.PhoneId });
        }
      }

      db.SaveChanges();

      return new
      {
        message = "success"
      };
    });
  }

  public async Task<List<object>> getCart(String userId)
  {
    return await PrismaExtension.runTransaction(async db =>
    {
      var user = await Util.useMemo(async () =>
      {
        return await db.Users.Include(u => u.Cart).Where(u => u.Uid == userId).FirstAsync();
      });

      if (user == null)
      {
        throw new HttpException("Unauthorized", HttpStatusCode.Unauthorized);
      }

      if (user.Role == UserRole.STORE)
      {
        throw new HttpException("You are not a buyer", HttpStatusCode.UnprocessableEntity);
      }

      var listPhoneId = await Util.useMemo(async () =>
      {
        return await db.Stringtemplates.Include(s => s.Cart).Where(s => s.Cart == user.Cart).ToListAsync();
      });

      if (listPhoneId == null)
      {
        var cart = new Cart();
        cart.Stringtemplates = new List<Stringtemplate>();
        user.Cart = cart;
        db.SaveChanges();

        return new List<object> { };
      }
      else
      {
        var listUniqueId = listPhoneId.DistinctBy(s => s.Value).ToList();
        var listPhone = new List<object> { };
        foreach (var phoneTemplate in listUniqueId)
        {
          var phoneCount = listPhoneId.Where(id => id.Value == phoneTemplate.Value).ToList().Count;

          var phone = await Util.useMemo(async () => await db.Phones.Include(p => p.Phoneoffers).Select(p => new
          {
            Uid = p.Uid,
            Name = p.Name,
            Images = p.Images,
            Count = phoneCount,
            Phoneoffers = p.Phoneoffers.Select(pof => new
            {
              Price = pof.Price,
              Count = pof.Count,
              Color = pof.Color,
              Storage = pof.Storage
            }),
          }).Where(p => p.Uid == phoneTemplate.Value).FirstAsync());

          if (phone != null)
          {
            listPhone.Add(phone);
          }
        }

        return listPhone;
      }
    });
  }

  public async Task<String> deleteProductFromCart(String userId, RemoveProductFromCartBody body)
  {
    return await PrismaExtension.runTransaction(async db =>
    {
      var user = await Util.useMemo(async () =>
      {
        return await db.Users.Include(u => u.Cart).Where(u => u.Uid == userId).FirstAsync();
      });

      if (user == null)
      {
        throw new HttpException("Unauthorized", HttpStatusCode.Unauthorized);
      }

      if (user.Role == UserRole.STORE)
      {
        throw new HttpException("You are not a buyer", HttpStatusCode.UnprocessableEntity);
      }

      var listPhoneId = await Util.useMemo(async () =>
      {
        if (user.Cart == null)
        {
          return new List<Stringtemplate>();
        }

        return await db.Stringtemplates.Where(s => s.CartId == user.Cart.Id).ToListAsync();
      });

      if (listPhoneId == null)
      {
        var cart = new Cart();
        cart.Stringtemplates = new List<Stringtemplate>();
        user.Cart = cart;
        db.SaveChanges();
        throw new HttpException("nothing to do", HttpStatusCode.NotModified);
      }
      else
      {
        var count = body.Count;
        for (var i = listPhoneId.Count - 1; i >= 0; i--)
        {
          if (listPhoneId[i].Value == body.PhoneId)
          {
            db.Stringtemplates.Remove(listPhoneId[i]);
            count--;
          }

          if (count == 0)
          {
            break;
          }
        }
        db.SaveChanges();
      }

      return "success to update cart";
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