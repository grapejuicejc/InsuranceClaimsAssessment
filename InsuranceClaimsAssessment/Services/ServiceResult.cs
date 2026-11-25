namespace InsuranceClaimsAssessment.Services
{
    public enum ServiceError
    {
        NotFound,
        Invalid
    }

    // Either a value, or the reason there is none
    public class ServiceResult<T>
    {
        public T? Value { get; private init; }

        public ServiceError? Error { get; private init; }

        public string? Message { get; private init; }

        public static ServiceResult<T> Success(T value)
        {
            return new ServiceResult<T> { Value = value };
        }

        public static ServiceResult<T> Failure(ServiceError error, string message)
        {
            return new ServiceResult<T> { Error = error, Message = message };
        }
    }
}
