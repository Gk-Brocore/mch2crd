using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace Game.Card
{
    public class CardViewUI : MonoBehaviour , ICard
    {
        public string cardID;
        public bool isMatched = false;
        [Header("UI Elements")]
        public Image cardImage;
        public TextMeshProUGUI debugLabel;

        [Header("Flip States")]
        public Image backSprite;
        public Image frontSprite;
        private bool isRevealed = false;

        public string clickSound = "CardClick";

        private Animator animator;

        private int frontAnimHash;
        private int backAnimHash;

        private int matchHash;
        private int mismatchHash;

        private bool isAnimDone;

        private Vector2Int coordinates;

        public bool IsAnimDone { get => isAnimDone; set => isAnimDone = value; }

        public string Id => cardID;
        public bool IsMatched => isMatched;
        public bool IsRevealed => isRevealed;
        public Vector2Int Coordinates => coordinates;

        private void Awake()
        {
            animator = GetComponent<Animator>();
            frontAnimHash = Animator.StringToHash("Front");
            backAnimHash = Animator.StringToHash("Back");
            matchHash = Animator.StringToHash("Match");
            mismatchHash = Animator.StringToHash("Mismatch");

        }

        public void Initialize(string _id, Vector2Int _pos, Sprite _sprite, Vector2 _imgSize, bool _showDebug = false)
        {

            animator = GetComponent<Animator>();
            coordinates = _pos;

            transform.localScale = Vector3.one;

            if (_sprite == null)
            {
                SetHidden();
                return;
            }

            cardID = _id;
            isMatched = false;
            isRevealed = false;

            if (frontSprite != null)
            {
                frontSprite.rectTransform.sizeDelta = _imgSize;
                frontSprite.sprite = _sprite;
            }

            if (debugLabel != null)
                debugLabel.text = _showDebug ? $"ID:{cardID}" : string.Empty;

        }

        public void SetHidden()
        {
            cardImage.color = new Color(0, 0, 0, 0);
            backSprite.gameObject.SetActive(false);
            animator.enabled = false;
        }

        public void Reveal()
        {
            if (isRevealed || isMatched)
                return;
            isRevealed = true;
            animator.SetTrigger(frontAnimHash);
        }

        public void Hide()
        {
            if (isMatched)
                return;
            isRevealed = false;
            animator.SetTrigger(backAnimHash);
        }

        public void SetMatched(bool matched)
        {
            isMatched = matched;
            if (matched)
            {

                animator.SetTrigger(matchHash);
            }
        }

        public void MatchedAnim()
        {
            isAnimDone = true;
        }

        public void Mismatch()
        {
            animator.SetTrigger(mismatchHash);
        }

        
   
    }
}
