using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace InvalidPlayer.Plugin
{
    public interface IExternalInterface
    {
        void AddCallback(string javaScriptFunc, Func<string, Task<string>> func);
        Task<string> Callback(string functionName, string param);
        Task<string> CallAsync(params string[] arguments);
    }


}