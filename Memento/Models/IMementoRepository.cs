using System.Linq;

namespace Memento.Models
{
    public interface IMementoRepository
    {
        IQueryable<Card> Cards { get; }
        IQueryable<Deck> Decks { get; }
        IQueryable<Image> Images { get; }
        IQueryable<Tag> Tags { get; }
        IQueryable<User> Users { get; }
    }
}
