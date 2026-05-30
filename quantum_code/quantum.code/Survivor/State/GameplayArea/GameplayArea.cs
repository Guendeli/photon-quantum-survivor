using System;
using Photon.Deterministic;

namespace Quantum
{
  unsafe partial struct GameplayArea
  {
    public FPVector2 NormalizedPosition(FPVector2 pos)
    {
      FP leftPoint = -Size.X/2;
      FP rightPoint = Size.X/2;
      FP topPoint = Size.Y/2;
      FP bottomPoint = -Size.Y/2;

      pos.X = FPMath.Max(pos.X, leftPoint);
      pos.X = FPMath.Min(pos.X, rightPoint);

      pos.Y = FPMath.Max(pos.Y, bottomPoint);
      pos.Y = FPMath.Min(pos.Y, topPoint);

      return pos;
    }

    public FPVector2 GetRandomEdgeSpawnPosition(Frame f)
    {
      return GetPositionFromAngle(f, Size.X, Size.Y);
    }

    public FPVector2 GetRandomRadiusSpawnPosition(Frame f, FPVector2 targetPosition)
    {
      var radians = f.RNG->Next(-FP.Pi, FP.Pi);
      var direction = FPVector2.Rotate(FPVector2.Right, radians);
      var rngDistance = f.RNG->Next(FP._10 + 2, FP._10 + 5);

      return targetPosition + direction * rngDistance;
    }

    public FPVector2 GetPositionFromAngle(Frame f, FP width, FP height)
    {

      int side = f.RNG->Next(0, 4);
      FP x = f.RNG->Next(-width / 2, width/ 2);
      FP y = f.RNG->Next(-height / 2, height / 2);

      if (side == 0)
      {
        y = height/2;
      }
      else if (side == 1)
      {
        x = width/2;
      }
      else if (side == 2)
      {
        y = -height/2;
      }
      else
      {
        x = -width/2;
      }
      return new FPVector2(x, y);
    }
  }
}
