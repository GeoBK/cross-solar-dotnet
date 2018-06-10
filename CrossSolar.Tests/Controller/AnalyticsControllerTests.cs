using System.Threading.Tasks;
using CrossSolar.Controllers;
using CrossSolar.Domain;
using CrossSolar.Models;
using CrossSolar.Repository;
using Microsoft.AspNetCore.Mvc;
using Moq;
using Xunit;

namespace CrossSolar.Tests.Controller
{
    public class AnalyticsControllerTests
    {
        public AnalyticsControllerTests()
        {
            _analyticsController = new AnalyticsController(_analyticsRepositoryMock.Object,_panelRepositoryMock.Object);
            _panelController = new PanelController(_panelRepositoryMock.Object);
        }

        private readonly AnalyticsController _analyticsController;
        private readonly PanelController _panelController;

        private readonly Mock<IPanelRepository> _panelRepositoryMock = new Mock<IPanelRepository>();
        private readonly Mock<IAnalyticsRepository> _analyticsRepositoryMock = new Mock<IAnalyticsRepository>();

        [Fact]
        public async Task Get_ShouldGetObject()
        {
            


            // Arrange
            var panel = new PanelModel
            {
                Brand = "Areva",
                Latitude = 12.345678,
                Longitude = 98.7655432,
                Serial = "AAAA1111BBBB2222"
            };
                        
            await _panelController.Register(panel);

            var panelId = "AAAA1111BBBB2222";
            // Act
            var result = await _analyticsController.Get(panelId);

            // Assert
            Assert.NotNull(result);

            var createdResult = result as OkResult;
            Assert.NotNull(createdResult);
            Assert.Equal(200, createdResult.StatusCode);
        }

        [Fact]
        public async Task DayResults_ShouldGetDayResults()
        {
            // Arrange
            var panel = new PanelModel
            {
                Brand = "Areva",
                Latitude = 12.345678,
                Longitude = 98.7655432,
                Serial = "AAAA1111BBBB2222"
            };

            await _panelController.Register(panel);
            var panelId = "AAAA1111BBBB2222";
            OneHourElectricityModel value = new OneHourElectricityModel
            {
                KiloWatt = 10,
                DateTime = new System.DateTime()
            };
            var temp = new OneDayElectricityModel()
            {
                Sum = 1,
                Average = 1,
                Maximum = 1,
                Minimum = 1,
                DateTime = new System.DateTime()
            };
            await _analyticsController.Post(panelId, value);

            
            // Act
            var result = await _analyticsController.DayResults(panelId);

           

            // Assert
            Assert.NotNull(result);

            var createdResult = result as OkObjectResult;
            Assert.NotNull(createdResult);
            Assert.Equal(200, createdResult.StatusCode);
        }


    }    


}
