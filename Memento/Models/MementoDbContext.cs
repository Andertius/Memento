using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace Memento.Models
{
    public class MementoDbContext : IdentityDbContext<User>
    {
        public MementoDbContext(DbContextOptions<MementoDbContext> options)
            : base(options) { }

        public DbSet<Card> Cards { get; set; }

        public DbSet<Deck> Decks { get; set; }

        public DbSet<CardTag> CardTags { get; set; }

        public DbSet<DeckTag> DeckTags { get; set; }

        public DbSet<Settings> Settings { get; set; }

        public DbSet<UserStats> Statistics { get; set; }

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
                .WithMany(user => user.CreatedDecks)
                .OnDelete(DeleteBehavior.NoAction);

            modelBuilder
                .Entity<User>()
                .HasIndex(user => user.Email)
                .IsUnique();

            modelBuilder
                .Entity<UserStats>()
                .HasOne(stat => stat.User)
                .WithMany(user => user.Statistics)
                .OnDelete(DeleteBehavior.Cascade);
        }
    }
}
