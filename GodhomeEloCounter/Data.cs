using System;
using System.Collections.Generic;
using System.Linq;

namespace GodhomeEloCounter {

    [Serializable]
    public class LocalData { 
        public List<Boss> bosses = [];

        public void UpdateBoss(string sceneName, int tier, bool hasWon, TimeSpan timeSpan, double baseELO) {
            Boss boss = FindOrCreateBoss(sceneName, tier, baseELO);
            boss.Update(hasWon, timeSpan);
        }

        public Boss FindOrCreateBoss(string sceneName, int tier, double baseELO) {
            foreach (Boss boss in bosses) {
                if (boss.sceneName == sceneName && boss.tier == tier) {
                    if (boss.wins + boss.losses == 0) {
                        boss.elo = baseELO;
                        boss.lastElo = baseELO;
                        boss.peakElo = baseELO;
                    }
                    return boss;
                }
            }
            bosses.Add(new Boss(sceneName, tier) { elo = baseELO, lastElo = baseELO, peakElo = baseELO, streak = 0, bestWinStreak = 0, wins = 0, losses = 0, timeSpent = TimeSpan.Zero });
            return bosses.Last();
        }
    }

    [Serializable]
    public class ModSettings {
        public int baseELO = 1000;
        public bool hideUIinFights = false;
        public bool hideUIinHoG = false;
        public bool hideBossName = false;
        public bool hideWinsLosses = false;
        public bool hideWinstreak = false;
        public bool hideMatchHistory = false;
        public bool hideTimeSpent = false;
        public bool naturalTime = true;
    }
}
