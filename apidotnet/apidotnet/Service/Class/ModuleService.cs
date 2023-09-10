using apidotnet.DTO;
using apidotnet.Service.Interface;
using Microsoft.EntityFrameworkCore;
using RevisionTool.Data;
using RevisionTool.Entity;
using System.Security.Claims;


namespace apidotnet.Service.Class
{
    public class ModuleService : IModuleService
    {
        private readonly RevisionToolContext context;

        public ModuleService(RevisionToolContext context)
        {
            this.context = context;
        }

        public async Task<IEnumerable<Module>> GetAll()
        {
            try
            {
                var modules = await context.Modules.ToListAsync();
                return modules;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve modules.", ex);
            }
        }

        public async Task<int> Create(ModuleRequest moduleRequest, string email)
        {
            try
            {
                if (await ModuleExist(moduleRequest))
                {
                    throw new Exception("Module already exists.");
                }
                var user = await GetUserByEmail(email);
                if (user == null)
                {
                    throw new Exception("User email id not found." + email);
                }
                var newModule = new Module
                {
                    ModuleName = moduleRequest.ModuleName,
                    CreatedBy = user.UserId,
                };

                context.Modules.Add(newModule);
                await context.SaveChangesAsync();

                return newModule.ModuleId; // Return the newly created module's ID
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to create module. " + ex.Message, ex);
            }
        }

        public async Task Update(int id, ModuleRequest moduleRequest)
        {
            try
            {
                var existingModule = await context.Modules.FirstOrDefaultAsync(m => m.ModuleId == id);

                if (existingModule == null)
                {
                    throw new Exception("Module not found.");
                }

                existingModule.ModuleName = moduleRequest.ModuleName;
                // Update other properties if needed

                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to update module.", ex);
            }
        }

        public async Task Delete(int id)
        {
            try
            {
                var moduleToDelete = await context.Modules.FirstOrDefaultAsync(m => m.ModuleId == id);

                if (moduleToDelete == null)
                {
                    throw new Exception("Module not found.");
                }

                context.Modules.Remove(moduleToDelete);
                await context.SaveChangesAsync();
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to delete module.", ex);
            }
        }

        private Task<bool> ModuleExist(ModuleRequest moduleRequest)
        {
            return context.Modules.AnyAsync(u => u.ModuleName == moduleRequest.ModuleName);
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


        public async Task<Module> GetById(int id)
        {
            try
            {
                var module = await context.Modules.FindAsync(id);
                if (module == null)
                {
                    throw new Exception("Module not found.");
                }
                return module;
            }
            catch (Exception ex)
            {
                throw new Exception("Failed to retrieve module by ID.", ex);
            }
        }

    }
}
