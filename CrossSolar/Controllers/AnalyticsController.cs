using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CrossSolar.Domain;
using CrossSolar.Models;
using CrossSolar.Repository;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace CrossSolar.Controllers
{
    [Route("panel")]
    public class AnalyticsController : Controller
    {
        private readonly IAnalyticsRepository _analyticsRepository;

        private readonly IPanelRepository _panelRepository;

        public AnalyticsController(IAnalyticsRepository analyticsRepository, IPanelRepository panelRepository)
        {
            _analyticsRepository = analyticsRepository;
            _panelRepository = panelRepository;
        }

        // GET panel/XXXX1111YYYY2222/analytics
        [Route("{panelId}/[controller]")]
        [HttpGet]
        public async Task<IActionResult> Get([FromRoute] string panelId)
        {
            
                var panel = _panelRepository.GetPanelsByPanelId(panelId);
                //.FirstOrDefaultAsync(x => x.Serial.Equals(panelId, StringComparison.CurrentCultureIgnoreCase));
                

                if (panel == null) return Ok();

            var analytics = await _analyticsRepository.Query()
                    .Where(x => x.PanelId.Equals(panelId, StringComparison.CurrentCultureIgnoreCase)).ToListAsync();

                var result = new OneHourElectricityListModel
                {
                    OneHourElectricitys = analytics.Select(c => new OneHourElectricityModel
                    {
                        Id = c.Id,
                        KiloWatt = c.KiloWatt,
                        DateTime = c.DateTime
                    })
                };
                return Ok(result);
            
            

            
            
        }

        // GET panel/XXXX1111YYYY2222/analytics/day
        [Route("{panelId}/[controller]/day")]
        [HttpGet]
        public async Task<IActionResult> DayResults([FromRoute] string panelId)
        {
            var result = new List<OneDayElectricityModel>();

            result = _analyticsRepository.GetOneDayMetrics(panelId);
            return Ok(result);
        }

        // POST panel/XXXX1111YYYY2222/analytics
        [Route("{panelId}/[controller]")]
        [HttpPost]
        public async Task<IActionResult> Post([FromRoute] string panelId, OneHourElectricityModel value)
        {
            if (!ModelState.IsValid) return BadRequest(ModelState);

            var oneHourElectricityContent = new OneHourElectricity
            {
                PanelId = panelId,
                KiloWatt = value.KiloWatt,
                DateTime = DateTime.UtcNow
            };

            await _analyticsRepository.InsertAsync(oneHourElectricityContent);

            var result = new OneHourElectricityModel
            {
                Id = oneHourElectricityContent.Id,
                KiloWatt = oneHourElectricityContent.KiloWatt,
                DateTime = oneHourElectricityContent.DateTime
            };

            return Created($"panel/{panelId}/analytics/{result.Id}", result);
        }
    }
}