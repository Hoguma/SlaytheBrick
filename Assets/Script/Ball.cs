using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Ball : MonoBehaviour
{
    public GameObject Greenball;
    public Rigidbody2D RB;
    public bool isMoving;

    public void Launch(Vector3 pos)
    {
        GameManager.Instance.shotTrigger = true;
        isMoving = true;
        RB.AddForce(pos * 14000);
    }

    private void OnCollisionEnter2D(Collision2D collision)
    {
        StartCoroutine(coll(collision));
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        StartCoroutine(Trigger(collision));
    }



    private IEnumerator coll(Collision2D collision)
    {
        GameObject Col = collision.gameObject;
        Physics2D.IgnoreLayerCollision(2, 2);

        Vector2 pos = RB.velocity.normalized;
        if(pos.magnitude != 0 && pos.y < 0.15f && pos.y > -0.15f)
        {
            RB.velocity = Vector2.zero;
            RB.AddForce(new Vector2(pos.x > 0 ? 1 : -1, -0.2f).normalized * 7000);
        }

        if(Col.CompareTag("Ground"))
        {
            RB.velocity = Vector2.zero;
            transform.position = new Vector2(collision.contacts[0].point.x, GameManager.Instance.groundY);
            GameManager.Instance.VeryFirstPosSet(transform.position);

            while (true)
            {
                yield return null;
                transform.position = Vector3.MoveTowards(transform.position, GameManager.Instance.veryFirstPos, 4);
                if (transform.position == GameManager.Instance.veryFirstPos) { isMoving = false; yield break; } 
            }
        }

        if(Col.CompareTag("Block"))
        {
            Text BlockText = collision.transform.GetChild(0).GetComponentInChildren<Text>();
            int blockValue = int.Parse(BlockText.text) - 1;
            GameManager.Instance.BlockColorChange();

            for(int i = 0; i < GameManager.Instance.S_Block.Length; i++)
            {
                if (GameManager.Instance.S_Block[i].isPlaying) continue;
                else { GameManager.Instance.S_Block[i].Play(); break; }
            }

            if(blockValue > 0)
            {
                BlockText.text = blockValue.ToString();
                //Col.GetComponent<Animator>().SetTrigger("shock");
            }
            else
            {
                Destroy(Col);
                Destroy(Instantiate(GameManager.Instance.p_particleRed, collision.transform.position, Quaternion.identity), 1);
            }
        }
    }

    private IEnumerator Trigger(Collider2D col)
    {
        if (col.gameObject.CompareTag("Coinorb"))
        {
            Destroy(col.gameObject);
            Destroy(Instantiate(GameManager.Instance.p_particleGreen, col.transform.position, GameManager.Instance.QI), 1);

            GameManager.Instance.S_GreenOrb.Play();
            GameManager.Instance.coin += 1 * GameManager.Instance.coinObtain;
            GameManager.Instance.CoinText.text = GameManager.Instance.coin.ToString();
            yield return null;
        }
    }
}
