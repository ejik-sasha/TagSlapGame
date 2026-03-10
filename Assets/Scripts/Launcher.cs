using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;
using TMPro;
using System.Collections;

public class Launcher : MonoBehaviourPunCallbacks
{
    public TMP_InputField playerNameInput;
    public Button playButton;
    public TMP_Text statusText;

    string gameVersion = "1.0";
    bool connecting = false;

    void Start()
    {
        PhotonNetwork.GameVersion = gameVersion;
        PhotonNetwork.AutomaticallySyncScene = true;

        playButton.onClick.AddListener(Connect);
        statusText.text = "Введи имя и нажми Play";
    }

    void Connect()
    {
        if (connecting) return;
        connecting = true;

        PhotonNetwork.NickName =
            string.IsNullOrEmpty(playerNameInput.text)
            ? "Player"
            : playerNameInput.text;

        playButton.interactable = false;
        statusText.text = "Подключение...";

        PhotonNetwork.ConnectUsingSettings();
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        RoomOptions options = new RoomOptions
        {
            MaxPlayers = 10,
            IsOpen = true,
            IsVisible = true
        };

        PhotonNetwork.CreateRoom(null, options);
    }

    public override void OnJoinedRoom()
    {
        statusText.text = "Комната найдена. Загрузка...";
        StartCoroutine(LoadGame());
    }

    IEnumerator LoadGame()
    {
        yield return new WaitForSeconds(1f);
        PhotonNetwork.LoadLevel("Game");
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        statusText.text = $"Ошибка: {cause}";
        playButton.interactable = true;
        connecting = false;
    }
}
