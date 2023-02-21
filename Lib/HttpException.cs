using System.Net;

namespace PhoneShop.Lib;

public class HttpException : Exception
{
    public HttpStatusCode status;
    public String message;

    public HttpException(String message, HttpStatusCode status = HttpStatusCode.BadRequest)
    {
        this.status = status;
        this.message = message;
    }
}
