using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using Photon.Pun;

public class GameManager : MonoBehaviour
{
    
    private void Start()
    {
        if (PhotonNetwork.IsMasterClient)
            PhotonNetwork.Instantiate("Pingu1", new Vector3(0, 0, 0), Quaternion.identity);
        else
            
            PhotonNetwork.Instantiate("Pingu2", new Vector3(60, 0, 0), Quaternion.identity);
            
    }
}
