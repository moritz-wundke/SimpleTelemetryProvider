"""
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
"""
import web
import ubt.view
import ubt.config
import ubt.db
import uuid
from ubt.view import render

# JSON RPC server taken from : https://github.com/moritz-wundke/simplejsonrpc
from ubt.simplejsonrpc import SimpleJSONRPCService, jsonremote
        
urls = (
    '/', 'index'
    , '/api/', 'json_handler'
    , '/api', 'json_handler'
)

api_service = SimpleJSONRPCService(api_version=1)

class index:        
    def GET(self):
        return render.base(ubt.view.listing())

class json_handler:        
    def POST(self):
        return api_service(web.webapi.data())

@jsonremote(api_service, doc='print api documentation')
def api(request):
    return api_service.api()

@jsonremote(api_service, doc='Generate uuid to be used to group events together')
def request_id(request):
    return str(uuid.uuid1())
    
@jsonremote(api_service, doc='ping server')
def ping(request):
    return "pong"

@jsonremote(api_service, doc='ping server')
def UBT(request, header, event):
    ubt.db.insert_event(header, event)
    return "ok"

if __name__ == "__main__":
    app = web.application(urls, globals())
    app.internalerror = web.debugerror
    app.run()
    