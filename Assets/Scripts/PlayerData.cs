using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class PlayerData
{
    [System.Serializable]
    public class PlayerDataEntry
    {
        public int id;
        public string username;
        public string password;
        public float positionX;
        public float positionZ;
        public float velocityX;
        public float velocityZ;
        public int health;
        public int isOnline;

        public PlayerDataEntry()
        {
            this.id = 0;
            this.username = "";
            this.password = "";
            this.positionX = 0;
            this.positionZ = 0;
            this.velocityX = 0;
            this.velocityZ = 0;
            this.health = 0;
            this.isOnline = 0;
        }
    }

    public List<PlayerDataEntry> entries;

    public PlayerData()
    {
        entries = new List<PlayerDataEntry>();
    }
}

public enum LoginType
{
    signIn,
    createAccount,
    invalidPassword,
    isLoggedInAlready
}