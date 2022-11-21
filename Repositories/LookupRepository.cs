using Microsoft.EntityFrameworkCore;
using projectman.Data;
using projectman.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace projectman.Repositories
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
