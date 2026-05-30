using Photon.Deterministic;

namespace Quantum
{
  public unsafe class ChoosePowerUpCommandSystem : SystemMainThread
  {
    public override void Update(Frame f)
    {
      for (int playerID = 0; playerID < Constants.MaxPlayers; playerID++)
      {
        ChoosePowerUpCommand command = f.GetPlayerCommand(playerID) as ChoosePowerUpCommand;
        if (command != null)
        {
          Log.Warn(" Command from player: "+ playerID + " Choose index: " + command.ChooseIndex);
          f.Signals.OnPlayerChoosePowerUp(playerID, command.ChooseIndex);
        }
      }
    }
  }
}
