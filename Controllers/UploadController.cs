

using System.Net;
using Microsoft.AspNetCore.Mvc;
using PhoneShop.Lib;
using PhoneShop.Middleware;

[HttpException]
[ApiController]
[Route("api/upload")]
public class UploadController : Controller
{
  private readonly UploadService _uploadService;
  public UploadController(UploadService uploadService)
  {
    _uploadService = uploadService;
  }

  [HttpPost]
  [Produces("multipart/form-data")]
  public async Task<IResult> uploadFile(IFormFile? file)
  {
    if (file == null)
    {
      throw new HttpException("File is required", HttpStatusCode.BadRequest);
    }
    return Results.Json(new
    {
      data = await _uploadService.addFile(file),
    });
  }

  [HttpGet("{fileId}")]
  public async Task<IResult> getFileStream(String fileId)
  {
    var file = await _uploadService.GetFileAsync(fileId);
    var filepath = Path.Join(Directory.GetCurrentDirectory(), file?.Path);
    if (file == null || !System.IO.File.Exists(filepath))
    {
      return Results.NotFound();
    }
    var fileStream = new FileStream(filepath, FileMode.Open);
    return Results.File(fileStream, file.Type);
  }

  [HttpDelete("{fileId}")]
  public async Task<IResult> deleteFile(String fileId)
  {
    await _uploadService.DeleteFileAsync(fileId);
    return Results.Json(new
    {
      message = "success to delete file"
    });
  }

  [HttpPut("{fileId}")]
  public async Task<IResult> updateFile(String fileId, IFormFile? file)
  {
    if (file == null)
    {
      throw new HttpException("File is required", HttpStatusCode.BadRequest);
    }
    await _uploadService.UpdateFileAsync(file, fileId);
    return Results.Json(new
    {
      message = "success to update file"
    });
  }
}
