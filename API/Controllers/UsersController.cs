using API.Dtos;
using API.Helpers.Errors;
using API.Services;
using Microsoft.AspNetCore.Mvc;

namespace API.Controllers;

public class UsersController : BaseApiController
{
	private readonly IUserService _userService;

	public UsersController(IUserService userService)
	{
		_userService = userService;
	}

	[HttpPost("register")]
	public async Task<ActionResult> RegisterAsync(RegisterDto model)
	{
		var result = await _userService.RegisterAsync(model);
		return Ok(new ApiResponse(201, result));
	}
	[HttpPost("token")]
	public async Task<IActionResult> GetTokenAsync(LoginDto model)
	{
		var result = await _userService.GetTokenAsync(model);
		return Ok(result);
    }

	[HttpPost("addrole")]
	public async Task<IActionResult> AddRoleAsync(AddRoleDto model)
	{
		var result = await _userService.AddRoleAsync(model);
        return Ok(new ApiResponse(200, result));
	}

}
