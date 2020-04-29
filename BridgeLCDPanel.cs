public Program()
{
Runtime.UpdateFrequency = UpdateFrequency.Update10;
}

void MaintainAirlock(IMyDoor door1, IMyDoor door2)
	{
	string turn_off = "OnOff_Off";
	string turn_on = "OnOff_On";
		if(door1.Status== DoorStatus.Open || door1.Status == DoorStatus.Opening)
			{
				if(door1.Enabled){
					door2.CloseDoor();
					door2.GetActionWithName(turn_off).Apply(door2);
				}else
				{
					door2.GetActionWithName(turn_on).Apply(door2);
					door2.CloseDoor();
					door2.GetActionWithName(turn_off).Apply(door2);
				}
				

			}
		if(door2.Status== DoorStatus.Open || door2.Status == DoorStatus.Opening)
			{
				if(door2.Enabled){
					door1.CloseDoor();
					door1.GetActionWithName(turn_off).Apply(door1);
				}else
				{
					door1.GetActionWithName(turn_on).Apply(door1);
					door1.CloseDoor();
					door1.GetActionWithName(turn_off).Apply(door1);
				}
				

			}
		if((door2.Status == DoorStatus.Closed && door1.Status == DoorStatus.Closed))
		{
			door1.GetActionWithName(turn_on).Apply(door1);
			door2.GetActionWithName(turn_on).Apply(door2);
			
		}
			
	}

public void Main()
{
	
	IMyCargoContainer largeCargo= GridTerminalSystem.GetBlockWithName("Large Cargo Inventory 1") as IMyCargoContainer;
	IMyInventory invCargoInv = largeCargo.GetInventory(0);
	IMyTextPanel LCDPanel = GridTerminalSystem.GetBlockWithName("Bridge LCD Panel") as IMyTextPanel;
	//LCDPanel.WriteText("Inventory Storage Space: \n " + currentInvVolume.ToString() + "out of " + maxInvVolume.ToString()+);

	IMyDoor upperInteriorDoor = GridTerminalSystem.GetBlockWithName("Interior Airlock Upper Door") as IMyDoor;
	IMyDoor upperExteriorDoor = GridTerminalSystem.GetBlockWithName("Exterior Airlock Upper Door") as IMyDoor;
	MaintainAirlock(upperInteriorDoor,upperExteriorDoor);
	
	IMyDoor lowerInteriorDoor = GridTerminalSystem.GetBlockWithName("Interior Airlock Lower Door") as IMyDoor;
	IMyDoor lowerExteriorDoor = GridTerminalSystem.GetBlockWithName("Exterior Airlock Lower Door") as IMyDoor;
	MaintainAirlock(lowerInteriorDoor,lowerExteriorDoor);
	
	//IMyDoor lowerExterior 
	//IMyDoor lowerInteriorDoor 
	//MaintainAirlock(

}
