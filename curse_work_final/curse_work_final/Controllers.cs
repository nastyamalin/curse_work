using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace curse_work_final
{
    [ApiController]
    [Route("[controller]")]
    public class Controller : ControllerBase
    {
        private readonly ILogger<Controller> _logger;
        private readonly Clients _clients;

        public Controller(ILogger<Controller> logger, Clients clients)
        {
            _logger = logger;
            _clients = clients;
        }

        [HttpGet("gst")]
        public async Task<IActionResult> GetGST([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var result = await _clients.GetGSTAsync(startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching GST data");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("flr")]
        public async Task<IActionResult> GetFLR([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var result = await _clients.GetFLRAsync(startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching FLR data");
                return StatusCode(500, "Internal server error");
            }
        }

        [HttpGet("sep")]
        public async Task<IActionResult> GetSEP([FromQuery] DateTime startDate, [FromQuery] DateTime endDate)
        {
            try
            {
                var result = await _clients.GetSEPAsync(startDate, endDate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error fetching SEP data");
                return StatusCode(500, "Internal server error");
            }
        }



        private static List<RatingModel> ratings = new List<RatingModel>();

        [HttpGet]
        public IActionResult GetRatings()
        {
            return Ok(ratings);
        }

        [HttpPost]
        public IActionResult PostRating([FromBody] RatingModel rating)
        {
            ratings.Add(rating);
            return CreatedAtAction(nameof(GetRatings), new { date = rating.Date }, rating);
        }

        [HttpPut]
        public IActionResult UpdateRating([FromBody] RatingUpdateModel ratingUpdate)
        {
            var rating = ratings.FirstOrDefault(r => r.UserAccount == ratingUpdate.UserAccount && r.Date == ratingUpdate.Date);
            if (rating == null)
            {
                return NotFound();
            }

            rating.Rating = ratingUpdate.NewRating;
            return NoContent();
        }

        [HttpDelete]
        public IActionResult DeleteRating([FromBody] RatingDeleteModel ratingDelete)
        {
            var rating = ratings.FirstOrDefault(r => r.UserAccount == ratingDelete.UserAccount && r.Date == ratingDelete.Date);
            if (rating == null)
            {
                return NotFound();
            }

            ratings.Remove(rating);
            return NoContent();
        }

       
        [HttpGet("statistics")]
        public async Task<IActionResult> GetStatisticsAsync()
        {
            if (ratings.Count == 0)
            {
                return Ok(new { Message = "No ratings available" });
            }

            var minDate = ratings.Min(r => r.Date);
            var maxDate = ratings.Max(r => r.Date);
            var avgRating = ratings.Average(r => r.Rating);

            var geomagneticStorms = await GetGeomagneticStorms(minDate, maxDate);

            return Ok(new
            {
                MinDate = minDate,
                MaxDate = maxDate,
                AvgRating = avgRating,
                GeomagneticStorms = geomagneticStorms
            });
        }
        private async Task<List<GST>> GetGeomagneticStorms(DateTime startDate, DateTime endDate)
        {
            using (HttpClient httpClient = new HttpClient())
            {
                string url = $"{Constants.AddressGST}?startDate={startDate:yyyy-MM-dd}&endDate={endDate:yyyy-MM-dd}&api_key={Constants.apikey}";
                var response = await httpClient.GetAsync(url);
                response.EnsureSuccessStatusCode();
                var content = await response.Content.ReadAsStringAsync();
                var result = JsonConvert.DeserializeObject<List<GST>>(content);
                return result;
            }
        }
     
    }
}
