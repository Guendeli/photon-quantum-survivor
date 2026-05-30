using Photon.Deterministic;
using Quantum.Task;

namespace Quantum
{
  public unsafe class WavesManagerSystem : SystemMainThread
  {
    private MapWaves _cachedWaves = null;

    public override void OnInit(Frame f)
    {
      _cachedWaves = f.FindAsset<MapWaves>(f.Map.UserAsset.Id);
      _cachedWaves.Init(f);
    }
    public override void Update(Frame f)
    {
      if (f.Unsafe.TryGetPointerSingleton<TeamProgression>(out var team))
      {
        if (f.ComponentCount<PlayerLink>() == 0 || team->State != GameState.Playing)
        {
          return;
        }
      }

      if (f.Unsafe.TryGetPointerSingleton<WavesManager>(out var wavesManager))
      {
        wavesManager->Update(f, _cachedWaves);
      }
    }
  }
}
