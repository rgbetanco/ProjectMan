using Microsoft.EntityFrameworkCore;
using projectman.Data;
using projectman.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectman.Repositories
{
    public class UserRepository : IUserRepository
    {
        private readonly DBContext _context;

        public UserRepository(DBContext context)
        {
            _context = context;
        }

        public async Task<User> Get(long ID, params string[] includeFields)
        {
            var query = _context.Users.AsQueryable();

            if (includeFields != null)
            {
                foreach (var field in includeFields)
                    query = query.Include(field);
            }

            var user = await query.FirstOrDefaultAsync(u => u.ID == ID);

            return user;
        }

        public async Task<User> Get(string username)
        {
            User user = await _context.Users.FirstOrDefaultAsync(u => u.username == username);

            return user;
        }

        public User Update(User user)
        {
            _context.Users.Update(user);

            return user;
        }
        public async Task<User> Create(User user)
        {
            user.date_created = DateTime.UtcNow;

            await _context.Users.AddAsync(user);

            return user;
        }

        public IQueryable<User> GetSalesPersons()
        {
            var result = _context.Users.AsQueryable().Where(m => m.isSalePerson == true);
            return result;
        }

        public IQueryable<User> Find(string keyword = null, UserPermission? role = null)
        {
            var result = _context.Users.AsQueryable();

            if (!String.IsNullOrWhiteSpace(keyword))
            {
                result = result.Where(u => u.name.Contains(keyword) || u.username.Contains(keyword));
            }

            if (role != null && role.Value != 0)
            {
                result = result.Where(u => (u.perm & role.Value) != 0);
            }



            return result;
        }

        public async Task<User> TryUserLogin(string username, string password)
        {
            User user = await Get(username);

            if (user == null)
                throw new ErrorCodeException(ErrorCode.NOT_FOUND_USER);

            if (!user.enabled)
                throw new ErrorCodeException(ErrorCode.USER_DISABLED);

            if (!user.VerifyPassword(password))
                throw new ErrorCodeException(ErrorCode.USER_PASSWORD_MISMATCH);

            return user;
        }


        public async Task<Group> GetGroup(long ID) {
            Group group = await _context.Groups.FindAsync(ID);

            return group;
        }

        public Group Update(Group group)
        {
            _context.Groups.Update(group);

            return group;
        }

        public async Task<Group> Create(Group group)
        {
            group.date_created = DateTime.UtcNow;

            await _context.Groups.AddAsync(group);

            return group;
        }

        public bool DelGroupUnsafe(long ID)
        {
            var s = new Group { ID = ID };

            _context.Groups.Attach(s);
            _context.Groups.Remove(s);

            return true;
        }

        public IQueryable<Group> GetGroups()
        {
            return _context.Groups;
        }

        public async Task<InternalCompany> GetInternalCompanySetting(long ID)
        {
            InternalCompany internalCompany = await _context.InternalCompanies.FindAsync(ID);

            return internalCompany;
        }

        public IQueryable<InternalCompany> GetInternalCompanySettings()
        {
            return _context.InternalCompanies;
        }

        public async Task<InternalCompany> CreateInternalCompanySettings(InternalCompany t)
        {
            await _context.InternalCompanies.AddAsync(t);

            return t;
        }

        public bool DelInternalCompanySettingUnsafe(long ID)
        {
            var s = new InternalCompany { ID = ID };

            _context.InternalCompanies.Attach(s);
            _context.InternalCompanies.Remove(s);

            return true;
        }

    }
}
