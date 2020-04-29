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

string gearsName;
string thrustersName;
string connectorsName;
string reactorsName;
string tanksName;
string batteriesName;

public Program() {
    Runtime.UpdateFrequency = UpdateFrequency.None;

    MyIniParseResult data;
    if (!this._ini.TryParse(Me.CustomData, out data)) {
        throw new Exception("failed to parse custom data");
    }

    this.gearsName = this._ini.Get("names", "gears").ToString();
    this.thrustersName = this._ini.Get("names", "thrusters").ToString();
    this.connectorsName = this._ini.Get("names", "connectors").ToString();
    this.reactorsName = this._ini.Get("names", "reactors").ToString();
    this.tanksName = this._ini.Get("names", "tanks").ToString();
    this.batteriesName = this._ini.Get("names", "batteries").ToString();
}

public void Main() {
    if (this.status == "launch") {
        Echo("parking");

        Echo("lock the landing gears");
        this.RunActionOnBlocks(this.gearsName, "Lock");

        Echo("turn off engines");
        this.RunActionOnBlocks(this.thrustersName, "OnOff_Off");

        Echo("turn off reactor");
        this.RunActionOnBlocks(this.reactorsName, "OnOff_Off");

        Echo("stockpile tanks");
        this.RunActionOnBlocks(this.tanksName, "Stockpile_On");

        Echo("recharge batteries");
        this.RunActionOnBlocks(this.batteriesName, "Recharge");

        Echo("lock connector");
        this.RunActionOnBlocks(this.connectorsName, "Lock");

        this.status = "park";
    } else if (this.status == "park") {
        Echo("launching");
        Echo("enable batteries");
        this.RunActionOnBlocks(this.batteriesName, "Auto");

        Echo("stop stockpile");
        this.RunActionOnBlocks(this.tanksName, "Stockpile_Off");

        Echo("turn on reactor");
        this.RunActionOnBlocks(this.reactorsName, "OnOff_On");

        Echo("turn on engines");
        this.RunActionOnBlocks(this.thrustersName, "OnOff_On");

        Echo("lock the landing gears");
        this.RunActionOnBlocks(this.gearsName, "Lock");

        Echo("unlock connector");
        this.RunActionOnBlocks(this.connectorsName, "Unlock");

        this.status = "launch";
    } else {
        throw new Exception("Unrecognized status " + this.status);
    }
}

void RunActionOnBlocks(string name, string action) {
    Echo("running <" + action + "> on block or group with name <" + name + ">");
    if (name != null && name != "") {
        List<IMyTerminalBlock> blocks = this.GetBlockGroup(name);
        foreach (IMyTerminalBlock block in blocks) {
            Echo("Do action");
            block.GetActionWithName(action).Apply(block);
        }
    }
}

List<IMyTerminalBlock> GetBlockGroup(string name) {
    List<IMyTerminalBlock> blocks = new List<IMyTerminalBlock>();
    var pistonsBlock = GridTerminalSystem.GetBlockGroupWithName(name);
    if (pistonsBlock != null) {
        Echo("Found block group");
        pistonsBlock.GetBlocks(blocks);
    } else {
        Echo("Look for individual block");
        IMyTerminalBlock block = GridTerminalSystem.GetBlockWithName(name);

        if (block != null) {
            Echo("Found block");
            blocks.Add(block);
        }

        Echo("found nothing");
    }
    return blocks;
}