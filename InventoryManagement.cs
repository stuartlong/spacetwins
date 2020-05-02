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
	IMyCargoContainer tempIce = GridTerminalSystem.GetBlockWithName("Temp Ice Storage") as IMyCargoContainer;
	IMyCargoContainer largeOre2 = GridTerminalSystem.GetBlockWithName("Large Cargo Ore 2") as IMyCargoContainer;

	IMyInventory invLargeInv1 = largeInv1.GetInventory(0);
	IMyInventory oreLargeInv1 = largeOre1.GetInventory(0);
	IMyInventory tempIceInv = tempIce.GetInventory(0);
	IMyInventory oreLargeInv2 = largeOre2.GetInventory(0);

	IMyTextPanel LCDPanel = GridTerminalSystem.GetBlockWithName("Inventory Manager Panel") as IMyTextPanel;
	List<IMyShipConnector> ListOfConnectors = GetConnectors();
	List<IMyAssembler> ListOfAssemblers = GetAssemblers();
	List<IMyCargoContainer> ListOfCargo = GetCargoContainers();
	
	//loop through the assemblers and move any completed items to the inventory cargo container
	foreach (IMyAssembler assembler in ListOfAssemblers) 
	{
		IMyInventory assemInventory = assembler.GetInventory(1);
		MoveAllItems(assemInventory, invLargeInv1);
		
		IMyInventory assemInventoryOre = assembler.GetInventory(0);
		if(assembler.CurrentProgress <= 0.0)
		{
			MoveAllItems(assemInventoryOre, oreLargeInv1);
			MoveAllItems(assemInventoryOre, oreLargeInv2);
		}
	}
	//loop through the connecters and move any ore to the ore container and any components to the sorter
	foreach (IMyShipConnector connector in ListOfConnectors)
	{
		
		IMyInventory connectorInventory = connector.GetInventory(0);
		//The sorters in the base should deal with not moving items where they don't belong.
		if(largeInv1.CubeGrid == connector.CubeGrid){
			
			MoveAllItems(connectorInventory,invLargeInv1);
		}
		MoveAllItems(connectorInventory,oreLargeInv1);
		MoveAllItems(connectorInventory, oreLargeInv2);
	}
	
	//move ice
	foreach (IMyCargoContainer largeIce in ListOfCargo)
	{
		IMyInventory largeIceInv = largeIce.GetInventory(0);
		if(largeIce.CustomName.Contains("Large Ice Cargo"))
		{
			MoveAllItems(largeIceInv,tempIceInv);
		} 
	}
	MyFixedPoint currentInvVolume = invLargeInv1.CurrentVolume;
	MyFixedPoint maxInvVolume = invLargeInv1.MaxVolume;

	LCDPanel.WriteText("Inventory Storage Space: \n " + currentInvVolume.ToString() + " out of " + maxInvVolume.ToString());
}
