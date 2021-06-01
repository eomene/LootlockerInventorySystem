using System;

namespace Cradaptive.ServerRequests
{
    public interface IServerRequest<TResponse>
    {
        void MakeRequest(Action<TResponse> onResponseReceived);
        bool ResponseIsSuccesful(TResponse response);
    }
}