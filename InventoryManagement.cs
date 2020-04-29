List<IMyAssembler> GetAssemblers() {
	List<IMyTerminalBlock> assemblers = new List<IMyTerminalBlock>();
	List<IMyAssembler> output = new List<IMyAssembler>();
	GridTerminalSystem.GetBlocksOfType<IMyAssembler>(assemblers);
	for (int i = 0; i < assemblers.Count; i++) {
		output.Add((IMyAssembler) assemblers[i]);
	}
	return output;
}
List<IMyCargoContainer> GetCargoContainers() {
	List<IMyTerminalBlock> cargo = new List<IMyTerminalBlock>();
	List<IMyCargoContainer> output = new List<IMyCargoContainer>();
	GridTerminalSystem.GetBlocksOfType<IMyCargoContainer>(cargo);
	for (int i = 0; i < cargo.Count; i++) {
		output.Add((IMyCargoContainer) cargo[i]);
	}
	return output;
}
List<IMyShipConnector> GetConnectors() {
	List<IMyTerminalBlock> connector = new List<IMyTerminalBlock>();
	List<IMyShipConnector> output = new List<IMyShipConnector>();
	GridTerminalSystem.GetBlocksOfType<IMyShipConnector>(connector);
	for (int i = 0; i < connector.Count; i++) {
		output.Add((IMyShipConnector) connector[i]);
	}
	return output;
}
void MoveAllItems(IMyInventory sourceInv, IMyInventory destinationInv) {

	List<MyInventoryItem> sourceItems = new List<MyInventoryItem>();

	sourceInv.GetItems(sourceItems);

	MyFixedPoint sourceMass = sourceInv.CurrentMass;
	if (sourceMass > 0) {

		foreach (MyInventoryItem item in sourceItems) {
			sourceInv.TransferItemTo(destinationInv, item, item.Amount);
		}

	}

}

public Program() {
	Runtime.UpdateFrequency = UpdateFrequency.Update100;
}

public void Main() {
	IMyCargoContainer largeInv1 = GridTerminalSystem.GetBlockWithName("Large Cargo Inventory 1") as IMyCargoContainer;
	IMyCargoContainer largeOre1 = GridTerminalSystem.GetBlockWithName("Large Cargo Ore 1") as IMyCargoContainer;
	

	IMyInventory invLargeInv1 = largeInv1.GetInventory(0);
	IMyInventory oreLargeInv1 = largeOre1.GetInventory(0);

	IMyTextPanel LCDPanel = GridTerminalSystem.GetBlockWithName("Inventory Manager Panel") as IMyTextPanel;
	List<IMyShipConnector> ListOfConnectors = GetConnectors();
	List<IMyAssembler> ListOfAssemblers = GetAssemblers();
	List<IMyCargoContainer> ListOfCargo = GetCargoContainers();
	
	//loop through the assemblers and move any completed items to the inventory cargo container
	foreach (IMyAssembler assembler in ListOfAssemblers) 
	{
		IMyInventory assemInventory = assembler.GetInventory(1);
		MoveAllItems(assemInventory, invLargeInv1);
	}
	//loop through the connecters and move any ore to the ore container and any components to the sorter
	foreach (IMyConnector connector in ListOfConnectors)
	{
		
		IMyInventory connectorInventory = connector.GetInventory(0);
		//The sorters in the base should deal with not moving items where they don't belong. 
		MoveAllItems(connectorInventory,invLargeInv1);
	}
	MyFixedPoint currentInvVolume = invLargeInv1.CurrentVolume;
	MyFixedPoint maxInvVolume = invLargeInv1.MaxVolume;

	LCDPanel.WriteText("Inventory Storage Space: \n " + currentInvVolume.ToString() + " out of " + maxInvVolume.ToString());
}
