﻿## ConfigureServices(IServiceCollection services)

    IConfigurationSection s = Configuration.GetSection("ProxySettings");
    var ps = new ProxySettings();
    s.Bind(ps);

    services.AddProxy()
        .AddPlugins(ps) //register plugins
        .AddHttpConnectors(ps); //register connectors


## Configure(IApplicationBuilder app, IWebHostEnvironment env)

    IConfigurationSection s = Configuration.GetSection("ProxySettings");
    var ps = new ProxySettings();
    s.Bind(ps);
    app.RunProxy(ps, 
        
        hooks=> {

            hooks.OnStatusResult(401, async (response, request, resender) =>
            {
                if (response.StatusCode == System.Net.HttpStatusCode.Unauthorized)
                {
                    //request.Headers.Remove("Authorization");
                    return await resender.ResendAsync(request);
                }
                else
                {
                    return response;
                }
            });                   

            hooks.SetCertificatesAsync = async (ctx, ep) => {
                return null;
            };

            hooks.BeforeSend = (ctx, request) =>
            {
                //var f = ctx.ServiceProvider.GetService<IHttpClientHandlerFactory>();

                //request.Headers.TryAddWithoutValidation("user_name", ctx?.HttpContext.User?.Identity?.Name);
                //return ctx;
            };

            hooks.BeforeResponse = (ctx, routePath, setting) =>
            {
                //ctx.HttpContext.Response.Cookies.Append("justbeforeresponse", "yes", new CookieOptions
                //{
                //    Path = routePath
                //});                        
            };
    });



    app.UseAuthentication();
    app.UseAuthorization();



## Configuration

### Http Connectors

Defining an http connectors pull for references use
```javascript
"Connectors": {
    "Local": {
        "BaseUrl": "http://localhost:5008",
        "SslSettings": {
            "SslProtocols": "3072",
            "CertificateSettings": {
                "FromFile": [
                    {
                        "FilePath": "./certs/private.pfx"
                    }
                ],
                "FromStore": [
                    {
                        "StoreLocation": "LocalMachine",
                        "StoreName": "My",
                        "SubjectName": "test"
                    }
                ]
            }
        }
    }
}
```

**Local** - Connector's key for references usage

**BaseUrl** - base url

**SslSettings** - use this if the endpoint required mtls connection

**CertificateSettings** - certificate location


### Endpoints
    "EndPoints": {
        "/api/test": {        
            "UpstreamServer": "/anon",
            "Request": {
            },
            "Response": {
            "CookiesPolicy": {
                "AdjustCookiesPath": true,
                "MinimumSameSitePolicy": "None"
            }, 
            "Headers": {
                "client_id": {
                    "Operation": "AddOrReplace",
                    "Value": "12332132123"
                }
            },  
            "Cookies": {       
                "X-Authenticate": {
                    "Operation": "AddOrReplace",
                    "Value": "Authorization",
                    "Source": "ResponseHeader",
                    "Expiration": "00:02:00",
                    "HttpOnly": true,
                    "SameSite": "Unspecified"
                    }
                }
            }
        },
        "/api/auth": {       
            "ConnectorKey": "Local", //connector ref
            "UpstreamServer": "/auth",
            "Request": {          
                "Authentication": {
                    "Provider": "OAuth",
                    "Jwt": {
                        "SigningKey": "A1E06B6586BF5203A7B3EC9C7D4CB53B862E4D19",
                        "HeaderName": "Authorization",
                        "HeaderValue": "Bearer {0}"
                    },
                    "OAuth": {
                        "ConnectorKey": "Local", //connector ref
                        "Url": "/auth/oauth",
                        "Payload": {
                            "ClientId": "client-id",
                            "ClientSecret": "{env: OAUTH_CLIENT_SECRET}",
                            "GrantType": "grant-type",
                            "Scope": "bearer"
                        },
                        "Response": {
                            "In": "body"
                        },
                        "HeaderKey": "Authorization",
                        "HeaderValue": "Bearer {0}"
                    }
                },
                "Payload": {
                    "Digest": {
                        "DigestOriginal": true,
                        "HashAlgorithm": "sha512",
                        "HeaderName": "",
                        "HeaderValue": ""
                    },
                    "Encryption": {
                        "Provider": "Plugin",
                        "Jwe": {
                            "SigningKey": "A1E06B6586BF5203A7B3EC9C7D4CB53B862E4D19"
                        },
                        "Plugin": {
                            "PluginPath": "./SimpleEncryptionPlugin.dll",
                            "ClassName": "MyIEncryptionHandler"
                        }
                    }
                }
                //"Headers": {
                //  "client_id": {
                //    "Operation": "AddOrReplace",
                //    "Value": "12332132123"
                //  },         
                //  "X-User-Name": {
                //    "Operation": "AddOrReplace",
                //    "Source": "UserName"
                //  },
                //  "Authorization": {
                //    "Operation": "AddOrReplace",
                //    "Source": "Cookie",
                //    "Value": "X-Authenticate",                
                //  }
                //},
                //"Cookies": {
                //  "X-Authenticate": {
                //    "Operation": "Remove"
                //  }
                //},
            },
            "Response": {
                "CookiesPolicy": {
                    "AdjustCookiesPath": true,
                    "MinimumSameSitePolicy": "None"
                },
                "Cookies": {
                    "X-Authenticate": {
                    "Operation": "AddOrReplace",
                    "Value": "Authorization",
                    "Source": "ResponseHeader",
                    "Expiration": "00:02:00",
                    "HttpOnly": true,
                    "SameSite": "Unspecified"
                    }
                }
            }
        }     
    }