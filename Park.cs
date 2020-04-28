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
        // lock the landing gears
        this.RunActionOnBlocks("gears", "Lock");

        // turn off engines
        this.RunActionOnBlocks("thrusters", "OnOff_Off");

        // lock connector
        this.RunActionOnBlocks("connector", "Connect");
        
        // turn off reactor
        this.RunActionOnBlocks("reactor", "OnOff_Off");

        // stockpile tanks
        this.RunActionOnBlocks("tanks", "Stockpile_On");

        // recharge batteries
        this.RunActionOnBlocks("batteries", "Recharge");

        this.status = "park";
    } else if (this.status == "park") {
        Echo("launching");
        // enable batteries
        this.RunActionOnBlocks("batteries", "Auto");

        // stop stockpile
        this.RunActionOnBlocks("tanks", "Stockpile_Off");
        
        // turn on reactor
        this.RunActionOnBlocks("reactor", "OnOff_On");

        // ulock connector
        this.RunActionOnBlocks("connector", "Disconnect");

        // turn on engines
        this.RunActionOnBlocks("thrusters", "OnOff_On");

        // lock the landing gears
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