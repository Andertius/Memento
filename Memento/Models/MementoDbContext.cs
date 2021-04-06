﻿using System;

using Microsoft.AspNetCore.Identity;
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
        }
    }
}
