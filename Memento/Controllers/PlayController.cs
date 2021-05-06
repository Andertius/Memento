﻿using System;
using System.Collections.Generic;
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
        private static PlayDeckModel _sd;

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

            //var userWithDecks = await _context.Users
            //    .Where(u => u.UserName == User.Identity.Name)
            //    .Include(u => u.Decks)
            //    .FirstOrDefaultAsync();
            //var deck = await userWithDecks.Decks.Where(x=>x==)
            var deck = await _context.Decks
                .FirstOrDefaultAsync();
            _sd = new PlayDeckModel
            {
                Id = deck.Id,
                Name = deck.Name,
                CreatorName = deck.Creator.UserName,
                Difficulty = Enum.GetName(typeof(Difficulty), deck.Difficulty),
            };

            return View(new PlayModel
            {
                Username = user.UserName,
                //CurrentCard = _sd.Cards.First(),
                PickedDeck = _sd
            });
        }

        [HttpGet("[controller]/[action]/{deckId}")]
        public async Task<FileResult> GetCover(long deckId)
        {
            var deck = (Deck)await _context.FindAsync(typeof(Deck), deckId);
            return File(deck.Cover, "image/*");
        }
    }
}

