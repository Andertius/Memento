using System.Linq;
using System.Threading.Tasks;

using Memento.Models;
using Memento.Models.ViewModels.BrowseDecks;

using Microsoft.AspNetCore.Authorization;
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
            var userWithDecks = await _context.Users
                .Where(u => u.UserName == User.Identity.Name)
                .Include(u => u.Decks)
                .FirstOrDefaultAsync();

            var model = new BrowseDecksModel
            {
                PopularDecks = _context.Decks
                    .OrderBy(deck => deck.Rating)
                    .Take(10)
                    .Select(deck => new DeckModel
                    {
                        Id = deck.Id,
                        Name = deck.Name,
                        Rating = deck.Rating,
                        Difficulty = deck.Difficulty,
                    })
                    .ToList(),
            };

            if (userWithDecks is not null)
            {
                model.CreatedDecks = _context.Decks
                    .Where(deck => deck.CreatorId == userWithDecks.Id)
                    .Select(deck => new DeckModel
                    {
                        Id = deck.Id,
                        Name = deck.Name,
                        Rating = deck.Rating,
                        Difficulty = deck.Difficulty,
                    })
                    .ToList();

                model.YourDecks = userWithDecks.Decks
                    .Select(deck => new DeckModel
                    {
                        Id = deck.Id,
                        Name = deck.Name,
                        Rating = deck.Rating,
                        Difficulty = deck.Difficulty,
                    })
                    .ToList();
            }

            return View(model);
        }

        [Authorize]
        [HttpGet("[controller]/[action]/{deckId}")]
        public async Task<IActionResult> AddDeck([FromRoute] long deckId)
        {
            var userWithDecks = await _context.Users
                .Where(u => u.UserName == User.Identity.Name)
                .Include(u => u.Decks)
                .FirstOrDefaultAsync();

            if (userWithDecks is not null)
            {
                var deck = await _context.Decks.FindAsync(deckId);
                userWithDecks.Decks.Add(deck);
                await _userManager.UpdateAsync(userWithDecks);
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpGet("[controller]/[action]/{deckId}")]
        public async Task<IActionResult> RemoveDeckFromCollection([FromRoute] long deckId)
        {
            var userWithDecks = await _context.Users
                .Where(u => u.UserName == User.Identity.Name)
                .Include(u => u.Decks)
                .FirstOrDefaultAsync();

            if (userWithDecks is not null)
            {
                var deck = await _context.Decks.FindAsync(deckId);
                userWithDecks.Decks.Remove(deck);
                await _userManager.UpdateAsync(userWithDecks);
            }

            return RedirectToAction(nameof(Index));
        }

        [Authorize]
        [HttpGet("[controller]/[action]/{deckId}")]
        public async Task<IActionResult> DeleteDeckFromDatabase([FromRoute] long deckId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is not null)
            {
                var deck = await _context.Decks.FindAsync(deckId);
                _context.Decks.Remove(deck);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(Index));
        }

        [Route(nameof(About))]
        public IActionResult About()
            => View();
    }
}
