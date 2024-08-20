using Microsoft.AspNetCore.Mvc;
using System.IO;
using System.Threading.Tasks;

[Route("api/[controller]")]
[ApiController]
public class JsonController : ControllerBase
{
    private readonly string jsonFilePath = "emaillist.json";

    [HttpPost("update")]
    public async Task<IActionResult> UpdateJsonFile([FromBody] List<Dictionary<string, string>> newContent)
    {
        try
        {
            string jsonContent = System.Text.Json.JsonSerializer.Serialize(newContent);
            await System.IO.File.WriteAllTextAsync(jsonFilePath, jsonContent);
            return Ok(new { message = "JSON file updated successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error updating JSON file", error = ex.Message });
        }
    }

    [HttpGet("read")]
    public async Task<IActionResult> ReadJsonFile()
    {
        try
        {
            string jsonContent = await System.IO.File.ReadAllTextAsync(jsonFilePath);
            var content = System.Text.Json.JsonSerializer.Deserialize<List<Dictionary<string, string>>>(jsonContent);
            return Ok(content);
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error reading JSON file", error = ex.Message });
        }
    }

    [HttpPost("add")]
    public async Task<IActionResult> AddEmail([FromBody] Dictionary<string, string> newEmail)
    {
        try
        {
            string jsonContent = await System.IO.File.ReadAllTextAsync(jsonFilePath);
            var content = System.Text.Json.JsonSerializer.Deserialize<List<Dictionary<string, string>>>(jsonContent);
            if (content == null)
            {
                throw new Exception("Error reading JSON file");
            }
            content.Add(newEmail);
            string updatedJsonContent = System.Text.Json.JsonSerializer.Serialize(content);
            await System.IO.File.WriteAllTextAsync(jsonFilePath, updatedJsonContent);
            return Ok(new { message = "Email added successfully" });
        }
        catch (Exception ex)
        {
            return StatusCode(500, new { message = "Error adding email", error = ex.Message });
        }
    }
}