using Microsoft.AspNetCore.Http;
using System.Net;

namespace Application.Tests.Fakes;

/// <summary>
/// IHttpContextAccessor with a fixed remote IP, so AuditEntry values are
/// deterministic in handler tests.
/// </summary>
public sealed class TestHttpContextAccessor : IHttpContextAccessor
{
    public const string TestIp = "203.0.113.10";

    public HttpContext? HttpContext { get; set; }

    public TestHttpContextAccessor()
    {
        HttpContext = new DefaultHttpContext
        {
            Connection = { RemoteIpAddress = IPAddress.Parse(TestIp) }
        };
    }
}
