using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossSolar.Domain;
using Microsoft.EntityFrameworkCore;

namespace CrossSolar.Repository
{
    public abstract class GenericRepository<T> : IGenericRepository<T>
        where T : class, new()
    {
        protected CrossSolarDbContext _dbContext { get; set; }

        public async Task<T> GetAsync(int id)
        {
            return await _dbContext.FindAsync<T>(id);
        }

        public IQueryable<T> Query()
        {
            return _dbContext.Set<T>().AsNoTracking();
        }
        public List<Panel> GetPanelsByPanelId(string panelId)
        {
            return _dbContext.Panels.Where(x => x.Serial == panelId).ToList();            

        }
        public async Task InsertAsync(T entity)
        {
            _dbContext.Set<T>().Add(entity);
            await _dbContext.SaveChangesAsync();
        }

        public async Task UpdateAsync(T entity)
        {
            _dbContext.Entry(entity).State = EntityState.Modified;
            await _dbContext.SaveChangesAsync();
        }
        public List<OneDayElectricityModel> GetOneDayMetrics(string panelId)
        {
            var historicalList = new List<OneDayElectricityModel>();
            var oldestDate= _dbContext.OneHourElectricitys.Min(x => x.DateTime.Date);
            var currentDate = DateTime.Now;
            while (currentDate.Date >= oldestDate.Date)
            {
                var todaysRecord = _dbContext.OneHourElectricitys.Where(x => x.DateTime.Date == currentDate.Date && x.PanelId == panelId);
                if (todaysRecord != null && todaysRecord.Any())
                {
                    var todaysModel = new OneDayElectricityModel();
                    todaysModel.Maximum = todaysRecord.Max(x => x.KiloWatt);
                    todaysModel.Minimum = todaysRecord.Min(x => x.KiloWatt);
                    todaysModel.Average = todaysRecord.Average(x => x.KiloWatt);
                    todaysModel.Sum =     todaysRecord.Sum(x => x.KiloWatt);
                    todaysModel.DateTime = currentDate;
                    historicalList.Add(todaysModel);
                }
                currentDate = currentDate.AddDays(-1);
            }            
            return historicalList;
        }
    }
}