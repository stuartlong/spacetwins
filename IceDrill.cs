string status = "";
int x = 0;
int y = 0;
int z = 0;

public Program() {
    Runtime.UpdateFrequency = UpdateFrequency.Update100;
    this.x = this.ExtendedIndex("X");
    this.y = this.ExtendedIndex("Y");
    this.z = this.ExtendedIndex("Z");
}

int ExtendedIndex(string axis) {
    List<IMyTerminalBlock> pistons = this.GetPistons(axis);

    int ret = 0;
    foreach (IMyTerminalBlock piston in pistons) {
        if ((piston as IMyPistonBase).Status == PistonStatus.Extended) {
            ret++;
        }
    }
    return ret;
}

List<IMyTerminalBlock> GetPistons(string axis) {
    List<IMyTerminalBlock> pistons = new List<IMyTerminalBlock>();
    var pistonsBlock = GridTerminalSystem.GetBlockGroupWithName("Ice Drill - Pistons " + axis);
    pistonsBlock.GetBlocks(pistons);
    return pistons;
}

bool Retract(string axis) {
    if (axis == "Z") {
        this.ToggleDrills(false);
    }

    List<IMyTerminalBlock> pistons = this.GetPistons(axis);

    bool retracted = true;
    foreach (IMyTerminalBlock piston in pistons) {
        if ((piston as IMyPistonBase).Status == PistonStatus.Extended) {
            piston.GetActionWithName("Retract").Apply(piston);
        }

        // retracting all the Zs at once is too bumpy
        if (axis == "Z" && (piston as IMyPistonBase).Status != PistonStatus.Retracted) {
            return false;
        }

        retracted = retracted && (piston as IMyPistonBase).Status == PistonStatus.Retracted;
    }

    if (axis == "Y") {
        List<IMyTerminalBlock> plusPistons = this.GetPistons("Y+");
        foreach (IMyTerminalBlock plusPiston in plusPistons) {
            plusPiston.GetActionWithName("Extend").Apply(plusPiston);
        }
    }

    return retracted;
}

bool Extend(string axis, int index, string nextAxis) {
    List<IMyTerminalBlock> pistons = this.GetPistons(axis);

    if (index == pistons.Count) {
        Echo("Extend - hit max");

        if (nextAxis == null) {
            this.status = "reset";
            Echo("Extend - try reset");
        } else {
            Echo("Extend - hit max - extend " + nextAxis);
            this.status = "extend - " + nextAxis;
        }

        return false;
    }

    IMyPistonBase piston = pistons[index] as IMyPistonBase;

    Echo("Extend - Status: " + piston.Status.ToString());
    if (piston.Status == PistonStatus.Extending) {
        Echo("Extend - currently extending");
        return false;
    }

    if (piston.Status == PistonStatus.Extended) {
        Echo("Extend - currently Extended");
        return true;
    }

    piston.GetActionWithName("Extend").Apply(piston);

    if (axis == "Y") {
        List<IMyTerminalBlock> plusPistons = this.GetPistons("Y+");
        plusPistons[index].GetActionWithName("Retract").Apply(plusPistons[index]);
    }
    return false;
}

void ToggleDrills(bool on) {
    Echo("turning drills" + on.ToString());
    List<IMyTerminalBlock> drills = new List<IMyTerminalBlock>();
    var drillGroup = GridTerminalSystem.GetBlockGroupWithName("Ice Drill - Drills");
    drillGroup.GetBlocks(drills);

    foreach (IMyTerminalBlock drill in drills) {
        string action = "OnOff_Off";
        if (on) {
            action = "OnOff_On";
        }
        drill.GetActionWithName(action).Apply(drill);
    }

}

bool RoomToRun() {
    var cargoGroup = GridTerminalSystem.GetBlockGroupWithName("Large Cargo - Ice");
    List<IMyTerminalBlock> cargo = new List<IMyTerminalBlock>();
    cargoGroup.GetBlocks(cargo);

    foreach (IMyTerminalBlock container in cargo) {
        IMyInventory inv = container.GetInventory(0);
        MyFixedPoint currentVolume = inv.CurrentVolume;
        if (currentVolume.RawValue < 1000) {
            return true;
        }
    }

    return false;
}

public void Main(string argument, UpdateType updateSource) {
    Echo("Main - " + argument);
    Echo("Status - " + this.status);
    Echo("x: " + this.x);
    Echo("y: " + this.y);
    Echo("z: " + this.z);

    if (argument != "" && argument != null) {
        Echo("setting status to " + "" + argument);
        this.status = argument;
    }

    if (this.status == "extend - Z") {
        if (this.z == 0) {
            if (!this.RoomToRun()) {
                return;
            }
            this.ToggleDrills(true);
        }

        if (this.Extend("Z", this.z, "Y")) {
            Echo("incrementing z");
            this.z++;
        }

        return;
    }

    if (this.status == "extend - Y") {
        if (this.z > 0) {
            if (this.Retract("Z")) {
                this.z = 0;
            }
            return;
        }

        if (this.Extend("Y", this.y, "X")) {
            this.y++;
            this.status = "extend - Z";
        }

        return;
    }

    if (this.status == "extend - X") {
        if (this.z > 0) {
            if (this.Retract("Z")) {
                this.z = 0;
            }
            return;
        }

        if (this.y > 0) {
            if (this.Retract("Y")) {
                this.y = 0;
            }
            return;
        }

        if (this.Extend("X", this.x, null)) {
            this.x++;
            this.status = "extend - Z";
        }
        return;
    }

    if (this.status == "reset") {
        if (this.z > 0) {
            if (this.Retract("Z")) {
                this.z = 0;
            }
            return;
        }

        if (this.y > 0) {
            if (this.Retract("Y")) {
                this.y = 0;
            }
            return;
        }

        if (this.x > 0) {
            if (this.Retract("X")) {
                this.x = 0;
            }
            return;
        }
    }
}