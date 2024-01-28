using Photon.Pun;
using Photon.Realtime;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Menu : MonoBehaviourPunCallbacks
{
    private bool Search;
    private float serchTime;
    public TextMeshProUGUI TextSearchGame;
    public TextMeshProUGUI ConsoleMenu;
    public Button[] MenuButtons;
    private string RoomName;
    public Button Solo;
    public void AddConsoleMessage(string message)
    {
        ConsoleMenu.text += "\n" + message;
    }
    private void Start()
    {
        AddConsoleMessage("Попытка подключение к мастеру серверов");
        PhotonNetwork.LocalPlayer.NickName = "Игрок" + Random.Range(0, 2);
        PhotonNetwork.AutomaticallySyncScene = true;
        foreach (Button item in MenuButtons)
        {
            item.interactable = false;
        }
        PhotonNetwork.ConnectUsingSettings();
    }
    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        AddConsoleMessage("Подключен к мастеру серверов");
        DisplayButtons();
    }
    private void DisplayButtons()
    {
        foreach (Button item in MenuButtons)
        {
            item.interactable = true;
        }
    }
    public void CreateRoom()
    {
        AddConsoleMessage("Попытка создать комнату...");
        Search = false;
        RoomOptions roomOptions = new RoomOptions()
        {
            MaxPlayers = 2
        };
        RoomName = "Комната " + Random.Range(0, 500);
        PhotonNetwork.CreateRoom(RoomName, roomOptions);
    }
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        AddConsoleMessage("Комната создана " + RoomName);
        AddConsoleMessage("Ждем пользователей");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        AddConsoleMessage("Ошибка создание комнаты " + message);
    }
    public void JoinRoomRandom()
    {
        if (PhotonNetwork.InRoom)
        {
            AddConsoleMessage("Ты уже в комнате алё");
            return;
        }
        Search = !Search;
        if (Search)
        {
            AddConsoleMessage("Попытка найти комнату...");
            StartCoroutine(JoinRoomCoroutine());
        }
        else
        {
            AddConsoleMessage("Поиск комнаты отключен");
            StopCoroutine(JoinRoomCoroutine());
            serchTime = 0;
            TextSearchGame.text = string.Empty;
        }
    }
    private IEnumerator JoinRoomCoroutine()
    {
        while (!PhotonNetwork.InRoom)
        {
            yield return new WaitForSecondsRealtime(1f);
            PhotonNetwork.JoinRandomRoom();
        }
    }
    public override void OnJoinedRoom()
    {
        base.OnJoinedRoom();
        Room room = PhotonNetwork.CurrentRoom;
        AddConsoleMessage("Подключились к комнате " + room.Name);
        Search = false;
        serchTime = 0;
        TextSearchGame.text = string.Empty;
        Solo.gameObject.SetActive(true);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        AddConsoleMessage("Поиск игры... " + message);
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        AddConsoleMessage("Хост: Игрок подключился. Начинаем через 3 сек");
        StartCoroutine(ChangeLevel(3f));
    }
    public void PlaySolo()
    {
        StartCoroutine(ChangeLevel(0f));
    }
    private void Update()
    {
        if (Search)
        {
            serchTime += Time.deltaTime;
            TextSearchGame.text = "Поиск комнаты.. " + (int)serchTime + " секунд прошло";
        }
    }
    public void ApplicationQuit()
    {
        Application.Quit();
    }
    private IEnumerator ChangeLevel(float value)
    {
        yield return new WaitForSecondsRealtime(value);
        PhotonNetwork.LoadLevel(1);
    }
}