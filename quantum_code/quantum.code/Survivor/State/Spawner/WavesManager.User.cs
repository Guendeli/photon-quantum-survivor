using System;
using Photon.Deterministic;

namespace Quantum
{
  unsafe partial struct WavesManager
  {
    public void Update(Frame f, MapWaves waves)
    {
      TeamProgression* tp = f.Unsafe.GetPointerSingleton<TeamProgression>();
      //TODO change to game
      tp->Timer += f.DeltaTime;

      int i = WavesCreatedCount;
      while (i < waves.Waves.Length)
      {
        WaveConfig wave = waves.GetOrderedWaveOnIndex(i);
        if (wave.StartTime <= tp->Timer)
        {
          RuntimeWave* RuntimeWave = wave.CreateWave(f);
          WavesCreatedCount++;
        }
        else
        {
          break;
        }

        i++;
      }
    }

    public bool Spawn(Frame f, AssetRefEntityPrototype entity, out EntityRef entityCreated, out Waveable* waveable, out FPVector2 position)
    {
      var e = f.Create(entity);
      if (!f.Unsafe.TryGetPointer<Transform2D>(e, out var t))
      {
        waveable = default;
        entityCreated = default;
        position = FPVector2.Zero;
        return false;
      }

      f.TryGetSingleton<GameplayArea>(out var gameplayArea);
      // optional
      //t->Position = gameplayArea.GetRandomRadiusSpawnPosition(f, targetTransform->Position);

      position = gameplayArea.GetRandomEdgeSpawnPosition(f);

      t->Position = position;
      waveable = f.Unsafe.GetPointer<Waveable>(e);
      entityCreated = e;
      return waveable != default;
    }
  }
}