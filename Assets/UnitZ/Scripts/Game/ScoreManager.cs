
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class ScoreManager : NetworkBehaviour
{

	public bool Toggle;
	private GUIKillBadgeManager guiBadgeManager;
//GUIKillBadgeManager 类型的变量，用于管理击杀徽章的显示。
	void Start ()
	{
		guiBadgeManager = (GUIKillBadgeManager)GameObject.FindObjectOfType (typeof(GUIKillBadgeManager));

	}
//在开始时，该方法会查找场景中的 GUIKillBadgeManager 对象，并将其赋值给 guiBadgeManager 变量。
	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Tab)) {
			if (UnitZ.gameManager)
				Toggle = !Toggle;
		}
	}
//每帧调用的方法，检查是否按下了 Tab 键。
//如果按下了 Tab 键，并且 UnitZ.gameManager 不为空，则切换 Toggle 变量的状态。
	public void UpdatePlayerScore (int id, int score, int dead)
	{
		if (!isServer || UnitZ.NetworkObject () == null || UnitZ.NetworkObject ().playersManager == null)
			return;

		UnitZ.NetworkObject ().playersManager.AddScore(id,score,dead);
	}
    //当服务器上的玩家得分信息需要更新时调用的方法。
	//首先检查当前是否在服务器上，如果不是或者相关的网络对象或玩家管理器为空，则直接返回。
	//否则，通过 UnitZ.NetworkObject() 获取网络对象，并调用其 playersManager 的 AddScore 方法来更新玩家的得分和死亡次数。
	public void AddScore (int score, int id)
	{
		UpdatePlayerScore (id, score, 0);	
	}
	//添加指定玩家的得分。
	//调用 UpdatePlayerScore 方法，将指定玩家的得分增加 score，死亡次数为0。
	public void AddDead (int dead, int id)
	{
		UpdatePlayerScore (id, 0, dead);
	}
	//添加指定玩家的死亡次数。
	//同样调用 UpdatePlayerScore 方法，将指定玩家的死亡次数增加 dead，得分为0。
	public void AddKillText (int killer, int victim, string killtype)
	{
		if(guiBadgeManager == null)
			guiBadgeManager = (GUIKillBadgeManager)GameObject.FindObjectOfType (typeof(GUIKillBadgeManager));

		if (UnitZ.NetworkObject () == null || UnitZ.NetworkObject ().playersManager == null)
			return;
		
		PlayersManager playersManager = UnitZ.NetworkObject ().playersManager;

		if (guiBadgeManager != null && playersManager) {
			//Debug.Log("add killer22");
			PlayerData killerData = playersManager.GetPlayerData (killer);
			PlayerData victimData = playersManager.GetPlayerData (victim);
			string killername = "N/A";
			string victimname = "N/A";

			if (killerData.Name != "") {
				killername = killerData.Name;
			}
			if (victimData.Name != "") {
				victimname = victimData.Name;
			}

			guiBadgeManager.PushKillText (killername, victimname, killtype);
		}
	}
	//添加击杀信息，并在界面上显示相应的徽章或文本。
	//首先检查 guiBadgeManager 是否为空，如果为空则尝试通过查找获取。
	//然后检查当前的网络对象和玩家管理器是否有效。
	//如果 guiBadgeManager 不为空且网络对象及玩家管理器有效，则获取击杀者和被击杀者的玩家数据。
	//将击杀者和被击杀者的名称获取出来，如果名称为空则使用 "N/A" 代替。
	//最后调用 guiBadgeManager 的 PushKillText 方法，将击杀者、被击杀者和击杀类型传递进去，用于显示击杀信息。


}
