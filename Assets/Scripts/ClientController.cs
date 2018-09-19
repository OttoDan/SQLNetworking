using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.UI;

public class ClientController : MonoBehaviour {
    public static ClientController Instance;

    string uri = "http://laienoper.de/SQLNetworking/";//"http://localhost/SQLNetworking/";

    [HideInInspector]
    public PlayerData playerData;


    public InputField nameInput;
    public InputField passwordInput;
    Coroutine loginRoutine;
    public Canvas loginCanvas;
    public Canvas logoutCanvas;

    Coroutine updateRoutine;
    public float updateFrequency = 0.5f;

    public Transform playerContainer;

    [HideInInspector]
    public int currentUserId = -1;
    [HideInInspector]
    public bool online = false;

    public void Awake()
    {
        if (Instance == null)
            Instance = this;
        else
            Destroy(gameObject);
    }


    public void GetPlayerDataDB(){
        StartCoroutine(UpdatePlayerDataDB());
    }

    IEnumerator UpdatePlayerDataDB(){
        UnityWebRequest www = UnityWebRequest.Get(uri + "playerData.php");
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
            Debug.LogError(www.error);
        else{
            Debug.Log(www.downloadHandler.text);

            //Parse json playerData
            playerData = JsonUtility.FromJson<PlayerData>(www.downloadHandler.text);


            //foreach (PlayerData.PlayerDataEntry entry in playerData.entries)
                //Debug.Log("Name: " + entry.username + "pos: " + entry.positionX + ", " + entry.positionZ);
        }
    }

    IEnumerator SwitchOnlineState(int id, int online){
        UnityWebRequest www = UnityWebRequest.Get(uri + "login.php?id="+id+"&isOnline="+online);
        yield return www.SendWebRequest();

        if (www.isNetworkError || www.isHttpError)
            Debug.LogError(www.error);
        else
        {
            Debug.Log(www.downloadHandler.text);
            if(www.downloadHandler.text == "Player Gone Online")
            {

            }
        }
    }



    IEnumerator LoginProcess(){
        yield return UpdatePlayerDataDB();

        int userId;
        LoginType loginType = ValidateLogin(nameInput.text, passwordInput.text, out userId);
        currentUserId = userId;
        switch (loginType)
        {
            case LoginType.signIn:
                Debug.Log("Password Correct user found - signing in.. User ID: " + userId);

                yield return SwitchOnlineState(userId, 1);
                PlayerSyncManager.Instance.PlacePlayers();
                GameManager.Instance.EnterGame(playerData.entries[userId - 1]);
                online = true;
                PlayerSyncManager.Instance.StartSync();
                loginCanvas.gameObject.SetActive(false);
                logoutCanvas.gameObject.SetActive(true);
                break;

            case LoginType.createAccount:
                Debug.Log("user not found creating new account..");
                break;

            case LoginType.invalidPassword:
                Debug.Log("Password not Correct user found - no login.");
                break;
            case LoginType.isLoggedInAlready:
                Debug.Log("is logged in already..");
                break;

            default:
                break;
        }
        loginRoutine = null;

    }
    IEnumerator LogoutProcess()
    {
        logoutCanvas.gameObject.SetActive(false);
        loginCanvas.gameObject.SetActive(true);
        yield return SwitchOnlineState(currentUserId, 0);
        Debug.Log("logging out id " + currentUserId);
        online = false;
        GameManager.Instance.BackToLoginScreen();
    }
    public void StartUpdateCoroutine(){
        if (updateRoutine != null)
            StopCoroutine(updateRoutine);
        updateRoutine = StartCoroutine(UpdatePlayer());
    }

    //IEnumerator updatePlayerPositions(){
    //    foreach(PlayerData.PlayerDataEntry entry in playerData.entries){
    //        if(entry.isOnline == 1)
    //        {
                
    //        }
    //    }
    //}

    IEnumerator UpdatePlayer()
    {
        yield return new WaitForSecondsRealtime(updateFrequency);
        while (GameManager.Instance.state == GameManager.State.WorldPlayer)
        {
            Transform playerT = GameManager.Instance.player.transform;
            UnityWebRequest www = UnityWebRequest.Get(uri + "updatePos.php?id=" + currentUserId + "&positionX=" + (int)playerT.position.x + "&positionZ=" + (int)playerT.position.z);
            yield return www.SendWebRequest();
            Debug.Log(uri + "updatePos.php?id=" + currentUserId + "&positionX=" + (int)playerT.position.x + "&positionZ=" + (int)playerT.position.z);

            if (www.isNetworkError || www.isHttpError)
                Debug.LogError(www.error);
            else
            {
                if (www.downloadHandler.text == "Position Updated!")
                {

                    Debug.Log(www.downloadHandler.text);
                }
            }
            yield return UpdatePlayerDataDB();

            yield return new WaitForSecondsRealtime(updateFrequency);
        }
        updateRoutine = null;
    }
    public LoginType ValidateLogin(string username, string password, out int id)
    {
        id = -1;
        foreach (PlayerData.PlayerDataEntry entry in ClientController.Instance.playerData.entries)
        {
            if (username == entry.username)
            {
                if (password == entry.password)
                {
                    id = entry.id;
                    if (entry.isOnline == 0)
                        return LoginType.signIn;
                    else if (entry.isOnline == 1)
                        return LoginType.isLoggedInAlready;
                    else
                        Debug.LogError("Something weird");
                }
                else
                    return LoginType.invalidPassword;
            }

        }
        id = -2;
        return LoginType.createAccount;
    }

    public void LoginButton()
    {
        if(loginRoutine == null)
            loginRoutine = StartCoroutine(LoginProcess());
    }

    public void LogoutButton()
    {
        StartCoroutine(LogoutProcess());
    }

}
