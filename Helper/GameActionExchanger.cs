namespace Helper
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Dictionary containing mappings between Elite Dangerous and Voice Attack Bindable Commands
    /// </summary>
    public class GameActionExchanger
    {
        private const string EDCommandUndefined = "Elite Dangerous Command: Undefined";
        private const string VACommandUndefined = "VoiceAttack Command: Undefined";
        private Dictionary<string, string> relationship = new Dictionary<string, string>();

        /// <summary>
        /// Map Elite Dangerous Action Term to Voice Attack Action Term
        /// </summary>
        public void Initialise()
        {
            this.relationship.Add("BackwardKey", VACommandUndefined);
            this.relationship.Add("BackwardThrustButton", VACommandUndefined);
            this.relationship.Add("BackwardThrustButton_Landing", VACommandUndefined);
            this.relationship.Add("CamPitchDown", VACommandUndefined);
            this.relationship.Add("CamPitchUp", VACommandUndefined);
            this.relationship.Add("CamTranslateBackward", VACommandUndefined);
            this.relationship.Add("CamTranslateDown", VACommandUndefined);
            this.relationship.Add("CamTranslateForward", VACommandUndefined);
            this.relationship.Add("CamTranslateLeft", VACommandUndefined);
            this.relationship.Add("CamTranslateRight", VACommandUndefined);
            this.relationship.Add("CamTranslateUp", VACommandUndefined);
            this.relationship.Add("CamTranslateZHold", VACommandUndefined);
            this.relationship.Add("CamYawLeft", VACommandUndefined);
            this.relationship.Add("CamYawRight", VACommandUndefined);
            this.relationship.Add("CamZoomIn", VACommandUndefined);
            this.relationship.Add("CamZoomOut", VACommandUndefined);
            this.relationship.Add("CycleFireGroupNext", "((Next Fire Group))");
            this.relationship.Add("CycleFireGroupPrevious", "((Previous Fire Group))");
            this.relationship.Add("CycleNextHostileTarget", "((Cycle Next Hostile Ship))");
            this.relationship.Add("CycleNextPanel", VACommandUndefined);
            this.relationship.Add("CycleNextSubsystem", "((Cycle Next Subsystem))");
            this.relationship.Add("CycleNextTarget", "((Cycle Next Ship))");
            this.relationship.Add("CyclePreviousHostileTarget", "((Cycle Previous Hostile Ship))");
            this.relationship.Add("CyclePreviousPanel", VACommandUndefined);
            this.relationship.Add("CyclePreviousSubsystem", "((Cycle Previous Subsystem))");
            this.relationship.Add("CyclePreviousTarget", "((Cycle Previous Ship))");
            this.relationship.Add("DeployHardpointToggle", "((Hardpoints))");
            this.relationship.Add("DeployHeatSink", "((Heatsink))");
            this.relationship.Add("DisableRotationCorrectToggle", VACommandUndefined);
            this.relationship.Add("DownThrustButton", VACommandUndefined);
            this.relationship.Add("DownThrustButton_Landing", VACommandUndefined);
            this.relationship.Add("EjectAllCargo", VACommandUndefined);
            this.relationship.Add("FireChaffLauncher", "((Chaff))");
            this.relationship.Add("FocusCommsPanel", "((Comms Panel))");
            this.relationship.Add("FocusLeftPanel", VACommandUndefined);
            this.relationship.Add("FocusRadarPanel", "((Radar))");
            this.relationship.Add("ForwardKey", VACommandUndefined);
            this.relationship.Add("ForwardThrustButton", VACommandUndefined);
            this.relationship.Add("ForwardThrustButton_Landing", VACommandUndefined);
            this.relationship.Add("GalaxyMapOpen", "((Galaxy Map))");
            this.relationship.Add("HeadLookPitchDown", VACommandUndefined);
            this.relationship.Add("HeadLookPitchUp", VACommandUndefined);
            this.relationship.Add("HeadLookReset", "((Look Ahead))");
            this.relationship.Add("HeadLookToggle", VACommandUndefined);
            this.relationship.Add("HeadLookYawLeft", VACommandUndefined);
            this.relationship.Add("HeadLookYawRight", VACommandUndefined);
            this.relationship.Add("Hyperspace", VACommandUndefined);
            this.relationship.Add("HyperSuperCombination", "((Jump Drive))");
            this.relationship.Add("IncreaseEnginesPower", "((Power To Engines))");
            this.relationship.Add("IncreaseSystemsPower", "((Power To Systems))");
            this.relationship.Add("IncreaseWeaponsPower", "((Power To Weapons))");
            this.relationship.Add("KeyAction", VACommandUndefined);
            this.relationship.Add("LandingGearToggle", "((Landing Gear))");
            this.relationship.Add("LeftThrustButton", VACommandUndefined);
            this.relationship.Add("LeftThrustButton_Landing", VACommandUndefined);
            this.relationship.Add("MicrophoneMute", VACommandUndefined);
            this.relationship.Add("MouseReset", VACommandUndefined);
            this.relationship.Add("OculusReset", VACommandUndefined);
            this.relationship.Add("Pause", VACommandUndefined);
            this.relationship.Add("PhotoCameraToggle", "((External camera))");
            this.relationship.Add("PitchDownButton", VACommandUndefined);
            this.relationship.Add("PitchDownButton_Landing", VACommandUndefined);
            this.relationship.Add("PitchUpButton", VACommandUndefined);
            this.relationship.Add("PitchUpButton_Landing", VACommandUndefined);
            this.relationship.Add("PrimaryFire", VACommandUndefined);
            this.relationship.Add("QuickCommsPanel", VACommandUndefined);
            this.relationship.Add("RadarDecreaseRange", "((Decrease Sensor Range))");
            this.relationship.Add("RadarIncreaseRange", "((Increase Sensor range))");
            this.relationship.Add("ResetPowerDistribution", "((Balance Power))");
            this.relationship.Add("RightThrustButton", VACommandUndefined);
            this.relationship.Add("RightThrustButton_Landing", VACommandUndefined);
            this.relationship.Add("RollLeftButton", VACommandUndefined);
            this.relationship.Add("RollLeftButton_Landing", VACommandUndefined);
            this.relationship.Add("RollRightButton", VACommandUndefined);
            this.relationship.Add("RollRightButton_Landing", VACommandUndefined);
            this.relationship.Add("SecondaryFire", VACommandUndefined);
            this.relationship.Add("SelectHighestThreat", "((Select Highest Threat))");
            this.relationship.Add("SelectTarget", "((Select Target Ahead))");
            this.relationship.Add("SelectTargetsTarget", "((Wingmans Target))");
            this.relationship.Add("SetSpeed100", "((100%))");
            this.relationship.Add("SetSpeed25", "((25%))");
            this.relationship.Add("SetSpeed50", "((50%))");
            this.relationship.Add("SetSpeed75", "((75%))");
            this.relationship.Add("SetSpeedMinus100", "((100% Reverse))");
            this.relationship.Add("SetSpeedMinus25", "((25% Reverse))");
            this.relationship.Add("SetSpeedMinus50", "((50% Reverse))");
            this.relationship.Add("SetSpeedMinus75", "((75% Reverse))");
            this.relationship.Add("SetSpeedZero", "((0%))");
            this.relationship.Add("ShipSpotLightToggle", "((Lights))");
            this.relationship.Add("Supercruise", "((Supercruise))");
            this.relationship.Add("SystemMapOpen", "((System Map))");
            this.relationship.Add("TargetNextRouteSystem", "((Next Route System))");
            this.relationship.Add("TargetWingman0", "((Wingman 1))");
            this.relationship.Add("TargetWingman1", "((Wingman 2))");
            this.relationship.Add("TargetWingman2", "((Wingman 3))");
            this.relationship.Add("ToggleButtonUpInput", "((Silent Running))");
            this.relationship.Add("ToggleCargoScoop", "((Cargo Scoop))");
            this.relationship.Add("ToggleFlightAssist", "((Flight Assist))");
            this.relationship.Add("ToggleReverseThrottleInput", VACommandUndefined);
            this.relationship.Add("UI_Back", VACommandUndefined);
            this.relationship.Add("UI_Down", VACommandUndefined);
            this.relationship.Add("UI_Left", VACommandUndefined);
            this.relationship.Add("UI_Right", VACommandUndefined);
            this.relationship.Add("UI_Select", VACommandUndefined);
            this.relationship.Add("UI_Up", VACommandUndefined);
            this.relationship.Add("UIFocus", VACommandUndefined);
            this.relationship.Add("UpThrustButton", VACommandUndefined);
            this.relationship.Add("UpThrustButton_Landing", VACommandUndefined);
            this.relationship.Add("UseBoostJuice", "((Afterburners))");
            this.relationship.Add("UseShieldCell", "((Shield Cell))");
            this.relationship.Add("WingNavLock", "((Wingman NavLock))");
            this.relationship.Add("YawLeftButton", VACommandUndefined);
            this.relationship.Add("YawLeftButton_Landing", VACommandUndefined);
            this.relationship.Add("YawRightButton", VACommandUndefined);
            this.relationship.Add("YawRightButton_Landing", VACommandUndefined);
            this.relationship.Add("YawToRollButton", VACommandUndefined);
        }

        /// <summary>
        /// Get VA Command
        /// </summary>
        /// {Dictionary Value}
        /// <param name="commandED"></param>
        /// <returns></returns>
        public string GetVA(string commandED)
        {
            try
            {
                return this.relationship[commandED];
            }
            catch
            {
                return EDCommandUndefined;
            }
        }

        /// <summary>
        /// Get ED Action
        /// </summary>
        /// {Dictionary Key}
        /// <param name="commandVA"></param>
        /// <returns></returns>
        public string GetED(string commandVA)
        {
            // Specials for these Voice Attack commands ...
            if (commandVA == "((Decrease Sensor Range Max))")
            {
                return "RadarDecreaseRange";
            }

            if (commandVA == "((Increase Sensor Range Max))")
            {
                return "RadarIncreaseRange";
            }

            // Find value from key ..
            string keyValue = this.relationship.FirstOrDefault(x => x.Value == commandVA).Key;
            
            // return with key value or default if null
            return keyValue != null ? keyValue : VACommandUndefined;
        }
    }
}