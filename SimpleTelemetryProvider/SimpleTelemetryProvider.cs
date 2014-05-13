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
        private string HostUrl = "http://<you/host/>:8080/api";
        private static string MethodName = "UBT";

        /// <summary>
        /// List of requests we will send at the end
        /// </summary>
        public List<JSONRequest> requests = new List<JSONRequest>();

        private string CommandLine = null;

        public SimpleTelemetryProvider()
        {
            try 
            {
                proxy = new JSONClient(HostUrl);
                // Ping the server if it faild null the provider
                proxy.Exec(proxy.CreateRequest("ping"));
            }
            catch (Exception Exception)
            {
                Log.TraceError("SimpleTelemetryProvider Exception: " + Exception);
                proxy = null;
            }
            
        }

        public void SendEvent(string EventName, IEnumerable<Tuple<string, string>> Attributes)
        {
            if (proxy != null)
            {
                // Search for the cmd atribute we use in the header
                
                if (CommandLine == null && Attributes != null && EventName.Contains("CommonAttributes"))
                {
                    foreach (var item in Attributes)
                    {
                        if (String.Compare("CommandLine", item.Item1) == 0)
                        {
                            CommandLine = item.Item2;
                            // We better do not split, this way we get builds, rebuilds, etc
                            //CommandLine = item.Item2.Split(' ')[0];
                            break;
                        }
                    }
                }
                requests.Add(proxy.CreateRequest(MethodName, EventName, CommandLine, Attributes));
            }
        }

        public void Dispose()
        {
            if (proxy != null)
            {
                // Submit all events
                foreach (JSONRequest request in requests)
                {
                    proxy.Exec(request);
                }

                // Dispose proxy
                proxy.Dispose();
            }
        }
    }
}
