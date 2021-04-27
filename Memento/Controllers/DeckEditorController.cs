using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

using Memento.Models;
using Memento.Models.ViewModels;
using Memento.Models.ViewModels.DeckEditor;

using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Memento.Controllers
{
    public class DeckEditorController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly MementoDbContext _context;

        public DeckEditorController(UserManager<User> userManager, MementoDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> ChooseDeck(ChooseDeckModel model = null)
        {
            var user = await _userManager.GetUserAsync(User);
            var decks = await _context.Decks
                .Where(deck => deck.CreatorId == user.Id)
                .ToListAsync();

            if (model.SearchFilter is not null)
            {
                return View(new ChooseDeckModel
                {
                    UserName = user.UserName,
                    Decks = decks
                        .Where(deck => deck.Name.ToLower().Contains(model.SearchFilter.ToLower()))
                        .Select(deck => new DeckModel { Id = deck.Id, Name = deck.Name })
                        .ToList()
                });
            }
            else
            {
                return View(new ChooseDeckModel
                {
                    UserName = user.UserName,
                    Decks = decks
                        .Select(deck => new DeckModel { Id = deck.Id, Name = deck.Name })
                        .ToList()
                });
            }
        }

        [HttpPost]
        [Authorize]
        public IActionResult ChooseDeck(DeckModel deck)
            => RedirectToAction(nameof(EditDeck), new { deckId = deck.Id });

        [HttpGet]
        [Authorize]
        public IActionResult CreateDeck()
            => View();

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> CreateDeck(CreateDeckModel model)
        {
            if (ModelState.IsValid)
            {
                var user = await _userManager.GetUserAsync(User);
                var deck = new Deck { Name = model.Name, Creator = user };

                await _context.AddAsync(deck);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(EditDeck), new { deckId = deck.Id });
            }

            return View();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DeleteDeck(long deckId)
        {
            var user = await _userManager.GetUserAsync(User);
            var deck = await _context.Decks.FindAsync(deckId);

            if (deck.CreatorId == user.Id)
            {
                _context.Remove(deck);
                _context.SaveChanges();

                return RedirectToAction(nameof(ChooseDeck));
            }

            return BadRequest();
        }

        [Authorize]
        [HttpGet("[controller]/[action]/{deckId}")]
        public async Task<IActionResult> EditDeck([FromRoute] long deckId)
        {
            var deck = await _context.Decks
                .Select(deck => new DeckDto
                {
                    Id = deck.Id,
                    Name = deck.Name,
                    Difficulty = deck.Difficulty,
                    IsPublic = deck.IsPublic,
                    CreatorId = deck.CreatorId,
                    Cards = deck.Cards.Select(card => card).ToList(),
                    Tags = deck.Tags.Select(tag => tag).ToList(),
                })
                .Where(deck => deck.Id == deckId)
                .FirstOrDefaultAsync();

            var user = await _userManager.GetUserAsync(User);

            if (deck.CreatorId != user.Id)
            {
                return NotFound();
            }

            var cards = deck.Cards.Select(card => new CardEditorModel
            {
                Id = card.Id,
                DeckId = deck.Id,
                Word = card.Word,
                Transcription = card.Transcription,
                Description = card.Description,
                Difficulty = card.Difficulty,
            }).ToList();

            var tags = new List<TagModel>();

            if (deck.Tags is not null)
            {
                tags = deck.Tags.Select(tag => new TagModel { Name = tag.Name }).ToList();
            }

            return View(new DeckEditorModel
            {
                Cards = cards,
                Deck = new DeckModel
                {
                    Id = deck.Id,
                    Name = deck.Name,
                    Difficulty = deck.Difficulty,
                    IsPublic = deck.IsPublic,
                },
                Tags = tags,
            });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> EditDeck(DeckEditorModel model)
        {
            var deck = await _context.Decks
                .Select(deck => new DeckDto
                {
                    Id = deck.Id,
                    Name = deck.Name,
                    Difficulty = deck.Difficulty,
                    IsPublic = deck.IsPublic,
                    CreatorId = deck.CreatorId,
                    Cards = deck.Cards.Select(card => card).ToList(),
                    Tags = deck.Tags.Select(tag => tag).ToList(),
                })
                .Where(deck => deck.Id == model.Deck.Id)
                .FirstOrDefaultAsync();

            var user = await _userManager.GetUserAsync(User);

            if (deck.CreatorId != user.Id)
            {
                return NotFound();
            }

            var cards = deck.Cards.Select(card => new CardEditorModel
            {
                Id = card.Id,
                DeckId = deck.Id,
                Word = card.Word,
                Transcription = card.Transcription,
                Description = card.Description,
                Difficulty = card.Difficulty,
            }).ToList();

            if (model.CardSearchFilter is not null && model.CardSearchFilter != "")
            {
                cards = cards.Where(card => card.Word.ToLower().Contains(model.CardSearchFilter.ToLower())).ToList();
            }

            var tags = deck.Tags.Select(tag => new TagModel { Name = tag.Name }).ToList();

            return View(new DeckEditorModel
            {
                Cards = cards,
                Deck = new DeckModel
                {
                    Id = deck.Id,
                    Name = deck.Name,
                    Difficulty = deck.Difficulty,
                    IsPublic = deck.IsPublic,
                },
                Tags = tags,
            });
        }

        [HttpGet("[controller]/[action]/{deckId}")]
        public async Task<FileResult> GetCover(long deckId)
        {
            var deck = (Deck)await _context.FindAsync(typeof(Deck), deckId);
            return File(deck.Cover, "image/*");
        }

        [HttpGet("[controller]/[action]/{deckId}")]
        public async Task<FileResult> GetThumb(long deckId)
        {
            var deck = (Deck)await _context.FindAsync(typeof(Deck), deckId);
            return File(deck.Thumbnail, "image/*");
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddTag(DeckEditorModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var deck = await _context.Decks
                .Where(deck => deck.Id == model.Deck.Id)
                .Include(deck => deck.Tags)
                .FirstOrDefaultAsync();

            if (deck.CreatorId == user.Id)
            {
                string tagName = model.TagInput.ToLower().Split(' ').Aggregate((x, y) => x += "_" + y);
                var tag = await _context.DeckTags.FindAsync(tagName);

                if (tag is null)
                {
                    tag = new DeckTag { Name = tagName };
                    await _context.DeckTags.AddAsync(tag);
                }

                deck.Tags.Add(tag);
                _context.Decks.Update(deck);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(EditDeck), new { model.Deck.Id });
        }

        [Authorize]
        [HttpGet("[controller]/[action]/{deckId}/{tag}")]
        public async Task<IActionResult> RemoveTag(long deckId, string tag)
        {
            var user = await _userManager.GetUserAsync(User);
            var deck = await _context.Decks
                .Where(deck => deck.Id == deckId)
                .Include(deck => deck.Tags)
                .FirstOrDefaultAsync();

            if (deck.CreatorId == user.Id)
            {
                var tagToRemove = await _context.DeckTags.FindAsync(tag);

                if (tagToRemove is null)
                {
                    return BadRequest();
                }

                deck.Tags.Remove(tagToRemove);
                _context.Decks.Update(deck);
                await _context.SaveChangesAsync();
            }

            return RedirectToAction(nameof(EditDeck), new { deckId });
        }

        [Authorize]
        [HttpGet("[controller]/[action]/{deckId}")]
        public async Task<IActionResult> ChooseCard([FromRoute] long deckId)
        {
            var user = await _context.Users
                .Where(u => u.UserName == User.Identity.Name)
                .Include(u => u.Decks)
                .FirstOrDefaultAsync();

            if (_context.Decks.Where(deck => deck.CreatorId == user.Id).Any())
            {
                return View(new ChooseCardModel { DeckId = deckId, SearchFilter = String.Empty, Cards = new List<CardModel>() });
            }

            return RedirectToAction(nameof(EditDeck), new { deckId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> ChooseCard(ChooseCardModel model)
        {
            var user = await _context.Users
                .Where(u => u.UserName == User.Identity.Name)
                .Include(u => u.Decks)
                .FirstOrDefaultAsync();

            if (_context.Decks.Where(deck => deck.CreatorId == user.Id).Any())
            {
                var cards = await _context.Cards
                    .Where(card => card.Word.ToLower().Contains(model.SearchFilter.ToLower()))
                    .Select(card => new CardModel { Id = card.Id, Word = card.Word, Description = card.Description })
                    .ToListAsync();

                return View(new ChooseCardModel { DeckId = model.DeckId, SearchFilter = model.SearchFilter, Cards = cards });
            }

            return RedirectToAction(nameof(EditDeck), new { model.DeckId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SaveChanges(DeckEditorModel model)
        {
            if (ModelState.IsValid)
            {
                var deck = await _context.Decks.FindAsync(model.Deck.Id);

                if (deck is not null)
                {
                    using (var ms = new MemoryStream())
                    {
                        if (model.Deck.Cover is not null)
                        {
                            model.Deck.Cover.CopyTo(ms);
                            deck.Cover = ms.ToArray();
                        }
                        else if (model.Deck.CoverRemoved)
                        {
                            deck.Cover = null;
                        }
                    }

                    using (var ms = new MemoryStream())
                    {
                        if (model.Deck.Thumb is not null)
                        {
                            model.Deck.Thumb.CopyTo(ms);
                            deck.Thumbnail = ms.ToArray();
                        }
                        else if (model.Deck.ThumbRemoved)
                        {
                            deck.Thumbnail = null;
                        }
                    }

                    deck.IsPublic = model.Deck.IsPublic;
                    deck.Name = model.Deck.Name;
                    deck.Difficulty = model.Deck.Difficulty;
                    _context.Decks.Update(deck);
                    _context.SaveChanges();
                }
            }

            return RedirectToAction(nameof(EditDeck), new { model.Deck.Id });
        }
    }
}
