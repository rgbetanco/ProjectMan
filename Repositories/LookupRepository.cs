using Microsoft.EntityFrameworkCore;
using repairman.Data;
using repairman.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace repairman.Repositories
{
    public class LookupRepository : ILookupRepository
    {
        private readonly DBContext _context;
        public LookupRepository(DBContext context)
        {
            _context = context;
        }

    }
}
