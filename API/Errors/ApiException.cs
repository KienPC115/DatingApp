namespace API.Errors;

public class ApiException
{

    public int StatusCode { get; set; }

    public string Message { get; set; }

    public string Details { get; set; } // this contain the stack trace


    public ApiException(int statusCode, string message, string details) 
    {
        StatusCode = statusCode;
        Message = message;
        Details = details;
    }
}