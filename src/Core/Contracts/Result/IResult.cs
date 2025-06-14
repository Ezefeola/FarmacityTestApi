using System.Net;

namespace Core.Contracts.Result;
public interface IResult<TResponse> : IResult
{

    public TResponse? Payload { get; set; }

    public Result<TResponse> WithDescription(string description);
    public Result<TResponse> WithErrors(List<string> errors);
    public Result<TResponse> WithPayload(TResponse payload);
    public abstract static Result<TResponse> Success(HttpStatusCode statusCode);
    public abstract static Result<TResponse> Failure(HttpStatusCode statusCode);
}

public interface IResult
{
    public bool IsSuccess { get; set; }
    public HttpStatusCode HttpStatusCode { get; set; }
    public string Description { get; set; }
    public List<string> Errors { get; set; }
}