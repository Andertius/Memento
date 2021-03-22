﻿using System.Linq;

namespace Memento.Models
{
    public class EFMementoRepository : IMementoRepository
    {
        private readonly MementoDbContext context;

        public EFMementoRepository(MementoDbContext ctx)
        {
            context = ctx;
        }

        public IQueryable<Card> Cards => context.Cards;

        public IQueryable<Deck> Decks => context.Decks;

        public IQueryable<Tag> Tags => context.Tags;

        public IQueryable<User> Users => context.Users;
    }
}