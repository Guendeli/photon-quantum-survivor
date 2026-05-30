using System.Reflection;
using Photon.Deterministic;

namespace Quantum
{
  public unsafe static class LevelUpHelper
  {
    public static int GetXPToLevel(int level)
    {
      if (level <= 1)
      {
        return 0;
      }
      return level + level * level;
    }
  }
}
