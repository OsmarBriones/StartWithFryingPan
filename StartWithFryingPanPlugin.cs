using BepInEx;
using HarmonyLib;


[BepInPlugin("com.borenfenix.startwithfryingpan", "Start With Frying Pan", "1.0.1")]
public class StartWithFryingPanPlugin : BaseUnityPlugin
{
	private void Awake()
	{
		Logger.LogInfo("[StartWithFryingPan] Plugin loaded and initialized. ");

		var harmony = new Harmony("com.borenfenix.startwithfryingpan");
		harmony.PatchAll();
	}
}