using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using apidotnet.DTO;
using apidotnet.Service.Interface;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RevisionTool.Entity;

namespace apidotnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class TopicController : ControllerBase
    {
        private readonly ITopicService _service;

        public TopicController(ITopicService service)
        {
            _service = service;
        }

        [Authorize]
        [HttpPost]
        public async Task<ActionResult<Topic>> PostTopic([FromBody] TopicRequest topicRequest)
        {
            try
            {
                var email = User.Claims.FirstOrDefault(c => c.Type == ClaimTypes.Email)?.Value;
                if (email == null)
                    return StatusCode(500, new { message = "Email not retrieved: " + email });

                var createdTopicId = await _service.Create(topicRequest, email);
                return CreatedAtAction("GetTopic", new { id = createdTopicId }, new { topicId = createdTopicId });
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "admin, teacher")]
        [HttpPut("{id}")]
        public async Task<IActionResult> PutTopic(int id, [FromBody] TopicRequest topicRequest)
        {
            try
            {
                await _service.Update(id, topicRequest);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize(Roles = "admin, teacher")]
        [HttpDelete("{id}")]
        public async Task<IActionResult> DeleteTopic(int id)
        {
            try
            {
                await _service.Delete(id);
                return NoContent();
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { message = ex.Message });
            }
        }

        [Authorize]
        [HttpGet("All")]
        public async Task<IEnumerable<Topic>> GetAll()
        {
            try
            {
                var topics = await _service.GetAll();
                return topics;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve topics.", ex);
            }
        }

        [Authorize]
        [HttpGet("All/{id}")]
        public async Task<IEnumerable<Topic>> GetByModuleId(int id)
        {
            try
            {
                var topics = await _service.GetByModuleId(id);
                return topics;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve topics.", ex);
            }
        }

        [Authorize]
        [HttpGet("{id}")]
        public async Task<Topic> GetTopic(int id)
        {
            try
            {
                var topic = await _service.GetById(id);
                if (topic == null)
                {
                    throw new Exception("Topic not found.");
                }
                return topic;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve topic by ID.", ex);
            }
        }
    }
}
