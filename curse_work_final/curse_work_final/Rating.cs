using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace curse_work_final
{
    public class RatingService
    {
        private List<Rating> ratings = new List<Rating>();

        public List<Rating> GetRatings()
        {
            return ratings;
        }

        public async Task PostRatingAsync(string userId, DateTime date, int rating)
        {
            await Task.Run(() => ratings.Add(new Rating { UserId = userId, Date = date, RatingValue = rating }));
        }

        public async Task UpdateRatingAsync(string userId, DateTime date, int newRating)
        {
            var rating = ratings.FirstOrDefault(r => r.UserId == userId && r.Date == date);
            if (rating != null)
            {
                await Task.Run(() => rating.RatingValue = newRating);
            }
        }

        public async Task DeleteRatingAsync(string userId, DateTime date)
        {
            var rating = ratings.FirstOrDefault(r => r.UserId == userId && r.Date == date);
            if (rating != null)
            {
                await Task.Run(() => ratings.Remove(rating));
            }
        }
    }

    public class Rating
    {
        public string UserId { get; set; }
        public DateTime Date { get; set; }
        public int RatingValue { get; set; }
    }
}
