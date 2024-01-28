using Photon.Pun;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class Client : MonoBehaviour
{
    public PhotonView pv;
    public Button[] Buttons;
    public Game Game;
    public Transform[] Positions;
    public Transform Ball;
    public int BallPosition;
    public void QuesionVariantID(int variantID)
    {
        pv.RPC("PlayerAnswer", RpcTarget.MasterClient, variantID, PhotonNetwork.LocalPlayer.NickName);
    }

    [PunRPC]
    public void PlayerAnswer(bool iscorrect, int id)
    {
        if (iscorrect)
        {
            Game.Variants[id].color = Color.green;
        }
        else
        {
            Game.Variants[id].color = Color.red;
        }
        HideButtons();
    }
    private void HideButtons()
    {
        foreach (Button item in Buttons)
        {
            item.interactable = false;
        }
    }
    [PunRPC]
    public void MoveBallClient(int newspep)
    {
        Ball.position = Positions[newspep].position;
    }
}