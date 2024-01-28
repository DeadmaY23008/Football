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
        AddConsoleMessage("������� ����������� � ������� ��������");
        PhotonNetwork.LocalPlayer.NickName = "�����" + Random.Range(0, 2);
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
        AddConsoleMessage("��������� � ������� ��������");
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
        AddConsoleMessage("������� ������� �������...");
        Search = false;
        RoomOptions roomOptions = new RoomOptions()
        {
            MaxPlayers = 2
        };
        RoomName = "������� " + Random.Range(0, 500);
        PhotonNetwork.CreateRoom(RoomName, roomOptions);
    }
    public override void OnCreatedRoom()
    {
        base.OnCreatedRoom();
        AddConsoleMessage("������� ������� " + RoomName);
        AddConsoleMessage("���� �������������");
    }
    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        base.OnCreateRoomFailed(returnCode, message);
        AddConsoleMessage("������ �������� ������� " + message);
    }
    public void JoinRoomRandom()
    {
        if (PhotonNetwork.InRoom)
        {
            AddConsoleMessage("�� ��� � ������� ��");
            return;
        }
        Search = !Search;
        if (Search)
        {
            AddConsoleMessage("������� ����� �������...");
            StartCoroutine(JoinRoomCoroutine());
        }
        else
        {
            AddConsoleMessage("����� ������� ��������");
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
        AddConsoleMessage("������������ � ������� " + room.Name);
        Search = false;
        serchTime = 0;
        TextSearchGame.text = string.Empty;
        Solo.gameObject.SetActive(true);
    }
    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        base.OnJoinRandomFailed(returnCode, message);
        AddConsoleMessage("����� ����... " + message);
    }
    public override void OnPlayerEnteredRoom(Photon.Realtime.Player newPlayer)
    {
        base.OnPlayerEnteredRoom(newPlayer);
        AddConsoleMessage("����: ����� �����������. �������� ����� 3 ���");
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
            TextSearchGame.text = "����� �������.. " + (int)serchTime + " ������ ������";
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