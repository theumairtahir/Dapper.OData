using Microsoft.Extensions.Logging;
using System;

namespace Dapper.OData.Infrastructure
{
    internal interface ITryCatch
    {
        T Try<T>(Func<T> tryAction, out bool isSuccessful, bool throwException = false, Func<T> catchAction = null, Action finallyAction = null);
    }

    internal class TryCatch : ITryCatch
    {
        //private readonly ILogger //_logger;

        public TryCatch(/*ILogger logger*/)
        {
           // //_logger = logger;
        }
        public T Try<T>(Func<T> tryAction, out bool isSuccessful, bool throwException = false, Func<T> catchAction = null, Action finallyAction = null)
        {
            try
            {
                //_logger?.LogInformation("Invoking try Function");
                var obj = tryAction.Invoke();
                //_logger?.LogInformation("Invoke Successful");
                isSuccessful = true;
                //_logger?.LogInformation($"Returning object of type: {typeof(T).Name}");
                return obj;
            }
            catch (Exception ex)
            {
                //_logger?.LogError(ex.Message);
                isSuccessful = false;
                if (catchAction is not null)
                {
                    //_logger?.LogError("Invoking Catch Action");
                    return catchAction.Invoke();
                }

                if (throwException)
                    throw;
                else
                    return Activator.CreateInstance<T>();
            }
            finally
            {
                finallyAction?.Invoke();
            }
        }
    }
}
