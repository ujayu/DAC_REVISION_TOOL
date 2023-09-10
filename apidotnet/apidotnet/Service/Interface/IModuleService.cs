using System.Collections.Generic;
using System.Threading.Tasks;
using apidotnet.DTO;
using RevisionTool.Entity;

namespace apidotnet.Service.Interface
{
    public interface IModuleService
    {
        Task<IEnumerable<Module>> GetAll();
        Task<Module> GetById(int id);
        Task<int> Create(ModuleRequest moduleRequest, string email);
        Task Update(int id, ModuleRequest moduleRequest);
        Task Delete(int id);
    }
}
