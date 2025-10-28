using System.Collections.Generic;

namespace GodhomeEloCounter
{

    public class SceneManager
    {
        public string PrevScene { get; private set; }
        public string LastBossScene { get; private set; }

        private readonly List<string> _nonBossRooms =
        [
            "GG_Workshop",
            "GG_Atrium",
            "GG_Mighty_Zote"
        ];

        private bool IsBossRoom(string scene)
        {
            return !_nonBossRooms.Contains(scene);
        }

        private string NormalizeBossName(string boss)
        {
            if (boss == "GG_Mantis_Lords_V") return boss;
            if (boss.EndsWith("_V")) return boss.Substring(0, boss.Length - 2);
            return boss;
        }

        public void HandleSceneChange(string nextScene)
        {
            GodhomeEloCounter mod = GodhomeEloCounter.Instance;

            // Hall of Gods -> Boss Room
            if (PrevScene == "GG_Workshop" && IsBossRoom(nextScene))
            {
                string boss = NormalizeBossName(nextScene);
                int tier = mod.selectedTier;
                mod.bossFight = new(boss, tier);

                LastBossScene = boss;

                mod.UI.Clear();
                if (!mod.config.hideUIinFights) mod.UI.DrawBossStats();
            }

            // Boss Room -> Hall of Gods
            if (nextScene == "GG_Workshop" && IsBossRoom(PrevScene)) mod.bossFight?.End();

            // Hall of Gods -> Anywhere
            if (nextScene.StartsWith("GG_Atrium") || !nextScene.StartsWith("GG_")) mod.UI.Clear();

            PrevScene = nextScene;
        }
    }
}
