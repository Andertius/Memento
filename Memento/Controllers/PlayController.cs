using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Memento.Models;
using Memento.Models.ViewModels.Play;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Memento.Controllers
{
    public class PlayController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly MementoDbContext _context;
        private static PlayDeckModel _sd;

        public PlayController(UserManager<User> userManager, MementoDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Play()
        {
            User user = await _userManager.GetUserAsync(User);

            //var userWithDecks = await _context.Users
            //    .Where(u => u.UserName == User.Identity.Name)
            //    .Include(u => u.Decks)
            //    .FirstOrDefaultAsync();
            //var deck = await userWithDecks.Decks.Where(x=>x==)
            var deck = await _context.Decks
                .Include(d => d.Cards)
                .FirstOrDefaultAsync();
            _sd = new PlayDeckModel
            {
                Id = deck.Id,
                Name = deck.Name,
                CreatorName = deck.Creator.UserName,
                Difficulty = Enum.GetName(typeof(Difficulty), deck.Difficulty),
                Cards = deck.Cards.Select(card => new PlayCardModel
                {
                    Id = card.Id,
                    Word = card.Word,
                    Transcription = card.Transcription,
                    Description = card.Description,
                    Difficulty = card.Difficulty,
                }).ToList()
            };

            Settings settings = _context.Settings.Find(user.Id);
            if (settings is null)
            {
                settings = new Settings();
            }

            switch (settings.CardsOrder)
            {
                case 1:
                    _sd.Cards = new List<PlayCardModel>(_sd.Cards.ToList().OrderBy(card => card.Difficulty));
                    break;
                case 2:
                   
                    _sd.Cards = new List<PlayCardModel>(_sd.Cards.ToList().OrderByDescending(card => card.Difficulty));
                    break;
                case 0:
                    _sd.Cards = new List<PlayCardModel>(_sd.Cards.ToList().OrderBy(x => Guid.NewGuid()));
                    break;
            }

            return View(new PlayModel
            {
                Username = user.UserName,
                CurrentCard = _sd.Cards.First(),
                PickedDeck = _sd
            });
        }

        [Authorize]
        [HttpGet("[controller]/[action]/{deckId}/{remember}")]
        public async Task<IActionResult> Process(long deckId, int remember)
        {
            //var deck = await _context.Decks
            //   .Include(d => d.Cards)
            //   .Where(d => d.Id == deckId)
            //   .FirstOrDefaultAsync();
            if (remember != 0)
            {
                List<PlayCardModel> cards = new List<PlayCardModel>(_sd.Cards.ToList());
                int new_position = 0;
                int cardsCount = _sd.Cards.Count;
                Random rnd = new Random();
                switch (remember)
                {
                    case 1:
                        //new_position = _sd.Cards.Count - (_sd.Cards.Count / 3);
                        new_position = rnd.Next(cardsCount / 2, cardsCount - 1);
                        break;
                    case 2:
                        //new_position = _sd.Cards.Count - (2 * _sd.Cards.Count / 3);
                        new_position = rnd.Next(cardsCount / 3, 2 * cardsCount / 3);
                        break;
                }
                cards.Insert(new_position, _sd.Cards.FirstOrDefault());
                _sd.Cards = cards;
            }

            if (_sd.Cards != null)
                _sd.Cards.Remove(_sd.Cards.FirstOrDefault());

            var card = _sd.Cards.FirstOrDefault();

            return View(new ProcessModel
            {
                DeckId = deckId,
                CurrentCard = new PlayCardModel
                {
                    Id = card.Id,
                    Word = card.Word,
                    Transcription = card.Transcription,
                    Description = card.Description,
                    Difficulty = card.Difficulty,
                },
                Username = (await _userManager.GetUserAsync(User)).UserName
            });
        }

        [HttpGet("[controller]/[action]/{deckId}")]
        public async Task<FileResult> GetCover(long deckId)
        {
            var deck = (Deck)await _context.FindAsync(typeof(Deck), deckId);
            return File(deck.Cover, "image/*");
        }

        [HttpGet("[controller]/[action]/{deckId}/{cardId}")]
        public async Task<FileResult> GetImage(long deckId, long cardId)
        {
            var deck = await _context.Decks
                .Where(deck => deck.Id == deckId)
                .Include(deck => deck.Cards)
                .FirstOrDefaultAsync();

            var card = deck.Cards
                .Where(card => card.Id == cardId)
                .FirstOrDefault();

            if (card is not null)
            {
                return File(card.Image, "image/*");
            }
            else
            {
                return File(Array.Empty<byte>(), "image/*");
            }
        }
    }
}

