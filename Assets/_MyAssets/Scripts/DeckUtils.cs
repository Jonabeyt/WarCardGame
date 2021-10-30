using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public static class DeckUtils
    {
        //shuffle a deck of cards
        public static void Shuffle(this List<CardInfo> deckOfCards)
        {
            System.Random rng = new System.Random();
            for (int i = deckOfCards.Count-1; i > 1; i--)
            {
                int randomCard = rng.Next(i + 1);
                CardInfo tempCardHolder = deckOfCards[randomCard];
                deckOfCards[randomCard] = deckOfCards[i];
                deckOfCards[i] = tempCardHolder;
            }
        }

        //checks if the given deck of cards is a a deck of distinct cards of the given amount, defaults to 52 for standard playing card decks,
        public static bool TestDeckLegality(this List<CardInfo> deckOfCards, int deckSize = 52)
        {
            IEnumerable<CardInfo> distinctCards = deckOfCards.Distinct();            
            if (distinctCards.Count() != deckOfCards.Count)
            {
                return false;
            }
            if (distinctCards.Count() != deckSize)
            {
                return false;
            }
            return true;            
        }
    }
}