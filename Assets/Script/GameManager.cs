using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    private static GameManager instance = null;
    public static GameManager Instance
    {
        get
        {
            if (null == instance)
            {
                return null;
            }
            return instance;
        }
    }

    [Header("GameObject")]
    public GameObject p_ball, p_block, p_particleBlue, p_particleGreen, p_particleRed, p_greenOrb;
    public GameObject BallPreview, Arrow, GameOverPanel, BallCountTextObj, BallPlusTextObj, Greenball;

    //[Header("Transform")]
    public Transform GreenBallGroup, BlockGroup, BallGroup;

    //[Header("Text")]
    public Text BestScoreText, ScoreText, BallCountText, BallPlusText, FinalScoreText, newRecordText, CoinText;

    //[Header("Sound")]
    public AudioSource S_GameOver, S_GreenOrb, S_Plus;
    public AudioSource[] S_Block;

    //[Header("ETC.")]
    public LineRenderer MouseLR, BallLR;
    public Color[] blockColor;
    public Color greenColor;
    public bool shotTrigger, shotable;
    public Quaternion QI = Quaternion.identity;
    public float groundY = -56;
    public Vector3 veryFirstPos;
    
    public int coin;
    public int cardID;
    public int COcount = 0;
    public int temp;

    public int coinObtain = 1;
    Vector3 firstPos, secondPos, gap;
    int score, timerCount, launchIndex;
    bool timerStart, isDie, isNewRecord, isBlockMoving;
    bool isGBallMove = true;
    bool shadow = false;
    bool ice = false;
    public bool gold = false;
    float timeDelay;


    void Awake()
    {
        Time.timeScale = 1;
        if (null == instance)
        {
            instance = this;
        }
        else
        {
            Destroy(this.gameObject);
        }
        Screen.SetResolution(1080, 1920, true);

        BlockGenerator(); 
            
        BestScoreText.text = "최고기록 : " + PlayerPrefs.GetInt("BestScore").ToString();
    }

    public void ReStart() => SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);

    public void VeryFirstPosSet(Vector3 pos) { if (veryFirstPos == Vector3.zero) veryFirstPos = pos; }


    void BlockGenerator()
    {
        ScoreText.text = "현재점수 :" + (++score).ToString();
        if (PlayerPrefs.GetInt("BestScore", 0) < score)
        {
            PlayerPrefs.SetInt("BestScore", score);
            BestScoreText.text = "최고기록 : " + PlayerPrefs.GetInt("BestScore").ToString();
            BestScoreText.color = greenColor;
            isNewRecord = true;
        }

        int count;
        int randBlock = Random.Range(0, 24);
        if (score <= 10) count = randBlock < 16 ? 1 : 2;
        else if (score <= 20) count = randBlock < 8 ? 1 : (randBlock < 16 ? 2 : 3);
        else if (score <= 40) count = randBlock < 9 ? 2 : (randBlock < 18 ? 3 : 4);
        else count = randBlock < 8 ? 2 : (randBlock < 16 ? 3 : (randBlock < 20 ? 4 : 5));


        List<Vector3> SpawnList = new List<Vector3>();
        for (int i = 0; i < 6; i++) SpawnList.Add(new Vector3(-46.7f + i * 18.68f, 51.2f, 0));

        for (int i = 0; i < count; i++)
        {
            int rand = Random.Range(0, SpawnList.Count);

            Transform TR = Instantiate(p_block, SpawnList[rand], QI).transform;
            TR.SetParent(BlockGroup);
            TR.GetChild(0).GetComponentInChildren<Text>().text = score.ToString();

            SpawnList.RemoveAt(rand);
        }
        Instantiate(p_greenOrb, SpawnList[Random.Range(0, SpawnList.Count)], QI).transform.SetParent(BlockGroup);

        isBlockMoving = true;
        for (int i = 0; i < BlockGroup.childCount; i++) StartCoroutine(BlockMoveDown(BlockGroup.GetChild(i)));

    }


    IEnumerator BlockMoveDown(Transform TR)
    {
        yield return new WaitForSeconds(0.2f);
        Vector3 targetpos = TR.position + new Vector3(0, -12.8f, 0);
        BlockColorChange();

        if (targetpos.y < -50)
        {
            if (TR.CompareTag("Block")) isDie = true;
            for (int i = 0; i < BallGroup.childCount; i++)
                BallGroup.GetChild(i).GetComponent<CircleCollider2D>().enabled = false;
        }

        float TT = 1.5f;
        while (true)
        {
            yield return null; TT -= Time.deltaTime * 1.5f;
            TR.position = Vector3.MoveTowards(TR.position, targetpos + new Vector3(0, -6, 0), TT);
            if (TR.position == targetpos + new Vector3(0, -6, 0)) break;
        }
        TT = 0.9f;
        while (true)
        {
            yield return null; TT -= Time.deltaTime;
            TR.position = Vector3.MoveTowards(TR.position, targetpos, TT);
            if (TR.position == targetpos) break;
        }
        isBlockMoving = false;

        if (targetpos.y < -50)
        {
            if (TR.CompareTag("Block"))
            {
                for (int i = 0; i < BallGroup.childCount; i++)
                    Destroy(BallGroup.GetChild(i).gameObject);
                Destroy(Instantiate(p_particleBlue, veryFirstPos, QI), 1);

                BallCountTextObj.SetActive(false);
                BallPlusTextObj.SetActive(false);
                BestScoreText.gameObject.SetActive(false);
                ScoreText.gameObject.SetActive(false);

                GameOverPanel.SetActive(true);
                FinalScoreText.text = "최종점수 : " + score.ToString();
                if (isNewRecord) newRecordText.gameObject.SetActive(true);

                S_GameOver.Play();
            }
            else
            {
                Destroy(TR.gameObject);
                Destroy(Instantiate(p_particleGreen, TR.position, QI), 1);

                for (int i = 0; i < BallGroup.childCount; i++)
                    BallGroup.GetChild(i).GetComponent<CircleCollider2D>().enabled = true;
            }
        }
    }

    public void BlockColorChange()
    {
        for(int i = 0; i < BlockGroup.childCount; i++)
        {
            if(BlockGroup.GetChild(i).CompareTag("Block"))
            {
                float per = int.Parse(BlockGroup.GetChild(i).GetChild(0).GetComponentInChildren<Text>().text) / (float)score;
                Color curColor;
                if (per <= 0.1428f) curColor = blockColor[6];
                else if (per <= 0.2856f) curColor = blockColor[5];
                else if (per <= 0.4284f) curColor = blockColor[4];
                else if (per <= 0.5172f) curColor = blockColor[3];
                else if (per <= 0.714f) curColor = blockColor[2];
                else if (per <= 0.8568f) curColor = blockColor[1];
                else curColor = blockColor[0];
                BlockGroup.GetChild(i).GetComponent<SpriteRenderer>().color = curColor;
            }
        }
    }

    // Update is called once per frame
    void Update()
    {


        // 죽으면 실행 안함
        if (isDie) return;

        //클릭하면 클릭한 지점을 저장
        if(Input.touchCount > 0 || Input.GetMouseButtonDown(0) && firstPos == Vector3.zero)
        {

            if(!EventSystem.current.IsPointerOverGameObject() || !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
                firstPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
        }


        shotable = true;
        //볼이 움직이고 있으면 쏠 수 없음
        for (int i = 0; i < BallGroup.childCount; i++)
            if (BallGroup.GetChild(i).GetComponent<Ball>().isMoving) shotable = false;

        //블럭이 내려올때 쏠 수 없음
        if (isBlockMoving) shotable = false;

        //쏠 수 없으면 실행안함
        if (!shotable) return;

        //
        if(shotTrigger && shotable)
        {
            shotTrigger = false;
            if (!ice)
                BlockGenerator();
            else
                ice = false;
            timeDelay = 0;


            for (int i = 0; i < GreenBallGroup.childCount; i++)
                StartCoroutine(GreenBallMove(GreenBallGroup.GetChild(i)));
            StartCoroutine(BallCountTextShow(GreenBallGroup.childCount));

            if (coinObtain == 3)
            {
                COcount++;
                if (COcount >= 5)
                { 
                    COcount = 0;
                    coinObtain = 1;
                }
            }

            if(shadow)
            {
                shadow = false;
                for(int i = 0; i < BallGroup.childCount / 2; i++)
                {
                    Destroy(BallGroup.GetChild(i).gameObject);
                }
                BallCountText.text = "x" + BallGroup.childCount.ToString();
            }
            if (gold)
                gold = false;
        }

        //
        timeDelay += Time.deltaTime;
        if (timeDelay < 0.1f) return;

        //입력
        bool isMouse = Input.GetMouseButton(0);
        if (Input.touchCount > 0 || isMouse)
        {
            if (!EventSystem.current.IsPointerOverGameObject() || !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                if (isGBallMove)
                {
                    isGBallMove = false;
                    for (int i = 0; i < GreenBallGroup.childCount; i++)
                        StartCoroutine(GreenBallMove(GreenBallGroup.GetChild(i)));
                    StartCoroutine(BallCountTextShow(GreenBallGroup.childCount));
                }

                secondPos = Camera.main.ScreenToWorldPoint(Input.mousePosition) + new Vector3(0, 0, 10);
                if ((secondPos - firstPos).magnitude < 1) return;

                gap = (secondPos - firstPos).normalized;
                gap = new Vector3(gap.y >= 0 ? gap.x : gap.x >= 0 ? 1 : -1, Mathf.Clamp(gap.y, 0.2f, 1), 0);

                Arrow.transform.position = veryFirstPos;
                Arrow.transform.rotation = Quaternion.Euler(0, 0, Mathf.Atan2(gap.y, gap.x) * Mathf.Rad2Deg);
                BallPreview.transform.position =
                    Physics2D.CircleCast(new Vector2(Mathf.Clamp(veryFirstPos.x, -54, 54), groundY), 1.7f, gap, 10000, 1 << LayerMask.NameToLayer("Wall") | 1 << LayerMask.NameToLayer("Block")).centroid;

                RaycastHit2D hit1 = Physics2D.Raycast(veryFirstPos, gap, 10000, 1 << LayerMask.NameToLayer("Wall"));

                MouseLR.SetPosition(0, firstPos);
                MouseLR.SetPosition(1, secondPos);
                BallLR.SetPosition(0, veryFirstPos);
                BallLR.SetPosition(1, (Vector3)hit1.point - gap * 1.5f);
            }
        }
        else
            isGBallMove = true;

        //터치중 일때 표시선 온
        BallPreview.SetActive(isMouse && !EventSystem.current.IsPointerOverGameObject() || Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId));
        Arrow.SetActive(isMouse&& !EventSystem.current.IsPointerOverGameObject() || Input.touchCount > 0 && !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId));

        //터치를 땟을때 초기화
        if (Input.touchCount > 0 || Input.GetMouseButtonUp(0))
        {
            Debug.Log("hi");

            if (!EventSystem.current.IsPointerOverGameObject() || !EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId))
            {
                Debug.Log("bye");
                if ((secondPos - firstPos).magnitude < 1) return;

                MouseLR.SetPosition(0, Vector3.zero);
                MouseLR.SetPosition(1, Vector3.zero);
                BallLR.SetPosition(0, Vector3.zero);
                BallLR.SetPosition(1, Vector3.zero);

                timerStart = true;
                veryFirstPos = Vector3.zero;
                firstPos = Vector3.zero;
            }
        }
    }

    private void FixedUpdate()
    {
        if(timerStart && ++timerCount == 3)
        {
            timerCount = 0;
            BallGroup.GetChild(launchIndex++).GetComponent<Ball>().Launch(gap);
            BallCountText.text = "x" + (BallGroup.childCount - launchIndex).ToString();
            if(launchIndex == BallGroup.childCount)
            {
                timerStart = false;
                launchIndex = 0;
                BallCountText.text = "";
            }
        }
    }

    public void BallAdd()
    {
        if (coin >= 1)
        {
            Transform TR = Instantiate(Greenball, new Vector3(Random.Range(-54f, 54f), groundY, 0), GameManager.Instance.QI).transform;
            TR.SetParent(GameObject.Find("CoinGroup").transform);
            coin--;
            CoinText.text = coin.ToString();
        }
    }

    IEnumerator BallCountTextShow(int greenBallCount)
    {
        BallCountTextObj.transform.position = new Vector3(Mathf.Clamp(veryFirstPos.x, -49.9f, 49.9f), -65, 0);
        BallCountText.text = "x" + BallGroup.childCount.ToString();

        yield return new WaitForSeconds(0.17f);

        if(greenBallCount != 0)
        {
            BallPlusTextObj.SetActive(true);
            BallPlusTextObj.transform.position = new Vector3(Mathf.Clamp(veryFirstPos.x, -49.9f, 49.9f), -47, 0);
            BallPlusText.text = "+" + greenBallCount.ToString();
            S_Plus.Play();

            yield return new WaitForSeconds(0.5f);
            BallPlusTextObj.SetActive(false);
        }
    }

    IEnumerator GreenBallMove(Transform TR)
    {
        Instantiate(p_ball, veryFirstPos, QI).transform.SetParent(BallGroup);
        float speed = (TR.position - veryFirstPos).magnitude / 12f;
        while(true)
        {
            yield return null;
            TR.position = Vector3.MoveTowards(TR.position, veryFirstPos, speed);
            if(TR.position == veryFirstPos) { Destroy(TR.gameObject); yield break; }
        }
    }

    void CameraRect()
    {
        //화면비 고정
        Camera camera = Camera.main;
        Rect rect = camera.rect;
        float scaleheight = ((float)Screen.width / Screen.height) / ((float)9 / 16);
        float scaleWidth = 1f / scaleheight;
        if (scaleheight < 1)
        {
            rect.height = scaleheight;
            rect.y = (1f - scaleWidth) / 2f;
        }
        else 
        {
            rect.width = scaleWidth;
            rect.x = (1f - scaleheight) / 2f;
        }
        camera.rect = rect;
    }

    public void CardEffect()
    {
        switch(cardID)
        {
            case 0:
                Debug.Log("아수라");
                int life = Random.Range(0, 999);
                if (life < 150)
                    Debug.Log("꽝");
                else if (life >= 150 || life < 400)
                    Instantiate(p_ball, veryFirstPos, QI).transform.SetParent(BallGroup);
                else if (life >= 400 || life < 900)
                {
                    for (int i = 0; i < 2; i++)
                        Instantiate(p_ball, veryFirstPos, QI).transform.SetParent(BallGroup);
                }
                else if (life >= 900 || life < 970)
                {
                    for (int i = 0; i < 3; i++)
                        Instantiate(p_ball, veryFirstPos, QI).transform.SetParent(BallGroup);
                }
                else if (life >= 970 || life < 999)
                {
                    for (int i = 0; i < 4; i++)
                        Instantiate(p_ball, veryFirstPos, QI).transform.SetParent(BallGroup);
                }
                else if (life == 999)
                {
                    for (int i = 0; i < 5; i++)
                        Instantiate(p_ball, veryFirstPos, QI).transform.SetParent(BallGroup);
                }
                BallCountText.text = "x" + BallGroup.childCount.ToString();
                break;
            case 1:
                ice = true;
                break;
            case 2:
                for(int i = 0; i < temp; i++)
                {
                    Instantiate(p_ball, veryFirstPos, QI).transform.SetParent(BallGroup);
                }
                BallCountText.text = "x" + BallGroup.childCount.ToString();
                shadow = true;
                break;
            case 3:
                coinObtain = 3;
                break;
            case 4:
                gold = true;
                break;
        }    
    }

    void Dragend()
    {
        
    }
}
