using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class ThisCard : MonoBehaviour , IBeginDragHandler, IEndDragHandler, IDragHandler, IDropHandler
{
    public List<Card> thisCard = new List<Card>();
    public int thisID;

    public string cardDescription;
    public string cardName;
    public int cost;
    public int id;

    public Text nameTex;
    public Text costTex;
    public Text descriptionTex;

    public Sprite thisSprite;
    public Image thatImage;

    public GameObject Hand;
    public int numberOfCardInDeck;

    Vector2 firstPos;

    public void OnBeginDrag(PointerEventData eventData)
    {
        firstPos = transform.position;
        transform.localScale *= 1.8f;
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 drag = Camera.main.ScreenToWorldPoint(eventData.position);
        transform.position = new Vector3(drag.x, drag.y, 0);
    }

    public void OnDrop(PointerEventData eventData)
    {
        if (transform.position.y >= -550f)
        { 
            Debug.Log("drop");
            UseCard();
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        transform.position = firstPos;
        transform.localScale /= 1.8f;
    }

    // Start is called before the first frame update
    void Start()
    {
        thisID = Random.Range(1, 4);
        thisCard[0] = CardDatabase.cardList[thisID];
    }

    // Update is called once per frame
    void Update()
    {
        id = thisCard[0].id;
        cardName = thisCard[0].cardName;
        cost = thisCard[0].cost;
        cardDescription = thisCard[0].cardDescription;

        thisSprite = thisCard[0].sprite;

        nameTex.text = "" + cardName;
        costTex.text = "" + cost;
        descriptionTex.text = "" + cardDescription;

        thatImage.sprite = thisSprite;
    }

    void UseCard()
    {
        if (GameManager.Instance.coin >= cost)
        { 
            GameManager.Instance.coin -= cost;
            GameManager.Instance.CoinText.text = GameManager.Instance.coin.ToString();
            GameManager.Instance.cardID = id;
            GameManager.Instance.CardEffect();
        }
        thisID = Random.Range(1, 4);
        thisCard[0] = CardDatabase.cardList[thisID];
    }
}
