using API.Extensions;
using Application.Dto.Request.Users;
using Application.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[ApiController]
[Route("[controller]")]
[Authorize]
public class UsersController : ControllerBase
{
    private readonly IUserService _userService;
    public UsersController(IUserService userService)
    {
        _userService = userService;
    }

    [HttpGet("{Id}")]
    public async Task<IResult> GetByIdAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _userService.GetByIdAsync(id, cancellationToken);
        return result.ToApiResult();
    }
    [HttpGet]
    public async Task<IResult> GetAllAsync(CancellationToken cancellationToken)
    {
        var result = await _userService.GetAllAsync(cancellationToken);
        return result.ToApiResult();
    }
    [HttpPut("{id}")]
    public async Task<IResult> UpdateAsync([FromRoute] Guid id, [FromBody] UserUpdateDto dto)
    {
        var result = await _userService.UpdateAsync(id, dto);
        return result.ToApiResult();
    }
    [HttpDelete("{id}")]
    public async Task<IResult> DeleteAsync([FromRoute] Guid id, CancellationToken cancellationToken)
    {
        var result = await _userService.DeleteAsync(id, cancellationToken);
        return result.ToApiResult();
    }


}
