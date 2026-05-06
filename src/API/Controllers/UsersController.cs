using API.Extensions;
using Application.Dto.Request.Users;
using Application.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[ApiController]
[Route("[controller]")]
[Authorize(Roles = "SuperAdmin, Admin")]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{id}")]
    public async Task<IResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _userService.GetByIdAsync(id, cancellationToken);
        return result.ToApiResult();
    }
    [HttpGet]
    public async Task<IResult> GetAllAsync([FromQuery] UserRequestDto dto, CancellationToken cancellationToken)
    {
        var result = await _userService.GetAllAsync(dto, cancellationToken);
        return result.ToApiResult();
    }
    [HttpPut("{id}")]
    public async Task<IResult> UpdateAsync([FromRoute] Guid id, [FromBody] UserUpdateDto dto, CancellationToken cancellationToken)
    {
        var result = await _userService.UpdateAsync(id, dto, cancellationToken);
        return result.ToApiResult();
    }
    [HttpDelete("{id}")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _userService.DeleteAsync(id, cancellationToken);
        return result.ToApiResult();
    }

    [HttpPost("{userId}/roles")]
    [Authorize(Roles = "SuperAdmin")]
    public async Task<IResult> AssignRoleAsync([FromRoute] Guid userId, [FromBody] Guid roleId, CancellationToken cancellationToken)
    {
        var result = await _userService.AssignRoleAsync(userId, roleId, cancellationToken);
        return result.ToApiResult();
    }
    [HttpGet("roles")]
    [Authorize(Roles ="SuperAdmin")]
    public async Task<IResult> GetRolesAsync(CancellationToken cancellationToken)
    {
        var result = await _userService.GetRolesAsync(cancellationToken);
        return result.ToApiResult();
    }

}
