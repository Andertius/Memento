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
    public class BrowseDecksController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly MementoDbContext _context;

        public BrowseDecksController(UserManager<User> userManager, MementoDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> SearchedDecks(SearchedDecksModel model)
            => View(await CreateSearchedDecks(model));

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveTagFromSearched(SearchedDecksModel model)
        {
            model.FilterTags = model.FilterTagsString
                    .Split("#", StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

            model.FilterTags.Remove(model.TagToRemove);
            model.TagFilter = String.Empty;

            if (model.FilterTags.Any())
            {
                model.FilterTagsString = model.FilterTags.Aggregate((x, y) => "#" + x + "#" + y + "#");

                if (!model.FilterTagsString.EndsWith('#'))
                {
                    model.FilterTagsString += "#";
                }
            }
            else
            {
                model.FilterTagsString = "#";
            }

            return View(nameof(SearchedDecks), await CreateSearchedDecks(model));
        }

        public async Task<SearchedDecksModel> CreateSearchedDecks(SearchedDecksModel model)
        {
            var decks = await _context.Decks
                .Where(deck => deck.IsPublic)
                .Include(deck => deck.Tags)
                .ToListAsync();

            var decksInModel = decks.Select(deck => deck);

            if (model.SearchValue is not null)
            {
                decksInModel = decksInModel
                    .Where(deck => deck.Name
                        .ToLower()
                        .Contains(model.SearchValue
                            .ToLower()));
            }

            if (!String.IsNullOrEmpty(model.TagFilter))
            {
                if (model.FilterTagsString is null)
                {
                    model.FilterTagsString = "#";
                }

                if (!model.FilterTagsString.Contains($"#{model.TagFilter}#"))
                {
                    model.FilterTagsString += $"{model.TagFilter.ToLower().Split(' ').Aggregate((x, y) => x += "_" + y)}#";
                }
            }

            if (!String.IsNullOrEmpty(model.FilterTagsString))
            {
                model.FilterTags = model.FilterTagsString
                    .Split("#", StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                decksInModel = decksInModel
                    .Where(deck => !model.FilterTags
                        .Except(deck.Tags is null ? new List<string>() : deck.Tags.Select(tag => tag.Name))
                        .Any());
            }

            model.Decks = decksInModel
                .Select(deck => new DeckModel
                {
                    Id = deck.Id,
                    Name = deck.Name,
                    Difficulty = deck.Difficulty,
                })
                .ToList();

            model.TagFilter = String.Empty;
            return model;
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

                if (deck is not null && deck.IsPublic)
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

            if (deck is null || !deck.IsPublic)
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
        public async Task<IActionResult> YourCollection()
        {
            var userWithDecks = await _context.Users
                .Where(u => u.UserName == User.Identity.Name)
                .Include(u => u.Decks)
                .FirstOrDefaultAsync();

            var model = new YourCollectionModel
            {
                YourDecks = userWithDecks.Decks
                    .Select(deck => new DeckModel { Name = deck.Name, Id = deck.Id })
                    .ToList(),
                FilterTagsString = String.Empty,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> YourCollection(YourCollectionModel model)
            => View(await CreateYourCollectionModel(model));

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveTagFromYourCollection(YourCollectionModel model)
        {
            model.FilterTags = model.FilterTagsString
                .Split("#", StringSplitOptions.RemoveEmptyEntries)
                .ToList();

            model.FilterTags.Remove(model.TagToRemove);
            model.TagFilter = String.Empty;

            if (model.FilterTags.Any())
            {
                model.FilterTagsString = model.FilterTags.Aggregate((x, y) => "#" + x + "#" + y + "#");

                if (!model.FilterTagsString.EndsWith('#'))
                {
                    model.FilterTagsString += "#";
                }
            }
            else
            {
                model.FilterTagsString = String.Empty;
            }

            return View(nameof(YourCollection), await CreateYourCollectionModel(model));
        }

        public async Task<YourCollectionModel> CreateYourCollectionModel(YourCollectionModel model)
        {
            var userWithDecks = await _context.Users
                   .Where(u => u.UserName == User.Identity.Name)
                   .Include(u => u.Decks)
                   .ThenInclude(deck => deck.Tags)
                   .FirstOrDefaultAsync();

            var decksInModel = userWithDecks.Decks.Select(deck => deck);

            if (model.SearchFilter is not null)
            {
                decksInModel = decksInModel
                    .Where(deck => deck.Name
                        .ToLower()
                        .Contains(model.SearchFilter
                            .ToLower()));
            }

            if (!String.IsNullOrEmpty(model.TagFilter))
            {
                if (model.FilterTagsString is null)
                {
                    model.FilterTagsString = "#";
                }

                if (!model.FilterTagsString.Contains($"#{model.TagFilter}#"))
                {
                    model.FilterTagsString += $"{model.TagFilter.ToLower().Split(' ').Aggregate((x, y) => x += "_" + y)}#";
                }
            }

            if (!String.IsNullOrEmpty(model.FilterTagsString))
            {
                model.FilterTags = model.FilterTagsString
                    .Split("#", StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                decksInModel = decksInModel
                    .Where(deck => !model.FilterTags
                        .Except(deck.Tags is null ? new List<string>() : deck.Tags.Select(tag => tag.Name))
                        .Any());
            }

            model.YourDecks = decksInModel
                .Select(deck => new DeckModel { Name = deck.Name, Id = deck.Id })
                .ToList();

            model.TagFilter = String.Empty;
            return model;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> CreatedDecks()
        {
            var user = await _userManager.GetUserAsync(User);
            var decks = _context.Decks.Where(deck => deck.CreatorId == user.Id);

            var model = new CreatedDecksModel
            {
                CreatedDecks = await decks
                    .Select(deck => new DeckModel { Name = deck.Name, Id = deck.Id })
                    .ToListAsync(),
            };

            return View(model);
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreatedDecks(CreatedDecksModel model)
            => View(await CreateCreatedDecksModel(model));

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveTagFromCreated(CreatedDecksModel model)
        {
            model.FilterTags = model.FilterTagsString
                    .Split("#", StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

            model.FilterTags.Remove(model.TagToRemove);
            model.TagFilter = String.Empty;

            if (model.FilterTags.Any())
            {
                model.FilterTagsString = model.FilterTags.Aggregate((x, y) => "#" + x + "#" + y + "#");

                if (!model.FilterTagsString.EndsWith('#'))
                {
                    model.FilterTagsString += "#";
                }
            }
            else
            {
                model.FilterTagsString = String.Empty;
            }

            return View(nameof(CreatedDecks), await CreateCreatedDecksModel(model));
        }

        public async Task<CreatedDecksModel> CreateCreatedDecksModel(CreatedDecksModel model)
        {
            var userWithDecks = await _context.Users
                .Where(u => u.UserName == User.Identity.Name)
                .FirstOrDefaultAsync();

            // var user = await _userManager.GetUserAsync(User);
            var decksInModel = _context.Decks
                .Where(deck => deck.CreatorId == userWithDecks.Id)
                .Include(deck => deck.Tags)
                .ToList();

            if (model.SearchFilter is not null)
            {
                decksInModel = decksInModel
                    .Where(deck => deck.Name
                        .ToLower()
                        .Contains(model.SearchFilter
                            .ToLower()))
                    .ToList();
            }

            if (!String.IsNullOrEmpty(model.TagFilter))
            {
                if (model.FilterTagsString is null)
                {
                    model.FilterTagsString = "#";
                }

                if (!model.FilterTagsString.Contains($"#{model.TagFilter}#"))
                {
                    model.FilterTagsString += $"{model.TagFilter.ToLower().Split(' ').Aggregate((x, y) => x += "_" + y)}#";
                }
            }

            if (!String.IsNullOrEmpty(model.FilterTagsString))
            {
                model.FilterTags = model.FilterTagsString
                    .Split("#", StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                decksInModel = decksInModel
                    .Where(deck => !model.FilterTags
                        .Except(deck.Tags is null ? new List<string>() : deck.Tags.Select(tag => tag.Name))
                        .Any())
                    .ToList();
            }

            model.CreatedDecks = decksInModel
                .Select(deck => new DeckModel { Name = deck.Name, Id = deck.Id })
                .ToList();

            model.TagFilter = String.Empty;
            return model;
        }

        [HttpGet]
        public async Task<IActionResult> PopularDecks()
        {
            var decks = await _context.Decks
                .Where(deck => deck.IsPublic)
                .Include(deck => deck.Tags)
                .ToListAsync();

            var model = new PopularDecksModel
            {
                PopularDecks = decks
                .Select(deck => new DeckModel
                {
                    Id = deck.Id,
                    Name = deck.Name,
                    Difficulty = deck.Difficulty,
                })
                .ToList(),
                FilterTagsString = String.Empty,
            };

            return View(model);
        }

        [HttpPost]
        public async Task<IActionResult> PopularDecks(PopularDecksModel model)
            => View(await CreatePopularDecksModel(model));

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> RemoveTagFromPopular(PopularDecksModel model)
        {
            model.FilterTags = model.FilterTagsString
                    .Split("#", StringSplitOptions.RemoveEmptyEntries)
                    .ToList();
            
            model.TagFilter = String.Empty;
            model.FilterTags.Remove(model.TagToRemove);

            if (model.FilterTags.Any())
            {
                model.FilterTagsString = model.FilterTags.Aggregate((x, y) => "#" + x + "#" + y + "#");

                if (!model.FilterTagsString.EndsWith('#'))
                {
                    model.FilterTagsString += "#";
                }
            }
            else
            {
                model.FilterTagsString = String.Empty;
            }

            return View(nameof(PopularDecks), await CreatePopularDecksModel(model));
        }

        public async Task<PopularDecksModel> CreatePopularDecksModel(PopularDecksModel model)
        {
            var decks = await _context.Decks
                   .Where(deck => deck.IsPublic)
                   .Include(deck => deck.Tags)
                   .ToListAsync();

            var decksInModel = decks.Select(deck => deck);

            if (model.SearchFilter is not null)
            {
                decksInModel = decksInModel
                    .Where(deck => deck.Name
                        .ToLower()
                        .Contains(model.SearchFilter
                            .ToLower()));
            }

            if (!String.IsNullOrEmpty(model.TagFilter))
            {
                if (model.FilterTagsString is null)
                {
                    model.FilterTagsString = "#";
                }

                if (!model.FilterTagsString.Contains($"#{model.TagFilter}#"))
                {
                    model.FilterTagsString += $"{model.TagFilter.ToLower().Split(' ').Aggregate((x, y) => x += "_" + y)}#";
                }
            }

            if (!String.IsNullOrEmpty(model.FilterTagsString))
            {
                model.FilterTags = model.FilterTagsString
                    .Split("#", StringSplitOptions.RemoveEmptyEntries)
                    .ToList();

                decksInModel = decksInModel
                    .Where(deck => !model.FilterTags
                        .Except(deck.Tags is null ? new List<string>() : deck.Tags.Select(tag => tag.Name))
                        .Any());
            }

            model.PopularDecks = decksInModel
                .Select(deck => new DeckModel
                {
                    Id = deck.Id,
                    Name = deck.Name,
                    Difficulty = deck.Difficulty,
                })
                .ToList();

            model.TagFilter = String.Empty;
            return model;
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
