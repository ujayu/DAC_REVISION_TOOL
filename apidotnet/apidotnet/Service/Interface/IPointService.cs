using apidotnet.DTO;
using RevisionTool.Entity;

namespace apidotnet.Service.Interface
{
    public interface IPointService
    {
        Task<IEnumerable<Point>> GetAll();
        Task<IEnumerable<Point>> GetByTopicId(int topicId);
        Task<Point> GetById(int id);
        Task<int> Create(PointResponse pointResponse, string email);
        Task Update(int id, PointResponse pointResponse);
        Task Delete(int id);
    }
}
