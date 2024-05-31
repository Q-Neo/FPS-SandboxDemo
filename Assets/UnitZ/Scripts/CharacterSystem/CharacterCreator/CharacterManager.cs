using UnityEngine;
using System.Collections;

public class CharacterManager : MonoBehaviour
{
	// this is chracter preset index.
	public int CharacterIndex = 0;
	// this is chracter preset.
	public CharacterPreset[] CharacterPresets;
	private CharacterSaveData characterCreate;
	public CharacterSaveData SelectedCharacter;
	public GameObject CharacterSelected;
	//CharacterIndex：角色预设的索引。
	//CharacterPresets：角色预设数组。
	//characterCreate：用于创建新角色的临时数据。
	//SelectedCharacter：当前选择的角色信息。
	//CharacterSelected：当前选择的角色对象。
	void Start ()
	{
		for (int i = 0; i < CharacterPresets.Length; i++) {
			UnitZ.gameNetwork.spawnPrefabs.Add (CharacterPresets [i].CharacterPrefab.gameObject);
		}
	}
	//在对象启动时调用，将所有角色预设的游戏对象添加到游戏网络的生成预设列表中。
	public bool CreateCharacter (string characterName)
	{
		// create new character
		if (UnitZ.gameManager) {
			CreateResult res = new CreateResult ();
			if (characterName != "") {
				// get new Chracter Key
				res = SaveNewCharacter (characterName);
				if (res.IsSuccess) {
					// if passed
					UnitZ.gameManager.UserName = characterName;
					UnitZ.gameManager.CharacterKey = res.CharacterKey;
					return true;
				}
			}
		}
		return false;
	}
	//创建新角色，并保存角色信息。

	public void SetCharacter ()
	{
		// when you select a character from the list.
		// you will get character preset index.
		CharacterIndex = SelectedCharacter.CharacterIndex;
		
		if (CharacterPresets.Length > 0) {
			// just chack if CharacterIndex is correct.
			if (CharacterIndex >= CharacterPresets.Length)
				CharacterIndex = CharacterPresets.Length - 1;
			
			if (CharacterIndex < 0)
				CharacterIndex = 0;
			
			// Set UserName and CharacterKey
			if (UnitZ.gameManager) {
				UnitZ.gameManager.UserName = SelectedCharacter.PlayerName;
				UnitZ.gameManager.CharacterKey = SelectedCharacter.CharacterKey;
				CharacterSelected = UnitZ.characterManager.CharacterPresets [CharacterIndex].CharacterPrefab.gameObject;
			}
		}
	}
//设置当前选择的角色，更新游戏管理器中的用户名和角色键。
	public void SetupCharacter (CharacterSaveData character)
	{
		// when you select a character from the list.
		// you will get character preset index.
		SelectedCharacter = character;
		CharacterIndex = SelectedCharacter.CharacterIndex;
		
		if (CharacterPresets.Length > 0) {
			// just chack if CharacterIndex is correct.
			if (CharacterIndex >= CharacterPresets.Length)
				CharacterIndex = CharacterPresets.Length - 1;
			
			if (CharacterIndex < 0)
				CharacterIndex = 0;
			
			// Set UserName and CharacterKey
			if (UnitZ.gameManager) {
				UnitZ.gameManager.UserName = SelectedCharacter.PlayerName;
				UnitZ.gameManager.CharacterKey = SelectedCharacter.CharacterKey;
				CharacterSelected = UnitZ.characterManager.CharacterPresets [CharacterIndex].CharacterPrefab.gameObject;
			}
		}
	}
//设置选择的角色信息，并更新游戏管理器中的用户名和角色键。
	public void SelectCreateCharacter (int index)
	{
		characterCreate = new CharacterSaveData ();
		characterCreate.CharacterIndex = index;
	}
//选择要创建的角色，设置角色预设的索引。
	public CreateResult SaveNewCharacter (string characterName)
	{
		// create new character and save.
		CreateResult res = new CreateResult ();
		res.IsSuccess = false;
		if (characterName != "" && UnitZ.gameManager && UnitZ.playerSave) {
			characterCreate.PlayerName = characterName;
			res = UnitZ.playerSave.CreateCharacter (characterCreate);
		}
		return res;
	}
//保存新创建的角色信息
	public void RemoveCharacter (CharacterSaveData character)
	{
		if (UnitZ.playerSave) {
			UnitZ.playerSave.RemoveCharacter (character);
		}
	}
}
//删除角色信息。
