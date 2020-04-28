public Program() {
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    this.current = 0;
}

int current = 0;
public void Main(string argument, UpdateType updateSource) {
    if (this.current == 0) {
        var retract = GridTerminalSystem.GetBlockWithName("Retract Drill");
        retract.GetActionWithName("OnOff_Off").Apply(retract);

        var drill = GridTerminalSystem.GetBlockWithName("Drill");
        drill.GetActionWithName("OnOff_On").Apply(drill);
    }

    var allPistons = new List<IMyTerminalBlock>();
    GridTerminalSystem.SearchBlocksOfName("Drill Piston", allPistons);

    if (this.current >= allPistons.Count) {
        var thisBlock = GridTerminalSystem.GetBlockWithName("Extend Drill");
        this.current = 0;
        thisBlock.GetActionWithName("OnOff_On").Apply(thisBlock);
    }

    var currentPiston = allPistons[this.current];
    currentPiston.GetActionWithName("Extend").Apply(currentPiston);
    if ((currentPiston as IMyPistonBase).Status != PistonStatus.Extended) {
        return;
    }

    this.current++;
}