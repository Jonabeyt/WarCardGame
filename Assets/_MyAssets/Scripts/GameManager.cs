using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace Cards
{
    public class GameManager : MonoBehaviour
    {
        //bool to check if game should take input from player. should be false during animations
        public bool m_TakingInput = true;

        Queue<CardInfo> m_PlayerDeck = new Queue<CardInfo>();
        Queue<CardInfo> m_OpponentDeck = new Queue<CardInfo>();
        [SerializeField]
        TextMeshProUGUI m_MessageText;
        [SerializeField, Tooltip("Time it takes for the card tween animation to finish.")]
        float m_AnimationTime = 0.5f;
        [SerializeField, Range(0.2f,0.8f), Tooltip("Number between 0.2 and 0.8 to determine animation distance. 1 being whole screen.")]
        float m_AnimationDistance = 0.3f;
        [SerializeField]
        Transform m_PlayerDeckTransform;
        [SerializeField] 
        Transform m_OpponentDeckTransform;        
        [SerializeField]
        GameObject m_CardPrefab;
        [SerializeField]

        Transform m_AvailableCardPool;
        Transform m_PlayedCards;

        void Start()
        {
            m_AvailableCardPool = new GameObject("AvailableCardPool").transform;
            m_AvailableCardPool.parent = transform;
            m_PlayedCards = new GameObject("PlayedCards").transform;
            m_PlayedCards.parent = transform;
            DealNewCardsToPlayers();
        }

        private void Update()
        {
            if (m_TakingInput && Input.GetMouseButtonDown(0))
            {
                StartCoroutine(PlayerClash());
                m_TakingInput = false;
            }
        }

        private void SetMessageText(string msg = "")
        {
            m_MessageText.text = msg;
        }

        private void DealNewCardsToPlayers()
        {
            List<CardInfo> deckOfCards = CreateStandardDeck();
            if (!deckOfCards.TestDeckLegality())
            {
                Debug.LogError("Illegal Deck");
                return;
            }
            deckOfCards.Shuffle();
            bool dealingCardToPlayer = true;
            while (deckOfCards.Count > 0)
            {
                if (dealingCardToPlayer)
                {
                    m_PlayerDeck.Enqueue(deckOfCards[0]);
                }
                else
                {
                    m_OpponentDeck.Enqueue(deckOfCards[0]);
                }
                dealingCardToPlayer = !dealingCardToPlayer;
                deckOfCards.RemoveAt(0);
            }
        }

        private List<CardInfo> CreateStandardDeck()
        {
            List<CardInfo> deckOfCards = new List<CardInfo>();
            for (int i = 0; i < System.Enum.GetValues(typeof(CardName)).Length; i++)
            {
                for (int j = 0; j < System.Enum.GetValues(typeof(CardSuit)).Length; j++)
                {
                    CardInfo card = new CardInfo();
                    card.m_CardName = (CardName)i;
                    card.m_CardSuit = (CardSuit)j;
                    deckOfCards.Add(card);
                }
            }
            if (deckOfCards.TestDeckLegality())
            {
                return deckOfCards;
            }
            else
            {
                Debug.LogError("Created Illegal Deck! check CreateStandardDeck() function");
                return null;
            }
        }     
        
        //gets an available, unused card object, if none are available, will instantiate a new card and add it to the object pool.
        GameObject GetAvailableCard(CardInfo info, bool showCardBack = false)
        {
            GameObject card;
            if (m_AvailableCardPool.childCount > 0)
            {
                card = m_AvailableCardPool.GetChild(0).gameObject;
                card.transform.parent = m_PlayedCards;
            }
            else
            {
                card = Instantiate(m_CardPrefab, m_PlayerDeckTransform.position, Quaternion.identity, m_PlayedCards);
            }

            if (card.GetComponent<CardObject>() == null)
            {
                Debug.LogError("Available Card does not have CardObject Component");
                return null;
            }
            card.GetComponent<CardObject>().m_Info = info;
            card.GetComponent<CardObject>().m_ShowCardBack = showCardBack;
            card.name = string.Format("{0} of {1}", info.m_CardName, info.m_CardSuit);
            card.SetActive(true);
            return card;
        }
        
        private void AnimateClash(CardInfo playerCardInfo, CardInfo opponentCardInfo, float animationTime, bool faceDown = false)
        {
            GameObject playerCard;
            GameObject opponentCard;
            playerCard = GetAvailableCard(playerCardInfo, faceDown);
            playerCard.transform.position = m_PlayerDeckTransform.position;
            opponentCard = GetAvailableCard(opponentCardInfo, faceDown);
            opponentCard.transform.position = m_OpponentDeckTransform.position;
            Vector3 playerCardPosition = CardAnimationTargetPoint();
            Vector3 opponentCardPosition = -CardAnimationTargetPoint();
            if (faceDown)
            {
                playerCardPosition *= 1+ m_AnimationDistance;
                playerCardPosition += Vector3.forward;
                opponentCardPosition *= 1 + m_AnimationDistance;
                opponentCardPosition += Vector3.forward;
            }
            
            playerCard.MoveTo(playerCardPosition, animationTime, 0);
            opponentCard.MoveTo(opponentCardPosition, animationTime, 0);
        }

        private Vector3 CardAnimationTargetPoint()
        {
            Vector3 screenPoint = new Vector3(Screen.width * m_AnimationDistance, Screen.height * m_AnimationDistance, 0);
            return Camera.main.ScreenToWorldPoint(screenPoint);
        }

        private IEnumerator AnimateTakeAll(Transform deckLocation, float animationTime)
        {            
            while(m_PlayedCards.childCount>0)
            {
                Transform card = m_PlayedCards.GetChild(0);
                card.gameObject.MoveTo(deckLocation.position, animationTime, 0);                
                card.parent = m_AvailableCardPool;
                yield return new WaitForSeconds(animationTime);
                card.gameObject.SetActive(false);                
            }
            SetMessageText();
            m_TakingInput = true;
        }

        private IEnumerator PlayerClash(List<CardInfo> ante = null)
        {
            Transform roundWinnerDeck;
            CardInfo playerCard = m_PlayerDeck.Dequeue();
            CardInfo opponentCard = m_OpponentDeck.Dequeue();
            AnimateClash(playerCard, opponentCard, m_AnimationTime);
            yield return new WaitForSeconds(m_AnimationTime);
            if (ante == null)
            {
                ante = new List<CardInfo>();
            }
            ante.Add(playerCard);
            ante.Add(opponentCard);
            CardNumberComparison comparer = new CardNumberComparison();
            int cardComparison = comparer.Compare(playerCard, opponentCard);
            
            if (cardComparison == 1)
            {
                roundWinnerDeck = m_PlayerDeckTransform;
                Debug.Log("player wins round");
                SetMessageText("Player wins the round");
                foreach (CardInfo card in ante)
                {
                    m_PlayerDeck.Enqueue(card);
                }
            }
            else if (cardComparison == -1)
            {
                roundWinnerDeck = m_OpponentDeckTransform;
                Debug.Log("opponent wins round");
                SetMessageText("Opponent wins the round");
                foreach (CardInfo card in ante)
                {
                    m_OpponentDeck.Enqueue(card);
                }
            }
            else
            {
                Debug.Log("tie! War!");
                SetMessageText("WAR! Play another card!");
                CardInfo playerAnteCardInfo = m_PlayerDeck.Dequeue();
                CardInfo opponentAnteCardInfo = m_OpponentDeck.Dequeue();                
                ante.Add(playerAnteCardInfo);
                ante.Add(opponentAnteCardInfo);
                AnimateClash(playerAnteCardInfo, opponentAnteCardInfo, m_AnimationTime, true);
                yield return new WaitForSeconds(m_AnimationTime);
                m_TakingInput = true;
                yield break;
            }
            yield return new WaitUntil(() => Input.GetMouseButtonDown(0));
            StartCoroutine(AnimateTakeAll(roundWinnerDeck, m_AnimationTime/2));
            if (m_PlayerDeck.Count == 0)
            {
                Debug.Log("OPPONENT WINS");
            }
            if (m_OpponentDeck.Count == 0)
            {
                Debug.Log("PLAYER WINS");
            }        
        }
    }
}