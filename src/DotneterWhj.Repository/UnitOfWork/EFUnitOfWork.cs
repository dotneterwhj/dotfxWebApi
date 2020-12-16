using System;
using System.Collections.Generic;
using System.Data.Common;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DotneterWhj.Repository.UnitOfWork
{
    public class EFUnitOfWork : IUnitOfWork, IDisposable
    {
        private readonly DbContext _dbContext;

        public EFUnitOfWork(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public DbTransaction BeginTransaction()
        {
            if (_dbContext.Database.CurrentTransaction == null || _dbContext.Database.CurrentTransaction.UnderlyingTransaction == null)
            {
                _dbContext.Database.BeginTransaction();
            }

            return _dbContext.Database.CurrentTransaction.UnderlyingTransaction;
        }

        public void CommitTransaction()
        {
            _dbContext.SaveChanges();
            _dbContext.Database.CurrentTransaction.Commit();
        }

        public void Dispose()
        {
            if (_dbContext != null)
            {
                _dbContext.Dispose();
            }
        }

        public void RollbackTransaction()
        {
            _dbContext.Database.CurrentTransaction.Rollback();
        }
    }
}
