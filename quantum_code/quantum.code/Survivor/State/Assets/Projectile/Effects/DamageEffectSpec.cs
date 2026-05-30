using System;
using Photon.Deterministic;

namespace Quantum
{

  [Serializable]
  public unsafe class DamageEffectSpec : EffectSpec
  {
    public override void PerformEffect(Frame f, EntityRef source, EntityRef target)
    {
      if (f.Unsafe.TryGetPointer<CharacterHealth>(target, out var characterHealth))
      {
        Projectile projectile = f.Get<Projectile>(source);
        characterHealth->Health -= projectile.Damage;
        if (characterHealth->Health <= 0)
        {
          if (f.Unsafe.TryGetPointer<Monster>(target, out var monster))
          {
            monster->ShouldDie = true;
          }
        }
      }

    }
  }
}
