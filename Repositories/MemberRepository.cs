using repairman.Data;
using repairman.Models;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace repairman.Repositories
{
    public class MemberRepository : IMemberRepository
    {
        private DBContext _context;

        public MemberRepository(DBContext context)
        {
            _context = context;
        }


        public async Task<Member> GetMember(long ID)
        {
            var user = await _context.Members.FindAsync(ID);

            return user;
        }

        public async Task<Member> GetMember(string username)
        {
            var user = await _context.Members.FirstOrDefaultAsync(u => u.username == username);

            return user;
        }

        public Member Update(Member user)
        {
            _context.Members.Update(user);

            return user;
        }
        public async Task<Member> Create(Member user)
        {
            user.date_created = DateTime.UtcNow;

            await _context.Members.AddAsync(user);

            return user;
        }


        public IQueryable<Member> FindMembers(string keyword = null)
        {
            var result = _context.Members.AsQueryable();

            if (!String.IsNullOrWhiteSpace(keyword))
            {
                result = result.Where(u => u.name.Contains(keyword) || u.username.Contains(keyword));
            }

            return result;
        }

        public async Task<Member> TryMemberLogin(string username, string password)
        {
            var user = await GetMember(username);

            if (user == null)
                throw new ErrorCodeException(ErrorCode.NOT_FOUND_USER);

            if (!user.enabled)
                throw new ErrorCodeException(ErrorCode.USER_DISABLED);

            if (!user.VerifyPassword(password))
                throw new ErrorCodeException(ErrorCode.USER_PASSWORD_MISMATCH);

            return user;
        }


    }
}
