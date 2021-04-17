using System;
using System.Collections.Generic;
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
                    .Select(deck => new DeckModel
                    {
                        Id = deck.Id,
                        Name = deck.Name,
                        Rating = deck.Rating,
                        Difficulty = deck.Difficulty,
                    })
                    .ToList(),
                CreatedDecks = new List<DeckModel>(),
                YourDecks = new List<DeckModel>(),
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

        [HttpGet("[controller]/[action]/{deckId}")]
        public async Task<IActionResult> DeckPage([FromRoute] long deckId)
        {
            var deck = await _context.Decks.FindAsync(deckId);
            bool hasInCollection = false;

            //if (!deck.IsPublic)
            //{
            //    return RedirectToAction(nameof(Index));
            //}

            if (User.Identity.IsAuthenticated)
            {
                var userWithDecks = await _context.Users
                   .Where(u => u.UserName == User.Identity.Name)
                   .Include(u => u.Decks)
                   .FirstOrDefaultAsync();

                hasInCollection = userWithDecks.Decks.Contains(deck);
            }

            var creator = await _userManager.FindByIdAsync(deck.CreatorId);

            return View(new DeckModel
            {
                Id = deck.Id,
                Name = deck.Name,
                Rating = deck.Rating,
                Difficulty = deck.Difficulty,
                CardNumber = deck.CardNumber,
                CreatorName = creator.UserName,
                HasInCollection = hasInCollection,
            });
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> YourCollection(YourCollectionModel model = null)
        {
            var userWithDecks = await _context.Users
                   .Where(u => u.UserName == User.Identity.Name)
                   .Include(u => u.Decks)
                   .FirstOrDefaultAsync();

            if (model.SearchFilter is not null)
            {
                model.YourDecks = userWithDecks.Decks
                    .Where(deck => deck.Name.ToLower().Contains(model.SearchFilter.ToLower()))
                    .Select(deck => new DeckModel { Name = deck.Name, Id = deck.Id })
                    .ToList();
            }
            else
            {
                model.YourDecks = userWithDecks.Decks
                    .Select(deck => new DeckModel { Name = deck.Name, Id = deck.Id })
                    .ToList();
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CreatedDecks(CreatedDecksModel model = null)
        {
            var user = await _userManager.GetUserAsync(User);
            var decks = _context.Decks.Where(deck => deck.CreatorId == user.Id);

            if (model.SearchFilter is not null)
            {
                model.CreatedDecks = decks
                    .Where(deck => deck.Name.ToLower().Contains(model.SearchFilter.ToLower()))
                    .Select(deck => new DeckModel { Name = deck.Name, Id = deck.Id })
                    .ToList();
            }
            else
            {
                model.CreatedDecks = decks
                    .Select(deck => new DeckModel { Name = deck.Name, Id = deck.Id })
                    .ToList();
            }

            return View(model);
        }

        [HttpGet]
        public IActionResult PopularDecks(PopularDecksModel model = null)
        {
            var decks = _context.Decks.Select(deck => deck);

            if (model.SearchFilter is not null)
            {
                model.PopularDecks = decks
                    .Where(deck => deck.Name.ToLower().Contains(model.SearchFilter.ToLower()))
                    .OrderBy(deck => deck.Rating)
                    .Select(deck => new DeckModel
                    {
                        Id = deck.Id,
                        Name = deck.Name,
                        Rating = deck.Rating,
                        Difficulty = deck.Difficulty,
                    })
                    .ToList();
            }
            else
            {
                model.PopularDecks = decks
                    .OrderBy(deck => deck.Rating)
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
        public async Task<FileResult> GetThumb(long deckId)
        {
            var deck = (Deck)await _context.FindAsync(typeof(Deck), deckId);
            return File(deck.Thumbnail, "image/*");
        }

        [Route(nameof(About))]
        public IActionResult About()
            => View();
    }
}
