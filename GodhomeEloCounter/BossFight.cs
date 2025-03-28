using UnityEngine;

namespace GodhomeEloCounter
{
    public class BossFight(string boss, int tier)
    {
        public string Boss { get; private set; } = boss;
        public int Tier { get; private set; } = tier;

        public bool IsPlayerDead { get; set; } = false;
        public int ComboDamage { get; private set; } = 0;
        public int PrevComboDamage { get; private set; } = 0;

        private readonly float _start = Time.time;
        private float _lastHitTime = 0;

        public void UpdateComboCounter(HitInstance hit)
        {
            if (Time.time >= _lastHitTime + 1)
            {
                PrevComboDamage = ComboDamage;
                ComboDamage = 0;
            }

            ComboDamage += (int)(hit.DamageDealt * hit.Multiplier);
            _lastHitTime = Time.time;

            GodhomeEloCounter.Instance.UI.DrawBossStats();
        }

        public void End()
        {
            float duration = Time.time - _start;

            GodhomeEloCounter mod = GodhomeEloCounter.Instance;
            mod._localData.UpdateBoss(Boss, Tier, !IsPlayerDead, duration);

            mod.UI.Clear();
            if (!mod.config.hideUIinHoG) mod.UI.DrawBossStats();
        }
    }
}
