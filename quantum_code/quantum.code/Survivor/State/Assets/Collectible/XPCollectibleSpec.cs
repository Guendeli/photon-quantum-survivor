using System;
using Photon.Deterministic;

namespace Quantum
{
  [Serializable]
  public unsafe partial class XPCollectibleSpec : CollectibleSpec
  {
    public FP Amount = 1;
    public override void PerformCollectibleEffect(Frame f, EntityRef entity, EntityRef target)
    {
      if (f.Unsafe.TryGetPointerSingleton<TeamProgression>(out var progression))
      {
        progression->XP += Amount;
        progression->TotalXP += Amount;
        if (progression->XP >= LevelUpHelper.GetXPToLevel(progression->Level + 1))
        {
          f.Signals.OnTeamLevelUp(progression);
        }
      }
    }
  }
}
