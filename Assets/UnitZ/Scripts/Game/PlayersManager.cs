
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class PlayersManager : NetworkBehaviour
{
	public float VersionCheckDelay = 5;
	[SyncVar (hook = "OnDataChanged")]
	public string PlayersData = "";
	public List<string> TeamsList = new List<string> ();
	public List<PlayerData> PlayerList = new List<PlayerData> ();
    //VersionCheckDelay：版本检查延迟，类型为浮点数，默认值为5。
	//PlayersData：玩家数据，标记为 [SyncVar] 表示它是一个同步变量，有一个名为 OnDataChanged 的钩子函数在其值改变时被调用。
	//TeamsList：队伍列表，存储了字符串类型的队伍名称。
	//PlayerList：玩家列表，存储了 PlayerData 类型的对象。
	void Start ()
	{

	}

	void Awake ()
	{
		ClearPlayers ();
	}


	public void ClearPlayers ()
	{
		PlayerList.Clear ();
		TeamsList.Clear ();
	}
//清除玩家数据，将 PlayerList 和 TeamsList 清空。
	public PlayerData GetPlayerData (int ID)
	{

		foreach (PlayerData player in PlayerList) {
			if (player.ID == ID) {
				return player;
			}
		}
		return new PlayerData ();
	}
//通过玩家ID获取玩家数据对象。
	public void AddPlayer (int id)
	{
		PlayerData play = new PlayerData ();
		play.Dead = 0;
		play.ID = id;
		play.Name = "";
		play.Team = "";
		PlayerList.Add (play);
	}
//添加新玩家到 PlayerList 中。
	private void AddTeam (string team)
	{
		if (TeamsList.Contains (team)) {
			return;
		}
		TeamsList.Add (team);
	}
//：添加队伍到 TeamsList 中。
	public void RemovePlayer (int id)
	{
		Debug.Log (id + " Removed");
		for (int i = 0; i < PlayerList.Count; i++) {
			if (id == PlayerList [i].ID) {
				PlayerList.RemoveAt (i);
				rewritePlayersData ();
				return;
			}
		}
	}
//：移除指定ID的玩家。
	public void UpdatePlayerInfo (int id, int score, int dead, string name, string team, string gameKey, bool isconnected)
	{
		bool have = false;
		foreach (PlayerData pp in PlayerList) {
			if (pp.ID == id) {
				have = true;
			}
		}
		if (!have) {
			AddPlayer (id);
		}

		PlayerData player = new PlayerData ();
		player.Dead = dead;
		player.ID = id;
		player.Name = name;
		player.Team = team;
		AddTeam (team);

		for (int i = 0; i < PlayerList.Count; i++) {
			if (id == PlayerList [i].ID) {
				PlayerList [i] = player;
				rewritePlayersData ();
				return;
			}
		}

	}
//更新玩家信息。
	public void AddScore (int id, int score, int dead)
	{

		for (int i = 0; i < PlayerList.Count; i++) {
			if (PlayerList [i].ID == id) {

				PlayerData player = new PlayerData ();
				player.Dead = PlayerList [i].Dead;
				player.ID = PlayerList [i].ID;
				player.Name = PlayerList [i].Name;
				player.Team = PlayerList [i].Team;
				player.Score = PlayerList [i].Score + score;
				player.Dead = PlayerList [i].Dead + dead;
				PlayerList [i] = player;

				rewritePlayersData ();
				return;
			}
		}
	}
//给指定ID的玩家添加得分和死亡次数。
	void rewritePlayersData ()
	{
		string idlist = "";
		string teamlist = "";
		string deadlist = "";
		string namelist = "";
		string scorelist = "";
		string allteamlist = "";

		foreach (PlayerData pp in PlayerList) {
			if (pp.ID != -1) {
				idlist += pp.ID + ",";
				teamlist += pp.Team + ",";
				scorelist += pp.Score + ",";
				deadlist += pp.Dead + ",";
				namelist += pp.Name + ",";
			}
		}
		foreach (string tt in TeamsList) {
			allteamlist += tt + ",";
		}

		PlayersData = idlist + "|" + teamlist + "|" + scorelist + "|" + deadlist + "|" + namelist + "|" + allteamlist;
	}
//此方法用于重新构建玩家数据字符串，以便将其发送到网络或存储在本地。它将玩家的ID、队伍、得分、死亡次数和名称等信息分别存储在不同的字符串中，并将它们连接在一起以构建完整的玩家数据字符串 PlayersData。
	public void ReadData (string data)
	{
		PlayerList.Clear ();
		TeamsList.Clear ();

		string[] dataget = data.Split ("|" [0]);
		if (dataget.Length >= 6) {
			string[] idget = dataget [0].Split ("," [0]);
			string[] teamget = dataget [1].Split ("," [0]);
			string[] scoreget = dataget [2].Split ("," [0]);
			string[] deadget = dataget [3].Split ("," [0]);
			string[] nameget = dataget [4].Split ("," [0]);
			string[] allteamget = dataget [5].Split ("," [0]);

			for (int i = 0; i < idget.Length; i++) {
				if (idget [i] != "") {
					PlayerData playerdata = new PlayerData ();
                    int.TryParse(idget[i], out playerdata.ID);
                    int.TryParse (scoreget [i], out playerdata.Score);
					int.TryParse (deadget [i], out playerdata.Dead);
					playerdata.Name = nameget [i];
					playerdata.Team = teamget [i];
					PlayerList.Add (playerdata);
				}
			}

			for (int i = 0; i < allteamget.Length; i++) {
				AddTeam (allteamget [i]);
			}
		}
	}
    //此方法用于从给定的数据字符串中读取玩家数据并更新 PlayerList 和 TeamsList。
	//它首先清空了 PlayerList 和 TeamsList，然后将给定的数据字符串根据分隔符 | 进行分割。
	//然后，它将分割后的数据分别解析成玩家的ID、队伍、得分、死亡次数和名称，并将它们存储在 PlayerData 结构体中。
	//最后，它将解析后的数据添加到 PlayerList 中，并调用 AddTeam() 方法将所有的队伍添加到 TeamsList 中。
	public void OnDataChanged (string data)
	{
		PlayersData = data;
		ReadData (data);
	}
    //这是一个与 SyncVar 注解中指定的钩子函数名称相对应的方法，当 PlayersData 的值发生改变时被调用。
	//它简单地将传入的数据字符串赋值给 PlayersData，然后调用 ReadData(string data) 方法来解析和更新玩家数据。

}



public struct PlayerData
{
	public int ID;
	public int Score;
	public string Team;
	public int Dead;
	public string Name;

}

