
using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class PlayerManager : MonoBehaviour
{
	[HideInInspector]
	public CharacterSystem PlayingCharacter;
	public float AutoRespawnDelay = 3;
	public bool AutoRespawn = false;
	public bool AskForRespawn = true;
	public float SaveInterval = 5;
	public bool SavePlayer = true;
	public bool SaveToServer = false;

	private SpectreCamera Spectre;
	private float timeTemp = 0;
	private bool savePlayerTemp;
	private float timeAlivetmp = 0;
	private bool autoRespawntmp = false;
	private bool askForRespawntmp = true;
	[HideInInspector]
	public float respawnTimer;
	//public CharacterSystem PlayingCharacter;：存储当前正在玩的角色。
	//public float AutoRespawnDelay = 3;：自动重生延迟时间，默认为3秒。
	//public bool AutoRespawn = false;：是否开启自动重生功能，默认为关闭状态。
	//public bool AskForRespawn = true;：是否询问玩家是否重生，默认为是。
	//public float SaveInterval = 5;：保存角色数据的时间间隔，默认为5秒。
	//public bool SavePlayer = true;：是否保存玩家数据，默认为是。
	//public bool SaveToServer = false;：是否保存到服务器，默认为否。
	//private SpectreCamera Spectre;：存储一个SpectreCamera对象的引用，用于处理玩家死亡后的观察效果。
	//private float timeTemp = 0;：存储上次保存角色数据的时间。
	//private bool savePlayerTemp; private float timeAlivetmp = 0; private bool autoRespawntmp = false;private bool askForRespawntmp = true;：存储保存状态和重生状态的临时变量。

	void Start ()
	{
		savePlayerTemp = SavePlayer;
		autoRespawntmp = AutoRespawn;
		askForRespawntmp = AskForRespawn;
	}
//在对象启用时调用的方法，保存初始状态。


	public void Reset ()
	{
		SavePlayer = savePlayerTemp;
		AutoRespawn = autoRespawntmp;
		AskForRespawn = askForRespawntmp;
	}
//
	void Update ()
	{
		OnPlaying ();
	}
//在每一帧更新时调用的方法，用于执行玩家角色管理的逻辑。

	public void Respawn (int spawner)
	{
		if (PlayingCharacter && !PlayingCharacter.IsAlive) {
			PlayingCharacter.ReSpawn (spawner);
		}
	}

	public void Respawn (byte team, int spawner)
	{
		if (PlayingCharacter && !PlayingCharacter.IsAlive) {
			PlayingCharacter.ReSpawn (team, spawner);
		}
	}
//重生玩家角色的方法，根据给定的生成点重新生成玩家角色。


	public void OnPlaying ()
	{
		if (PlayingCharacter) {
			if (UnitZ.playerSave && PlayingCharacter.IsAlive) {
				if (Time.time >= timeTemp + SaveInterval) {
					timeTemp = Time.time;
					if (SavePlayer) {
						PlayingCharacter.SaveCharacterData (SaveToServer);
						//Debug.Log ("Chaacter saved ("+Time.timeSinceLevelLoad+")");
					}
				}
				if (UnitZ.Hud.IsPanelOpened ("Afterdead")) {
					UnitZ.Hud.ClosePanelByName ("Afterdead");
				}
				timeAlivetmp = Time.time;
			}
			if (!PlayingCharacter.IsAlive && AutoRespawn) {
				respawnTimer = (timeAlivetmp + AutoRespawnDelay) - Time.time;

				if (Time.time > timeAlivetmp + AutoRespawnDelay) {
					Respawn (-1);
					UnitZ.Hud.ClosePanelByName ("Afterdead");
					Debug.Log ("Chaacter respawned ("+Time.timeSinceLevelLoad+")");
				}
			}
			if (!PlayingCharacter.IsAlive && AskForRespawn) {
				MouseLock.MouseLocked = false;
				if (!UnitZ.Hud.IsPanelOpened ("Afterdead")) {
					UnitZ.Hud.OpenPanelByName ("Afterdead");
				}
			}

		} else {
			findPlayerCharacter ();
			if (PlayingCharacter == null) {
				//Debug.LogWarning ("Can't find player (CharacterSystem) object in the scene. this is may drain game performance (" + Time.timeSinceLevelLoad + ")");
			}
		} 
		
		if (Spectre != null && PlayingCharacter) {
			if (!PlayingCharacter.IsAlive) {
				Spectre.Active (true);
			} else {
				Spectre.Active (false);	
				Spectre.LookingAt (PlayingCharacter.gameObject.transform.position);
				PlayingCharacter.spectreThis = true;
			}
		} else {
			
			Spectre = (SpectreCamera)GameObject.FindObjectOfType (typeof(SpectreCamera));	
			if (Spectre == null) {
				//Debug.LogWarning ("Can't find (SpectreCamera) object in the scene. this is may drain game performance (" + Time.timeSinceLevelLoad + ")");
			}
		}
	}
//处理当前正在玩的玩家角色的逻辑，包括保存数据、自动重生、询问重生等操作。
	private void findPlayerCharacter ()
	{
		if (PlayingCharacter == null) {
			CharacterSystem[] go = (CharacterSystem[])GameObject.FindObjectsOfType (typeof(CharacterSystem));
			for (int i = 0; i < go.Length; i++) {
				CharacterSystem character = go [i];
				if (character) {
					if (character.IsMine) {
						PlayingCharacter = character;
						PlayingCharacter.LoadCharacterData (SaveToServer);
					}
				}
			}
		}
	}
//查找当前玩家角色的方法。


	public Vector3 FindASpawnPoint (int spawner)
	{
		Vector3 spawnposition = Vector3.zero;
		PlayerSpawner[] spawnPoint = (PlayerSpawner[])GameObject.FindObjectsOfType (typeof(PlayerSpawner));
		
		if (spawner < 0 || spawner >= spawnPoint.Length) {
			spawner = Random.Range (0, spawnPoint.Length);
		}
		if (spawnPoint.Length > 0) {
			spawnposition = spawnPoint [spawner].transform.position;
		}

		return spawnposition;
	}
//查找生成点位置的方法。
	public GameObject InstantiatePlayer (int playerID, string userID, string userName, string characterKey, int characterIndex, byte team, int spawner)
	{
		if (UnitZ.characterManager == null || UnitZ.characterManager.CharacterPresets.Length <= characterIndex || characterIndex < 0)
			return null;

		CharacterSystem characterspawn = UnitZ.characterManager.CharacterPresets [characterIndex].CharacterPrefab;

		if (characterspawn) {

			GameObject player = (GameObject)GameObject.Instantiate (characterspawn.gameObject, FindASpawnPoint (spawner), Quaternion.identity);
			CharacterSystem character = player.GetComponent<CharacterSystem> ();
			character.NetID = playerID;
			character.Team = team;
			character.CharacterKey = characterKey;
			character.UserName = userName;
			character.UserID = userID;
			MouseLock.MouseLocked = true;
			return player;
		}
		return null;
	}

}
//生成玩家角色的方法。