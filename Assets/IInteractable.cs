using Photon.Pun;
using UnityEngine;

public class IInteractable : MonoBehaviour
{
    [SerializeField] private AudioClip audioclip;
    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            string prefabName = gameObject.name.Replace("(Clone)", "");

            InteractableManager.Instance.ObjectCollected(prefabName);

            AudioManager.PlayAudio(audioclip);
            PhotonNetwork.Destroy(gameObject);
        }
    }
}

