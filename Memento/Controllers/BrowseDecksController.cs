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
    public class BrowseDecksController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly MementoDbContext _context;

        public BrowseDecksController(UserManager<User> userManager, MementoDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        public async Task<IActionResult> SearchedDecks(BrowseDecksModel model)
        {
            List<DeckModel> decks;

            if (model.SearchValue is not null)
            {
                decks = await _context.Decks
                    .Where(deck => deck.Name.ToLower().Contains(model.SearchValue.ToLower()))
                    .Select(deck => new DeckModel
                    {
                        Id = deck.Id,
                        Name = deck.Name,
                    })
                    .ToListAsync();
            }
            else
            {
                decks = await _context.Decks
                    .Select(deck => new DeckModel
                    {
                        Id = deck.Id,
                        Name = deck.Name,
                    })
                    .ToListAsync();
            }

            return View(new SearchedDecksModel { Decks = decks, SearchValue = model.SearchValue });
        }

        //[HttpGet]
        //public async Task<IActionResult> SearchedDecks(SearchedDecksModel model)
        //{
        //    var decks = await _context.Decks
        //        .Where(deck => deck.Name.ToLower().Contains(model.SearchValue.ToLower()))
        //        .Select(deck => new DeckModel
        //        {
        //            Id = deck.Id,
        //            Name = deck.Name,
        //        })
        //        .ToListAsync();

        //    return View(new SearchedDecksModel { Decks = decks, SearchValue = model.SearchValue });
        //}

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

                if (deck is not null)
                {
                    userWithDecks.Decks.Add(deck);
                    await _userManager.UpdateAsync(userWithDecks);
                }
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
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

                if (deck is not null)
                {
                    userWithDecks.Decks.Remove(deck);
                    await _userManager.UpdateAsync(userWithDecks);
                }
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [Authorize]
        [HttpGet("[controller]/[action]/{deckId}")]
        public async Task<IActionResult> DeleteDeckFromDatabase([FromRoute] long deckId)
        {
            var user = await _userManager.GetUserAsync(User);

            if (user is not null)
            {
                var deck = await _context.Decks.FindAsync(deckId);

                if (deck is not null && deck.CreatorId == user.Id)
                {
                    _context.Decks.Remove(deck);
                    await _context.SaveChangesAsync();
                }
            }

            return RedirectToAction(nameof(HomeController.Index), "Home");
        }

        [HttpGet("[controller]/[action]/{deckId}")]
        public async Task<IActionResult> DeckPage([FromRoute] long deckId)
        {
            var deck = await _context.Decks
                .Select(deck => new DeckDto
                {
                    Id = deck.Id,
                    Name = deck.Name,
                    CreatorId = deck.CreatorId,
                    Difficulty = deck.Difficulty,
                    CardNumber = deck.CardNumber,
                    IsPublic = deck.IsPublic,
                    Ratings = deck.Ratings.Select(rating => rating).ToList(),
                    Tags = deck.Tags.Select(tag => tag).ToList(),
                })
                .Where(deck => deck.Id == deckId)
                .FirstOrDefaultAsync();

            if (deck is null)
            {
                return RedirectToAction(nameof(HomeController.Index), "Home");
            }

            bool hasInCollection = false;
            int userRating = 0;

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

                var rating = deck.Ratings.Where(r => r.UserId == userWithDecks.Id).FirstOrDefault();
                userRating = rating is null ? 0 : rating.Rating;

                hasInCollection = userWithDecks.Decks.Contains(await _context.Decks.FindAsync(deckId));
            }

            var creator = await _userManager.FindByIdAsync(deck.CreatorId);

            return View(new DeckModel
            {
                Id = deck.Id,
                Name = deck.Name,
                Difficulty = deck.Difficulty,
                CardNumber = deck.CardNumber,
                CreatorName = creator.UserName,
                HasInCollection = hasInCollection,
                AverageRating = deck.Ratings.Any() ? deck.Ratings.Average(r => r.Rating) : 0,
                UserRating = userRating,
                RatingNumber = deck.Ratings.Count,
                Tags = deck.Tags.Select(tag => tag.Name).ToList(),
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
                model.CreatedDecks = await decks
                    .Where(deck => deck.Name.ToLower().Contains(model.SearchFilter.ToLower()))
                    .Select(deck => new DeckModel { Name = deck.Name, Id = deck.Id })
                    .ToListAsync();
            }
            else
            {
                model.CreatedDecks = await decks
                    .Select(deck => new DeckModel { Name = deck.Name, Id = deck.Id })
                    .ToListAsync();
            }

            return View(model);
        }

        [HttpGet]
        public async Task<IActionResult> PopularDecks(PopularDecksModel model = null)
        {
            var decks = await _context.Decks
                .Select(deck => deck)
                .Include(deck => deck.Tags)
                .ToListAsync();

            if (model.SearchFilter is not null)
            {
                model.PopularDecks = decks
                    .Where(deck => deck.Name.ToLower().Contains(model.SearchFilter.ToLower()))
                    .Select(deck => new DeckModel
                    {
                        Id = deck.Id,
                        Name = deck.Name,
                        Difficulty = deck.Difficulty,
                    })
                    .ToList();
            }
            else
            {
                model.PopularDecks = decks
                    .Select(deck => new DeckModel
                    {
                        Id = deck.Id,
                        Name = deck.Name,
                        Difficulty = deck.Difficulty,
                    })
                    .ToList();
            }

            if (model.FilterTags is not null && model.FilterTags.Any())
            {
                model.PopularDecks = decks
                    .Where(deck => !model.FilterTags
                        .Except(deck.Tags is null ? new List<string>() : deck.Tags.Select(tag => tag.Name))
                        .Any())
                    .Select(deck => new DeckModel
                    {
                        Id = deck.Id,
                        Name = deck.Name,
                        Difficulty = deck.Difficulty,
                    })
                    .ToList();
            }

            return View(model);
        }

        [HttpGet]
        [Authorize]
        public IActionResult AddTagFilter(List<string> tagsList)
        {
            //TagsList.Add(newTag);
            var model = new PopularDecksModel { FilterTags = tagsList };
            return RedirectToAction(nameof(PopularDecks), new { model });
        }

        [Authorize]
        [HttpGet("[controller]/[action]/{deckId}/{rating}")]
        public async Task<IActionResult> RateDeck(long deckId, int rating)
        {
            var user = await _context.Users
                .Where(user => user.UserName == User.Identity.Name)
                .Include(user => user.Ratings)
                .FirstOrDefaultAsync();

            var deck = await _context.Decks
                .Where(deck => deck.Id == deckId)
                .Include(deck => deck.Ratings)
                .FirstOrDefaultAsync();

            var currentRating = deck.Ratings
                .Where(rating => rating.UserId == user.Id)
                .FirstOrDefault();

            if (currentRating is null)
            {
                _context.Ratings.Add(new UserRating { UserId = user.Id, DeckId = deckId, Rating = rating });
            }
            else if (currentRating.Rating == rating)
            {
                _context.Ratings.Remove(currentRating);
            }
            else
            {
                currentRating.Rating = rating;
                _context.Ratings.Update(currentRating);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(DeckPage), new { deckId });
        }

        [Authorize]
        [HttpGet("[controller]/[action]/{deckId}")]
        public async Task<FileResult> GetThumb(long deckId)
        {
            var deck = (Deck)await _context.FindAsync(typeof(Deck), deckId);
            return File(deck.Thumbnail, "image/*");
        }
    }
}
