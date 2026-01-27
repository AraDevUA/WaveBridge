using API.Extensions;
using Application.Dto.Request.Transfers;
using Application.Services.Contracts;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;
[ApiController]
[Route("[controller]")]
[Authorize(Roles = "User")]
public class TransfersController : ControllerBase
{
    private readonly ITransferService _transferService;
    public TransfersController(ITransferService transferService)
    {
        _transferService = transferService;
    }
    [HttpPost]
    public async Task<IResult> StartPlaylistTransferAsync(StartPlaylistTransferRequestDto dto)
    {
        var result = await _transferService.StartPlaylistTransferAsync(User.GetUserId(), dto);
        return result.ToApiResult();
    }
}
