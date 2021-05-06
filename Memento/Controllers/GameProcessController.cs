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
    public class GameProcessController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly MementoDbContext _context;
        private static ProcessDeckModel _sd;

        public GameProcessController(UserManager<User> userManager, MementoDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }


        [Authorize]
        [HttpGet("[controller]/[action]/{deckId}")]
        public async Task<IActionResult> Start(long deckId)
        {
            User user = await _userManager.GetUserAsync(User);

            var deck = await _context.Decks
                .Include(d => d.Cards)
                .Where(d=>d.Id==deckId)
                .FirstOrDefaultAsync();

            _sd = new ProcessDeckModel
            {
                Id = deck.Id,
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

            return View(new ProcessModel
            {
                Username = user.UserName,
                CurrentCard = _sd.Cards.First(),
                DeckId=deckId,
                ShowImages=settings.ShowImages
            });
        }

        [Authorize]
        [HttpGet("[controller]/[action]/{deckId}/{remember}")]
        public async Task<IActionResult> NextCard(long deckId, int remember)
        {
            User user = await _userManager.GetUserAsync(User);

            if (remember != 0)
            {
                List<PlayCardModel> cards = new List<PlayCardModel>(_sd.Cards.ToList());
                int new_position = 0;
                int cardsCount = _sd.Cards.Count;
                Random rnd = new Random();
                switch (remember)
                {
                    case 1:
                        new_position = rnd.Next(cardsCount / 2, cardsCount - 1);
                        break;
                    case 2:
                        new_position = rnd.Next(cardsCount / 3, 2 * cardsCount / 3);
                        break;
                }
                cards.Insert(new_position, _sd.Cards.FirstOrDefault());
                _sd.Cards = cards;
            }

            if (_sd.Cards != null)
                _sd.Cards.Remove(_sd.Cards.FirstOrDefault());

            Settings settings = _context.Settings.Find(user.Id);
            if (settings is null)
            {
                settings = new Settings();
            }

            return View(new ProcessModel
            {
                DeckId = deckId,
                CurrentCard=_sd.Cards.FirstOrDefault(),
                Username = user.UserName,
                ShowImages = settings.ShowImages
            });
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
