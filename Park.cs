/******************

Auto-parks a ship. Requires relevant block or group names to be supplied in custom data as follows:

[names]
gears=My Landing Gear Names
thrusters=My Thruster Names
connector=My Connector Name
reactors=My Reactors
tanks=My H2 / O2 Tanks
batteries=My batteries

********************/

MyIni _ini = new MyIni();
string status = "park";

public Program() {
    Runtime.UpdateFrequency = UpdateFrequency.None;

    MyIniParseResult data;
    if (!this._ini.TryParse(Me.CustomData, out data)) {
        throw new Exception("failed to parse custom data");
    }
}

public void Main() {
    if (this.status == "launch") {
        Echo("parking");
        Echo("lock the landing gears");
        this.RunActionOnBlocks("gears", "Lock");

        Echo("turn off engines");
        this.RunActionOnBlocks("thrusters", "OnOff_Off");

        Echo("lock connector");
        this.RunActionOnBlocks("connectors", "Connect");
        
        Echo("turn off reactor");
        this.RunActionOnBlocks("reactors", "OnOff_Off");

        Echo("stockpile tanks");
        this.RunActionOnBlocks("tanks", "Stockpile_On");

        Echo("recharge batteries");
        this.RunActionOnBlocks("batteries", "Recharge");

        this.status = "park";
    } else if (this.status == "park") {
        Echo("launching");
        Echo("enable batteries");
        this.RunActionOnBlocks("batteries", "Auto");

        Echo("stop stockpile");
        this.RunActionOnBlocks("tanks", "Stockpile_Off");
        
        Echo("turn on reactor");
        this.RunActionOnBlocks("reactor", "OnOff_On");

        Echo("ulock connector");
        this.RunActionOnBlocks("connector", "Disconnect");

        Echo("turn on engines");
        this.RunActionOnBlocks("thrusters", "OnOff_On");

        Echo("lock the landing gears");
        this.RunActionOnBlocks("gears", "Lock");

        this.status = "launch";
    } else {
        throw new Exception("Unrecognized status " + this.status);
    }
}

void RunActionOnBlocks(string name, string action) {
    string blockNames = this._ini.Get("names", name).ToString();
    if (blockNames != null && blockNames != "") {
        List<IMyTerminalBlock> blocks = this.GetBlockGroup(blockNames);
        foreach(IMyTerminalBlock block in blocks) {
            block.GetActionWithName(action).Apply(block);
        }
    }
}

List<IMyTerminalBlock> GetBlockGroup(string name) {
    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    var pistonsBlock = GridTerminalSystem.GetBlockGroupWithName(name);
    if (pistonsBlock) {
        pistonsBlock.GetBlocks(blocks);
    } else {
        blocks.Add(GridTerminalSystem.GetBlockWithName(name));
    } 
    return blocks;
}