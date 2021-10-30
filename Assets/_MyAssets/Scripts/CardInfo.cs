using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    [System.Serializable]
    public class CardInfo : IComparable<CardInfo>
    {
        public CardName m_CardName;
        public CardSuit m_CardSuit;

        public CardInfo()
        {
            m_CardName = 0;
            m_CardSuit = 0;
        }
        public CardInfo(CardName name, CardSuit suit)
        {          
            m_CardName = name;
            m_CardSuit = suit;
        }        

        public int CompareTo(CardInfo otherCard)
        {
            if (m_CardName>otherCard.m_CardName)
            {
                return 1;
            }
            if (m_CardName < otherCard.m_CardName)
            {
                return -1;
            }
            return 0;
        }
    }

    
    public enum CardName
    {
        two,
        three,
        four,
        five,
        six,
        seven,
        eight,
        nine,
        ten,
        jack,
        queen,
        king,
        ace
    }
    public enum CardSuit
    {
        clubs,
        spades,
        hearts,        
        diamonds
    }

    public class CardNumberComparison : IComparer<CardInfo>
    {
        public int Compare(CardInfo firstCard, CardInfo secondCard)
        {
            return firstCard.m_CardName.CompareTo(secondCard.m_CardName);
        }
    }
    public class WholeCardComparison : IComparer<CardInfo>
    {
        public  int Compare(CardInfo firstCard, CardInfo secondCard)
        {
            if (firstCard.m_CardName.CompareTo(secondCard.m_CardName)!=0)
            {
                return firstCard.m_CardSuit.CompareTo(secondCard.m_CardSuit);
            }
            return 0;
        }
    }
}
