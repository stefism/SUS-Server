using BattleCards.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace BattleCards.Services
{
    public class CardService : ICardService
    {
        private readonly ApplicationDbContext db;

        public CardService(ApplicationDbContext db)
        {
            this.db = db;
        }

        public void AddCard()
        {
            throw new NotImplementedException();
        }
    }
}
