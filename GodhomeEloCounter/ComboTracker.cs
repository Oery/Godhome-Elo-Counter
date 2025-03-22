using UnityEngine;

namespace GodhomeEloCounter {
    public class ComboTracker {
        public int comboDamage = 0;
        public int previousComboDamage = 0;
        private float lastHitTime = 0;

        public void Reset()
        {
            comboDamage = 0;
            previousComboDamage = 0;
            lastHitTime = 0;
        }

        public void Update(HealthManager self, HitInstance hitInstance)
        {
            if (Time.time > lastHitTime + 1)
            {
                previousComboDamage = comboDamage;
                comboDamage = 0;
            }

            comboDamage += (int)(hitInstance.DamageDealt * hitInstance.Multiplier);
            lastHitTime = Time.time;

            GodhomeEloCounter.Instance.UpdateUI();
        }       
    }
}
