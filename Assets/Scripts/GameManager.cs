using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour {

    public static GameManager Instance; 
    public enum State {
        LoginScreen,
        WorldPlayer,
        WorldSpectator
    }

    public State state = State.LoginScreen;
    [HideInInspector]
    public PlayerController player;
    public GameObject playerPrefab;
    public Transform playerContainer;

    private void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);

        DontDestroyOnLoad(this);

        switch(state){
            case State.LoginScreen:
                {
                    ClientController.Instance.GetPlayerDataDB();

                    break;
                }


            case State.WorldPlayer:
                {
                    break;
                }
            case State.WorldSpectator:
                {

                    break;
                }
        }
    }

    public void EnterGame(PlayerData.PlayerDataEntry data){
        InsertPlayer(data);
        state = State.WorldPlayer;
    }

    public void BackToLoginScreen()
    {
        player.Kill();
        state = State.LoginScreen;
    }

    void InsertPlayer(PlayerData.PlayerDataEntry data){
        if (player != null)
            Destroy(player);

        player = PlayerSyncManager.Instance.PlaceClientPlayer(data);

        //Transform playerT = Instantiate(playerPrefab, playerContainer).transform;

        //playerT.position = data.positionX * Vector3.right + data.positionZ * Vector3.forward;

        //playerT.name = "ID: " + data.id + "Name: " + data.username;

        //player = PlayerSyncManager.Instance.transform.GetChild(data.id-1).gameObject.AddComponent<PlayerController>();



    }
}
