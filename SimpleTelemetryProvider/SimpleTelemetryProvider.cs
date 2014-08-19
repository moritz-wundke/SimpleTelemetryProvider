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
using System.Web.Script.Serialization;
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
        private string SessionID = null;

        /// <summary>
        /// URL of the telemetry backend
        /// </summary>
        public static string HostURL = Utils.GetEnvironmentVariable("ue.UBT.TelemetryProviderURL", "http://<you/host/>:8080/api");

        /// <summary>
        /// Method name of the JSON RPC backend
        /// </summary>
        public static string MethodName = Utils.GetEnvironmentVariable("ue.UBT.TelemetryProviderMethodName", "UBT");

        /// <summary>
        /// The Project name is used to be able to use a single backend for multiple projects
        /// </summary>
        public static string ProjectName = Utils.GetEnvironmentVariable("ue.UBT.TelemetryProviderProjectName", "MyProject");

        /// <summary>
        /// Enable UBT telemetry or not
        /// </summary>
        public static bool bEnableTelemetryProvider = Utils.GetEnvironmentVariable("ue.UBT.bEnableTelemetryProvider", true);

        /// <summary>
        /// Print RPC debug information into the log
        /// </summary>
        public static bool bDebugRPCCalls = Utils.GetEnvironmentVariable("ue.UBT.bDebugRPCCalls", false);


        /// <summary>
        /// List of requests we will send at the end
        /// </summary>
        public JSONRequest request;

        public class SessionResponse
        {
            public string result { get; set; }
        }

        public SimpleTelemetryProvider()
        {
            if (SimpleTelemetryProvider.bEnableTelemetryProvider)
            {
                try 
                {
                    proxy = new JSONClient(HostURL);
                    // Ping the server if it faild null the provider
                    var response = proxy.Exec(proxy.CreateRequest("request_id"));
                    SessionID = (new JavaScriptSerializer()).Deserialize<SessionResponse>(response).result;

                    // Create the request
                    request = proxy.CreateRequest(SimpleTelemetryProvider.MethodName, SessionID);
                }
                catch (Exception Exception)
                {
                    Log.TraceError("SimpleTelemetryProvider Exception: " + Exception);
                    proxy = null;
                    SessionID = null;
                }
            }            
        }

        public void SendEvent(string EventName, IEnumerable<Tuple<string, string>> Attributes)
        {
            if (proxy != null)
            {
                // Add the new request
                try
                {
                    request.AddEvent(EventName, Attributes);
                }
                catch (Exception Exception)
                {
                    Log.TraceError("SimpleTelemetryProvider Exception: " + Exception);
                }
            }
        }

        public void Dispose()
        {
            if (proxy != null)
            {
                // Submit event
                proxy.Exec(request);

                // Dispose proxy
                proxy.Dispose();
            }
        }
    }
}
