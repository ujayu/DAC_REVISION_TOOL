using apidotnet.DTO;
using apidotnet.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using RevisionTool.Entity;
using System.Security.Claims;

namespace apidotnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class PointController : ControllerBase
    {
        private readonly IPointService service;

        public PointController(IPointService service)
        {
            this.service = service;
        }

        [Authorize]
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<Point>>> GetPoints()
        {
            try
            {
                var points = await service.GetAll();
                return Ok(points);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("All/{id}")]
        public async Task<ActionResult<IEnumerable<Point>>> GetPoints(int id)
        {
            try
            {
                var points = await service.GetByTopicId(id);
                return Ok(points);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Point>> GetPoint(int id)
        {
            try
            {
                var point = await service.GetById(id);

                if (point == null)
                {
                    return NotFound();
                }

                return Ok(point);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "admin, teacher")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutPoint(int id, [FromBody] PointResponse pointResponse)
        {
            try
            {
                await service.Update(id, pointResponse);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "admin, teacher")]
        [HttpPost]
        public async Task<ActionResult<Point>> PostPoint([FromBody] PointResponse pointResponse)
        {
            try
            {
                var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                if (email == null)
                    return StatusCode(500, new { message = "Email not get " + email });
                var createdPointId = await service.Create(pointResponse, email);
                return CreatedAtAction("GetPoint", new { id = createdPointId }, new { pointId = createdPointId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "admin, teacher")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeletePoint(int id)
        {
            try
            {
                await service.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }
    }
}
