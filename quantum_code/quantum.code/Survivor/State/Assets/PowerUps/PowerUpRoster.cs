namespace Quantum
{
  public unsafe partial class PowerUpRoster : AssetObject
  {
    public AssetRefPowerUpSpec[] PowerUpSpecs;


    public PowerUpSelection FillOptions(Frame f, EntityRef character)
    {
      // include component PowerUpSelection, fill in with the valid options (per player)
      var selection = new PowerUpSelection();
      selection.Options[0] = PowerUpSpecs[0];
      selection.Options[1] = PowerUpSpecs[1];
      selection.Options[2] = PowerUpSpecs[2];
      f.Add(character, selection);
      return selection;
    }

  }
}