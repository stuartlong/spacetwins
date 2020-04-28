public Program()
{

}

public void Main(string argument, UpdateType updateSource)
{
	IMyCargoContainer largeCargo= GridTerminalSystem.GetBlockWithName("Large Cargo Inventory 1") as IMyCargoContainer;
	IMyInventory invCargoInv = largeCargo.GetInventory(0);
	IMyTextPanel LCDPanel = GridTerminalSystem.GetBlockWithName("Bridge LCD Panel") as IMyTextPanel;
	LCDPanel.WriteText("Inventory Storage Space: \n " + currentInvVolume.ToString() + "out of " + maxInvVolume.ToString()+);
	

}
