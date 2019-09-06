using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FF_Fishing.Core.Helpers
{
    public static class FishingSkillsInfo
    {
        private const int _patienceCost = 470;
        public static double GpRegenerationPerMinute { get; set; } = 100;
        private static Dictionary<FishingSkill, DateTime> _skillLastUsage = new Dictionary<FishingSkill, DateTime>();
        private static Dictionary<FishingSkill, int> _skillCost = new Dictionary<FishingSkill, int>();

        static FishingSkillsInfo()
        {
            var values = Enum.GetValues(typeof(FishingSkill)).Cast<Enum>();
            foreach (var v in values)
            {
                var nAttributes = v.GetType().GetField(v.ToString()).GetCustomAttributes(typeof(SkillCostAttribute), false);
                if (nAttributes.Any())
                {
                    var cost = (nAttributes.First() as SkillCostAttribute)?.GPCost ?? 500;
                    _skillCost.Add((FishingSkill)v, cost);
                }
            }
        }

        private static int GetSkillCooldownTime(FishingSkill skill)
        {
            return (int)((_skillCost[skill] / GpRegenerationPerMinute) * 60 * 1000);
        }

        private static DateTime GetLastTimeUsage(FishingSkill skill)
        {
            if (!_skillLastUsage.ContainsKey(skill))
            {
                _skillLastUsage.Add(skill, DateTime.MinValue);
            }

            return _skillLastUsage[skill];
        }

        public static bool TryUseSkill(FishingSkill skill)
        {
            var now = DateTime.UtcNow;
            var lastTime = GetLastTimeUsage(skill);
            var cd = GetSkillCooldownTime(skill);
            var ret = (now - lastTime).TotalMilliseconds > cd;
            if (ret)
            {
                _skillLastUsage[skill] = now;
            }

            return ret;
        }
    }

    public class SkillCostAttribute : Attribute
    {
        // ReSharper disable once InconsistentNaming
        public int GPCost { get; }

        public SkillCostAttribute(int gPCost)
        {
            GPCost = gPCost;
        }
    }
}
