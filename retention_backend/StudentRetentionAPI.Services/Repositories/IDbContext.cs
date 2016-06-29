using System.Data.Entity;
using System.Data.Entity.Infrastructure;

namespace StudentRetentionAPI.Services.Repositories
{
	/// <summary>
	/// Interface for Entity Framework DataContext
	/// </summary>
	public interface IDbContext
	{
		DbSet<T> Set<T>() where T : class;
		DbEntityEntry<T> Entry<T>(T entity) where T : class;
		int SaveChanges();
		void BeginTransaction();
		void Commit();
		void Rollback();
		void Dispose();
	}
}
