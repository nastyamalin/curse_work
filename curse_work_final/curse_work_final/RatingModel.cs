using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace curse_work_final
{
    public class RatingModel
    {
        public string UserAccount { get; set; }
        public DateTime Date { get; set; }
        public int Rating { get; set; }
    }
    public class RatingUpdateModel
    {
        public string UserAccount { get; set; }
        public DateTime Date { get; set; }
        public int NewRating { get; set; }
    }

    public class RatingDeleteModel
    {
        public string UserAccount { get; set; }
        public DateTime Date { get; set; }
    }
}
