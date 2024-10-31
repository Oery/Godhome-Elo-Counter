using System;
using System.Collections.Generic;
using System.Linq;

namespace GodhomeEloCounter {

    [Serializable]
    public class LocalData { 
        public List<Boss> bosses = [];

        public void UpdateBoss(string sceneName, int tier, bool hasWon, TimeSpan timeSpan) {
            Boss boss = FindOrCreateBoss(sceneName, tier);
            boss.Update(hasWon, timeSpan);
        }

        public Boss FindOrCreateBoss(string sceneName, int tier) {
            foreach (Boss boss in bosses) {
                if (boss.sceneName == sceneName && boss.tier == tier) {
                    return boss;
                }
            }
            bosses.Add(new Boss(sceneName, tier) { elo = 1000.0, lastElo = 1000.0, streak = 0, bestWinStreak = 0, wins = 0, losses = 0, timeSpent = TimeSpan.Zero });
            return bosses.Last();
        }
    }
}
