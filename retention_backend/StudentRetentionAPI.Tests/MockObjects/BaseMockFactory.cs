using System;
using System.Collections.Generic;
using System.Linq;

namespace StudentRetentionAPI.Tests.MockObjects
{
	class BaseMockFactory
	{
		protected readonly Dictionary<Type, object> _repositories;

		public BaseMockFactory()
		{
			_repositories = new Dictionary<Type, object>();
		}

		/// <summary>
		/// Returns mock data for a certain entity
		/// </summary>
		/// <typeparam name="T">Type of entity</typeparam>
		/// <returns>Mock data for a certain entity</returns>
		public List<T> GetMockData<T>() where T : class
		{
			if (_repositories.Keys.Contains(typeof(T)))
			{
				return _repositories[typeof(T)] as List<T>;
			}
			return null;
		}
	}
}
