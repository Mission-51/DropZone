using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Photon.Pun;

public class PlayerInfoBasic : MonoBehaviourPun
{
    public TMP_Text userNick;

    void Awake()
    {
        if (photonView.IsMine)
        {
            userNick.text = PhotonNetwork.NickName;
        }
        else
        {
            userNick.text = photonView.Owner.NickName;
        }
    }
}
