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

            List<UserStats> data = context.Statistics
                .Where(stats => stats.UserId == id)
                .ToList();

            if (days > data.Count)
            {
                DateTime comparator = DateTime.UtcNow.AddDays(-30);

                comparator = new DateTime(comparator.Year, comparator.Month, comparator.Day);

                for (int i = data.Count - 1; i > 1; i--)
                {
                    DateTime date = data[i].Date;

                    date = new DateTime(date.Year, date.Month, date.Day);

                    if (date < comparator)
                    {
                        context.Remove(data[i]);
                        context.SaveChanges();
                    }

                }

                data = context.Statistics
                    .Where(stats => stats.UserId == id)
                    .ToList();
                if(data.Count != 0)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        DateTime date = data[i].Date;

                        date = new DateTime(date.Year, date.Month, date.Day);

                        comparator = DateTime.UtcNow.AddDays(-days);
                        comparator = new DateTime(comparator.Year, comparator.Month, comparator.Day);

                        if(date >= comparator)
                        {
                            string returnDate = $"{date.Day}/{date.Month}/{date.Year}";

                            list.Add(new object[] { returnDate, data[i].HoursPerDay });
                        }
                        
                    }
                }
                else
                {
                    DateTime date = DateTime.UtcNow;

                    date = new DateTime(date.Year, date.Month, date.Day);

                    string returnDate = $"{date.Day}/{date.Month}/{date.Year}";
                    list.Add(new object[] { returnDate, 0});
                }
                
            }
            else
            {
                DateTime comparator = DateTime.UtcNow.AddDays(-30);

                comparator = new DateTime(comparator.Year, comparator.Month, comparator.Day);

                for (int i = data.Count - 1; i > 1; i--)
                {
                    DateTime date = data[i].Date;

                    date = new DateTime(date.Year, date.Month, date.Day);

                    if (date < comparator)
                    {
                        context.Remove(data[i]);
                        context.SaveChanges();
                    }
                }

                data = context.Statistics
                    .Where(stats => stats.UserId == id)
                    .ToList();

                if (data.Count != 0)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        DateTime date = data[i].Date;

                        date = new DateTime(date.Year, date.Month, date.Day);

                        comparator = DateTime.UtcNow.AddDays(-days);
                        comparator = new DateTime(comparator.Year, comparator.Month, comparator.Day);

                        if (date >= comparator)
                        {
                            string returnDate = $"{date.Day}/{date.Month}/{date.Year}";

                            list.Add(new object[] { returnDate, data[i].HoursPerDay });
                        }
                    }
                }
                else
                {
                    DateTime date = DateTime.UtcNow;

                    date = new DateTime(date.Year, date.Month, date.Day);
                    string returnDate = $"{date.Day}/{date.Month}/{date.Year}";
                    list.Add(new object[] { returnDate, 0 });
                }

            }
            
            return list;
        }

        public List<object> GetAverageHours(string id, int days)
        {
            var list = new List<object>
            {
                new[] { "Date", "Average Hours" }
            };

            List<UserStats> data = context.Statistics
                .Where(stats => stats.UserId == id)
                .ToList();

            if (days > data.Count)
            {
                DateTime comparator = DateTime.UtcNow.AddDays(-30);

                comparator = new DateTime(comparator.Year, comparator.Month, comparator.Day);

                for (int i = data.Count - 1; i > 1; i--)
                {
                    DateTime date = data[i].Date;

                    date = new DateTime(date.Year, date.Month, date.Day);

                    if (date < comparator)
                    {
                        context.Remove(data[i]);
                        context.SaveChanges();
                    }

                }

                data = context.Statistics
                    .Where(stats => stats.UserId == id)
                    .ToList();

                if (data.Count != 0)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        DateTime date = data[i].Date;

                        date = new DateTime(date.Year, date.Month, date.Day);

                        comparator = DateTime.UtcNow.AddDays(-days);
                        comparator = new DateTime(comparator.Year, comparator.Month, comparator.Day);

                        if (date >= comparator)
                        {
                            string returnDate = $"{date.Day}/{date.Month}/{date.Year}";

                            list.Add(new object[] { returnDate, data[i].AverageHoursPerDay });
                        }
                    }
                }
                else
                {
                    DateTime date = DateTime.UtcNow;

                    date = new DateTime(date.Year, date.Month, date.Day);
                    string returnDate = $"{date.Day}/{date.Month}/{date.Year}";
                    list.Add(new object[] { returnDate, 0 });
                }
                
            }
            else
            {
                DateTime comparator = DateTime.UtcNow.AddDays(-30);

                comparator = new DateTime(comparator.Year, comparator.Month, comparator.Day);

                for (int i = data.Count - 1; i > 1; i--)
                {
                    DateTime date = data[i].Date;

                    date = new DateTime(date.Year, date.Month, date.Day);

                    if (date < comparator)
                    {
                        context.Remove(data[i]);
                        context.SaveChanges();
                    }

                }

                data = context.Statistics
                    .Where(stats => stats.UserId == id)
                    .ToList();

                if (data.Count != 0)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        DateTime date = data[i].Date;

                        date = new DateTime(date.Year, date.Month, date.Day);
                        comparator = DateTime.UtcNow.AddDays(-days);
                        comparator = new DateTime(comparator.Year, comparator.Month, comparator.Day);

                        if (date >= comparator)
                        {
                            string returnDate = $"{date.Day}/{date.Month}/{date.Year}";

                            list.Add(new object[] { returnDate, data[i].AverageHoursPerDay });
                        }
                    }
                }
                else
                {
                    DateTime date = DateTime.UtcNow;

                    date = new DateTime(date.Year, date.Month, date.Day);
                    string returnDate = $"{date.Day}/{date.Month}/{date.Year}";
                    list.Add(new object[] { returnDate, 0 });
                }
            }

            return list;
        }

        public List<object> GetCards(string id, int days)
        {
            var list = new List<object>();

            list.Add(new[] { "Date", "Cards" });

            List<UserStats> data = context.Statistics
                .Where(stats => stats.UserId == id)
                .ToList();

            if (days > data.Count)
            {
                DateTime comparator = DateTime.UtcNow.AddDays(-30);

                comparator = new DateTime(comparator.Year, comparator.Month, comparator.Day);

                for (int i = data.Count - 1; i > 1; i--)
                {
                    DateTime date = data[i].Date;

                    date = new DateTime(date.Year, date.Month, date.Day);

                    if (date < comparator)
                    {
                        context.Remove(data[i]);
                        context.SaveChanges();
                    }

                }

                data = context.Statistics
                    .Where(stats => stats.UserId == id)
                    .ToList();

                if (data.Count != 0)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        DateTime date = data[i].Date;

                        date = new DateTime(date.Year, date.Month, date.Day);
                        comparator = DateTime.UtcNow.AddDays(-days);
                        comparator = new DateTime(comparator.Year, comparator.Month, comparator.Day);

                        if (date >= comparator)
                        {
                            string returnDate = $"{date.Day}/{date.Month}/{date.Year}";

                            list.Add(new object[] { returnDate, data[i].CardsPerDay });
                        }
                    }
                }
                else
                {
                    DateTime date = DateTime.UtcNow;

                    date = new DateTime(date.Year, date.Month, date.Day);
                    string returnDate = $"{date.Day}/{date.Month}/{date.Year}";
                    list.Add(new object[] { returnDate, 0 });
                }
            }
            else
            {
                DateTime comparator = DateTime.UtcNow.AddDays(-30);

                comparator = new DateTime(comparator.Year, comparator.Month, comparator.Day);

                for (int i = data.Count - 1; i > 1; i--)
                {
                    DateTime date = data[i].Date;

                    date = new DateTime(date.Year, date.Month, date.Day);

                    if (date < comparator)
                    {
                        context.Remove(data[i]);
                        context.SaveChanges();
                    }

                }

                data = context.Statistics
                    .Where(stats => stats.UserId == id)
                    .ToList();

                if (data.Count != 0)
                {
                    for (int i = 0; i < data.Count; i++)
                    {
                        DateTime date = data[i].Date;

                        date = new DateTime(date.Year, date.Month, date.Day);
                        comparator = DateTime.UtcNow.AddDays(-days);
                        comparator = new DateTime(comparator.Year, comparator.Month, comparator.Day);

                        if (date >= comparator)
                        {
                            string returnDate = $"{date.Day}/{date.Month}/{date.Year}";

                            list.Add(new object[] { returnDate, data[i].CardsPerDay });
                        }
                    }
                }
                else
                {
                    DateTime date = DateTime.UtcNow;

                    date = new DateTime(date.Year, date.Month, date.Day);
                    string returnDate = $"{date.Day}/{date.Month}/{date.Year}";
                    list.Add(new object[] { returnDate, 0 });
                }

            }
            return list;
        }

        public List<object> GetTodayStats(string id)
        {
            var list = new List<object>();

            list.Add(new[] { "hours", "averageHours", "cards" });

            List<UserStats> data = context.Statistics
                .Where(stats => stats.UserId == id)
                .ToList();

            var stats = context.Settings
                .Where(user => user.UserId == id)
                .FirstOrDefault();

            var todayStats = data[^1];

            int todayHours;

            if (todayStats.HoursPerDay >= stats.HoursPerDay)
            {
                todayHours = 100;
            }
            else
            {
                int answer = Convert.ToInt32((todayStats.HoursPerDay / stats.HoursPerDay) * 100);
                todayHours = answer;
            }

            float todayAverage = todayStats.AverageHoursPerDay;

            int todayCards;

            if (todayStats.CardsPerDay >= stats.CardsPerDay)
            {
               todayCards = 100;
            }
            else
            {
                var test = Convert.ToDouble(todayStats.CardsPerDay) / Convert.ToDouble(stats.CardsPerDay);

                int answer = Convert.ToInt32(test * 100);
                todayCards = answer;
            }

            list.Add(new object[] { todayHours, todayAverage, todayCards });

            return list;
        }
    }
}
