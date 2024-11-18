using System.Collections.Generic;

namespace GodhomeEloCounter {
    public class BossMapping(string statueName, string sceneName, string displayName)
    {
        public string StatueName { get; private set; } = statueName;
        public string SceneName { get; private set; } = sceneName;
        public string DisplayName { get; private set; } = displayName;
    }

    public static class BossMappings {
        private static readonly Dictionary<string, BossMapping> statueToMapping;
        private static readonly Dictionary<string, BossMapping> sceneToMapping;

        static BossMappings()
        {
            statueToMapping = [];
            sceneToMapping = [];

            var mappings = new List<BossMapping>
            {
                new("NAME_BIGFLY", "GG_Gruz_Mother", "Gruz Mother"),
                new("NAME_BIGBUZZER", "GG_Vengefly", "Vengefly King"),
                new("NAME_MAWLEK", "GG_Brooding_Mawlek", "Brooding Mawlek"),
                new("NAME_FALSEKNIGHT", "GG_False_Knight", "False Knight"),
                new("NAME_FAILED_CHAMPION", "GG_Failed_Champion", "Failed Champion"),
                new("NAME_HORNET_1", "GG_Hornet_1", "Hornet (Protector)"),
                new("NAME_HORNET_2", "GG_Hornet_2", "Hornet (Sentinel)"),
                new("NAME_MEGA_MOSS_CHARGER", "GG_Mega_Moss_Charger", "Mega Moss Charger"),
                new("NAME_FLUKE_MOTHER", "GG_Flukemarm", "Flukemarm"),
                new("NAME_MANTIS_LORD", "GG_Mantis_Lords", "Mantis Lords"),
                new("NAME_MANTIS_LORD_V", "GG_Mantis_Lords_V", "Sisters of Battle"),
                new("NAME_OBLOBBLE", "GG_Oblobbles", "Oblobbles"),
                new("NAME_HIVE_KNIGHT", "GG_Hive_Knight", "Hive Knight"),
                new("NAME_INFECTED_KNIGHT", "GG_Broken_Vessel", "Broken Vessel"),
                new("NAME_LOST_KIN", "GG_Lost_Kin", "Lost Kin"),
                new("NAME_MIMIC_SPIDER", "GG_Nosk", "Nosk"),
                new("NAME_NOSK_HORNET", "GG_Nosk_Hornet", "Noskette"),
                new("NAME_JAR_COLLECTOR", "GG_Collector", "The Collector"),
                new("NAME_LOBSTER_LANCER", "GG_God_Tamer", "God Tamer"),
                new("NAME_MEGA_BEAM_MINER_1", "GG_Crystal_Guardian", "Crystal Guardian"),
                new("NAME_MEGA_BEAM_MINER_2", "GG_Crystal_Guardian_2", "Enraged Guardian"),
                new("NAME_MEGA_JELLYFISH", "GG_Uumuu", "Uumuu"),
                new("NAME_TRAITOR_LORD", "GG_Traitor_Lord", "Traitor Lord"),
                new("NAME_GREY_PRINCE", "GG_Grey_Prince_Zote", "Grey Prince Zote"),
                new("NAME_MAGE_KNIGHT", "GG_Mage_Knight", "Soul Warrior"),
                new("NAME_MAGE_LORD", "GG_Soul_Master", "Soul Master"),
                new("NAME_SOUL_TYRANT", "GG_Soul_Tyrant", "Soul Tyrant"),
                new("NAME_DUNG_DEFENDER", "GG_Dung_Defender", "Dung Defender"),
                new("NAME_WHITE_DEFENDER", "GG_White_Defender", "White Defender"),
                new("NAME_BLACK_KNIGHT", "GG_Watcher_Knights", "Watcher Knights"),
                new("NAME_GHOST_NOEYES", "GG_Ghost_No_Eyes", "No Eyes"),
                new("NAME_GHOST_MARMU", "GG_Ghost_Marmu", "Marmu"),
                new("NAME_GHOST_XERO", "GG_Ghost_Xero", "Xero"),
                new("NAME_GHOST_MARKOTH", "GG_Ghost_Markoth", "Markoth"),
                new("NAME_GHOST_GALIEN", "GG_Ghost_Galien", "Galien"),
                new("NAME_GHOST_ALADAR", "GG_Ghost_Gorb", "Gorb"),
                new("NAME_GHOST_HU", "GG_Ghost_Hu", "Elder Hu"),
                new("NAME_NAILMASTERS", "GG_Nailmasters", "Oro & Mato"),
                new("NAME_PAINTMASTER", "GG_Painter", "Sheo"),
                new("NAME_SLY", "GG_Sly", "Sly"),
                new("NAME_HK_PRIME", "GG_Hollow_Knight", "Pure Vessel"),
                new("NAME_GRIMM", "GG_Grimm", "Grimm"),
                new("NAME_NIGHTMARE_GRIMM", "GG_Grimm_Nightmare", "Nightmare King Grimm"),
                new("NAME_FINAL_BOSS", "GG_Radiance", "Absolute Radiance"),

                // Not really compatible with them for now so they're fully merged to avoid menu crashes
                new("NAME_THK", "GG_Hollow_Knight", "Pure Vessel"),
                new("NAME_RADIANCE", "GG_Radiance", "Absolute Radiance"),
            };

            foreach (var mapping in mappings)
            {
                statueToMapping[mapping.StatueName] = mapping;
                sceneToMapping[mapping.SceneName] = mapping;
            }
        }

        public static string GetSceneFromStatue(string statueName)
        {
            if (statueName.EndsWith("_V") && statueName != "NAME_MANTIS_LORD_V") { statueName = statueName.Substring(0, statueName.Length - 2); }

            return statueToMapping.TryGetValue(statueName, out var mapping) 
                ? mapping.SceneName 
                : null;
        }

        public static string GetStatueFromScene(string sceneName)
        {
            if (sceneName.EndsWith("_V") && sceneName != "GG_Mantis_Lords_V") { sceneName = sceneName.Substring(0, sceneName.Length - 2); }
            
            return sceneToMapping.TryGetValue(sceneName, out var mapping) 
                ? mapping.StatueName 
                : null;
        }

        public static string GetDisplayFromStatue(string statueName)
        {
            if (statueName.EndsWith("_V") && statueName != "NAME_MANTIS_LORD_V") { statueName = statueName.Substring(0, statueName.Length - 2); }

            return statueToMapping.TryGetValue(statueName, out var mapping) 
                ? mapping.DisplayName 
                : null;
        }

        public static string GetDisplayFromScene(string sceneName)
        {
            if (sceneName.EndsWith("_V") && sceneName != "GG_Mantis_Lords_V") { sceneName = sceneName.Substring(0, sceneName.Length - 2); }

            return sceneToMapping.TryGetValue(sceneName, out var mapping) 
                ? mapping.DisplayName 
                : null;
        }
    }
}
