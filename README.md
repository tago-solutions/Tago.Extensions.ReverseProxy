
# Tago - Reverse Proxy

'Tago - Reverse Proxy' is a .net core reverse proxy server acts as an intermediate connection point positioned at a network's edge. It receives initial HTTP connection requests, acting like the actual endpoint. Essentially your network's traffic cop, the reverse proxy serves as a gateway between users and your application origin server.

## ConfigureServices(IServiceCollection services)

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
		"/api/oauth": {
			"routes": {
				"enforce": true,
				"items": [
				{
					"templates": [ "{path?}/get1/{itemid?}", "{path?}/items/{itemid?}" ],
					"verbs": [ "*" ]
				}
				]
			},
			//"UseAuthorization": true,       
			"ConnectorKey": "Local",
			"UpstreamServer": "/auth",
			"Request": {         
				"Authentication": {
					"Provider": "OAuth",
					"OAuth": {
						"ConnectorKey": "Local",
						"Url": "/auth/oauth",
						"Headers": {
						"client_id": {
							"Operation": "AddOrReplace",
							"Value": "12332132123"
						},
						"X-User-Name": {
							"Operation": "AddOrReplace",
							"Source": "UserName"
						},
						"X-TRANSACTION-ID": {
							"Operation": "AddOrReplace",
							"Source": "Cookie",
							"Value": "X-TRANSACTION-ID",
							"EmptyPolicy": null,
							"DefaultValue": "{\"user\": \"{{username}}\", \"remoteip\": \"{{ remoteip }}\"}"

						},
						"X-UUID-ID": {
							"Operation": "AddOrReplace",
							"Source": "Header",
							"Value": "X-UUID-ID",
							"EmptyPolicy": null,
							"DefaultValue": "{{uuid}}"

						},
						"ITEM-ID": {
							"Operation": "AddOrReplace",
							"EmptyPolicy": null,
							"DefaultValue": "{{$uri.itemId}}"

						},
						"body-sample": {
							"Operation": "AddOrReplace",
							"EmptyPolicy": null,
							"DefaultValue": "{{$body.test}}"
						}
						},
						"Payload": {
						"ClientId": "client-id",
						"ClientSecret": "{env: OAUTH_CLIENT_SECRET}",
						"GrantType": "grant-type",
						"Scope": "bearer"
						},            
						"ResultObjectKey": "$oauth.",
						"MapTo": {
							"Headers": {
								"Authorization": {
								"Value": "{{$oauth.Scope}} {{$oauth.AccessToken}}"
								}
							}
						}
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
					"Provider": "Jwe",
					"Jwe": {
						"SigningKey": "oauth"
					},
					"Plugin": {
						"PluginPath": "./SimpleEncryptionPlugin.dll",
						"ClassName": "MyIEncryptionHandler"
					}
				}
				},
				"Headers": {
					"body-sample": {
					"Operation": "AddOrReplace",
					"EmptyPolicy": null,
					"DefaultValue": "{{$body.test}}"
				}            
				}

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
					},
					"X-Item": {
						"Operation": "AddOrReplace",
						"Value": "{{$body.test}}",
						//"Source": "ResponseHeader",
						"Expiration": "00:02:00",
						"HttpOnly": true,
						"SameSite": "Unspecified"
					}
				}
			}
		  },
		  "/api/jwt": {
			"routes": {
			  "enforce": true,
			  "items": [
				{
				  "templates": [ "{path?}/get1/{itemid?}", "{path?}/items/{itemid?}" ],
				  "verbs": [ "*" ]
				}
			  ]
			},
			//"UseAuthorization": true,        
			"ConnectorKey": "Local",
			"UpstreamServer": "/auth",
			"Request": {          
			  "Authentication": {
					"Provider": "Jwt",
					"Jwt": {
						"SigningKey": "A1E06B6586BF5203A7B3EC9C7D4CB53B862E4D19_1_1",
						"HeaderName": "Authorization",
						"HeaderValue": "Bearer {0}",
						"Claims": {
							"orig-user-name": "{{username}}"
							"json-test": {
								"body": "test"
							}
						}
					}            
			  },          
			  "Headers": {
				//"body-sample": {
				//  "Operation": "AddOrReplace",
				//  "EmptyPolicy": null,
				//  "DefaultValue": "{{$body.test}}"
				//}
				//  "client_id": {
				//    "Operation": "AddOrReplace",
				//    "Value": "12332132123"
				//  },
				//  "Authorization": {
				//    "Operation": "AddOrReplace",
				//    "Source": "Cookie",
				//    "Value": "X-Authenticate",
				//    "EmptyPolicy": null
				//  }
				//},
				//"Cookies": {
				//  "X-Authenticate": {
				//    "Operation": "Remove"
				//  }
			  }

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
					},
					"X-Item": {
						"Operation": "AddOrReplace",
						"Value": "{{$body.test}}",
						//"Source": "ResponseHeader",
						"Expiration": "00:02:00",
						"HttpOnly": true,
						"SameSite": "Unspecified"
					}
				}
			}
		}
    }


## Request Manipulation

### Settings Headers

| Operations |
||
| Add |
| AddOrReplace |
| Remove |

| Sources |
|-|
| Const (Default)|
| Header |
| Cookie |
| UserName |

    "Request": {         
          "Headers": {          
            "client_id": {
              "Operation": "AddOrReplace",
              "Value": "12332132123"
            },                    
            "Authorization": {
              "Operation": "AddOrReplace",
              "Source": "Cookie", // copy header value from cookie
              "Value": "X-Authenticate", // cookie key
            }                   
        }
    }

