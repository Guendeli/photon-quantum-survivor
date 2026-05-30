using System;
using System.Collections.Generic;
using Photon.Deterministic;

namespace Quantum
{

  [Serializable]

  public class UpgradeValues
  {
    public FP DamageMidifier;
    public FP CooldownMidifier;
    public FP RotationSpeedModifier;
    public FP RadiansModifier;
    public FP TimeModifier;
  }

  [Serializable]
  public unsafe class AbilityPowerUpSpec : PowerUpSpec
  {

    public AssetRefProjectileSpec AbilityProjectileSpec;

    public UpgradeValues[] Upgrades;

    public override void PerformPowerUpEffect(Frame f, EntityRef character)
    {
      Ability* ability = GetAbility(f, character);
      if (ability == default)
      {
        return;
      }

      if (ability->IsEnabled == false)
      {
        ability->IsEnabled = true;
        ability->Level = 1;
        return;
      }

      if (ability->Level == Upgrades.Length)
      {
        Log.Info("Ability on Level Max!");
        return;
      }

      UpgradeValues values = Upgrades[ability->Level];
      ability->Level++;
        
      ability->Damage += values.DamageMidifier;
      ability->Cooldown += values.CooldownMidifier;
      ability->RotationSpeed += values.RotationSpeedModifier;
      ability->Radians += values.RadiansModifier;
      ability->Time += values.TimeModifier;


    }

    private Ability* GetAbility(Frame f, EntityRef character)
    {
      Abilities* abilities = f.Unsafe.GetPointer<Abilities>(character);
      for (int i = 0; i < abilities->Slot.Length; i++)
      {
        if (abilities->Slot[i].Spec.Id == AbilityProjectileSpec.Id)
        {
          return abilities->Slot.GetPointer(i);
        }
      }
      return default;
    }
  }
}
