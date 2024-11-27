using Microsoft.AspNetCore.Mvc;
using DbLib;


namespace DbLiApi
{
    [ApiController]
    [Route("api/[controller]")]
    public class MyApiController : ControllerBase
    {
        [HttpGet]
        public IActionResult GetMessage()
        {
            return Ok("Hello from your API!");
        }

        // Neue POST-Methode, die JSON-Daten entgegennimmt
        [HttpPost]
        public IActionResult PostData([FromBody] MyDataDto data)
        {
            if (data == null)
            {
                return BadRequest("Invalid data.");
            }

            // Hier könntest du die Daten verarbeiten, z.B. in die DB speichern
            return Ok(new { message = "Data received successfully", receivedData = data });
        }
    }

    // Data Transfer Object (DTO) zur Repräsentation der JSON-Daten
    public class MyDataDto
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
    }
}

