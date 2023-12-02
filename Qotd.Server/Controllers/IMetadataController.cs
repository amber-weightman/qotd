using Qotd.Application.Models;

namespace Qotd.Api.Controllers;

internal interface IMetadataController
{
    internal Metadata? Metadata { get; set; }
}
