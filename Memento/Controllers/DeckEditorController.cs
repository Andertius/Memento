using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Memento.Models;
using Memento.Models.ViewModels;
using Memento.Models.ViewModels.DeckEditor;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
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
            => RedirectToAction(nameof(EditDeck), new { deckId = deck.Id });

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
                return RedirectToAction(nameof(EditDeck), new { deckId = deck.Id });
            }

            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DeleteDeck(long deckId)
        {
            var deck = await _context.Decks.FindAsync(deckId);
            _context.Remove(deck);
            _context.SaveChanges();

            return RedirectToAction(nameof(ChooseDeck));
        }

        [Authorize]
        [HttpGet("[controller]/[action]/{deckId}")]
        public async Task<IActionResult> EditDeck([FromRoute] long deckId)
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

            return View(new DeckEditorModel { Cards = cards, Deck = new DeckModel
            {
                Id = deck.Id,
                Name = deck.Name,
                Difficulty = deck.Difficulty,
                IsPublic = deck.IsPublic,
            } });
        }

        [HttpGet("[controller]/[action]/{deckId}")]
        public async Task<FileResult> GetCover(long deckId)
        {
            var deck = (Deck)await _context.FindAsync(typeof(Deck), deckId);
            return File(deck.Cover, "image/jpeg");
        }

        [HttpGet("[controller]/[action]/{deckId}")]
        public async Task<FileResult> GetThumb(long deckId)
        {
            var deck = (Deck)await _context.FindAsync(typeof(Deck), deckId);
            return File(deck.Thumbnail, "image/jpeg");
        }

        [Authorize]
        [HttpPost]
        public async Task<IActionResult> SaveChanges(DeckEditorModel model)
        {
            var deck = await _context.Decks.FindAsync(model.Deck.Id);

            if (deck is not null)
            {
                using (var ms = new MemoryStream())
                {
                    if (model.Deck.Cover is not null)
                    {
                        model.Deck.Cover.CopyTo(ms);
                        deck.Cover = ms.ToArray();
                    }
                    else if (model.Deck.CoverRemoved)
                    {
                        deck.Cover = null;
                    }

                    if (model.Deck.Thumb is not null)
                    {
                        model.Deck.Thumb.CopyTo(ms);
                        deck.Thumbnail = ms.ToArray();
                    }
                    else if (model.Deck.ThumbRemoved)
                    {
                        deck.Thumbnail = null;
                    }
                }

                deck.IsPublic = model.Deck.IsPublic;
                deck.Name = model.Deck.Name;
                deck.Difficulty = model.Deck.Difficulty;
                _context.Decks.Update(deck);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(EditDeck), new { model.Deck.Id });
        }
    }
}
