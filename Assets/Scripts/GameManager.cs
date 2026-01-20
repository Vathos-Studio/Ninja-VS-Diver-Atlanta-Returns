using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class GameManager : MonoBehaviour
{
    bool pingu2chosen = false;
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Instantiate("Pingu1", new Vector3(3, 1, 0), Quaternion.identity);
        else
            
            PhotonNetwork.Instantiate("Pingu2", new Vector3(-3, 1, 0), Quaternion.identity);
            
    }
}
