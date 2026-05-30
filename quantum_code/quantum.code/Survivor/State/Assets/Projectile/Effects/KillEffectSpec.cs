using System;
using Photon.Deterministic;

namespace Quantum
{

  [Serializable]
  public unsafe class KillEffectSpec : EffectSpec
  {
    public override void PerformEffect(Frame f, EntityRef source,  EntityRef target)
    {
      if (f.Unsafe.TryGetPointer<Monster>(target, out var monster))
      {
        monster->ShouldDie = true;
      }
    }
  }
}
