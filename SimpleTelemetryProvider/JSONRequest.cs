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

namespace SimpleTelemetry
{
    /// <summary>
    /// JSON RPC 2.0 compilant request
    /// </summary>
    class JSONRequest
    {
        /// <summary>
        /// JSON RPC Protocol version
        /// </summary>
        public string jsonrpc = "2.0";

        /// <summary>
        /// Request id
        /// </summary>
        public int id  { get; set; }

        /// <summary>
        /// Method name
        /// </summary>
        public string method  { get; set; }

        /// <summary>
        /// Request parameters
        /// </summary>
        public List<Dictionary<string, string>> parameters  { get; set; }

        /// <summary>
        /// Create a JSONRequest
        /// </summary>
        /// <param name="id"></param>
        /// <param name="method"></param>
        /// <param name="methodParams"></param>
        public JSONRequest(int id, string method, IEnumerable<Tuple<string, string>> parameters)
        {
            this.id = id;
            this.method = method;
            this.parameters = new List<Dictionary<string,string>>();
            
            Dictionary<string, string> local_params = new Dictionary<string,string>();
            foreach (var item in parameters)
            {
                local_params.Add(item.Item1, item.Item2);
            }

            this.parameters.Add(local_params);
        }

        public string ToJSON()
        {
            string JSON_string = (new JavaScriptSerializer()).Serialize(this);
            // Specification requires us to use params instead of parameters, but params is a reserved
            // keyword in C# :D
            return JSON_string.Replace("\"parameters\"", "\"params\"");
        }
    }
}
