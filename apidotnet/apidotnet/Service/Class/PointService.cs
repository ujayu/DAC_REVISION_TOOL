using apidotnet.DTO;
using apidotnet.Service.Interface;
using Microsoft.AspNetCore.Http.HttpResults;
using Microsoft.EntityFrameworkCore;
using RevisionTool.Data;
using RevisionTool.Entity;

namespace apidotnet.Service.Class
{
    public class PointService : IPointService
    {
        private readonly RevisionToolContext context;

        public PointService(RevisionToolContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Point>> GetAll()
        {
            try
            {
                var points = await context.Points.ToListAsync();
                return points;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve points.", ex);
            }
        }

        public async Task<int> Create(PointResponse pointResponse, string email)
        {
            try
            {
                var user = await GetUserByEmail(email);
                if (user == null)
                {
                    throw new Exception("User email id not found." + email);
                }
                var newPoint = new Point
                {
                    TopicId = pointResponse.TopicId,
                    Point1 = pointResponse.Point1,
                    Description = pointResponse.Description,
                    CreateBy = user.UserId,
                };

                context.Points.Add(newPoint);
                await context.SaveChangesAsync();

                return newPoint.PointId; // Return the newly created point's ID
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create point. " + ex.Message, ex);
            }
        }

        public async Task Update(int id, PointResponse pointResponse)
        {
            try
            {
                var existingPoint = await context.Points.FirstOrDefaultAsync(m => m.PointId == id);

                if (existingPoint == null)
                {
                    throw new Exception("Point not found.");
                }

                existingPoint.Point1 = pointResponse.Point1;
                existingPoint.Description = pointResponse.Description;
                // Update other properties if needed

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update point.", ex);
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                var pointToDelete = await context.Points.FirstOrDefaultAsync(m => m.PointId == id);

                if (pointToDelete == null)
                {
                    throw new Exception("Point not found.");
                }

                context.Points.Remove(pointToDelete);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete point.", ex);
            }
        }

        public async Task<User> GetUserByEmail(string email)
        {
            try
            {
                var user = await context.Users.FirstOrDefaultAsync(u => u.Email == email);
                return user;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve user by email.", ex);
            }
        }


        public async Task<Point> GetById(int id)
        {
            try
            {
                var point = await context.Points.FindAsync(id);
                if (point == null)
                {
                    throw new Exception("Point not found.");
                }
                return point;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve point by ID.", ex);
            }
        }

        public async Task<IEnumerable<Point>> GetByTopicId(int topicId)
        {
            try
            {
                var points = await context.Points
                    .Where(point => point.TopicId == topicId)
                    .ToListAsync();

                return points;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve points by topic ID.", ex);
            }
        }
    }
}
