using System.Collections.Generic;
using System.Linq;

namespace Memento.Models
{
    public class Rating
    {
        public List<int> Stars { get; set; }

        public double Average => Stars.Sum() / (double)Stars.Count;
    }
}
