using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CardDatabase : MonoBehaviour
{
    public static List<Card> cardList = new List<Card>();

    private void Awake()
    {
        //cardList.Add(new Card(0, "관통", 4, "이번 턴에 공이 블럭을 관통한다."));
        cardList.Add(new Card(0, "관통", 4, "이번 턴에 공이 블럭을 관통한다.", Resources.Load<Sprite>("Skill")));
        cardList.Add(new Card(1, "눈보라", 2, "이번 턴에 블럭이 내려오지 않는다.", Resources.Load<Sprite>("Ice")));
        cardList.Add(new Card(2, "어쌔신 슈퍼 매직 그림자분신술", 7, "이번 턴에 공이 2배가 된다.", Resources.Load<Sprite>("Card_double")));
        cardList.Add(new Card(3, "코획 200%", 5, "앞으로 5턴동안 얻는 코인이 2배가 된다.", Resources.Load<Sprite>("card_coin")));
    }
}
