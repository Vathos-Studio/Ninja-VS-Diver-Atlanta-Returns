using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine.SceneManagement;

public class RoomManager : MonoBehaviourPunCallbacks
{
    public static RoomManager Instance;

    void Awake()
    {
        // Evitar duplicados
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(this.gameObject);
    }

    void OnEnable()
    {
        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    void OnDisable()
    {
        SceneManager.sceneLoaded -= OnSceneLoaded;
    }

    void OnSceneLoaded(Scene scene, LoadSceneMode loadSceneMode)
    {
        // Escena de juego (ejemplo: buildIndex 1)
        if (scene.buildIndex == 1)
        {
            // Solo instanciar si el jugador no existe ya
            if (PhotonNetwork.LocalPlayer.TagObject == null)
            {
                GameObject player = PhotonNetwork.Instantiate("Pingu1", Vector3.zero, Quaternion.identity);

                // Guardar referencia del jugador para que no se vuelva a instanciar
                PhotonNetwork.LocalPlayer.TagObject = player;

                // Dar color aleatorio
                SpriteRenderer sr = player.GetComponent<SpriteRenderer>();
                if (sr != null)
                    sr.color = new Color(Random.value, Random.value, Random.value);
            }
        }
    }

    // -------------------------
    // Gestión de desconexión
    // -------------------------
    public override void OnLeftRoom()
    {
        // Cuando un jugador abandona la sala, volver al menú principal
        Debug.Log("Has salido de la sala, volviendo al menú");

        PhotonNetwork.LocalPlayer.TagObject = null;
        SceneManager.LoadScene(0); // Asume que la escena 0 es el menú

    }

    public void Disconnect()
    {
        if (PhotonNetwork.IsConnected)
        {
            PhotonNetwork.LeaveRoom();
            PhotonNetwork.Disconnect();
        }
    }

    public override void OnDisconnected(DisconnectCause cause)
    {
        Debug.Log("Desconectado: " + cause);
        SceneManager.LoadScene(0); // Volver al menú principal
    }
}