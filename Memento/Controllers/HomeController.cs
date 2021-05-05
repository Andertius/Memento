using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

using Memento.Models;
using Memento.Models.ViewModels;
using Memento.Models.ViewModels.BrowseDecks;

using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Memento.Controllers
{
    public class HomeController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly MementoDbContext _context;

        public HomeController(UserManager<User> userManager, MementoDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            var user = await _userManager.GetUserAsync(User);

            var model = new BrowseDecksModel
            { 
                Username = (user is null) ? "" : user.UserName,
                PopularDecks = _context.Decks
                    .Where(deck => deck.IsPublic)
                    .Select(deck => new DeckModel
                    {
                        Id = deck.Id,
                        Name = deck.Name,
                        Difficulty = deck.Difficulty,
                    })
                    .ToList(),
                CreatedDecks = new List<DeckModel>(),
                YourDecks = new List<DeckModel>(),
            };

            if (User.Identity.IsAuthenticated)
            {
                var userWithDecks = await _context.Users
                    .Where(u => u.UserName == user.UserName)
                    .Include(u => u.Decks)
                    .FirstOrDefaultAsync();

                if (userWithDecks is not null)
                {
                    model.CreatedDecks = _context.Decks
                        .Where(deck => deck.CreatorId == userWithDecks.Id)
                        .Select(deck => new DeckModel
                        {
                            Id = deck.Id,
                            Name = deck.Name,
                            Difficulty = deck.Difficulty,
                        })
                        .ToList();

                    model.YourDecks = userWithDecks.Decks
                        .Select(deck => new DeckModel
                        {
                            Id = deck.Id,
                            Name = deck.Name,
                            Difficulty = deck.Difficulty,
                        })
                        .ToList();
                }
            }

            return View(model);
        }

        [HttpGet("[controller]/[action]/{deckId}")]
        public async Task<FileResult> GetThumb(long deckId)
        {
            var deck = (Deck)await _context.FindAsync(typeof(Deck), deckId);
            return File(deck.Thumbnail, "image/*");
        }

        [Route(nameof(About))]
        public async Task<IActionResult> About()
        {
            var user = await _userManager.GetUserAsync(User);
            return View(new AboutModel { Username = user is null ? "" : user.UserName });
        }
    }
}
