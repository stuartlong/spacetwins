List<IMyAssembler> GetAssemblers () {
	List<IMyTerminalBlock> assemblers = new List<IMyTerminalBlock> ();
	List<IMyAssembler> output = new List<IMyAssembler> ();
	GridTerminalSystem.GetBlocksOfType<IMyAssembler> (assemblers);
	for (int i = 0; i < assemblers.Count; i++) {
		output.Add ((IMyAssembler) assemblers[i]);
	}
	return output;
}
List<IMyCargoContainer> GetCargoContainers () {
	List<IMyTerminalBlock> cargo = new List<IMyTerminalBlock> ();
	List<IMyCargoContainer> output = new List<IMyCargoContainer> ();
	GridTerminalSystem.GetBlocksOfType<IMyCargoContainer> (cargo);
	for (int i = 0; i < cargo.Count; i++) {
		output.Add ((IMyCargoContainer) cargo[i]);
	}
	return output;
}
List<IMyShipConnector> GetConnectors () {
	List<IMyTerminalBlock> connector = new List<IMyTerminalBlock> ();
	List<IMyShipConnector> output = new List<IMyShipConnector> ();
	GridTerminalSystem.GetBlocksOfType<IMyShipConnector> (connector);
	for (int i = 0; i < connector.Count; i++) {
		output.Add ((IMyShipConnector) connector[i]);
	}
	return output;
}
void MoveAllItems (IMyInventory sourceInv, IMyInventory destinationInv) {

	List<MyInventoryItem> sourceItems = new List<MyInventoryItem> ();

	sourceInv.GetItems (sourceItems);

	MyFixedPoint sourceMass = sourceInv.CurrentMass;
	if (sourceMass > 0) {

		foreach (MyInventoryItem item in sourceItems) {
			sourceInv.TransferItemTo (destinationInv, item, item.Amount);
		}

	}

}

public Program () {
	Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Main () {
	IMyCargoContainer largeInv1 = GridTerminalSystem.GetBlockWithName ("Large Cargo Inventory 1") as IMyCargoContainer;
	IMyTextPanel LCDPanel = GridTerminalSystem.GetBlockWithName ("Inventory Manager Panel") as IMyTextPanel;
	IMyInventory invLargeInv1 = largeInv1.GetInventory (0);
	int i = 0;

	List<IMyShipConnector> ListOfConnectors = GetConnectors ();
	List<IMyAssembler> ListOfAssemblers = GetAssemblers ();
	List<IMyCargoContainer> ListOfCargo = GetCargoContainers ();
	foreach (IMyAssembler assembler in ListOfAssemblers) // Loop through List with foreach
	{
		IMyInventory assemInventory = assembler.GetInventory (1);
		MoveAllItems (assemInventory, invLargeInv1);
	}
	MyFixedPoint currentInvVolume = invLargeInv1.CurrentVolume;
	MyFixedPoint maxInvVolume = invLargeInv1.MaxVolume;

	LCDPanel.WriteText ("Inventory Storage Space: \n " + currentInvVolume.ToString () + " out of " + maxInvVolume.ToString ());
}