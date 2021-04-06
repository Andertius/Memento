using System.Linq;

namespace Memento.Models
{
    public interface IMementoRepository
    {
        IQueryable<Card> Cards { get; }
        IQueryable<Deck> Decks { get; }
        IQueryable<CardTag> CardTags { get; }
        IQueryable<DeckTag> DeckTags { get; }
        IQueryable<User> Users { get; }
        IQueryable<Settings> Settings { get; }
    }
}
