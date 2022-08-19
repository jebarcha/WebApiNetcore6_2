namespace API.Helpers.Errors;

public class ApiResponse
{
    public int StatusCode { get; set; }
    public string Message { get; set; }
    public ApiResponse(int statusCode, string message = null)
    {
        StatusCode = statusCode;
        Message = message ?? GetDefaultMessage(statusCode);
    }
    private string GetDefaultMessage(int statusCode)
    {
        return statusCode switch
        {
            400 => "Incorrect request.",
            401 => "User not authorized",
            404 => "The resource does not exists",
            405 => "HTTP method is not allowed",
            500 => "Error in the server. Contact your administrator"
        };
    }
}
