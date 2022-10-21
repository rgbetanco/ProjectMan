using repairman.Models;
using System.Linq;
using System.Threading.Tasks;

namespace repairman.Repositories
{
    public interface IMemberRepository
    {
        public Task<Member> GetMember(long ID);
        public Task<Member> GetMember(string username);
        public Member Update(Member user);
        public Task<Member> Create(Member user);
        public IQueryable<Member> FindMembers(string keyword = null);
        public Task<Member> TryMemberLogin(string username, string password);

    }
}