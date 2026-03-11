using Photon.Pun;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement; // Necesario para cargar escenas

public class ContadorFinJuego : MonoBehaviourPun
{
    public static ContadorFinJuego Instance;

    public float gameTime = 120f;
    public TextMeshProUGUI timerText;

    private double startTime;
    private bool ended = false;

    void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject); // destruye duplicados
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            startTime = PhotonNetwork.Time;
            photonView.RPC("SyncStartTime", RpcTarget.AllBuffered, startTime);
        }
    }

    void Update()
    {
        if (ended) return;

        if (startTime == 0) return;

        double elapsed = PhotonNetwork.Time - startTime;
        float remaining = Mathf.Max((float)(gameTime - elapsed), 0);

        timerText.text = Mathf.Ceil(remaining).ToString();

        if (remaining <= 0 && PhotonNetwork.IsMasterClient)
        {
            EndGame();
        }
    }

    [PunRPC]
    void SyncStartTime(double time)
    {
        startTime = time;
    }

    void EndGame()
    {
        ended = true;

        // Solo el MasterClient calcula el ganador
        if (PhotonNetwork.IsMasterClient)
        {
            Player[] players = FindObjectsOfType<Player>();
            Player winner = null;
            int maxPoints = -1;

            foreach (Player p in players)
            {
                if (p.points > maxPoints)
                {
                    maxPoints = p.points;
                    winner = p;
                }
            }

            string message;
            if (winner != null)
                message = "GANADOR: " + winner.NickName + "\nPuntos: " + maxPoints;
            else
                message = "EMPATE!";

            // Llamar RPC para que todos actualicen UI y posición
            photonView.RPC("ShowWinnerUI", RpcTarget.All, message);

            // Reiniciar juego desde MasterClient
            StartCoroutine(RestartGame());
        }
    }

    private IEnumerator RestartGame()
    {
        yield return new WaitForSecondsRealtime(5f); // usa WaitForSecondsRealtime porque el juego está pausado
        PhotonNetwork.LeaveRoom();
        PhotonNetwork.LocalPlayer.TagObject = null;
        Destroy(RoomManager.Instance.gameObject);
        SceneManager.LoadScene(0); // Asume que la escena 0 es el menú
    }

    [PunRPC]
    void ShowWinnerUI(string message)
    {
        // Actualizar texto
        timerText.text = message;

      
     
    }
}