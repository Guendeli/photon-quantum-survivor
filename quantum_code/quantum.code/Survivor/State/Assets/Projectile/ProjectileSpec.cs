using System;
using Photon.Deterministic;

namespace Quantum
{
  public unsafe abstract partial class ProjectileSpec : AssetObject
  {
    public AssetRefEntityPrototype ProjectilePrototype;
    public FP TTL = FP._2;
    public LayerMask Mask;
    public AssetRefEffectSpec EffectSpec;
    public FP Speed = FP._5;
    public bool DestroyOnHit = true;
    public bool EffectOnDestroy = false;
    public bool HasKnockback = true;



    protected static QueryOptions QueryOptions = QueryOptions.HitDynamics | QueryOptions.HitKinematics;

    public virtual void InjectQuery(Frame f, ref ProjectileQueryInjectSystem.Filter filter)
    {
      FPVector2 direction = filter.Projectile->Direction * 2;
      filter.Projectile->QueryID = f.Physics2D.AddLinecastQuery(filter.Transform->Position, filter.Transform->Position + (direction * f.DeltaTime), false, Mask, QueryOptions);
    }

    public virtual FPVector2 GetDirection(Frame f, EntityRef character, Transform2D* characterTransform, Ability* ability)
    {
      var radians = ability->Radians;
      if (ability->RandomDirection)
      {
        radians = f.RNG->Next(-FP.Pi, FP.Pi);

      }
      else
      {
        ability->Radians += ability->RotationSpeed;
      }
      return FPVector2.Rotate(FPVector2.Right, radians);
    }

    public virtual void UpdateQuery(Frame f, ref ProjectileQueryUpdateSystem.Filter filter)
    {
      Projectile* projectile = filter.Projectile;
      if (projectile->QueryID == -1)
      {
        return;
      }

      var hits = f.Physics2D.GetQueryHits(projectile->QueryID);
      for (int i = 0; i < hits.Count; i++)
      {
        ApplyEffect(f, projectile, filter.Entity, hits[i].Entity);
        if (f.Exists(filter.Entity) == false)
        {
          return;
        }
        else if (DestroyOnHit)
        {
          OnDestroy(f, filter.Entity, filter.Projectile);
          return;
        }
      }
    }


    public virtual void ApplyEffect(Frame f, Projectile* projectile, EntityRef sourceEntity, EntityRef targetEntity)
    {
      var effect = f.Assets.EffectSpec(projectile->EffectSpec);
      effect.PerformEffect(f, sourceEntity, targetEntity);

      if (HasKnockback)
      {
        if (f.Unsafe.TryGetPointer<Monster>(targetEntity, out var monster))
        {
          ApplyKnockout(f, projectile, targetEntity);
        }
      }
    }

    private void ApplyKnockout(Frame f, Projectile* projectile, EntityRef target)
    {
      Transform2D* targetTransform = f.Unsafe.GetPointer<Transform2D>(target);
      targetTransform->Position += projectile->Direction.Normalized;
    }

    public virtual void Move(Frame f, ref ProjectileQueryUpdateSystem.Filter filter)
    {
      filter.Transform->Position += filter.Projectile->Direction * f.DeltaTime;
    }

    public virtual void Spawn(Frame f, FPVector2 position, FPVector2 direction, FP damage)
    {
      var projectile = f.Create(ProjectilePrototype);
      if (f.Unsafe.TryGetPointer<Transform2D>(projectile, out var t) &&
          f.Unsafe.TryGetPointer<Projectile>(projectile, out var p))
      {
        p->Damage = damage;
        p->Direction = direction * Speed;
        t->Position = position;
        t->Rotation = FPMath.Atan2(direction.Y, direction.X);
        p->Spec = this;
        p->TTL = TTL;

        p->EffectSpec = EffectSpec;
      }
    }

    public virtual void OnDestroy(Frame f, EntityRef entity, Projectile* projectile)
    {
      if (EffectOnDestroy)
      {
        var effect = f.Assets.EffectSpec(projectile->EffectSpec);
        effect.PerformEffect(f, entity, EntityRef.None);
      }
      f.Destroy(entity);
    }
  }
}