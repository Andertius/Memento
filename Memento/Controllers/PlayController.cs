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
        private readonly PlayDeckModel _d;
        private static int asd = 0;

        private static PlayDeckModel _sd = new PlayDeckModel
        {
            Id = deck.Id,
            Name = deck.Name,
            Cards = deck.Cards.Select(card => new PlayCardModel
            {
                Id = card.Id,
                //Deck = new PlayDeckModel { Name = deck.Name, Id = deck.Id },
                Word = card.Word,
                Transcription = card.Transcription,
                Description = card.Description,
                Difficulty = card.Difficulty,
            }).ToList()
        };

        public PlayController(UserManager<User> userManager, MementoDbContext context)
        {
            _userManager = userManager;
            _context = context;
            var deck = _context.Decks
            .Include(deck => deck.Cards)
            .FirstOrDefault();

        }

        public static void init()
        {

        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Play()
        {
            User user = await _userManager.GetUserAsync(User);
            //var deck = _context.Decks.First();
            //var deck = await _context.Decks
            //   .Include(deck => deck.Cards)
            //   .FirstOrDefaultAsync();
            //PlayDeckModel deckModel = new PlayDeckModel
            //{
            //    Id = deck.Id,
            //    Name = deck.Name,
            //    Cards = deck.Cards.Select(card => new PlayCardModel
            //    {
            //        Id = card.Id,
            //        //Deck = new PlayDeckModel { Name = deck.Name, Id = deck.Id },
            //        Word = card.Word,
            //        Transcription = card.Transcription,
            //        Description = card.Description,
            //        Difficulty = card.Difficulty,
            //    }).ToList()
            //};


            return View(new PlayModel
            {
                Username = user.UserName,
                CurrentCard = _d.Cards.First(),
                PickedDeck = _d
            });
        }

        [Authorize]
        [HttpGet]
        //[HttpGet("[controller]/[action]/{cardId}")]
        // public async Task<IActionResult> NextCard([FromRoute] long cardId)
        public async Task<IActionResult> NextCard()
        {
            asd++;
            return View(new PlayModel
            {
                CurrentCard = _d.Cards.ElementAt(asd),
                PickedDeck = _d
            });
        }

    }
}

