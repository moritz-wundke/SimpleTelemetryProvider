simplejsonrpc
=============

[![Build Status](https://travis-ci.org/moritz-wundke/simplejsonrpc.svg?branch=master)](https://travis-ci.org/moritz-wundke/simplejsonrpc)

Simple JSON-RPC 2.0 compilant middleware for python using decorators. With just a few lines of code you get a JSONRPC 2.0 compilant web API running and ready for your needs!

Usage
----

Just create a service class to store your API calls and decorate your methods:

```python
from simplejsonrpc import *

loginservice = SimpleJSONRPCService(api_version=1)

@jsonremote(loginservice, name='login', doc='Method used to log a user in')
def login(request, user_name, user_pass):
    (...)
```

More complex example interating it into [Web.py!]

```python
import web
        
urls = (
    '/', 'index'
    , '/api/', 'json_handler'
    , '/api', 'json_handler'
)
app = web.application(urls, globals())

api_service = SimpleJSONRPCService(api_version=1)

class index:        
    def GET(self, name):
        if not name: 
            name = 'World'
        return 'Hello, ' + name + '!'

class json_handler:        
    def POST(self):
        return api_service(web.webapi.data())

@jsonremote(api_service, doc='print api documentation')
def api(request):
    return api_service.api()
    
@jsonremote(api_service, doc='ping server')
def ping(request):
    return "pong"

if __name__ == "__main__":
    app.run()
```

Adding API versioning to the previous example

```python
import web
        
urls = (
    '/', 'index'
    , '/api/1/', 'json_handler_v1'
    , '/api/1', 'json_handler_v1'
    , '/api/2/', 'json_handler_v2'
    , '/api/2', 'json_handler_v2'
)
app = web.application(urls, globals())

api_service_v1 = SimpleJSONRPCService(api_version=1)
api_service_v2 = SimpleJSONRPCService(api_version=2)

class index:        
    def GET(self, name):
        if not name: 
            name = 'World'
        return 'Hello, ' + name + '!'

class json_handler_v1:        
    def POST(self):
        return api_service_v1(web.webapi.data())

class json_handler_v1:        
    def POST(self):
        return api_service_v2(web.webapi.data())

@jsonremote(api_service_v1, doc='print api documentation')
def api(request):
    return api_service_v1.api()

@jsonremote(api_service_v1, doc='ping server')
def ping(request):
    return "pong"

@jsonremote(api_service_v2, name='api', doc='print api documentation')
def api_v2(request):
    return api_service_v2.api()

@jsonremote(api_service_v2, name='ping', doc='ping server')
def ping_v2(request):
    return "pong"

if __name__ == "__main__":
    app.run()
```

License
----

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

[Web.py!]:http://webpy.org/
