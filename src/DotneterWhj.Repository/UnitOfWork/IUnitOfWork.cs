using System.Data.Common;

namespace DotneterWhj.Repository.UnitOfWork
{
    public interface IUnitOfWork
    {
        DbTransaction BeginTransaction();

        void CommitTransaction();

        void RollbackTransaction();
    }
}