using apidotnet.DTO;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using RevisionTool.Data;
using RevisionTool.Entity;
using System.Security.Claims;

namespace apidotnet.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    [Authorize]
    public class MainController : ControllerBase
    {
        private readonly RevisionToolContext _context;

        public MainController(RevisionToolContext context)
        {
            _context = context;
        }


        [HttpGet("module/{moduleId}")]
        public async Task<IActionResult> GetModuleData(int moduleId)
        {
            try
            {
                DateTime currentTime = DateTime.Now;

                // Get user email from JWT token
                var userEmailClaim = User.FindFirst(ClaimTypes.Email);
                if (userEmailClaim == null)
                {
                    return Unauthorized("Email claim missing in token.");
                }

                string userEmail = userEmailClaim.Value;

                int? userId = await GetUserIdFromEmailAsync(userEmail);

                if (userId == null)
                {
                    return Unauthorized("User not found.");
                }

                var module = await _context.Modules.Include(m => m.Topics).FirstOrDefaultAsync(m => m.ModuleId == moduleId);

                if (module == null)
                {
                    return NotFound("Module not found.");
                }

                var pointsInRevision = await _context.PointsInRevisions
                    .Where(pir => pir.UserId == userId && pir.IsActive == 1)
                    .Select(pir => pir.PointId)
                    .ToListAsync();

                int forModulePointsLessNextTimeIfNotThenPointsNotInHistoryButInRevision = 0; // Default value for additional points
                var modulePointsWithLessNextTime = await _context.PointsHistories
                    .Where(p => p.Point.Topic.ModuleId == module.ModuleId && p.NextTime < currentTime && p.User.UserId == userId)
                    .Select(p => p.PointId)
                    .ToListAsync();

                var modulePointsNotInHistoryButInRevision = await _context.PointsInRevisions
                    .Where(pir => pir.UserId == userId && pir.IsActive == 1 && pir.Point.Topic.ModuleId == module.ModuleId
                                 && !modulePointsWithLessNextTime.Contains(pir.PointId))
                    .Select(pir => pir.PointId)
                    .ToListAsync();

                if (modulePointsWithLessNextTime.Count > 0)
                {
                    forModulePointsLessNextTimeIfNotThenPointsNotInHistoryButInRevision = modulePointsWithLessNextTime[0];
                }
                else if (modulePointsNotInHistoryButInRevision.Count > 0)
                {
                    forModulePointsLessNextTimeIfNotThenPointsNotInHistoryButInRevision = modulePointsNotInHistoryButInRevision[0];
                }

                var moduleData = new
                {
                    ModuleName = module.ModuleName,
                    TotalPointsInRevision = pointsInRevision.Count,

                    Topics = module.Topics.Select(topic => new
                    {
                        TopicId = topic.TopicId,
                        TopicName = topic.TopicName
                    }).ToList(),

                    CompletedRevisions = await _context.PointsHistories
                        .Where(ph => ph.User.UserId == userId && ph.AskedTime < currentTime && ph.AskedTime > DateTime.Now)
                        .CountAsync(),

                    RevisionsToDoTomorrow = await _context.PointsHistories
                        .Where(ph => ph.User.UserId == userId && ph.AskedTime.Date == currentTime.Date.AddDays(1))
                        .CountAsync(),

                    ForModulePointsLessNextTimeIfNotThenPointsNotInHistoryButInRevision =
            forModulePointsLessNextTimeIfNotThenPointsNotInHistoryButInRevision
                };

                return Ok(moduleData);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("topic/{topicId}")]
        public async Task<IActionResult> GetPointsForTopic(int topicId)
        {
            try
            {
                // Get user email from JWT token
                var userEmailClaim = User.FindFirst(ClaimTypes.Email);
                if (userEmailClaim == null)
                {
                    return Unauthorized("Email claim missing in token.");
                }

                string userEmail = userEmailClaim.Value;

                int? userId = await GetUserIdFromEmailAsync(userEmail);

                if (userId == null)
                {
                    return Unauthorized("User not found.");
                }

                var points = await _context.Points
                    .Where(p => p.TopicId == topicId)
                    .Select(p => new
                    {
                        PointId = p.PointId,
                        PointName = p.Point1,
                        IsActive = _context.PointsInRevisions
                            .Any(pr => pr.PointId == p.PointId && pr.UserId == userId && pr.IsActive == 1)
                    })
                    .ToListAsync();

                return Ok(points);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


        [HttpGet("point/{pointId}")]
        public async Task<IActionResult> GetPointData(int pointId)
        {
            try
            {
                // Get user email from JWT token
                var userEmailClaim = User.FindFirst(ClaimTypes.Email);
                if (userEmailClaim == null)
                {
                    return Unauthorized("Email claim missing in token.");
                }

                string userEmail = userEmailClaim.Value;

                int? userId = await GetUserIdFromEmailAsync(userEmail);

                if (userId == null)
                {
                    return Unauthorized("User not found.");
                }

                var point = await _context.Points
                    .Where(p => p.PointId == pointId)
                    .Select(p => new
                    {
                        PointId = p.PointId,
                        PointName = p.Point1,
                        IsActive = _context.PointsInRevisions
                            .Any(pr => pr.PointId == p.PointId && pr.UserId == userId && pr.IsActive == 1)
                    })
                    .FirstOrDefaultAsync();

                if (point == null)
                {
                    return NotFound("Point not found.");
                }

                return Ok(point);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPut("pointInRevision/{pointId}")]
        public async Task<IActionResult> TogglePointIsActive(int pointId)
        {
            try
            {
                // Get user email from JWT token
                var userEmailClaim = User.FindFirst(ClaimTypes.Email);
                if (userEmailClaim == null)
                {
                    return Unauthorized("Email claim missing in token.");
                }

                string userEmail = userEmailClaim.Value;

                int? userId = await GetUserIdFromEmailAsync(userEmail);

                if (userId == null)
                {
                    return Unauthorized("User not found.");
                }

                var point = await _context.Points
                    .FirstOrDefaultAsync(p => p.PointId == pointId);

                if (point == null)
                {
                    return NotFound("Point not found.");
                }

                var pointsInRevision = await _context.PointsInRevisions
                    .FirstOrDefaultAsync(pir => pir.UserId == userId && pir.PointId == pointId);

                if (pointsInRevision == null)
                {
                    // Add the point to PointsInRevisions if not present
                    pointsInRevision = new PointsInRevision
                    {
                        UserId = userId.Value,
                        PointId = pointId,
                        IsActive = 1
                    };
                    _context.PointsInRevisions.Add(pointsInRevision);
                }
                else
                {
                    // Toggle IsActive
                    pointsInRevision.IsActive = pointsInRevision.IsActive == 1 ? (sbyte)0 : (sbyte)1;
                }

                await _context.SaveChangesAsync();

                return Ok(new { IsActive = pointsInRevision.IsActive });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }






        private async Task<int?> GetUserIdFromEmailAsync(string userEmail)
        {
            try
            {
                var user = await _context.Users.FirstOrDefaultAsync(u => u.Email == userEmail);
                return user?.UserId;
            }
            catch (Exception)
            {
                return null;
            }
        }


        [HttpGet("GetData")]
        public async Task<IActionResult> GetData()
        {
            try
            {
                DateTime currentTime = DateTime.Now;

                // Get user email from JWT token
                var userEmailClaim = User.FindFirst(ClaimTypes.Email);
                if (userEmailClaim == null)
                {
                    return Unauthorized("Email claim missing in token.");
                }

                string userEmail = userEmailClaim.Value;

                int? userId = await GetUserIdFromEmailAsync(userEmail);

                if (userId == null)
                {
                    return Unauthorized("User not found.");
                }

                // Use the server's current time as the token assignment time
                DateTime tokenAssignTime = DateTime.Now;

                // Calculate aggregate data for all modules
                int pointsWithNextTimeLessThanCurrent_AllModules = await _context.PointsHistories
                    .Where(p => p.NextTime < currentTime)
                    .CountAsync();

                int pointsWithAskTimeGreaterThanTokenAssignTime_AllModules = await _context.PointsHistories
                    .Where(p => p.AskedTime > tokenAssignTime)
                    .CountAsync();

                int pointsWithNextTimeForTomorrow_AllModules = await _context.PointsHistories
                    .Where(p => p.NextTime.Date == currentTime.Date.AddDays(1))
                    .CountAsync();

                int pointsLessNextTimeIfNotThenpointsNotInHistoryButInRevision = 0; // Default value for additional points

                // Calculate data for each module
                var modules = await _context.Modules.ToListAsync();
                var moduleDataList = new List<object>();

                foreach (var module in modules)
                {
                    int pointsWithNextTimeLessThanCurrent = await _context.PointsHistories
                        .Where(p => p.Point.Topic.ModuleId == module.ModuleId && p.NextTime < currentTime)
                        .CountAsync();

                    int pointsWithAskTimeGreaterThanTokenAssignTime = await _context.PointsHistories
                        .Where(p => p.Point.Topic.ModuleId == module.ModuleId && p.AskedTime > tokenAssignTime)
                        .CountAsync();

                    int pointsWithNextTimeForTomorrow = await _context.PointsHistories
                        .Where(p => p.Point.Topic.ModuleId == module.ModuleId && p.NextTime.Date == currentTime.Date.AddDays(1))
                        .CountAsync();

                    int forModulePointsLessNextTimeIfNotThenPointsNotInHistoryButInRevision = 0; // Default value for additional points
                    var modulePointsWithLessNextTime = await _context.PointsHistories
                        .Where(p => p.Point.Topic.ModuleId == module.ModuleId && p.NextTime < currentTime && p.User.UserId == userId)
                        .Select(p => p.PointId)
                        .ToListAsync();

                    var modulePointsNotInHistoryButInRevision = await _context.PointsInRevisions
                        .Where(pir => pir.UserId == userId && pir.IsActive == 1 && pir.Point.Topic.ModuleId == module.ModuleId
                                     && !modulePointsWithLessNextTime.Contains(pir.PointId))
                        .Select(pir => pir.PointId)
                        .ToListAsync();

                    if (modulePointsWithLessNextTime.Count > 0)
                    {
                        forModulePointsLessNextTimeIfNotThenPointsNotInHistoryButInRevision = modulePointsWithLessNextTime[0];
                    }
                    else if (modulePointsNotInHistoryButInRevision.Count > 0)
                    {
                        forModulePointsLessNextTimeIfNotThenPointsNotInHistoryButInRevision = modulePointsNotInHistoryButInRevision[0];
                    }

                    moduleDataList.Add(new
                    {
                        ModuleId = module.ModuleId,
                        ModuleName = module.ModuleName,
                        PointsWithNextTimeLessThanCurrent = pointsWithNextTimeLessThanCurrent,
                        PointsWithAskTimeGreaterThanTokenAssignTime = pointsWithAskTimeGreaterThanTokenAssignTime,
                        PointsWithNextTimeForTomorrow = pointsWithNextTimeForTomorrow,
                        ForModulePointsLessNextTimeIfNotThenPointsNotInHistoryButInRevision =
            forModulePointsLessNextTimeIfNotThenPointsNotInHistoryButInRevision
                    });
                }

                var pointsWithLessNextTime = await _context.PointsHistories
            .Where(p => p.User.UserId == userId)
            .GroupBy(p => p.PointId)
            .OrderByDescending(g => g.Max(p => p.NextTime))
            .Select(g => g.Key)
            .ToListAsync();

                var pointsNotInHistoryButInRevision = await _context.PointsInRevisions
                    .Where(pir => pir.UserId == userId && pir.IsActive == 1 && !pointsWithLessNextTime.Contains(pir.PointId))
                    .Select(pir => pir.PointId)
                    .ToListAsync();

                // Exclude points where the maximum NextTime is greater than current time
                var pointsWithMaxNextTimeGreaterThanCurrent = pointsWithLessNextTime
                    .Where(pointId =>
                        _context.PointsHistories
                            .Any(p => p.PointId == pointId && p.NextTime > currentTime))
                    .ToList();

                pointsWithLessNextTime.RemoveAll(pointId => pointsWithMaxNextTimeGreaterThanCurrent.Contains(pointId));



                // Update the additional point IDs
                if (pointsWithLessNextTime.Count > 0)
                {
                    pointsLessNextTimeIfNotThenpointsNotInHistoryButInRevision = pointsWithLessNextTime[0];
                }
                else if (pointsNotInHistoryButInRevision.Count > 0)
                {
                    pointsLessNextTimeIfNotThenpointsNotInHistoryButInRevision = pointsNotInHistoryButInRevision[0];
                }

                // Combine the aggregate data, module-specific data, and additional point IDs in the response
                var result = new
                {
                    AllModulesData = new
                    {
                        PointsWithNextTimeLessThanCurrent_AllModules = pointsWithNextTimeLessThanCurrent_AllModules,
                        PointsWithAskTimeGreaterThanTokenAssignTime_AllModules = pointsWithAskTimeGreaterThanTokenAssignTime_AllModules,
                        PointsWithNextTimeForTomorrow_AllModules = pointsWithNextTimeForTomorrow_AllModules,
                        PointsLessNextTimeIfNotThenpointsNotInHistoryButInRevision = pointsLessNextTimeIfNotThenpointsNotInHistoryButInRevision
                    },
                    ModuleDataList = moduleDataList
                };

                return Ok(result);
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }
        [HttpGet("AllPoint")]
        public async Task<IActionResult> GetPointIdForAllPoints()
        {
            try
            {
                DateTime currentTime = DateTime.Now;

                // Get user email from JWT token
                var userEmailClaim = User.FindFirst(ClaimTypes.Email);
                if (userEmailClaim == null)
                {
                    return Unauthorized("Email claim missing in token.");
                }

                string userEmail = userEmailClaim.Value;

                int? userId = await GetUserIdFromEmailAsync(userEmail);

                if (userId == null)
                {
                    return Unauthorized("User not found.");
                }

                var pointsWithLessNextTime = await _context.PointsHistories
            .Where(p => p.User.UserId == userId)
            .GroupBy(p => p.PointId)
            .OrderByDescending(g => g.Max(p => p.NextTime))
            .Select(g => g.Key)
            .ToListAsync();

                var pointsNotInHistoryButInRevision = await _context.PointsInRevisions
                    .Where(pir => pir.UserId == userId && pir.IsActive == 1 && !pointsWithLessNextTime.Contains(pir.PointId))
                    .Select(pir => pir.PointId)
                    .ToListAsync();

                // Exclude points where the maximum NextTime is greater than current time
                var pointsWithMaxNextTimeGreaterThanCurrent = pointsWithLessNextTime
                    .Where(pointId =>
                        _context.PointsHistories
                            .Any(p => p.PointId == pointId && p.NextTime > currentTime))
                    .ToList();

                pointsWithLessNextTime.RemoveAll(pointId => pointsWithMaxNextTimeGreaterThanCurrent.Contains(pointId));

                int pointId;

                // Update the additional point IDs
                if (pointsWithLessNextTime.Count > 0)
                {
                    pointId = pointsWithLessNextTime[0];
                }
                else if (pointsNotInHistoryButInRevision.Count > 0)
                {
                    pointId = pointsNotInHistoryButInRevision[0];
                }
                else
                {
                    return NotFound("No eligible points found.");
                }

                return Ok(new { PointId = pointId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }





        [HttpGet("ModulePoint/{moduleId}")]
        public async Task<IActionResult> GetPointIdForModule(int moduleId)
        {
            try
            {
                DateTime currentTime = DateTime.Now;

                // Get user email from JWT token
                var userEmailClaim = User.FindFirst(ClaimTypes.Email);
                if (userEmailClaim == null)
                {
                    return Unauthorized("Email claim missing in token.");
                }

                string userEmail = userEmailClaim.Value;

                int? userId = await GetUserIdFromEmailAsync(userEmail);

                if (userId == null)
                {
                    return Unauthorized("User not found.");
                }

                var pointsWithLessNextTime = await _context.PointsHistories
                    .Where(p => p.NextTime < currentTime && p.User.UserId == userId && p.Point.Topic.ModuleId == moduleId)
                    .Select(p => p.PointId)
                    .ToListAsync();

                var pointsNotInHistoryButInRevision = await _context.PointsInRevisions
                    .Where(pir => pir.UserId == userId && pir.IsActive == 1 && pir.Point.Topic.ModuleId == moduleId
                                 && !pointsWithLessNextTime.Contains(pir.PointId))
                    .Select(pir => pir.PointId)
                    .ToListAsync();

                int pointId;

                if (pointsWithLessNextTime.Count > 0)
                {
                    pointId = pointsWithLessNextTime[0];
                }
                else if (pointsNotInHistoryButInRevision.Count > 0)
                {
                    pointId = pointsNotInHistoryButInRevision[0];
                }
                else
                {
                    return NotFound("No eligible points found.");
                }

                return Ok(new { PointId = pointId });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpGet("lastInterval/{pointId}")]
        public async Task<IActionResult> GetLastIntervalForPoint(int pointId)
        {
            try
            {
                // Get user email from JWT token
                var userEmailClaim = User.FindFirst(ClaimTypes.Email);
                if (userEmailClaim == null)
                {
                    return Unauthorized("Email claim missing in token.");
                }

                string userEmail = userEmailClaim.Value;

                int? userId = await GetUserIdFromEmailAsync(userEmail);

                if (userId == null)
                {
                    return Unauthorized("User not found.");
                }

                var pointHistoriesForUser = await _context.PointsHistories
                    .Where(p => p.PointId == pointId && p.User.UserId == userId)
                    .ToListAsync();

                if (pointHistoriesForUser.Count == 0)
                {
                    return Ok(new { IntervalInMinutes = 0 }); // Return 0 if no point history is found
                }

                var maxNextTime = pointHistoriesForUser.Max(p => p.NextTime);

                var maxAskedTime = pointHistoriesForUser
                    .Where(p => p.NextTime == maxNextTime)
                    .Max(p => p.AskedTime);

                var interval = maxNextTime != null && maxAskedTime != null
                            ? (int)(maxNextTime - maxAskedTime).TotalMinutes
                    : 0;

                return Ok(new { IntervalInMinutes = interval });
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }

        [HttpPost("nextAsk/{pointId}")]
        public async Task<IActionResult> UpdateNextAsk(int pointId, [FromBody] PointsHistoryDTO pointsHistoryDTO)
        {
            try
            {
                // Get user email from JWT token
                var userEmailClaim = User.FindFirst(ClaimTypes.Email);
                if (userEmailClaim == null)
                {
                    return Unauthorized("Email claim missing in token.");
                }

                string userEmail = userEmailClaim.Value;

                int? userId = await GetUserIdFromEmailAsync(userEmail);

                if (userId == null)
                {
                    return Unauthorized("User not found.");
                }

                var point = await _context.Points.FirstOrDefaultAsync(p => p.PointId == pointId);

                if (point == null)
                {
                    return NotFound("Point not found.");
                }

                var currentTime = DateTime.Now;
                var nextTimeInMinutes = pointsHistoryDTO.NextTime;

                var nextTimeDateTime = currentTime.AddMinutes(nextTimeInMinutes);

                var pointsHistory = new PointsHistory
                {
                    UserId = userId.Value,
                    PointId = pointId,
                    AskedTime = currentTime,
                    NextTime = nextTimeDateTime
                };

                _context.PointsHistories.Add(pointsHistory);
                await _context.SaveChangesAsync();

                var pointIdResult = await GetPointIdForAllPoints();

                return pointIdResult;
            }
            catch (Exception ex)
            {
                return BadRequest(ex.Message);
            }
        }


    }
}
