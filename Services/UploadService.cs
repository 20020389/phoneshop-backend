

using System.Net;
using FS = System.IO.File;
using Microsoft.EntityFrameworkCore;
using PhoneShop.Lib;
using PhoneShop.Lib.Extension;
using PhoneShop.Model;

public class UploadService
{

  public async Task<string> addFile(IFormFile file)
  {
    return await PrismaExtension.runTransaction(async db =>
    {
      var uid = Guid.NewGuid().ToString();
      var filename = $"{uid}";
      var uploadPath = Path.Combine(Directory.GetCurrentDirectory(), "Upload");
      var path = Path.Combine(uploadPath, filename);

      var fileModel = new Filemodel
      {
        Uid = uid,
        Path = $"/Upload/{filename}",
        Type = file.ContentType
      };

      await db.Filemodels.AddAsync(fileModel);
      await db.SaveChangesAsync();

      using (var stream = new FileStream(path, FileMode.Create))
      {
        if (Directory.Exists(uploadPath))
        {
          Directory.CreateDirectory(uploadPath);
        }
        await file.CopyToAsync(stream);
      }

      return $"/api/upload/{uid}";
    });
  }

  public async Task<Filemodel?> GetFileAsync(String fileId)
  {
    return await PrismaExtension.runTask<Filemodel?>(async db => await db.Filemodels.Where(file => file.Uid == fileId).FirstAsync());
  }

  public async Task DeleteFileAsync(String fileId)
  {
    await PrismaExtension.runTransaction(async db =>
    {
      var file = await db.Filemodels.Where(file => file.Uid == fileId).FirstAsync();
      if (file == null)
      {
        throw new HttpException("File not found", HttpStatusCode.NotFound);
      }

      var filepath = Path.Join(Directory.GetCurrentDirectory(), file.Path);

      db.Filemodels.Remove(file);

      if (System.IO.File.Exists(filepath))
      {
        FS.Delete(filepath);
      }

      db.SaveChanges();
    });
  }

  public async Task<string> UpdateFileAsync(IFormFile file, String fileId)
  {
    return await PrismaExtension.runTransaction(async db =>
    {
      var fileData = await db.Filemodels.Where(file => file.Uid == fileId).FirstAsync();
      if (file == null)
      {
        throw new HttpException("File not found", HttpStatusCode.NotFound);
      }

      var filepath = Path.Join(Directory.GetCurrentDirectory(), fileData.Path);

      fileData.Type = file.ContentType;

      db.SaveChanges();

      if (System.IO.File.Exists(filepath))
      {
        FS.Delete(filepath);
      }

      using (var stream = new FileStream(filepath, FileMode.Create))
      {
        await file.CopyToAsync(stream);
      }

      return $"/api/upload/{fileData.Uid}";
    });
  }
}