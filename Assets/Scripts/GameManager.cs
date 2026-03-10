using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;

public class GameManager : MonoBehaviourPunCallbacks
{
    public static GameManager Instance;

    public Transform[] spawnPoints;
    public TMP_Text taggerText;

    bool gameStarted = false;

    public float matchTime = 300f; 
    float timer;    

    public TMP_Text timerText;

    public GameObject winScreen;
    public GameObject loseScreen;

    void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    void Start()
    {
        timer = matchTime;

        winScreen.SetActive(false);
        loseScreen.SetActive(false);
    }

    public override void OnJoinedRoom()
    {
        SpawnPlayer();

        if (PhotonNetwork.IsMasterClient)
            TryStartGame();
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (PhotonNetwork.IsMasterClient)
            TryStartGame();
    }

    void SpawnPlayer()
    {
        if (spawnPoints == null || spawnPoints.Length == 0) return;

        int index = Random.Range(0, spawnPoints.Length);
        PhotonNetwork.Instantiate(
            "Player",
            spawnPoints[index].position,
            Quaternion.identity
        );
    }

    void TryStartGame()
    {
        if (gameStarted) return;
        StartCoroutine(StartGameWhenReady());
    }

    IEnumerator StartGameWhenReady()
    {
        yield return new WaitUntil(() =>
            FindObjectsOfType<PlayerTag>().Length >= 2
        );

        yield return new WaitForSeconds(0.2f);
        PlayerTag[] players = FindObjectsOfType<PlayerTag>();

        foreach (var p in players)
        {
            p.photonView.RPC("SetTagger", RpcTarget.All, false);
        }

        int rand = Random.Range(0, players.Length);
        players[rand].photonView.RPC("SetTagger", RpcTarget.All, true);

        gameStarted = true;

        yield break;
    }

    void Update()
    {
        if (taggerText)
        {
            PlayerTag tagger = FindTagger();
            taggerText.text = tagger
                ? "Догоняльщик: " + tagger.photonView.Owner.NickName
                : "Ожидание игроков...";
        }


        if (!gameStarted) return;

        timer -= Time.deltaTime;

        int minutes = Mathf.FloorToInt(timer / 60);
        int seconds = Mathf.FloorToInt(timer % 60);

        timerText.text = minutes.ToString("00") + ":" + seconds.ToString("00");

        if (timer <= 0  && gameStarted)
        {
            EndGame();
        }
    }


    PlayerTag FindTagger()
    {
        foreach (var p in FindObjectsOfType<PlayerTag>())
            if (p.isTagger)
                return p;

        return null;
    }
    [PunRPC]
    public void RequestTagSwap(int oldTaggerViewID, int newTaggerViewID)
    {
        if (!PhotonNetwork.IsMasterClient) return;

        PhotonView oldPV = PhotonView.Find(oldTaggerViewID);
        PhotonView newPV = PhotonView.Find(newTaggerViewID);

        if (!oldPV || !newPV) return;

        PlayerTag oldTagger = oldPV.GetComponent<PlayerTag>();
        PlayerTag newTagger = newPV.GetComponent<PlayerTag>();

        if (!oldTagger || !newTagger) return;


        oldTagger.photonView.RPC("SetTagger", RpcTarget.All, false);
        newTagger.photonView.RPC("SetTagger", RpcTarget.All, true);
    }

    void EndGame()
    {
        gameStarted = false;

        PlayerTag tagger = FindTagger();

        if (tagger == null) return;

        if (tagger.photonView.IsMine)
            loseScreen.SetActive(true);
        else
            winScreen.SetActive(true);
    }


}
