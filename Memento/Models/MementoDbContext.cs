using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Microsoft.EntityFrameworkCore;

namespace Memento.Models
{
    public class MementoDbContext : DbContext
    {
        public MementoDbContext(DbContextOptions<MementoDbContext> options)
            : base(options) { }

        public DbSet<Card> Cards { get; set; }

        public DbSet<Deck> Decks { get; set; }

        public DbSet<Image> Images { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<User> Users { get; set; }
    }
}
