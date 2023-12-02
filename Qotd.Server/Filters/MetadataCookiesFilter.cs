using Microsoft.AspNetCore.Mvc.Filters;
using Qotd.Api.Controllers;
using Qotd.Application.Models;

namespace Qotd.Api.Filters;

public class MetadataCookiesFilter : ActionFilterAttribute
{
    private static CookieOptions CookieOptions = new CookieOptions { Expires = DateTime.Now.AddYears(1) };

    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (filterContext.Controller is not IMetadataController controller)
        {
            return;
        }

        var metadata = GetMetadata(filterContext.HttpContext);
        controller.Metadata = metadata;
    }

    public override void OnActionExecuted(ActionExecutedContext filterContext)
    {
        if (filterContext.Controller is not IMetadataController controller)
        {
            return;
        }

        UpdateMetadata(filterContext.HttpContext, controller.Metadata);
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
}
