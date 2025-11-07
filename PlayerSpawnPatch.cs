using HarmonyLib;
using Photon.Pun;
using System;
using System.Collections.Generic;
using UnityEngine;

[HarmonyPatch(typeof(LevelGenerator), "PlayerSpawn")]
public static class PlayerSpawnPatch
{
	// Track which players already received a pan this level (handles re-calls of PlayerSpawn)
	private static readonly HashSet<string> SpawnedForSteamIds = new HashSet<string>();

	static void Postfix()
	{
		// Avoid spawning in menus/lobby
		if (SemiFunc.MenuLevel() || GameDirector.instance == null || StatsManager.instance == null)
		{
			return;
		}

		// Only run on actual level scenes
		if (!SemiFunc.RunIsLevel() || RunManager.instance == null)
		{
			return;
		}

		Item panItem = FindPanItem();
		if (panItem == null || panItem.prefab == null || (!panItem.prefab.IsValid()))
		{
			Debug.LogWarning("[PlayerSpawnPatch] Could not find a Frying Pan item to spawn.");
			return;
		}

		foreach (var player in GameDirector.instance.PlayerList)
		{
			if (player == null)
				continue;

			Vector3 pos = player.transform.position + player.transform.right * 0.75f + Vector3.up * 0.75f;
			Quaternion rot = player.transform.rotation;

			try
			{
				if (GameManager.instance != null && GameManager.instance.gameMode != 0)
				{
					// Multiplayer: spawn networked room object
					PhotonNetwork.InstantiateRoomObject(panItem.prefab.ResourcePath, pos, rot, 0);
				}
				else
				{
					// Singleplayer: local instantiate
					UnityEngine.Object.Instantiate(panItem.prefab.Prefab, pos, rot);
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("[PlayerSpawnPatch] Failed to spawn Frying Pan: " + ex.Message);
			}
		}
	}

	private static Item FindPanItem()
	{
		var dict = StatsManager.instance.itemDictionary;
		if (dict.TryGetValue("Item Melee Frying Pan", out Item item))
		{
			return item;
		}
		return null;
	}
}