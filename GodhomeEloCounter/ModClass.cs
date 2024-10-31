using Modding;
using System.Collections.Generic;
using UnityEngine;

namespace GodhomeEloCounter
{
    public class GodhomeEloCounter : Mod
    {
        internal static GodhomeEloCounter Instance;

        public GodhomeEloCounter() : base("Godhome Elo Counter") {}

        public override string GetVersion() => "v0.1";

        private bool isPlayerFighting;
        private bool isPlayerDead;
        private List<string> peacefulScenes = new List<string> { "GG_Atrium", "GG_Workshop", "GG_Blue_Room" };

        public override void Initialize(Dictionary<string, Dictionary<string, GameObject>> preloadedObjects)
        {
            Instance = this;

            ModHooks.BeforeSceneLoadHook += OnSceneLoad;
            ModHooks.TakeHealthHook += OnDamageTaken;
        }

        private int OnDamageTaken(int damage)
        {
            if (damage > PlayerData.instance.health)
            {
                isPlayerDead = true;
            }

            return damage;
        }

        private void OnBossEnter()
        {
            isPlayerFighting = true;
            isPlayerDead = false;
        }

        private void OnBossExit() 
        {
            if (isPlayerDead) { Log("Player Lost"); }
            else { Log("Player Won"); }

            isPlayerFighting = false;
            isPlayerDead = false;
        }

        private string OnSceneLoad(string name)
        {
            Log($"Loading new scene = {name}");

            if (!name.StartsWith("GG_") || peacefulScenes.Contains(name))
            {
                if (isPlayerFighting) { OnBossExit(); }
                return name;
            }

            OnBossEnter();

            return name;
        }

    }
}
