using System;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace RfidApi.Errors;

public static class CustomErrorHandlers
{
    public static ProblemDetails NotFoundProblem(string detail)
    {
        return new ProblemDetails
        {
            Status = StatusCodes.Status404NotFound,
            Title = "Not Found",
            Detail = detail,
        };
    }

    public static ProblemDetails ConflictProblem(string detail = "rfid_tag already exists")
    {
        return new ProblemDetails
        {
            Status = StatusCodes.Status409Conflict,
            Title = "Conflict",
            Detail = detail,
        };
    }

    public static ProblemDetails ForbiddenProblem(string detail = "Retailer not accredited.")
    {
        return new ProblemDetails
        {
            Status = StatusCodes.Status403Forbidden,
            Title = "Forbidden",
            Detail = detail,
        };
    }
}
