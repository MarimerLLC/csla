using System;
using System.Collections.Generic;
using System.Data;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using Rolodex.Data;

namespace Rolodex.DataAccess
{
    /// <summary>
    /// Write repository
    /// </summary>
    public class Repository : IDisposable
    {
        #region Setup

        private readonly CompanyContext _context;

        static Repository()
        {
            Database.SetInitializer(new CompanyInitializer());
        }

        public Repository()
        {
            _context = new CompanyContext(ConfigurationManager.ConnectionStrings["CompanyConnection"].ConnectionString);
        }

        #endregion

        #region Update/Insert/Delete

        /// <summary>
        /// Insert new item into database
        /// </summary>
        /// <typeparam name="TItem">Type of item to insert</typeparam>
        /// <param name="item">Item to insert</param>
        /// <param name="saveImmediately">If set to true, saved occurs right away</param>
        /// <returns>Inserted item</returns>
        public TItem Insert<TItem>(TItem item, bool saveImmediately = true)
            where TItem : class
        {
            var set = _context.Set<TItem>();
            set.Add(item);
            if (saveImmediately)
            {
                _context.SaveChanges();
            }
            return item;
        }

        /// <summary>
        /// Update an item
        /// </summary>
        /// <typeparam name="TItem">Type of item to update</typeparam>
        /// <param name="item">Item to update</param>
        /// <param name="saveImmediately">If set to true, saved occurs right away</param>
        /// <returns>Updated item</returns>
        public TItem Update<TItem>(TItem item, bool saveImmediately = true)
            where TItem : class
        {
            var set = _context.Set<TItem>();
            var entry = _context.Entry(item);
            if (entry != null && entry.State != EntityState.Detached)
            {
                // entity is already in memory
                entry.State = EntityState.Modified;
            }
            else
            {
                set.Attach(item);
                _context.Entry(item).State = EntityState.Modified;
            }

            if (saveImmediately)
            {
                _context.SaveChanges();
            }
            return item;
        }

        /// <summary>
        /// Delete an item
        /// </summary>
        /// <typeparam name="TItem">Type of item to delete</typeparam>
        /// <param name="saveImmediately">If set to true, saved occurs right away</param>
        /// <param name="item">Item to delete</param>
        public void Delete<TItem>(TItem item, bool saveImmediately = true)
           where TItem : class
        {
            var set = _context.Set<TItem>();
            var entry = _context.Entry(item);
            if (entry != null && entry.State != EntityState.Detached)
            {
                // entity is already in memory
                entry.State = EntityState.Deleted;
            }
            else
            {
                set.Attach(item);
                _context.Entry(item).State = EntityState.Deleted;
            }

            if (saveImmediately)
            {
                _context.SaveChanges();
            }
        }

        /// <summary>
        /// Save all pending changes
        /// </summary>
        public void Save()
        {
            _context.SaveChanges();
        }

        /// <summary>
        /// Dispose context
        /// </summary>
        public void Dispose()
        {
            if (_context != null)
            {
                _context.Dispose();
            }
        }

        #endregion

        #region Selects

        public User GetUser(string userLogin)
        {
            return _context.Users.Where(one => one.UserLogin == userLogin).FirstOrDefault();
        }

        public Company GetCompany(int companyID)
        {
            return _context.Companies.Where(one => one.CompanyID == companyID).Include(one => one.Emlpoyees).FirstOrDefault();
        }

        public IEnumerable<EmployeeStatus> GetEmployeeStatuses()
        {
            return _context.EmployeeStatuses.OrderBy(one => one.StatusName);
        }

        public IEnumerable<CompanyInfo> GetCompanyInfos(string partialName)
        {
            return _context.Companies
                .OrderBy(one => one.CompanyName)
                .Where(one => one.CompanyName.Contains(partialName) || partialName == "" || partialName == null)
                .Select(one => new CompanyInfo { CompanyID = one.CompanyID, CompanyName = one.CompanyName });
        }

        #endregion
    }
}
