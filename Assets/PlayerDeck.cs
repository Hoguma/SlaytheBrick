using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerDeck : MonoBehaviour
{
    public List<Card> Deck = new List<Card>();

    public int x;

    // Start is called before the first frame update
    void Start()
    {
        x = 0;
        for(int i =0; i < 2; i++)
        {
            Debug.Log("d");
            x = Random.Range(1, 4);
            Deck[i] = CardDatabase.cardList[x];
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Shuffle()
    {
        //for(int i = 0; i < Deck.Count)
    }
}
