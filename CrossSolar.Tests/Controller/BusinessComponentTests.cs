using System;
using System.Threading.Tasks;
using CrossSolar.Controllers;
using CrossSolar.Domain;
using CrossSolar.Models;
using CrossSolar.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Xunit;

namespace CrossSolar.Tests.Controller
{
    public class BusinessComponentTests
    {
        
        static DbContextOptionsBuilder<CrossSolarDbContext> builder = new DbContextOptionsBuilder<CrossSolarDbContext>()
                .UseInMemoryDatabase();
        static CrossSolarDbContext context = new CrossSolarDbContext(builder.Options);
            
                
        private readonly IAnalyticsRepository _analyticsRepository = new AnalyticsRepository(context);

        private readonly IPanelRepository _panelRepository = new PanelRepository(context);

       
        [Fact]
        public async Task GetPanelsByPanelId_ShouldGetPanelist()
        {
            // Arrange
            var id = 1;
            await _analyticsRepository.GetAsync(id);
            var temp = new OneHourElectricity()
            {
                KiloWatt = 1,
                PanelId = "AAAA1111BBBB2222",
                DateTime = DateTime.Now
            };
            await _analyticsRepository.InsertAsync(temp);
            // Act
             _analyticsRepository.GetOneDayMetrics(temp.PanelId);
            var panel = new Panel
            {
                Brand = "Areva",
                Latitude = 12.345678,
                Longitude = 98.7655432,
                Serial = "AAAA1111BBBB2222"
            };
            await _panelRepository.InsertAsync(panel);

            // Act
            var result = _panelRepository.GetPanelsByPanelId(panel.Serial);

            // Assert
            Assert.NotNull(result);
        }
        
        

    }
}
