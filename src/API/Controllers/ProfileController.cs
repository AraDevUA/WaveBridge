using API.Extensions;
using Application.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

[ApiController]
[Route("[controller]")]
[Authorize]
public class ProfileController : ControllerBase
{
    private readonly IUserService _userService;

    public ProfileController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet]
    public async Task<IResult> Get(CancellationToken cancellationToken)
    {
        var result = await _userService.GetCurrentProfileAsync(User.GetUserId(), cancellationToken);
        return result.ToApiResult();
    }
}
