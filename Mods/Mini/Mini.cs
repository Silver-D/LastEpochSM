using MelonLoader;
using HarmonyLib;
using LastEpochSM.Managers;
using UnityEngine;
using Newtonsoft.Json.Linq;
using System.Linq;
using System.Collections.Generic;
using LE.Gameplay.Monolith;

namespace LastEpochSM.Mods
{
    [RegisterTypeInIl2Cpp]
    class Mini : MonoBehaviour
    {
        public class Main : MelonMod
        {
            public static bool transfersInitialized { get; private set; }

            public override void OnLateInitializeMelon()
            {
                if (System.Type.GetType("LastEpochSM.Main, LastEpochSM") == null)
                    Unregister("LastEpochSM.dll is not loaded");
            }

            public override void OnUpdate()
            {
                if (LastEpochSM.Main.instance.IsNullOrDestroyed()) {
                    Unregister("LastEpochSM.dll is not loaded"); return; }

                if (Mod_Manager.CanPatch())
                {
                    Mod_Manager.Register<Mini>();
                    MelonEvents.OnSceneWasInitialized.Subscribe(Mini.instance.OnSceneWasInitialized);

                    MelonEvents.OnUpdate.Unsubscribe(this.OnUpdate);
                }
            }

            public override void OnLateUpdate()
            {
                if (Mini.instance.IsNullOrDestroyed() || !Mod_Manager.CanPatch())
                    return;

                if (Monolith.InitializeTransfers())
                {
                    transfersInitialized = true;

                    MelonEvents.OnLateUpdate.Unsubscribe(this.OnLateUpdate);
                }
            }
        }

        public Mini(System.IntPtr ptr) : base(ptr) { }
        public static Mini instance { get; private set; }

        public static Logger Log = new Logger("Mini");

        void Awake()
        {
            instance = this;

            Conf.Load();
        }

        void OnSceneWasInitialized(int buildIndex, string sceneName)
        {
            if (Scenes.IsGameScene())
                Conf.ReLoad();
        }

        class Forge
        {
            [HarmonyPatch(typeof(ItemData), "getChanceToSealAffix")]
            class ItemData_getChanceToSealAffix
            {
                [HarmonyPostfix]
                static void Postfix(ref float __result, int __0)
                {
                    if (Mod_Manager.CanPatch(Conf.Get<bool>("Forge.alwaysSealAffixWithGlyph")))
                        __result = 1.0f;
                }
            }

            static bool rollsApplied;

            [HarmonyPatch(typeof(CraftingManager), "Forge")]
            class CraftingManager_Forge
            {
                [HarmonyPrefix]
                static void Prefix(ref CraftingManager __instance)
                {
                    if (Mod_Manager.CanPatch(Conf.Get<bool>("Forge.noForgingPotentialCost")))
                        __instance.debugNoForgingPotentialCost = true;
                }

                [HarmonyPostfix]
                static void Postfix(ref CraftingManager __instance)
                {
                    rollsApplied = false;
                    ItemData forgeItem = null;

                    if (!__instance.main.IsNullOrDestroyed() && !__instance.main.content.IsNullOrDestroyed())
                        forgeItem = __instance.main.content.data;

                    if (!Mod_Manager.CanPatch(!forgeItem.IsNullOrDestroyed()))
                        return;

                    byte minImpVal = (byte)(Conf.Get<float>("Forge.minImplicitRangePercentInt") / 100f * 255).Clamp(0, 255);
                    byte minAffVal = (byte)(Conf.Get<float>("Forge.minAffixRangePercentInt") / 100f * 255).Clamp(0, 255);
                    byte minUniVal = minAffVal;

                    rollsApplied = (minImpVal > 0 || minAffVal > 0 || minUniVal > 0);

                    if (!rollsApplied)
                        return;

                    for (byte i = 0; i < forgeItem.implicitRolls.Count; i++)
                    {
                        if (forgeItem.implicitRolls[i] < minImpVal)
                            forgeItem.implicitRolls[i] = (byte)Random.RandomRange(minImpVal, 255);
                    }

                    foreach (ItemAffix aff in forgeItem.affixes)
                    {
                        if (aff.affixRoll < minAffVal)
                            aff.affixRoll = (byte)Random.RandomRange(minAffVal, 255);
                    }

                    if (forgeItem.isUniqueSetOrLegendary())
                    {
                        for (byte i = 0; i < forgeItem.uniqueRolls.Count; i++)
                        {
                            if (forgeItem.uniqueRolls[i] < minUniVal)
                                forgeItem.uniqueRolls[i] = (byte)Random.RandomRange(minUniVal, 255);
                        }
                    }

                    forgeItem.RefreshIDAndValues();
                }
            }

