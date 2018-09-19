using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerSyncManager : MonoBehaviour {

    public static PlayerSyncManager Instance;
    public GameObject playerPrefab;
    public float syncFrequency = 0.5f;
    Coroutine syncCoroutine;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }

    public  void PlacePlayers()
    {
        foreach (PlayerData.PlayerDataEntry entry in ClientController.Instance.playerData.entries)
        {
            if (entry.isOnline == 1)
            {
                GameObject newPlayer = Instantiate(playerPrefab,transform);
                newPlayer.transform.position = entry.positionX * Vector3.right + entry.positionZ * Vector3.forward + Vector3.up;
                newPlayer.transform.name = "ID: " + entry.id + "Name: " + entry.username;
                //if (entry.id == ClientController.Instance.currentUserId)
                //newPlayer.AddComponent<PlayerController>();

            }
        }
    }

    public PlayerController PlaceClientPlayer(PlayerData.PlayerDataEntry entry){
        GameObject newPlayer = Instantiate(playerPrefab, transform);
        newPlayer.transform.position = entry.positionX * Vector3.right + entry.positionZ * Vector3.forward + Vector3.up;
        newPlayer.transform.name = "ID: " + entry.id + "Name: " + entry.username;
        //if (entry.id == ClientController.Instance.currentUserId)
        return newPlayer.AddComponent<PlayerController>();
    }

    public void StartSync(){

        if (syncCoroutine != null)
            StopCoroutine(syncCoroutine);

        syncCoroutine = StartCoroutine(UpdatePositions());
    }

    IEnumerator UpdatePositions()
    {
        while(ClientController.Instance.online)
        {
            foreach(Transform child in transform)
            {
                if(child != GameManager.Instance.player.transform)
                {
                    PlayerData.PlayerDataEntry entry = ClientController.Instance.playerData.entries[child.GetSiblingIndex()];
                    child.position = Vector3.right * entry.positionX + Vector3.forward * entry.positionZ;
                    Debug.Log("updated position of " + entry.username);
                    //yield return null;
                }
                yield return new WaitForSecondsRealtime(syncFrequency);
            }
        }

        syncCoroutine = null;
    }
}
