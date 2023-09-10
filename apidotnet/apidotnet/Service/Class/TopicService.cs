using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using apidotnet.DTO;
using Microsoft.EntityFrameworkCore;
using RevisionTool.Data;
using RevisionTool.Entity;
using System.Linq;
using apidotnet.Service.Interface;

namespace apidotnet.Service.Class
{
    public class TopicService : ITopicService
    {
        private readonly RevisionToolContext context;

        public TopicService(RevisionToolContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Topic>> GetAll()
        {
            try
            {
                var topics = await context.Topics.ToListAsync();
                return topics;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve topics.", ex);
            }
        }

        public async Task<IEnumerable<Topic>> GetByModuleId(int moduleId)
        {
            try
            {
                var topics = await context.Topics
                    .Where(topic => topic.ModuleId == moduleId)
                    .ToListAsync();

                return topics;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve topics by module ID.", ex);
            }
        }


        public async Task<int> Create(TopicRequest topicRequest, string email)
        {
            try
            {
                var user = await GetUserByEmail(email);
                if (user == null)
                {
                    throw new Exception("User email not found: " + email);
                }

                var newTopic = new Topic
                {
                    TopicName = topicRequest.TopicName,
                    ModuleId = topicRequest.ModuleId,
                    CreateBy = user.UserId,
                };

                context.Topics.Add(newTopic);
                await context.SaveChangesAsync();

                return newTopic.TopicId; // Return the newly created topic's ID
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create topic. " + ex.Message, ex);
            }
        }

        // Similar implementations for Update, Delete, and other methods


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

        public async Task<Topic> GetById(int id)
        {
            try
            {
                var topic = await context.Topics.FindAsync(id);
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

        public async Task Update(int id, TopicRequest topicRequest)
        {
            try
            {
                var existingTopic = await context.Topics.FirstOrDefaultAsync(t => t.TopicId == id);

                if (existingTopic == null)
                {
                    throw new Exception("Topic not found.");
                }

                existingTopic.TopicName = topicRequest.TopicName;
                // Update other properties if needed

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update topic.", ex);
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                var topicToDelete = await context.Topics.FirstOrDefaultAsync(t => t.TopicId == id);

                if (topicToDelete == null)
                {
                    throw new Exception("Topic not found.");
                }

                context.Topics.Remove(topicToDelete);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete topic.", ex);
            }
        }

    }
}
