using System.Linq;
using System.Threading.Tasks;

using Memento.Models;
using Memento.Models.ViewModels.DeckEditor;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Memento.Controllers
{
    public class CardEditorController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly MementoDbContext _context;

        public CardEditorController(UserManager<User> userManager, MementoDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize]
        [HttpPost("[controller]/[action]/{deckId}")]
        public IActionResult EditCard([FromRoute] long deckId, [FromForm] CardModel model)
            => RedirectToAction(nameof(EditCard), new { deckId, cardId = model.Id });

        [Authorize]
        [HttpGet("[controller]/[action]/{deckId}")]
        public async Task<IActionResult> NewCard(long deckId)
        {
            var user = await _userManager.GetUserAsync(User);
            var deck = await _context.Decks.Where(deck => deck.Id == deckId)
                .Include(deck => deck.Cards)
                .FirstOrDefaultAsync();

            var card = new Card { Word = "test", Description = "test1" };
            deck.Cards.Add(card);
            _context.SaveChanges();

            return View(nameof(EditCard), new CardEditorModel
            {
                UserName = user.UserName,
                Deck = new DeckModel { Name = deck.Name, Id = deck.Id },
                Card = new CardModel(),
            });
        }

        [Authorize]
        [HttpGet("[controller]/[action]/{deckId}/{cardId}")]
        public async Task<IActionResult> EditCard(long deckId, long cardId)
        {
            var user = await _userManager.GetUserAsync(User);
            var deck = await _context.Decks.FindAsync(deckId);
            var card = await _context.Cards.FindAsync(cardId);

            if (card is null)
            {
                return View(new CardEditorModel
                {
                    UserName = user.UserName,
                    Deck = new DeckModel { Name = deck.Name, Id = deck.Id },
                    Card = new CardModel(),
                });
            }
            else
            {
                return View(new CardEditorModel
                {
                    UserName = user.UserName,
                    Deck = new DeckModel { Name = deck.Name, Id = deck.Id },
                    Card = new CardModel
                    {
                        Id = card.Id,
                        Deck = new DeckModel { Name = deck.Name, Id = deck.Id },
                        Word = card.Word,
                        Transcription = card.Transcription,
                        Description = card.Description,
                        Difficulty = card.Difficulty,
                        Image = card.Image,
                    },
                });
            }
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DeleteCard(long deckId, long cardId)
        {
            var deck = await _context.Decks
                .Where(deck => deck.Id == deckId)
                .Include(deck => deck.Cards)
                .FirstOrDefaultAsync();

            var card = deck.Cards
                .Where(card => card.Id == cardId)
                .FirstOrDefault();

            deck.Cards.Remove(card);
            _context.SaveChanges();

            return RedirectToAction(nameof(DeckEditorController.EditDeck), nameof(DeckEditorController), new { deckId });
        }
    }
}
