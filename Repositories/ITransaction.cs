using Microsoft.AspNetCore.Http;
using repairman.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace repairman.Repositories
{
    public interface ITransaction
    {
        void Begin();

        Task<int> Commit();
        void ResetCommit();
        void Rollback();
    }
}