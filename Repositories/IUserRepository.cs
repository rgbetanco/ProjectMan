using repairman.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace repairman.Repositories
{
    public interface IUserRepository
    {
        Task<User> Get(long ID, params string[] includeFields);
        Task<User> Get(string username);

        User Update(User user);
        Task<User> Create(User user);
        IQueryable<User> GetSalesPersons();
        IQueryable<User> Find(string keyword = null, UserPermission? role = null);
        Task<User> TryUserLogin(string username, string password);


        Task<Group> GetGroup(long ID);

        Group Update(Group group);
        Task<Group> Create(Group group);

        bool DelGroupUnsafe(long ID);
        IQueryable<Group> GetGroups();
    }
}
