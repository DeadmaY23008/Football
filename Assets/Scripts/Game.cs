using Photon.Pun;
using Photon.Realtime;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Game : MonoBehaviour
{
    public List<Qustions> Data = new List<Qustions>();
    public PhotonView pv;
    public TextMeshProUGUI QuestionText;
    public TextMeshProUGUI[] Variants;
    public Button[] VariantButtons;
    public TextMeshProUGUI Score;
    public Transform ContentPage;
    public List<Player> PlayerList = new List<Player>();
    public Client Client;
    public int CurrentQuestionID;
    public bool GameStarted = false;
    private float timer = 3f;
    public TextMeshProUGUI TimerText;
    public int BallPosition = 2;

    private void Start()
    {
        foreach (Player player in PhotonNetwork.PlayerList)
        {
            PlayerList.Add(player);
        }
        UpdateScore();

        if (PhotonNetwork.IsMasterClient)
        {
            StartGame();
        }
    }
    [PunRPC]
    private void UpdateScore()
    {
        if (PlayerList.Count == 2)
        {
            Score.text = PlayerList[0].NickName + " ����" + PlayerList[0].Score + " " + PlayerList[1]?.NickName + " ����" + PlayerList[1]?.Score;
        }
        else
        {
            Score.text = "���� �� ������ ��� ���� � ����";
        }
    }

    private void StartGame()
    {
        CurrentQuestionID = Random.Range(0, Data.Count);
        pv.RPC("NextQuestionClient", RpcTarget.All, CurrentQuestionID);
        GameStarted = true;
    }

    [PunRPC]
    private void NextQuestionClient(int seed)
    {
        QuestionText.text = Data[seed].Qustion;

        foreach (Button item in VariantButtons)
        {
            item.interactable = true;
        }

        Variants[0].text = Data[seed].Variants[0];
        Variants[1].text = Data[seed].Variants[1];
        Variants[2].text = Data[seed].Variants[2];
        Variants[3].text = Data[seed].Variants[3];

        Variants[0].color = Color.white;
        Variants[1].color = Color.white;
        Variants[2].color = Color.white;
        Variants[3].color = Color.white;
    }

    private void Update()
    {
        Master();
        TimerText.text = "����� �� ����� " + timer;
        if (timer >= 0)
        {
            timer -= Time.deltaTime;
        }
    }
    [PunRPC]
    public void Timer(float time)
    {
        timer = time;
    }
    private void Master()
    {
        if (PhotonNetwork.IsMasterClient == false)
        {
            return;
        }
        if (GameStarted)
        {
            if (timer <= 0)
            {
                MoveBall();
                StartGame();
                pv.RPC("Timer", RpcTarget.All, 10f);
                timer = 10f;
            }
        }
    }

    private void MoveBall()
    {
        if (PlayerList.Count == 2)
        {
            if (PlayerList[0].isAnswered == false & PlayerList[1].isAnswered == false)
            {
                return;
            }
            if (PlayerList[0].isAnswered == true & PlayerList[1].isAnswered == true)
            {
                return;
            }
            if (PlayerList[0].isAnswered == true & PlayerList[1].isAnswered == false)
            {
                BallPosition += 1;
            }
            if (PlayerList[1].isAnswered == true & PlayerList[0].isAnswered == false)
            {
                BallPosition -= 1;
            }

            BallPositionIsInfinity();

            pv.RPC("MoveBallClient", RpcTarget.All, BallPosition);

            PlayerList[0].isAnswered = false;
            PlayerList[1].isAnswered = false;
        }
        else
        {
            if (PlayerList[0].isAnswered == false)
            {
                return;
            }
            if (PlayerList[0].isAnswered == true)
            {
                BallPosition += 1;

                BallPositionIsInfinity();

                pv.RPC("MoveBallClient", RpcTarget.All, BallPosition);
            }
            PlayerList[0].isAnswered = false;
        }
        UpdateScore();
    }
    private void BallPositionIsInfinity()
    {
        if (PlayerList.Count == 2)
        {
            if (BallPosition > 4)
            {
                PlayerList[1].Score++;
                BallPosition = 2;
            }
            if (BallPosition < 0)
            {
                PlayerList[0].Score++;
                BallPosition = 2;
            }
        }
        else
        {
            if (BallPosition > 4)
            {
                PlayerList[0].Score++;
                BallPosition = 2;
            }
        }
    }
    [PunRPC]
    public void PlayerAnswer(int id, string nick)
    {
        Player player = null;
        foreach (Player item in PlayerList)
        {
            if (item.NickName == nick)
            {
                player = item;
            }
        }
        if (player == null)
        {
            Debug.Log("Bug!!");
            return;
        }
        if (Data[CurrentQuestionID].Correct == id)
        {
            pv.RPC("PlayerAnswer", player, true, id);
            player.isAnswered = true;
            return;
        }
        pv.RPC("PlayerAnswer", player, false, id);
        player.isAnswered = false;
    }
}
[System.Serializable]
public class Qustions
{
    public string Qustion;
    public string[] Variants;
    public int Correct;
}