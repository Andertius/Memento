using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Memento.Models
{
    public class GetUserStats
    {
        private readonly MementoDbContext context;

        public GetUserStats(MementoDbContext db)
        {
            context = db;
        }

        public List<object> GetHours(string id, int days)
        {
            var list = new List<object>();

            list.Add(new[] { "Date", "Hours" });

            List<UserStats> data = context.Statistics.Where(stats => stats.UserId == id).ToList();

            if (days > data.Count)
            {
                for (int i = 0; i < data.Count; i++)
                {
                    list.Add(new Object[] { data[i].Date, data[i].HoursPerDay });
                }
            }
            else
            {

            }
            
            return list;
        }

        public List<object> GetAverageHours(string id, int days)
        {
            var list = new List<object>();

            list.Add(new[] { "Date", "Average Hours" });

            List<UserStats> data = context.Statistics.Where(stats => stats.UserId == id).ToList();
            for (int i = 0; i < data.Count; i++)
            {
                list.Add(new Object[] { data[i].Date, data[i].AverageHoursPerDay });
            }
            return list;
        }

        public List<object> GetCards(string id, int days)
        {
            var list = new List<object>();

            list.Add(new[] { "Date", "Cards" });

            List<UserStats> data = context.Statistics.Where(stats => stats.UserId == id).ToList();
            for (int i = 0; i < data.Count; i++)
            {
                list.Add(new Object[] { data[i].Date, data[i].CardsPerDay });
            }
            return list;
        }
    }
}
