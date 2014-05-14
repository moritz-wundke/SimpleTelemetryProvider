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
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading;
using UnrealBuildTool;

namespace SimpleTelemetry
{
    /// <summary>
    /// Simple JSON RPC 2.0 client
    /// </summary>
    class JSONClient : IDisposable
    {
        /// <summary>
        /// Curren request id used to keep track of requests
        /// </summary>
        private int currentId;

        /// <summary>
        /// URI to where our RPC host sits
        /// </summary>
        private readonly Uri uri;

        /// <summary>
        /// Simple HTTP web client
        /// </summary>
        private readonly WebClient webClient;

        /// <summary>
        /// Headers used for every request
        /// </summary>
        public WebHeaderCollection Headers { get { return this.webClient.Headers; } }

        /// <summary>
        /// Delegate used to get fired when an async rpc completes
        /// </summary>
        /// <param name="id">ID of the response</param>
        /// <param name="response">Server response</param>
        public delegate void AsyncCompletedDelegate(int id, string response);

        /// <summary>
        /// Event used to be called when an async rpc completes
        /// </summary>
        public event AsyncCompletedDelegate AsyncCompleted;
        
        public JSONClient(string url)
        {
            this.uri = new Uri(url);
            this.webClient = new WebClient();
            this.webClient.Headers.Add("Content-Type", "application/json");
            this.webClient.UploadDataCompleted += OnUploadDataCompleted;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="request"></param>
        /// <returns>Server response</returns>
        public string Exec(JSONRequest request)
        {
            try
            {
                string requestSerialized = request.ToJSON();
                Log.WriteLineIf(SimpleTelemetryProvider.bDebugRPCCalls, System.Diagnostics.TraceEventType.Information, requestSerialized);
                byte[] requestBinary = Encoding.UTF8.GetBytes(requestSerialized);
                byte[] resultBinary;
                lock (this.webClient)
                {
                    resultBinary = this.webClient.UploadData(this.uri, "POST", requestBinary);
                }
                string responseSerialized = Encoding.UTF8.GetString(resultBinary);
                Log.WriteLineIf(SimpleTelemetryProvider.bDebugRPCCalls, System.Diagnostics.TraceEventType.Information, responseSerialized);
                return responseSerialized;
            }
            catch (Exception Exception)
            {
                Log.TraceError("JSONClient Exception: " + Exception);
            }
            return "";
        }

        public int ExecAsync(JSONRequest request)
        {
            try {
                string requestSerialized = request.ToJSON();
                Log.WriteLineIf(SimpleTelemetryProvider.bDebugRPCCalls, System.Diagnostics.TraceEventType.Information, requestSerialized);
                byte[] requestBinary = Encoding.UTF8.GetBytes(requestSerialized);
                lock (webClient)
                {
                    this.webClient.UploadDataAsync(this.uri, "POST", requestBinary, request.id);
                }
                return request.id;
            }
            catch (Exception Exception)
            {
                Log.TraceError("JSONClient Exception: " + Exception);
            }
            return -1;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="method"></param>
        /// <param name="methodParams"></param>
        /// <returns></returns>
        public JSONRequest CreateRequest(string method, string SessionID = null, string eventName = null, string commandLine = null, IEnumerable<Tuple<string, string>> parameters = null)
        {
            return new JSONRequest(Interlocked.Increment(ref currentId), method, SessionID, eventName, commandLine, parameters);
        }

        /// <summary>
        /// Called when a async web call has been completed (NOT USED)
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="uploadDataCompletedEventArgs"></param>
        private void OnUploadDataCompleted(object sender, UploadDataCompletedEventArgs uploadDataCompletedEventArgs)
        {
            try
            {
                int id = (int)uploadDataCompletedEventArgs.UserState;
                byte[] responseBinary = uploadDataCompletedEventArgs.Result;
                string responseSerialized = Encoding.UTF8.GetString(responseBinary);
                Log.WriteLineIf(SimpleTelemetryProvider.bDebugRPCCalls, System.Diagnostics.TraceEventType.Information, responseSerialized);

                // fire event.
                if (AsyncCompleted != null)
                    AsyncCompleted(id, responseSerialized); // TODO: Add response
            }
            catch (Exception Exception)
            {
                Log.TraceError("JSONClient Exception: " + Exception);
            }
        }

        public void Dispose()
        {
            lock (this.webClient)
            {
                //this.webClient.UploadDataCompleted -= WebClientOnUploadDataCompleted;
                this.webClient.Dispose();
            }
        }
    }
}
