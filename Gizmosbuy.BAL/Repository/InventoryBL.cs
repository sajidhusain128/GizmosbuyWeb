using Gizmosbuy.BAL.Commons;
using Gizmosbuy.BAL.Interfaces;
using Gizmosbuy.Core.Interfaces;
using Gizmosbuy.DAL.Data;
using Gizmosbuy.DAL.Models;

namespace Gizmosbuy.BAL.Repository
{
    public class InventoryBL : IInventoryBL
    {
        private readonly ApplicationDbContext _applicationDbContext;
        public InventoryBL(ApplicationDbContext applicationDbContext)
        {
            _applicationDbContext = applicationDbContext;
        }

        public async Task<Object> GetRawData(IDateRange dateRange, IPager pager)
        {
            try
            {
                var rawDataResults = await _applicationDbContext.Procedures.spGetRawDataAsync(dateRange.StartDate, dateRange.EndDate);

                int start = pager.PageStart;
                int length = pager.PageLength;
                string searchValue = pager.SearchValue ?? "";

                List<spGetRawDataResult> mainData = null;

                if (searchValue != "")
                {
                    mainData = rawDataResults.Where(Utility.GetSearchValue<spGetRawDataResult>(searchValue)).ToList();
                }
                else
                {
                    mainData = rawDataResults;
                }

                var totalCount = mainData.Count;
                var filterCount = mainData.Count;

                mainData = mainData
                    .Skip(start)
                    .Take(length)
                    .ToList();

                var data = new
                {
                    data = mainData,
                    draw = pager.Draw,
                    recordsTotal = totalCount,
                    recordsFiltered = filterCount
                };

                return await Task.FromResult(data);
            }
            catch (Exception)
            {
                throw;
            }
        }
    }
}
