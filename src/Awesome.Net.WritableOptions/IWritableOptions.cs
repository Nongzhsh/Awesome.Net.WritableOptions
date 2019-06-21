using System;
using Microsoft.Extensions.Options;

namespace Awesome.Net.WritableOptions
{
    public interface IWritableOptions<out T> : IOptionsSnapshot<T> where T : class, new()
    {
        void Update(Action<T> updateAction, bool reload = true);
    }
}