using System;

namespace StudentRetentionAPI.Services.Repositories
{
	/// <summary>
	/// Interface for Unit Of Work pattern
	/// </summary>
	public interface IUnitOfWork : IDisposable
	{
		IRepository<TEntity> GetRepository<TEntity>() where TEntity : class;
		void Save();
		Transaction BeginTransaction();
		void Commit();
		void Rollback();
	}

	public class Transaction : IDisposable
	{
		private bool _committed;
		private readonly IUnitOfWork _uow;

		public Transaction(IUnitOfWork uow)
		{
			_uow = uow;
		}

		public void Commit()
		{
			_uow.Commit();
			_committed = true;
		}

		public void Dispose()
		{
			if (!_committed)
			{
				_uow.Rollback();
			}
		}
	}

	public interface IUnitOfWorkDataWarehouse : IUnitOfWork
	{
	}
}
