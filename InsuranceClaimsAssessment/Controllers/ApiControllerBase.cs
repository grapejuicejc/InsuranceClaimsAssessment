using InsuranceClaimsAssessment.Services;
using Microsoft.AspNetCore.Mvc;

namespace InsuranceClaimsAssessment.Controllers
{
    [ApiController]
    public abstract class ApiControllerBase : ControllerBase
    {
        // From the token, never from the request
        protected int UserId => int.Parse(User.FindFirst("sub")!.Value);

        protected ObjectResult ToProblem(ServiceError error, string? message)
        {
            var statusCode = error switch
            {
                ServiceError.NotFound => StatusCodes.Status404NotFound,
                ServiceError.Invalid => StatusCodes.Status400BadRequest,
                ServiceError.Forbidden => StatusCodes.Status403Forbidden,
                ServiceError.Conflict => StatusCodes.Status409Conflict,
                _ => StatusCodes.Status500InternalServerError
            };

            return Problem(title: message, statusCode: statusCode);
        }
    }
}
