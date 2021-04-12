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
    public class CardEditorController : Controller
    {
        private readonly UserManager<User> _userManager;
        private readonly MementoDbContext _context;

        public CardEditorController(UserManager<User> userManager, MementoDbContext context)
        {
            _userManager = userManager;
            _context = context;
        }

        [Authorize]
        [HttpPost("[controller]/[action]/{deckId}")]
        public IActionResult EditCard([FromRoute] long deckId, [FromForm] CardEditorModel model)
            => RedirectToAction(nameof(EditCard), new { deckId, cardId = model.Id });

        [Authorize]
        [HttpGet("[controller]/[action]/{deckId}/{cardId?}")]
        public async Task<IActionResult> EditCard(long deckId, long cardId = 0)
        {
            var user = await _userManager.GetUserAsync(User);
            var deck = await _context.Decks.FindAsync(deckId);
            var card = await _context.Cards
                .Where(card => card.Id == cardId)
                .Include(card => card.Tags)
                .FirstOrDefaultAsync();

            if (user.Id == deck.CreatorId)
            {
                if (card is null)
                {
                    return View(new CardEditorModel
                    {
                        DeckId = deck.Id,
                        Tags = new List<TagModel>(),
                    });
                }
                else
                {
                    return View(new CardEditorModel
                    {
                        DeckId = deck.Id,
                        Id = card.Id,
                        Word = card.Word,
                        Transcription = card.Transcription,
                        Description = card.Description,
                        Difficulty = card.Difficulty,
                        Tags = card.Tags.Select(tag => new TagModel { Name = tag.Name }).ToList(),
                    });
                }
            }

            return BadRequest();
        }

        [HttpGet]
        [Authorize]
        public async Task<IActionResult> DeleteCard(long deckId, long cardId)
        {

            var user = await _userManager.GetUserAsync(User);
            var deck = await _context.Decks
                .Where(deck => deck.Id == deckId)
                .Include(deck => deck.Cards)
                .FirstOrDefaultAsync();
            if (user.Id == deck.CreatorId)
            {
                var card = deck.Cards
                    .Where(card => card.Id == cardId)
                    .FirstOrDefault();

                deck.Cards.Remove(card);
                deck.CardNumber--;
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(DeckEditorController.EditDeck), "DeckEditor", new { deckId });
        }

        [Authorize]
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

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> AddTag(CardEditorModel model)
        {
            var user = await _userManager.GetUserAsync(User);
            var deck = await _context.Decks.FindAsync(model.DeckId);

            if (deck.CreatorId == user.Id)
            {
                var card = await _context.Cards
                    .Where(card => card.Id == model.Id)
                    .Include(card => card.Tags)
                    .FirstOrDefaultAsync();

                string tagName = model.TagInput
                    .ToLower()
                    .Split(' ')
                    .Aggregate((x, y) => x += "_" + y);

                var tag = await _context.CardTags.FindAsync(tagName);

                if (tag is null)
                {
                    tag = new CardTag { Name = tagName };
                    await _context.CardTags.AddAsync(tag);
                }

                card.Tags.Add(tag);
                _context.Cards.Update(card);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(EditCard), new { deckId = model.DeckId, cardId = model.Id });
        }

        [Authorize]
        [HttpGet("[controller]/[action]/{deckId}/{cardId}/{tag}")]
        public async Task<IActionResult> RemoveTag(long deckId, long cardId, string tag)
        {
            var user = await _userManager.GetUserAsync(User);
            var deck = await _context.Decks.FindAsync(deckId);

            if (deck.CreatorId == user.Id)
            {
                var card = await _context.Cards
                    .Where(card => card.Id == cardId)
                    .Include(card => card.Tags)
                    .FirstOrDefaultAsync();

                var tagToRemove = await _context.CardTags.FindAsync(tag);

                if (tagToRemove is null)
                {
                    return BadRequest();
                }

                card.Tags.Remove(tagToRemove);
                _context.Cards.Update(card);
                _context.SaveChanges();
            }

            return RedirectToAction(nameof(EditCard), new { deckId, cardId });
        }

        [HttpPost]
        [Authorize]
        public async Task<IActionResult> SaveChanges(CardEditorModel model)
        {
            if (ModelState.IsValid)
            {
                var deck = await _context.Decks
                    .Where(deck => deck.Id == model.DeckId)
                    .Include(deck => deck.Cards)
                    .FirstOrDefaultAsync();

                var card = deck.Cards
                    .Where(card => card.Id == model.Id)
                    .FirstOrDefault();

                if (deck is not null)
                {
                    if (card is not null)
                    {
                        using (var ms = new MemoryStream())
                        {
                            if (model.Image is not null)
                            {
                                model.Image.CopyTo(ms);
                                card.Image = ms.ToArray();
                            }
                            else if (model.ImageRemoved)
                            {
                                card.Image = null;
                            }
                        }

                        card.Word = model.Word;
                        card.Description = model.Description;
                        card.Transcription = model.Transcription;
                        card.Difficulty = model.Difficulty;
                        _context.Cards.Update(card);
                    }
                    else
                    {
                        card = new Card
                        {
                            Word = model.Word,
                            Description = model.Description,
                            Transcription = model.Transcription,
                            Difficulty = model.Difficulty,
                        };

                        using (var ms = new MemoryStream())
                        {
                            if (model.Image is not null)
                            {
                                model.Image.CopyTo(ms);
                                card.Image = ms.ToArray();
                            }
                            else if (model.ImageRemoved)
                            {
                                card.Image = null;
                            }
                        }

                        //_context.Cards.Add(card);
                        deck.Cards.Add(card);
                        deck.CardNumber++;
                        _context.Decks.Update(deck);
                        _context.SaveChanges();
                    }

                    _context.SaveChanges();
                }

                return RedirectToAction(nameof(EditCard), new { deckId = model.DeckId, cardId = card.Id });
            }

            return RedirectToAction(nameof(EditCard), new { deckId = model.DeckId });
        }
    }
}
