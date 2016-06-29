using System;
using System.Data.Entity;
using StudentRetentionAPI.Services.Models.Entitys;

namespace StudentRetentionAPI.Services.Repositories
{
	public class AppDataContext : DbContext, IDbContext
	{
		private DbContextTransaction _transaction;

		public void BeginTransaction()
		{
			if (_transaction == null)
			{
				_transaction = Database.BeginTransaction();
			}
			else
			{
				throw new ArgumentException("A transaction is already pending");
			}
		}

		public void Commit()
		{
			if (_transaction == null)
			{
				throw new ArgumentException("No transaction is in progress");
			}
			_transaction.Commit();
		}

		public void Rollback()
		{
			if (_transaction == null)
			{
				throw new ArgumentException("No transaction is in progress");
			}
			_transaction.Rollback();
		}

		public DbSet<Student>        Student        { get; set; }
		public DbSet<Enrollment>     Enrollment     { get; set; }
		public DbSet<Course>         Course         { get; set; }
		public DbSet<Assignment>     Assignment     { get; set; }
		public DbSet<Grade>          Grade          { get; set; }
		public DbSet<StudentHistory> Studenthistory { get; set; }
		public DbSet<Communication>  Communication  { get; set; }

		public AppDataContext() : base("name=StudentRetentionConnectionString")
		{
			Configuration.LazyLoadingEnabled = false;
		}
	}
}