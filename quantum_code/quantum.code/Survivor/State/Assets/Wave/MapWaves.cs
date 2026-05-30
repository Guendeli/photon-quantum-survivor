using System;
using System.Linq;
using Photon.Deterministic;

namespace Quantum
{

  public unsafe partial class MapWaves : AssetObject
  {
    public AssetRefWaveConfig[] Waves;

    private WaveConfig[] _orderedWaves;

    public void Init(Frame f)
    {
      _orderedWaves = new WaveConfig[Waves.Length];

      for (int i = 0; i < Waves.Length; i++)
      {
        _orderedWaves[i] = f.FindAsset<WaveConfig>(Waves[i].Id);
        _orderedWaves[i].ConfigRef = Waves[i];
      }

      _orderedWaves = _orderedWaves.ToList().OrderBy(i => i.StartTime).ToArray();
      Log.Info("Loaded waves.");
    }

    public WaveConfig GetOrderedWaveOnIndex(int index)
    {
      return _orderedWaves[index];
    }
  }
}
