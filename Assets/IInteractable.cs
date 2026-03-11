using Photon.Pun;
using UnityEngine;

public class IInteractable : MonoBehaviourPun
{
    [SerializeField] private AudioClip audioclip;
    bool gived = false;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (!other.CompareTag("Player")) return;

        PhotonView playerView = other.GetComponent<PhotonView>();

        // SOLO el dueño del jugador puede dar puntos
        if (!playerView.IsMine) return;

        if (!gived)
        {
            playerView.RPC("RPC_AddPoints", RpcTarget.All, 200);
            gived = true;
        }

        photonView.RPC("Collect", RpcTarget.MasterClient);
    }

    bool collected = false;

    [PunRPC]
    void Collect()
    {
        if (!PhotonNetwork.IsMasterClient) return;
        if (collected) return;

        collected = true;

        string prefabName = gameObject.name.Replace("(Clone)", "");

        InteractableManager.Instance.ObjectCollected(prefabName);

        PhotonNetwork.Destroy(gameObject);
    }
}

