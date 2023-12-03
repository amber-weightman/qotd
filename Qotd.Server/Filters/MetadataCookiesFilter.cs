using Microsoft.AspNetCore.Mvc.Filters;
using Qotd.Api.Controllers;
using Qotd.Application.Models;

namespace Qotd.Api.Filters;

public class MetadataCookiesFilter : ActionFilterAttribute
{
    private static CookieOptions CookieOptions = new CookieOptions { Expires = DateTime.Now.AddYears(1) };

    private readonly bool _delete;

    public MetadataCookiesFilter()
    {

    }

    public MetadataCookiesFilter(bool delete)
    {
        _delete = delete;
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        if (context.Controller is not IMetadataController controller)
        {
            return;
        }

        var metadata = GetMetadata(context.HttpContext);
        controller.Metadata = metadata;
    }

    public override void OnActionExecuted(ActionExecutedContext context)
    {
        if (context.Controller is not IMetadataController controller)
        {
            return;
        }

        if (_delete)
        {
            DeleteCookies(context.HttpContext);
        }
        else
        {
            UpdateMetadata(context.HttpContext, controller.Metadata);
        }

    }

    private Metadata GetMetadata(HttpContext context)
    {
        var metadata = context.Request.Cookies
            .Where(c => c.Key.StartsWith(Cookies.Cookies.CookiePrefix))
            .ToDictionary(x => x.Key.Substring(Cookies.Cookies.CookiePrefix.Length), x => x.Value);
        
        return new Metadata { Values = metadata };
    }

    private void UpdateMetadata(HttpContext context, Metadata? metadata)
    {
        if (metadata == null)
        {
            return;
        }

        foreach (var m in metadata.Values)
        {
            context.Response.Cookies.Append($"{Cookies.Cookies.CookiePrefix}{m.Key}", m.Value, CookieOptions);
        }
    }

    private void DeleteCookies(HttpContext context)
    {
        var cookiesToDelete = context.Request.Cookies
            .Where(c => c.Key.StartsWith(Cookies.Cookies.CookiePrefix));

        foreach (var m in cookiesToDelete)
        {
            context.Response.Cookies.Delete($"{Cookies.Cookies.CookiePrefix}{m.Key}");
        }
    }
}
