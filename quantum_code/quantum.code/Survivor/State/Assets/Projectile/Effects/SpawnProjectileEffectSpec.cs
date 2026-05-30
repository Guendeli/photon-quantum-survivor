using System;
using Photon.Deterministic;

namespace Quantum
{

  [Serializable]
  public unsafe class SpawnProjectileEffectSpec : EffectSpec
  {
    public AssetRefProjectileSpec ProjectileToSpawnSpec;
    public override void PerformEffect(Frame f, EntityRef source, EntityRef target)
    {
      Transform2D sourceTransform = f.Get<Transform2D>(source);
      var toSpawnSpec = f.Assets.ProjectileSpec(ProjectileToSpawnSpec);
      Projectile projectile = f.Get<Projectile>(source);
      FPVector2 spawnPosition = sourceTransform.Position + projectile.Direction * f.DeltaTime;
      toSpawnSpec.Spawn(f, spawnPosition, projectile.Direction, projectile.Damage);
    }
  }
}