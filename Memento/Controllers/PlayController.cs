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
            var deck = _context.Decks.First();
            var c = deck.Cards;
            var card = _context.Decks.First().Cards.First();
            DeckModel a = new DeckModel();
            a.Id = deck.Id;
            a.Name = deck.Name;
            CardModel b = new CardModel();
            b.Word = card.Word;
            //return View(new PlayModel());

            return View(new PlayModel
            {
                PickedDeck = a,
                CurrentCard = b
            });
        }
    }
}

