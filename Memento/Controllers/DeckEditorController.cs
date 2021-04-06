using System.Linq;
using System.Threading.Tasks;

using Memento.Models;
using Memento.Models.ViewModels;
using Memento.Models.ViewModels.DeckEditor;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Memento.Controllers
{
    public class DeckEditorController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly MementoDbContext _context;

        public DeckEditorController(UserManager<User> userManager, MementoDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChooseDeck()
        {
            var user = await _userManager.GetUserAsync(User);
            var decks = await _context.Decks.Where(deck => deck.CreatorId == user.Id).ToListAsync();

            return View(new ChooseDeckModel
            {
                UserName = user.UserName,
                Decks = decks
                    .Select(deck => new DeckModel { Id = deck.Id, Name = deck.Name })
                    .ToList()
            });
        }

        [HttpPost]
        [Authorize]
        public IActionResult ChooseDeck(DeckModel deck)
            => RedirectToAction(nameof(ChooseCard), new { deckId = deck.Id });

        [HttpGet]
        [Authorize]
        public IActionResult CreateDeck()
            => View();

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateDeck(CreateDeckModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var deck = new Deck { Name = model.Name, Creator = user };

                await _context.AddAsync(deck);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(EditCard), new { deckId = deck.Id });
            }

            return View();
        }

        [Authorize]
        [HttpGet("[controller]/[action]/{deckId}")]
        public async Task<IActionResult> ChooseCard([FromRoute] long deckId)
        {
            var deck = await _context.Decks
                .Where(deck => deck.Id == deckId)
                .Include(deck => deck.Cards)
                .FirstOrDefaultAsync();

            var cards = deck.Cards.Select(card => new CardModel
            {
                Id = card.Id,
                Deck = new DeckModel { Name = deck.Name, Id = deck.Id },
                Word = card.Word,
                Transcription = card.Transcription,
                Description = card.Description,
                Difficulty = card.Difficulty,
                Image = card.Image,
            }).ToList();

            return View(new ChooseCardModel { Cards = cards, Deck = new DeckModel { Id = deck.Id, Name = deck.Name } });
        }

        [Authorize]
        [HttpPost("[controller]/[action]/{deckId}")]
        public IActionResult ChooseCard([FromRoute] long deckId, [FromForm] CardModel model)
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

            return View(nameof(EditCard), new DeckEditorModel
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
            
            return View(new DeckEditorModel
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

        [HttpGet]
        public async Task<FileResult> GetImage(long deckId)
        {
            var deck = (Deck)await _context.FindAsync(typeof(Deck), deckId);
            return File(deck.Cover, "image/jpeg");
        }

        [HttpGet]
        public void AddImage()
        {

        }
    }
}
