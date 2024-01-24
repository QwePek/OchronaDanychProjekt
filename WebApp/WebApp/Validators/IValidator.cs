using WebApp.Shared;

namespace WebApp.Validators
{
	public interface IValidator<T>
	{
		Response Validate(T obj);
	}
}
