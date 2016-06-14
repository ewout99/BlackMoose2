using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Network;
using System.Collections;


public class ExtendedLobbyHook : LobbyHook {
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        IngamePlayer gameRef = gamePlayer.GetComponent<IngamePlayer>();
        LobbyPlayer lobbyRef = lobbyPlayer.GetComponent<LobbyPlayer>();
        gameRef.nameIngame = lobbyRef.playerName;
        gameRef.typeIngame = lobbyRef.playerSprite;
        gameRef.colorIngame = lobbyRef.playerColor;
        base.OnLobbyServerSceneLoadedForPlayer(manager, lobbyPlayer, gamePlayer);
    }
}
