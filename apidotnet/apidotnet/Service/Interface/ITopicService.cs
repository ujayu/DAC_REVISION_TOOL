using apidotnet.DTO;
using RevisionTool.Entity;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace apidotnet.Service.Interface
{
    public interface ITopicService
    {
        Task<IEnumerable<Topic>> GetAll();
        Task<IEnumerable<Topic>> GetByModuleId(int moduleId);
        Task<Topic> GetById(int id);
        Task<int> Create(TopicRequest topicRequest, string email);
        Task Update(int id, TopicRequest topicRequest);
        Task Delete(int id);
    }
}
