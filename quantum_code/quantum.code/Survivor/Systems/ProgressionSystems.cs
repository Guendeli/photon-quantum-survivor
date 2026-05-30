using Photon.Deterministic;
using Quantum.Task;

namespace Quantum
{
  public unsafe class ProgressionSystem : SystemMainThread
  {
    public override void Update(Frame f)
    {
      if (f.Unsafe.TryGetPointerSingleton<TeamProgression>(out var progression))
      {
        if (progression->State == GameState.Selecting)
        {
          progression->TimeToSelect -= f.DeltaTime;
          if (progression->TimeToSelect <= 0 || f.ComponentCount<PowerUpSelection>() == 0)
          {
            progression->State = GameState.Playing;
            f.Events.OnHideLevelUpPanel();
          }
        }
        
      }
    }
  }
  
  public unsafe class ProgressionQueryInjectSystem : SystemMainThreadFilter<ProgressionQueryInjectSystem.Filter>
  {
    public struct Filter
    {
      public EntityRef Entity;
      public Transform2D* Transform;
      public CharacterQuery* CharacterQuery;
    }


    public override void Update(Frame f, ref Filter filter)
    {
      filter.CharacterQuery->Radius = FP._1_50;
      var shape = Shape2D.CreateCircle(filter.CharacterQuery->Radius);
      filter.CharacterQuery->QueryID = f.Physics2D.AddOverlapShapeQuery(*filter.Transform, shape,
        filter.CharacterQuery->Mask, QueryOptions.HitKinematics | QueryOptions.HitDynamics);
    }
  }

  public unsafe class ProgressionQueryUpdateSystem : SystemMainThreadFilter<ProgressionQueryUpdateSystem.Filter>, ISignalOnCharacterDamage
  {
    public struct Filter
    {
      public EntityRef Entity;
      public Transform2D* Transform;
      public CharacterQuery* CharacterQuery;
    }

    public override void Update(Frame f, ref Filter filter)
    {
      var hits = f.Physics2D.GetQueryHits(filter.CharacterQuery->QueryID);
      for (int i = 0; i < hits.Count; i++)
      {
        var other = hits[i].Entity;
        if (f.Unsafe.TryGetPointer<Monster>(other, out var monster))
        {
          ProcessMonster(f, ref filter, monster, other);
        }

      }
    }

    private void ProcessMonster(Frame f, ref Filter filter, Monster* monster, EntityRef monsterEntity)
    {
      if (f.Exists(filter.Entity) == false)
      {
        return;
      }
      // I am target now
      monster->Target = filter.Entity;
      if (f.Unsafe.TryGetPointer<Transform2D>(monsterEntity, out var monsterTransform))
      {
        PhysicsCollider2D* monsterCollider = f.Unsafe.GetPointer<PhysicsCollider2D>(monsterEntity);
        PhysicsCollider2D* characterCollider = f.Unsafe.GetPointer<PhysicsCollider2D>(filter.Entity);
        FP monsterRadius = monsterCollider->Shape.BroadRadius;
        FP characterRadius = characterCollider->Shape.BroadRadius;

        var direction = monsterTransform->Position - filter.Transform->Position;
        var distance = direction.Magnitude;
        if (distance < (characterRadius + monsterRadius))
        {
          monsterTransform->Position = filter.Transform->Position + (direction.Normalized * (characterRadius + monsterRadius));
          f.Signals.OnCharacterDamage(filter.Entity, 1);
        }

        // if a bot, set closest enemy
        if (f.Unsafe.TryGetPointer<BotData>(filter.Entity, out var botData))
        {
          botData->CloseMonster = monsterEntity;
        }
      }
    }

    public void OnCharacterDamage(Frame f, EntityRef target, int damage)
    {
      if (f.Unsafe.TryGetPointer<CharacterHealth>(target, out var health))
      {
        health->Health -= damage;
        if (health->Health <= 0)
        {
          f.Destroy(target);
        }
      }
    }
  }
}