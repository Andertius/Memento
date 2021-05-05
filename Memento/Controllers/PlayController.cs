using System;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Memento.Models;
using Memento.Models.ViewModels;
using Memento.Models.ViewModels.Play;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Memento.Controllers
{
    public class PlayController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly MementoDbContext _context;
        private readonly DeckModel _d;

        public PlayController(UserManager<User> userManager, MementoDbContext context)
        {
            _userManager = userManager;
            _context = context;
            _d = new DeckModel();
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> Play()
        {
            User user = await _userManager.GetUserAsync(User);
            //var deck = _context.Decks.First();
            var deck = await _context.Decks
               .Include(deck => deck.Cards)
               .FirstOrDefaultAsync();

            _d.Id = deck.Id;
            _d.Name = deck.Name;
            _d.Cards = deck.Cards.Select(card => new CardModel
            {
                Id = card.Id,
                Deck = new DeckModel { Name = deck.Name, Id = deck.Id },
                Word = card.Word,
                Transcription = card.Transcription,
                Description = card.Description,
                Difficulty = card.Difficulty,
                Image = card.Image,
            }).ToList();

            return View(new PlayModel
            {
                Username = user.UserName,
                CurrentCard = _d.Cards.First(),
            });
        }

        [Authorize]
        [HttpGet]
        public async Task<IActionResult> NextCard()
        {
            _d.Cards.ToList().RemoveAt(0);
            return View(new PlayModel
            {
                Username = (await _userManager.GetUserAsync(User)).UserName,
                CurrentCard = _d.Cards.First()
            });
        }

    }
}

