namespace KeepUp.Application.Common
{
    public class Result<T>
    {
        public bool IsSuccess { get; private set; }
        public T? Data { get; private set; }
        public string? ErrorMessage { get; private set; }


        public Result() { }

        public static Result<T> Success(T data) => new()
        {
            IsSuccess = true,
            Data = data,
        };

        public static Result<T> Error(string error) => new()
        {
            ErrorMessage = error,
            IsSuccess = false,
        };

    }
}
