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
    public class ModuleController : ControllerBase
    {
        private readonly IModuleService service;

        public ModuleController(IModuleService service)
        {
            this.service = service;
        }

        [Authorize]
        [HttpGet("All")]
        public async Task<ActionResult<IEnumerable<Module>>> GetModules()
        {
            try
            {
                var modules = await service.GetAll();
                return Ok(modules);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<ActionResult<Module>> GetModule(int id)
        {
            try
            {
                var module = await service.GetById(id);

                if (module == null)
                {
                    return NotFound();
                }

                return Ok(module);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "admin, teacher")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutModule(int id, [FromBody] ModuleRequest moduleRequest)
        {
            try
            {
                await service.Update(id, moduleRequest);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "admin, teacher")]
        [HttpPost]
        public async Task<ActionResult<Module>> PostModule([FromBody] ModuleRequest moduleRequest)
        {
            try
            {
                var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                if (email == null)
                    return StatusCode(500, new { message = "Email not get " + email});
                var createdModuleId = await service.Create(moduleRequest, email);
                return CreatedAtAction("GetModule", new { id = createdModuleId }, new { moduleId = createdModuleId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "admin, teacher")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteModule(int id)
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
