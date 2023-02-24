﻿using Microsoft.AspNetCore.Mvc;
using MyNotes.API.Models.DtoModels;
using MyNotes.API.Models.ErrorResponse;
using MyNotes.API.Repositories;

namespace MyNotes.API.Controllers;

[Route("/api/user")]
[ApiController]
public class UsersController : Controller
{
    private readonly IUsersRepository _usersRepository;

    public UsersController(IUsersRepository usersRepository)
    {
        _usersRepository = usersRepository;
    }

    [HttpGet]
    public async Task<IActionResult> GetUserInformation([FromQuery] string email, [FromQuery] string password)
    {
        try
        {
            var credentials = new UserCredentials
            {
                Email = email,
                Password = password
            };
            var user = await _usersRepository.GetUserInformation(credentials);
            if (user != null) return Ok(user);

            return NotFound("No user with those credentials was found.");
        }
        catch (Exception e)
        {
            return Problem(e.ToString(), ToString(), 500, "Server ERROR");
        }
    }

    [HttpPost]
    public async Task<IActionResult> CreateUser([FromBody] UserUpload userUpload)
    {
        try
        {
            var added = await _usersRepository.CreateUser(userUpload);
            if (added.StatusType == StatusType.Success) return Ok(added);

            return BadRequest(added);
        }
        catch (Exception e)
        {
            return Problem(e.ToString(), ToString(), 500, "Server ERROR");
        }
    }

    [HttpPut]
    public async Task<IActionResult> UpdateUser([FromBody] UserUpload userUpload, [FromQuery] int userId,
        CancellationToken token)
    {
        try
        {
            var updateUser = await _usersRepository.UpdateUser(userId, userUpload, token);
            if (updateUser.StatusType == StatusType.Success) return Ok(updateUser);

            return BadRequest(updateUser);
        }
        catch (Exception e)
        {
            return Problem(e.ToString(), ToString(), 500, "Server ERROR");
        }
    }

    [HttpDelete]
    public async Task<IActionResult> DeleteUser([FromQuery] int userId, CancellationToken token)
    {
        try
        {
            var deletedUser = await _usersRepository.DeleteUser(userId, token);
            if (deletedUser.StatusType == StatusType.Success) return Ok(deletedUser);

            return BadRequest(deletedUser);
        }
        catch (Exception e)
        {
            return Problem(e.ToString(), ToString(), 500, "Server ERROR");
        }
    }
}