            [HarmonyPatch(typeof(CraftingSlotManager), "Forge")]
            class CraftingSlotManager_Forge
            {
                [HarmonyPostfix]
                static void Postfix(ref CraftingSlotManager __instance)
                {
                    if (!Mod_Manager.CanPatch(rollsApplied && !__instance.fractureChanceHolder.IsNullOrDestroyed()))
                        return;

                    GameObject resultPanel = __instance.fractureChanceHolder.gameObject;
                    GameObject resultHeader = GUI_Manager.Functions.GetChild(resultPanel, "FractureChanceHeader");

                    resultHeader.GetComponent<TMPro.TextMeshProUGUI>().SetText("crafting outcome not available");
                    GUI_Manager.Functions.GetChild(resultPanel, "Outcome Text").active = false;
                }
            }
        }

        class Monolith
        {
            [HarmonyPatch(typeof(EchoWebIsland), "onDiedInEcho")]
            class EchoWebIsland_onDiedInEcho
            {
                [HarmonyPrefix]
                static bool Prefix(ref EchoWebIsland __instance, ref MonolithRun run)
                {
                    if (!Mod_Manager.CanPatch(Conf.Get<bool>("Monolith.noLoseWhenDie")))
                        return true;

                    return false;
                }
            }

            [HarmonyPatch(typeof(EchoWeb), "getNewCorruptionForShadeEchoOfTier")]
            class EchoWeb_getNewCorruptionForShadeEchoOfTier
            {
                [HarmonyPrefix]
                static void Prefix(ref EchoWeb __instance, int __0, int __1, ref int __2)
                {
                   if (!Mod_Manager.CanPatch())
                       return;

                    __2 += Conf.Get<int>("Monolith.bonusCorruptionPerGaze");
                }
            }

            [HarmonyPatch(typeof(MonolithTimeline), "getShadeEchoChance")]
            class MonolithTimeline_getShadeEchoChance
            {
                [HarmonyPrefix]
                static void Prefix(ref MonolithTimeline __instance, ref int __state)
                {
                    if (!Mod_Manager.CanPatch(Conf.Get<bool>("Monolith.noMinIslandTierForShade")))
                        return;

                    __state = __instance.minTierForShade;

                    MonolithRun currentRun = MonolithGameplayManager.ActiveRun;
                    int islandTier = MonolithGameplayManager.ActiveIslandTier;

                    if (currentRun.IsNullOrDestroyed())
                        return;

                    if (currentRun.IsEmpowered && !currentRun.web.HasShadeEcho() && __instance.minTierForShade > islandTier)
                        __instance.minTierForShade = islandTier;
                }

                [HarmonyPostfix]
                static void Postfix(ref MonolithTimeline __instance, int __state)
                {
                    if (!Mod_Manager.CanPatch(Conf.Get<bool>("Monolith.noMinIslandTierForShade")))
                        return;

                    __instance.minTierForShade = __state;
                }
            }

            static bool error;

            public static bool InitializeTransfers()
            {
                if (Main.transfersInitialized || !Conf.Initialized)
                    return false;

                if (!TimelineList.instance.IsNullOrDestroyed() && !TimelineList.instance.timelines.IsNullOrDestroyed())
                {
                    if (Conf.Get<bool>("Monolith.enableBlessingTransfers"))
                        Monolith.TransferBlessings();

                    return true;
                }

                return false;
            }

            static void OnError(string str)
            {
                Log.Error(str + ". Aborting.");
                error = true;
            }

            static bool CheckBlessingIndex(List<int> array, byte bpos, string what)
            {
                if (bpos < 0)
                    OnError(bpos + ": invalid blessing index for " + what);
                else if (bpos >= array.Count)
                    OnError(bpos + ": blessing index doesn't exist in " + what);
                else if (array[bpos] == -1)
                    OnError(bpos + ": blessing at index was already moved or removed from " + what);

                return (!error);
            }

