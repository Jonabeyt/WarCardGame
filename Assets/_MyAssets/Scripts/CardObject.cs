using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace Cards
{
    public class CardObject : MonoBehaviour
    {
        public bool m_ShowCardBack = false;
        public CardInfo m_Info;
        [SerializeField]
        Sprite[] m_SpriteArray;
        [SerializeField]
        Sprite m_BackSprite;

        private void OnEnable()
        {
            SetSprite();
        }

        //function that sets the sprite of the card according to its info or if its face down
        private void SetSprite()
        {                        
            Sprite spriteToSet;
            SpriteRenderer spriteRenderer = GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                string errorMsg = String.Format("Sprite Renderer not found on card {0}",
                    gameObject.name);
                Debug.LogError(errorMsg);
                return;
            }
            if (m_ShowCardBack)
            {
                spriteToSet = m_BackSprite;
            }
            else
            {
                int cardPositionInArray = ((int)m_Info.m_CardSuit * Enum.GetValues(typeof(CardName)).Length) + (int)m_Info.m_CardName;
                spriteToSet = m_SpriteArray[cardPositionInArray];
            }
            spriteRenderer.sprite = spriteToSet;
        }
    }    
}