using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Network;
using System.Collections;


public class ExtendedLobbyHook : LobbyHook {
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        LobbyPlayer lobbyRef = lobbyPlayer.GetComponent<LobbyPlayer>();
        IngamePlayer gameRef = gamePlayer.GetComponent<IngamePlayer>();

        // Implement lobby player selection load the right Animation Controller
        // And load the right weapons sprite


        gameRef.nameIngame = lobbyRef.playerName;
        gameRef.typeIngame = lobbyRef.playerSprite;

        //gameRef.colorIngame = lobbyRef.playerColor;
        Debug.Log("Color and name set");

        base.OnLobbyServerSceneLoadedForPlayer(manager, lobbyPlayer, gamePlayer);
    }
}
