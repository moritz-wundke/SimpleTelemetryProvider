/*********************** Simple Telementry Provider ************************\
The MIT License (MIT)

Copyright (c) 2014 Moritz Wundke

Permission is hereby granted, free of charge, to any person obtaining a copy
of this software and associated documentation files (the "Software"), to deal
in the Software without restriction, including without limitation the rights
to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
copies of the Software, and to permit persons to whom the Software is
furnished to do so, subject to the following conditions:

The above copyright notice and this permission notice shall be included in all
copies or substantial portions of the Software.

THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE
SOFTWARE.
\***************************************************************************/
using fastJSON;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnrealBuildTool;

namespace SimpleTelemetry
{
    /// <summary>
    /// Very simple telemetry provider sender event to a compatible JSON RPC 2.0 web service
    /// </summary>
    [Telemetry.Provider]
    class SimpleTelemetryProvider : Telemetry.IProvider
    {
        private JSONClient proxy;
        private string HostUrl = "<your/host/url/>";
        private static string MethodName = "UBT";
        public SimpleTelemetryProvider()
        {
            try 
            {
                proxy = new JSONClient(HostUrl);
            }
            catch (Exception Exception)
            {
                Log.TraceError("UnrealBuildTool Exception: " + Exception);
            }
            
        }

        public void SendEvent(string EventName, IEnumerable<Tuple<string, string>> Attributes)
        {
            if (proxy != null)
            {
                proxy.ExecAsync(proxy.CreateRequest(MethodName, Attributes));
            }
        }

        public void Dispose()
        {
            if (proxy != null)
            {
                proxy.Dispose();
            }
        }
    }
}
