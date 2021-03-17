using Microsoft.EntityFrameworkCore;

namespace Memento.Models
{
    public class MementoDbContext : DbContext
    {
        public MementoDbContext(DbContextOptions<MementoDbContext> options)
            : base(options) { }

        public DbSet<Card> Cards { get; set; }

        public DbSet<Deck> Decks { get; set; }

        public DbSet<Tag> Tags { get; set; }

        public DbSet<User> Users { get; set; }

        protected override void OnModelCreating(ModelBuilder modelBuilder)
        {
            base.OnModelCreating(modelBuilder);

            modelBuilder
                .Entity<Deck>()
                .HasMany(deck => deck.Users)
                .WithMany(user => user.Decks);

            modelBuilder
                .Entity<Deck>()
                .HasOne(deck => deck.Creator)
                .WithMany(user => user.CreatedDecks);

            modelBuilder
                .Entity<User>()
                .HasIndex(user => user.Email)
                .IsUnique();
        }
    }
}