            static void TransferBlessings()
            {
                JObject tl_settings = Conf.Get<JObject>("Monolith.blessingTransfers");

                string[] slotOptions = { "firstSlotBlessings", "otherSlotBlessings", "anySlotBlessings" };
                string[] actions = { "clear", "transfer", "remove", "swap" };

                Dictionary<string, MonolithTimeline> timelines = new Dictionary<string, MonolithTimeline>();

                Dictionary<string, Dictionary<byte, Dictionary<string, List<int>>>> tl_data =
                    new Dictionary<string, Dictionary<byte, Dictionary<string, List<int>>>>();

                byte diffNum = (byte)TimelineList.instance.timelines[0].difficulties.Count;

                for (int i = 0; i < TimelineList.instance.timelines.Count; i++)
                {
                    string tl_name = TimelineList.instance.timelines[i].displayName.ToLower();

                    timelines.Add(tl_name, TimelineList.instance.timelines[i]);

                    tl_data.Add(tl_name, new Dictionary<byte, Dictionary<string, List<int>>>());

                    for (byte j = 0; j < diffNum; j++)
                    {
                        tl_data[tl_name].Add(j, new Dictionary<string, List<int>>());

                        foreach(string s in slotOptions)
                            tl_data[tl_name][j].Add(s, new List<int>());

                        foreach(int b in timelines[tl_name].difficulties[j].firstSlotBlessings)
                            tl_data[tl_name][j][slotOptions[0]].Add(b);

                        foreach(int b in timelines[tl_name].difficulties[j].otherSlotBlessings)
                            tl_data[tl_name][j][slotOptions[1]].Add(b);

                        foreach(int b in timelines[tl_name].difficulties[j].anySlotBlessings)
                            tl_data[tl_name][j][slotOptions[2]].Add(b);
                    }
                }

                Log.Msg("Prepairing to transfer blessings...");

                error = false;

                foreach(var to_tl in tl_settings)
                {
                    if (!tl_data.ContainsKey(to_tl.Key))
                    {
                        OnError(to_tl.Key + ": is not a timeline that exists. (Case is important: all lower case)");
                        return;
                    }

                    Log.Msg(to_tl.Key + "");

                    foreach(var to_slot in (JObject)to_tl.Value)
                    {
                        if (!slotOptions.Contains(to_slot.Key))
                        {
                            OnError(to_slot.Key + ": is not a valid blessing slot. (Case is important)");
                            return;
                        }

                        Log.Msg(" " + to_slot.Key + "");

                        foreach(var act in (JObject)to_slot.Value)
                        {
                            if (!actions.Contains(act.Key.ToLower()))
                            {
                                OnError(act.Key + ": is not a valid action");
                                return;
                            }

                            Log.Msg("  " + act.Key + "");

                            if (act.Key.ToLower() == "clear")
                            {
                                if ((bool)act.Value)
                                {
                                    for (byte i = 0; i < diffNum; i++)
                                    {
                                        for (byte b = 0; b < tl_data[to_tl.Key][i][to_slot.Key].Count; b++)
                                            tl_data[to_tl.Key][i][to_slot.Key][b] = -1;
                                    }
                                }
                                else
                                    Log.Msg("  " + "(not clearing: was false)");
                            }

                            else if (act.Key.ToLower() == "remove")
                            {
                                string temp = "";

                                foreach(byte bpos in act.Value)
                                {
                                    if (!CheckBlessingIndex(tl_data[to_tl.Key][0][to_slot.Key], bpos, to_tl.Key))
                                        return;

                                    for (byte i = 0; i < diffNum; i++)
                                        tl_data[to_tl.Key][i][to_slot.Key][bpos] = -1;

                                    temp += bpos + " ";
                                }

                                Log.Msg("   " + temp);
                            }

                            else if (act.Key.ToLower() == "transfer")
                            {
                                foreach(var from_tl in (JObject)act.Value)
                                {
                                    if (!tl_data.ContainsKey(from_tl.Key))
                                    {
                                        OnError(from_tl.Key + ": is not a timeline that exists. (Case is important: all lower case)");
                                        return;
                                    }

                                    Log.Msg("   " + from_tl.Key + "");

                                    foreach(var from_slot in (JObject)from_tl.Value)
                                    {
                                        if (!slotOptions.Contains(from_slot.Key))
                                        {
                                            OnError(from_slot.Key + ": is not a valid blessing slot. (Case is important)");
                                            return;
                                        }

                                        Log.Msg("    " + from_slot.Key + "");

                                        string temp = "     ";

                                        foreach(byte bpos in from_slot.Value)
                                        {
                                            temp += bpos + " ";

                                            if (!CheckBlessingIndex(tl_data[from_tl.Key][0][from_slot.Key], bpos, from_slot.Key))
                                                return;

                                            for (byte i = 0; i < diffNum; i++)
                                            {
                                                tl_data[to_tl.Key][i][to_slot.Key].Add(tl_data[from_tl.Key][i][from_slot.Key][bpos]);
                                                tl_data[from_tl.Key][i][from_slot.Key][bpos] = -1;
                                            }
                                        }

                                        Log.Msg(temp);
                                    }
                                }
                            }

                            else if (act.Key.ToLower() == "swap")
                            {
                                foreach(var to_bl in (JObject)act.Value)
                                {
                                    byte bpos1 = byte.Parse(to_bl.Key);

                                    if (!CheckBlessingIndex(tl_data[to_tl.Key][0][to_slot.Key], bpos1, "swap to: " + to_tl.Key))
                                        return;

                                    string from_tlKey = to_bl.Value.First().Value<JProperty>().Name;

                                    JToken from_slotVal = to_bl.Value[from_tlKey].First();
                                    string from_slotKey = from_slotVal.Value<JProperty>().Name;

                                    byte bpos2 = 0;

                                    try
                                    {
                                        byte[] bpos3 = from_slotVal.First().ToObject<byte[]>();
                                        bpos2 = bpos3[0];
                                    }
                                    catch { bpos2 = from_slotVal.ToObject<byte>(); }

                                    if (!CheckBlessingIndex(tl_data[from_tlKey][0][from_slotKey], bpos2, "swap from: " + from_slotKey))
                                        return;

                                    Log.Msg("   " + bpos1 + " <-> " + from_tlKey + ":" + from_slotKey + ":" + bpos2);

                                    for (byte i = 0; i < diffNum; i++)
                                    {
                                        int swap = tl_data[to_tl.Key][i][to_slot.Key][bpos1];

                                        tl_data[to_tl.Key][i][to_slot.Key][bpos1] = tl_data[from_tlKey][i][from_slotKey][bpos2];
                                        tl_data[from_tlKey][i][from_slotKey][bpos2] = swap;
                                    }
                                }
                            }
                        }
                    }
                }

                if (error)
                    return;

                Log.Msg("Checking result...");

                foreach(var tl in tl_data)
                {
                    Dictionary<string, byte> slotTotals = new Dictionary<string, byte>();

                    foreach(string sl in slotOptions)
                        slotTotals.Add(sl, 0);

                    foreach(var tl_slot in tl.Value[0])
                    {
                        foreach(int tl_bl in tl_slot.Value)
                        {
                            if (tl_bl > -1)
                                slotTotals[tl_slot.Key]++;
                        }
                    }

                    if (slotTotals[slotOptions[0]] + slotTotals[slotOptions[1]] + slotTotals[slotOptions[2]] <= 0)
                        OnError("Timeline " + tl.Key + " has no blessings");
                    else if (slotTotals[slotOptions[0]] == 0 && slotTotals[slotOptions[1]] > 0 && slotTotals[slotOptions[2]] == 0)
                    {
                        for (byte i = 0; i < diffNum; i++)
                        {
                            for (byte j = 0; j < tl_data[tl.Key][i][slotOptions[1]].Count; j++)
                                timelines[tl.Key].difficulties[i].anySlotBlessings.Add(timelines[tl.Key].difficulties[i].otherSlotBlessings[j]);

                            timelines[tl.Key].difficulties[i].otherSlotBlessings.Clear();
                        }
                    }

                    if (error)
                        return;
                }

                for (byte i = 0; i < diffNum; i++)
                {
                    foreach(var tl in tl_data)
                    {
                        foreach(var tl_slot in tl.Value[i])
                        {
                            Il2CppSystem.Collections.Generic.List<int> tl_blessings;

                            if (tl_slot.Key == slotOptions[0])
                                tl_blessings = timelines[tl.Key].difficulties[i].firstSlotBlessings;
                            else if (tl_slot.Key == slotOptions[1])
                                tl_blessings = timelines[tl.Key].difficulties[i].otherSlotBlessings;
                            else
                                tl_blessings = timelines[tl.Key].difficulties[i].anySlotBlessings;


                            tl_blessings.Clear();

                            foreach(int tl_bl in tl_slot.Value)
                            {
                                if (tl_bl > -1)
                                    tl_blessings.Add(tl_bl);
                            }
                        }
                    }
                }

                Log.Msg("Done.");
                Log.Msg("Make sure to unequip/reequip any Blessings that you've moved/replaced.");
            }
        }
    }
}
