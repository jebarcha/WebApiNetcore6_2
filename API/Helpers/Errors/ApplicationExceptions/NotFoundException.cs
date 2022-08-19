namespace API.Helpers.Errors.ApplicationExceptions;
public class NotFoundException : ApplicationException
{
	public NotFoundException(string name, object key) : base ($"{name} ({key}) was not found")
	{

	}
}
