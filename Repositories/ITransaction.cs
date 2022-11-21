using Microsoft.AspNetCore.Http;
using projectman.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectman.Repositories
{
    public interface ITransaction
    {
        void Begin();

        Task<int> Commit();
        void ResetCommit();
        void Rollback();
    }
}