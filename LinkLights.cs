/******************

Ensures lights are only on when a given target is on

[names]
lights=My Lights Names
targets=My Targets Names

********************/

MyIni _ini = new MyIni();

string lightsName;
string targetsName;

public Program() {
    MyIniParseResult data;
    if (!this._ini.TryParse(Me.CustomData, out data)) {
        throw new Exception("failed to parse custom data");
    }
    Runtime.UpdateFrequency = UpdateFrequency.10;

    this.lightsName = this._ini.Get("names", "lights").ToString();
    this.targetsName = this._ini.Get("names", "targets").ToString();
}

public void Main() {
    List<IMyTerminalBlock> targets = this.GetBlockGroup(targetsName);
    string action = "OnOff_Off";

    foreach (IMyTerminalBlock target in targets) {
        if ((target as IMyFunctionalBlock).Enabled) {
            action = "OnOff_On";
            break;
        }
    }

    this.RunActionOnBlocks(this.lightsName, action);    
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
    var blockGroup = GridTerminalSystem.GetBlockGroupWithName(name);
    if (blockGroup != null) {
        Echo("Found block group");
        blockGroup.GetBlocks(blocks);
    } else {
        Echo("Look for individual block");
        IMyTerminalBlock block = GridTerminalSystem.GetBlockWithName(name);

        if (block != null) {
            Echo("Found block");
            blocks.Add(block);
        } else {
            Echo("found nothing");
        }
    }
    return blocks;
}