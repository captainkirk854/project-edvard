namespace Helpers
{
    using System.Collections.Generic;
    using System.Linq;

    /// <summary>
    /// Dictionary containing mappings between Elite Dangerous Actions and Voice Attack Actions
    /// </summary>
    public class ActionExchange
    {
        private const string EDActionUndefined = "Elite Dangerous Action: Undefined";
        private const string VAActionUndefined = "VoiceAttack Action: Undefined";
        private Dictionary<string, string> relationship = new Dictionary<string, string>();

        /// <summary>
        /// Map Elite Dangerous Action Term to Voice Attack Action Term
        /// </summary>
        public void Initialise()
        {
            this.relationship.Add("BackwardKey", VAActionUndefined);
            this.relationship.Add("BackwardThrustButton", VAActionUndefined);
            this.relationship.Add("BackwardThrustButton_Landing", VAActionUndefined);
            this.relationship.Add("CamPitchDown", VAActionUndefined);
            this.relationship.Add("CamPitchUp", VAActionUndefined);
            this.relationship.Add("CamTranslateBackward", VAActionUndefined);
            this.relationship.Add("CamTranslateDown", VAActionUndefined);
            this.relationship.Add("CamTranslateForward", VAActionUndefined);
            this.relationship.Add("CamTranslateLeft", VAActionUndefined);
            this.relationship.Add("CamTranslateRight", VAActionUndefined);
            this.relationship.Add("CamTranslateUp", VAActionUndefined);
            this.relationship.Add("CamTranslateZHold", VAActionUndefined);
            this.relationship.Add("CamYawLeft", VAActionUndefined);
            this.relationship.Add("CamYawRight", VAActionUndefined);
            this.relationship.Add("CamZoomIn", VAActionUndefined);
            this.relationship.Add("CamZoomOut", VAActionUndefined);
            this.relationship.Add("CycleFireGroupNext", "((Next Fire Group))");
            this.relationship.Add("CycleFireGroupPrevious", "((Previous Fire Group))");
            this.relationship.Add("CycleNextHostileTarget", "((Cycle Next Hostile Ship))");
            this.relationship.Add("CycleNextPanel", VAActionUndefined);
            this.relationship.Add("CycleNextSubsystem", "((Cycle Next Subsystem))");
            this.relationship.Add("CycleNextTarget", "((Cycle Next Ship))");
            this.relationship.Add("CyclePreviousHostileTarget", "((Cycle Previous Hostile Ship))");
            this.relationship.Add("CyclePreviousPanel", VAActionUndefined);
            this.relationship.Add("CyclePreviousSubsystem", "((Cycle Previous Subsystem))");
            this.relationship.Add("CyclePreviousTarget", "((Cycle Previous Ship))");
            this.relationship.Add("DeployHardpointToggle", "((Hardpoints))");
            this.relationship.Add("DeployHeatSink", "((Heatsink))");
            this.relationship.Add("DisableRotationCorrectToggle", VAActionUndefined);
            this.relationship.Add("DownThrustButton", VAActionUndefined);
            this.relationship.Add("DownThrustButton_Landing", VAActionUndefined);
            this.relationship.Add("EjectAllCargo", VAActionUndefined);
            this.relationship.Add("FireChaffLauncher", "((Chaff))");
            this.relationship.Add("FocusCommsPanel", "((Comms Panel))");
            this.relationship.Add("FocusLeftPanel", VAActionUndefined);
            this.relationship.Add("FocusRadarPanel", "((Radar))");
            this.relationship.Add("ForwardKey", VAActionUndefined);
            this.relationship.Add("ForwardThrustButton", VAActionUndefined);
            this.relationship.Add("ForwardThrustButton_Landing", VAActionUndefined);
            this.relationship.Add("GalaxyMapOpen", "((Galaxy Map))");
            this.relationship.Add("HeadLookPitchDown", VAActionUndefined);
            this.relationship.Add("HeadLookPitchUp", VAActionUndefined);
            this.relationship.Add("HeadLookReset", "((Look Ahead))");
            this.relationship.Add("HeadLookToggle", VAActionUndefined);
            this.relationship.Add("HeadLookYawLeft", VAActionUndefined);
            this.relationship.Add("HeadLookYawRight", VAActionUndefined);
            this.relationship.Add("Hyperspace", VAActionUndefined);
            this.relationship.Add("HyperSuperCombination", "((Jump Drive))");
            this.relationship.Add("IncreaseEnginesPower", "((Power To Engines))");
            this.relationship.Add("IncreaseSystemsPower", "((Power To Systems))");
            this.relationship.Add("IncreaseWeaponsPower", "((Power To Weapons))");
            this.relationship.Add("KeyAction", VAActionUndefined);
            this.relationship.Add("LandingGearToggle", "((Landing Gear))");
            this.relationship.Add("LeftThrustButton", VAActionUndefined);
            this.relationship.Add("LeftThrustButton_Landing", VAActionUndefined);
            this.relationship.Add("MicrophoneMute", VAActionUndefined);
            this.relationship.Add("MouseReset", VAActionUndefined);
            this.relationship.Add("OculusReset", VAActionUndefined);
            this.relationship.Add("Pause", VAActionUndefined);
            this.relationship.Add("PhotoCameraToggle", "((External camera))");
            this.relationship.Add("PitchDownButton", VAActionUndefined);
            this.relationship.Add("PitchDownButton_Landing", VAActionUndefined);
            this.relationship.Add("PitchUpButton", VAActionUndefined);
            this.relationship.Add("PitchUpButton_Landing", VAActionUndefined);
            this.relationship.Add("PrimaryFire", VAActionUndefined);
            this.relationship.Add("QuickCommsPanel", VAActionUndefined);
            this.relationship.Add("RadarDecreaseRange", "((Decrease Sensor Range))");
            this.relationship.Add("RadarIncreaseRange", "((Increase Sensor range))");
            this.relationship.Add("ResetPowerDistribution", "((Balance Power))");
            this.relationship.Add("RightThrustButton", VAActionUndefined);
            this.relationship.Add("RightThrustButton_Landing", VAActionUndefined);
            this.relationship.Add("RollLeftButton", VAActionUndefined);
            this.relationship.Add("RollLeftButton_Landing", VAActionUndefined);
            this.relationship.Add("RollRightButton", VAActionUndefined);
            this.relationship.Add("RollRightButton_Landing", VAActionUndefined);
            this.relationship.Add("SecondaryFire", VAActionUndefined);
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
            this.relationship.Add("ToggleReverseThrottleInput", VAActionUndefined);
            this.relationship.Add("UI_Back", VAActionUndefined);
            this.relationship.Add("UI_Down", VAActionUndefined);
            this.relationship.Add("UI_Left", VAActionUndefined);
            this.relationship.Add("UI_Right", VAActionUndefined);
            this.relationship.Add("UI_Select", VAActionUndefined);
            this.relationship.Add("UI_Up", VAActionUndefined);
            this.relationship.Add("UIFocus", VAActionUndefined);
            this.relationship.Add("UpThrustButton", VAActionUndefined);
            this.relationship.Add("UpThrustButton_Landing", VAActionUndefined);
            this.relationship.Add("UseBoostJuice", "((Afterburners))");
            this.relationship.Add("UseShieldCell", "((Shield Cell))");
            this.relationship.Add("WingNavLock", "((Wingman NavLock))");
            this.relationship.Add("YawLeftButton", VAActionUndefined);
            this.relationship.Add("YawLeftButton_Landing", VAActionUndefined);
            this.relationship.Add("YawRightButton", VAActionUndefined);
            this.relationship.Add("YawRightButton_Landing", VAActionUndefined);
            this.relationship.Add("YawToRollButton", VAActionUndefined);
        }

        /// <summary>
        /// Get VA Action
        /// </summary>
        /// {Dictionary Value}
        /// <param name="actionED"></param>
        /// <returns></returns>
        public string GetVA(string actionED)
        {
            try
            {
                return this.relationship[actionED];
            }
            catch
            {
                return EDActionUndefined;
            }
        }

        /// <summary>
        /// Get ED Action
        /// </summary>
        /// {Dictionary Key}
        /// <param name="actionVA"></param>
        /// <returns></returns>
        public string GetED(string actionVA)
        {
            // Specials for these Voice Attack commands ...
            if (actionVA == "((Decrease Sensor Range Max))")
            {
                return "RadarDecreaseRange";
            }

            if (actionVA == "((Increase Sensor Range Max))")
            {
                return "RadarIncreaseRange";
            }

            // Find value from key ..
            string keyValue = this.relationship.FirstOrDefault(x => x.Value == actionVA).Key;
            
            // return with key value or default if null
            return keyValue != null ? keyValue : VAActionUndefined;
        }
    }
}