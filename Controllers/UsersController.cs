using DotnetElastic.Models;
using DotnetElastic.Services;
using Microsoft.AspNetCore.Mvc;

namespace DotnetElastic.Controllers;

[ApiController]
[Route("[controller]")]
public class UsersController: ControllerBase{
    private readonly ILogger<UsersController> _logger;
    private readonly IElasticService _elasticService;

    public UsersController(ILogger<UsersController> logger, IElasticService elasticService){
        _logger = logger;
        _elasticService = elasticService;
    }

    [HttpPost("create-index")]
    public async Task<IActionResult> CreateIndex(string indexName){
        await _elasticService.CreateIndexIfNotExistsAsync(indexName);
        return Ok($"Index {indexName} created or already exists");
    }

    [HttpPost("add-user")]
    public async Task<IActionResult> AddUser([FromBody] AppUser appuser){
        var result = await _elasticService.AddOrUpdate(appuser);

        return result ? Ok("User added or updated sucessfully"): StatusCode(500, "Error adding or updating user");
    }

    
    [HttpPost("update-user")]
    public async Task<IActionResult> UpdateUser([FromBody] AppUser appuser){
        var result = await _elasticService.AddOrUpdate(appuser);

        return result ? Ok("User added or updated sucessfully"): StatusCode(500, "Error adding or updating user");
    }

    [HttpGet("get-user/{key}")]
    public async Task<IActionResult> GetUser(string key){
        var user = await _elasticService.Get(key);
        return user != null ? Ok(user) : StatusCode(404, "User Not Found");
    }

    [HttpGet("get-all-users")]
    public async Task<IActionResult> GetAllUsers(){
        var user = await _elasticService.GetAll();
        return user != null ? Ok(user) : StatusCode(500, "Error retrieving users");
    }

     [HttpDelete("delete-user/{key}")]
     public async Task<IActionResult> DeleteUser(string key){
        var result = await _elasticService.Delete(key);
        return result ? Ok("User deleted sucessfully") : StatusCode(404, "User Not Found");
     }

    [HttpGet("test")]
    public IActionResult Test()
    {
        return Ok("API is working!");
    }

}