namespace Cradaptive.ServerRequests
{
    public interface IServerGetRequest<TResponse, TResult> : IServerRequest<TResponse>
    {
        TResult ExtractResult(TResponse response);
    }
}