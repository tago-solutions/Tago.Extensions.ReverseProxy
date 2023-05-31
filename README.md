# Reverse Proxy Utility Documentation

The Reverse Proxy Utility is a .NET Core-based utility that acts as a reverse proxy, forwarding incoming requests to upstream servers while providing various authentication and authorization mechanisms. It can be packed as a Docker container for easy deployment and scalability.

## Table of Contents
- [Introduction](#introduction)
- [Configuration](#configuration)
  - [ProxySettings](#proxysettings)
  - [JwtSigner](#jwtsigner)
  - [JwtSettings](#jwtsettings)
  - [ClientCertificates](#clientcertificates)
  - [Connectors](#connectors)
  - [EndPoints](#endpoints)
- [Usage](#usage)
- [Conclusion](#conclusion)

## Introduction
The Reverse Proxy Utility is a .NET Core-based utility that can be packed as a Docker container. It acts as a reverse proxy, forwarding incoming requests to upstream servers while providing various authentication and authorization mechanisms.

## Configuration File
The utility utilizes a configuration file to define its behavior. The configuration file consists of several sections: ProxySettings, JwtSigner, JwtSettings, ClientCertificates, Connectors, and EndPoints.

## ProxySettings
The ProxySettings section contains general settings for the reverse proxy utility. It includes properties such as the listening port, request/response manipulations, static content responses, and more.

## JwtSigner
The JwtSigner section allows you to configure policies for signing JWT tokens. These tokens can be used for upstream calls based on defined policies.

## JwtSettings
The JwtSettings section allows you to configure the retrieval of JWT tokens from an OAuth provider for upstream calls.

## ClientCertificates
The ClientCertificates section allows you to define certificate policies for client authentication. Each policy can specify validation rules based on attributes such as Common Name (CN) and email. These policies can be referenced in the ProxySettings section for authentication requirements.

### Policy Configuration
To define a client certificate policy, provide a unique name for the policy and specify the validation rules using regular expressions. The available fields for validation include:
- `cn`: Common Name (CN) of the certificate
- `email`: Email address associated with the certificate
- `issuer`: Issuer of the certificate
- `serialNumber`: Serial number of the certificate
- `thumbprint`: Thumbprint of the certificate
- `displayName`: Display name of the certificate subject

In the ClientCertificates section, define each policy with its validation rules.

### Validation Types
The `Type` property in the validation section determines the type of validation to perform. The available types are:
- `None`: No validation is performed.
- `Chain`: The certificate chain is validated.
- `Peer`: Only the peer certificate is validated.
- `PeerOrChain`: Either the peer certificate or the certificate chain is validated.

# Connectors

The Connectors section defines upstream URLs and their associated settings. Each connector represents an upstream destination to which the reverse proxy will route requests. The settings within the Connectors section include headers, client certificate usage, request timeout, SSL/TLS protocols, certificate settings, and more.

## Configuration

The Connectors section should be defined within the main configuration file. It includes the following properties:

- `BaseUrl`: The base URL of the upstream server to which requests will be forwarded.
- `Headers`: Additional headers to be included in the outgoing requests to the upstream server.
- `ClientCertificate`: Specifies the client certificate policy to be used for authentication with the upstream server.
- `Timeout`: The timeout duration for requests sent to the upstream server.
- `SslSettings`: SSL/TLS settings for secure connections with the upstream server.
- `SslProtocols`: Specifies the supported SSL protocols for secure connections.
- `CertificateSettings`: Specifies the settings for client certificate usage.

## SslProtocols

The `SslProtocols` property within the `SslSettings` section allows you to specify the supported SSL protocols for secure connections with the upstream server. It determines which SSL/TLS versions the reverse proxy utility can use when establishing a secure connection.

You can specify one or multiple SSL protocols by separating them with commas.

## CertificateSettings

The `CertificateSettings` property within the `SslSettings` section allows you to configure the loading of client certificates for authentication. It provides options for loading certificates from files or the certificate store.

- `FromFile`: Specifies client certificates to be loaded from a file. Each certificate should be defined with the `FilePath` and `Password` properties.
- `FromStore`: Specifies client certificates to be loaded from the certificate store. Each certificate should be defined with the `StoreLocation`, `StoreName`, and `SubjectName` properties.




## EndPoints
The EndPoints section allows you to define specific endpoints and their settings. Each endpoint can have its own authentication requirements, allowed HTTP methods, request/response manipulations, upstream URL, and static content response.




### Endpoint: /api/jwt


## Endpoint Configuration

- **Endpoint**: /api/jwt
  - The path of the endpoint to be configured.

- **ApiKeys**:
  - Inherit: Specifies whether to inherit API keys from the parent configuration.
  - HeaderName: The name of the header used for API key authentication.
  - Keys: An array of API keys allowed for this endpoint.

- **Authentication**:
  - Disabled: Specifies whether authentication is disabled for this endpoint.
  - Scheme: The authentication scheme to be used, such as "Bearer" or "Negotiate".
  - ClientCertificate:
    - Require: Specifies whether client certificate authentication is required.
    - Policies: An array of client certificate validation policies to enforce.

- **Routes**:
  - Enforce: Specifies whether to enforce route configuration for this endpoint.
  - Items: An array of route items containing route templates, HTTP verbs, and additional configurations.

- **ConnectorKey**: The key of the connector to be used for routing requests to the upstream server.

- **UpstreamServer**: The URL or path of the upstream server to which requests will be forwarded.

- **Request**:
  - HttpMethod: The HTTP method to be used for the upstream request.
  - UpstreamPayload: Specifies whether to include the payload of the incoming request in the upstream request.
  - Authentication: 
    - Provider: The authentication provider to be used, such as "Jwt" for JWT-based authentication.
    - Jwt: Configuration for the JWT generation for authenticating the upstream request.
      - SigningKey: The key used to sign the JWT.
      - HeaderName: The name of the header to be set for JWT authentication with the upstream request.
      - HeaderValue: The value of the JWT authentication header.
      - WithPayload: Specifies whether to add client's request payload to the signed JWT.
      - Claims: Custom claims to be added to the JWT.
      - Upstream: set values upstream request.
        -  Header: header configuration
            - Name: header key
            - ValueFormat: string format where **{0}** is the placeholder for the JWT token

    #### example:
    ```yaml
    "Authentication": {
        "Provider": "Jwt",
        "Jwt": {
            "SigningKey": "test2",
            "WithPayload": true,
            "Claims": {
                "identity": "{{username}}",
                "test-claim": "test",
                "payload": "{{$body.some-payload-key}}"
            },
            "Upstream": {
                "Header": {
                    "Name": "Authorization",
                    "ValueFormat": "Bearer {0}"
                }
            }
        }
    }
    ```


- **Headers**: Custom headers to be added or manipulated in the request.
  - Each header configuration consists of:
    - Operation: The operation to be performed on the header, such as "AddOrReplace".
    - DefaultValue: The default value for the header.

- **Response**:
  - CookiesPolicy: Configuration options for handling response cookies.
    - AdjustCookiesPath: Specifies whether to adjust the path of response cookies.
    - MinimumSameSitePolicy: The minimum SameSite policy for response cookies.
  - Cookies: Custom response cookies to be added or manipulated.
    - Each cookie configuration consists of:
      - Operation: The operation to be performed on the cookie, such as "AddOrReplace".
      - Value: The value of the cookie.
      - Source: The source of the cookie, such as "ResponseHeader".
      - Expiration: The expiration time of the cookie.
      - HttpOnly: Specifies whether the cookie is HTTP-only.
      - SameSite: The SameSite policy for the cookie.



<!--
### Endpoint Configuration Example
- **ApiKeys**:
  - Inherit: false
  - HeaderName: ""
  - Keys: []

- **Authentication**:
  - Disabled: true
  - Scheme: "Bearer, Negotiate" (Windows in IIS)
  - ClientCertificate:
    - Require: false
    - Policies: ["Client1"]

- **Routes**:
  - Enforce: true
  - Items:
    - ApiKeys:
      - Inherit: true
      - HeaderName: ""
      - Keys: ["ddd"]
    - Templates: ["{path?}/get1/{itemid?}", "{path?}/items/{itemid?}"]
    - Verbs: ["*"]

- **ConnectorKey**: "LocalSsl"
- **UpstreamServer**: "/authp"

- **Request**:
  - HttpMethod: "POST"
  - UpstreamPayload: false
  - Authentication:
    - Provider: "Jwt"
    - Jwt:
      - SigningKey: "test2"
      - HeaderName: "Authorization"
      - HeaderValue: "Bearer {0}"
      - FromPayload: "{{$body.}}"
      - Claims:
        - identity: "{{username}}"
        - orig-client-cert-email: "{{$certificate.email}}"
        - orig-client-cert-issuer: "{{$certificate.issuer}}"
        - orig-client-cert-id: "{{$certificate.id}}"

- **Headers**:
  - body-sample:
    - Operation: "AddOrReplace"
    - EmptyPolicy: null
    - DefaultValue: "{{$body.test}}"

- **Response**:
  - CookiesPolicy:
    - AdjustCookiesPath: true
    - MinimumSameSitePolicy: "None"
  - Cookies:
    - X-Authenticate:
      - Operation: "AddOrReplace"
      - Value: "Authorization"
      - Source: "ResponseHeader"
      - Expiration: "00:02:00"
      - HttpOnly: true
      - SameSite: "Unspecified"
    - X-Item:
      - Operation: "AddOrReplace"
      - Value: "{{$body.test}}"
      - Expiration: "00:02:00"
      - HttpOnly: true
      - SameSite: "Unspecified"
-->


## Conclusion
This documentation provides an overview of the Reverse Proxy Utility and its configuration options. By utilizing the configuration file, you can customize the behavior of the reverse proxy, including authentication, authorization, request/response manipulations, and routing to upstream servers.

Please review the provided documentation and let me know if you would like any changes or additions.



