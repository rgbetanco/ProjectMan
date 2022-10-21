using CSHelper.Extensions;
using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Storage;
using repairman.Data;
using repairman.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace repairman.Repositories
{
    public class Transaction : ITransaction
    {

        private readonly DBContext _context;
        IDbContextTransaction _transaction;

        public Transaction(DBContext context)
        {
            _context = context;
        }

        public void Begin()
        {
            _transaction = _context.Database.BeginTransaction();
        }

        public async Task<int> Commit()
        {
            int result = await _context.SaveChangesAsync();
            if (_transaction != null)
            {
                _transaction.Commit();
                _transaction.Dispose();
                _transaction = null;
            }

            return result;
        }

        public void ResetCommit()
        {
            _context.ChangeTracker.ResetAllChanges();
        }

        public void Rollback()
        {
            if (_transaction != null)
            {
                _transaction.Rollback();
                _transaction.Dispose();
                _transaction = null;
            }
        }

    }
}
