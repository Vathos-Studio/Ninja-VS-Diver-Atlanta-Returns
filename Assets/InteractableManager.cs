using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class InteractableManager : MonoBehaviour
{
    public static InteractableManager Instance;

    [Header("Prefabs posibles")]
    public GameObject[] interactables;

    [Header("Límites del mapa")]
    public float minX;
    public float maxX;
    public float minY;
    public float maxY;

    
     int initialAmount = 200;

    void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        if (PhotonNetwork.IsMasterClient)
        {
            GenerateInitialObjects();
        }
    }

    void GenerateInitialObjects()
    {
        List<int> usados = new List<int>();

        initialAmount = interactables.Length;
        for (int i = 0; i < initialAmount; i++)
        {
            int index;

            do
            {
                index = Random.Range(0, interactables.Length);
            }
            while (usados.Contains(index));

            usados.Add(index);

            SpawnObject(interactables[index].name);
        }
    }

    void SpawnObject(string prefabname)
    {
        Vector2 pos = new Vector2(
            Random.Range(minX, maxX),
            Random.Range(minY, maxY)
        );

        PhotonNetwork.InstantiateRoomObject(prefabname, pos, Quaternion.identity);

    }


    public void ObjectCollected(string prefabname)
    {
        if (PhotonNetwork.IsMasterClient)
        {
            StartCoroutine(Respawn(prefabname));
        }
    }

    IEnumerator Respawn(string prefabname)
    {
        yield return new WaitForSeconds(10f);

        SpawnObject(prefabname);
    }


}
