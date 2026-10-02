using Asp.Versioning;
using Microsoft.AspNetCore.Mvc;
using GDB.Api.Common.Constants;

namespace GDB.Api.Application.Controllers.Templates
{
    /// <summary>
    /// Reference controller showing the versioning and routing patterns to follow in real controllers.
    /// Every route comes from <see cref="ApiConstants"/>; no route strings are hardcoded here.
    ///
    /// Scenarios covered:
    ///  1. Action available in every version the controller supports       GET  /api/v1/templates, /api/v2/templates
    ///  2. Same route, different implementation per version ([MapToApiVersion]) GET  /api/v{1|2}/templates/{id}
    ///  3. Action added in a newer version only                              POST /api/v2/templates
    ///  4. Deprecated version (still works, advertised as deprecated)       v1 responses carry "api-deprecated-versions: 1.0"
    ///  5. [action] token route                                             GET  /api/v1/templates/summary
    ///  6. Nested route with constraints + query string                     GET  /api/v1/templates/{id}/items/{itemId}?includeDetails=true
    ///  7. Version-neutral endpoint with an absolute route ("~/")           GET  /api/templates/ping
    ///  8. Reading the requested version inside an action                  (see GetAll)
    ///
    /// Alternative pattern (not shown): one controller per version, e.g. TemplateV1Controller with
    /// [ApiVersion(ApiConstants.Version1)] and TemplateV2Controller with [ApiVersion(ApiConstants.Version2)],
    /// both using [Route(ApiConstants.BaseTemplates)]. Prefer that when most actions change between versions.
    /// </summary>
    [ApiVersion(ApiConstants.Version1, Deprecated = true)]
    [ApiVersion(ApiConstants.Version2)]
    [Route(ApiConstants.BaseTemplates)]
    [ApiController]
    public class TemplateController : ControllerBase
    {
        // Scenarios 1 and 8: no [MapToApiVersion], so it serves both v1 and v2.
        [HttpGet]
        public IActionResult GetAll()
        {
            ApiVersion? version = HttpContext.RequestedApiVersion;
            return Ok(new { version = version?.ToString(), items = new[] { 1, 2, 3 } });
        }

        // Scenario 2: v1 implementation of GET {id}.
        [HttpGet(ApiConstants.TemplateById)]
        [MapToApiVersion(ApiConstants.Version1)]
        public IActionResult GetByIdV1(int id)
        {
            return Ok(new { id, name = $"Template {id}" });
        }

        // Scenario 2: v2 implementation of the same route with a richer response.
        [HttpGet(ApiConstants.TemplateById)]
        [MapToApiVersion(ApiConstants.Version2)]
        public IActionResult GetByIdV2(int id)
        {
            return Ok(new { id, name = $"Template {id}", createdOn = DateTime.UtcNow });
        }

        // Scenario 3: only exists in v2; POST /api/v1/templates returns 400 (unsupported API version).
        [HttpPost]
        [MapToApiVersion(ApiConstants.Version2)]
        public IActionResult Create([FromBody] string name)
        {
            const int newId = 4;
            return CreatedAtAction(nameof(GetByIdV2), new { id = newId, version = HttpContext.RequestedApiVersion?.ToString() }, new { id = newId, name });
        }

        // Scenario 5: [action] is replaced with the method name, giving .../templates/summary.
        [HttpGet(ApiConstants.TemplateAction)]
        public IActionResult Summary()
        {
            return Ok(new { total = 3 });
        }

        // Scenario 6: {id:int} and {itemId:guid} constraints; anything else returns 404.
        [HttpGet(ApiConstants.TemplateItem)]
        public IActionResult GetItem(int id, Guid itemId, [FromQuery] bool includeDetails = false)
        {
            return Ok(new { id, itemId, includeDetails });
        }

        // Scenario 7: version-neutral and outside the versioned route, for health checks and similar endpoints.
        [HttpGet(ApiConstants.TemplatePing)]
        [ApiVersionNeutral]
        public IActionResult Ping()
        {
            return Ok("pong");
        }
    }
}
