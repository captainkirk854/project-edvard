namespace Binding
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Xml.Linq;

    /// <summary>
    /// Dictionary with mappings between Elite Dangerous Bindings and Voice Attack Commands
    /// </summary>
    public class KeyBindingAndCommandConnector
    {
        private const string UnknownBinding = "*Unknown Binding";
        private const string UnknownCommand = "*Unknown Command";

        // Create dictionary which allows case-insensitive keys/value queries ..
        private Dictionary<string, string> relationship = new Dictionary<string, string>(StringComparer.InvariantCultureIgnoreCase);

        /// <summary>
        /// Initializes a new instance of the <see cref="KeyBindingAndCommandConnector" /> class.
        /// </summary>
        /// <remarks>
        /// A dictionary of Voice Attack Action Term {key} : Elite Dangerous Action Term {value}
        /// Note: There is a many-to-one relationship between VoiceAttack Commands and an Elite Dangerous Action
        /// </remarks>
        public KeyBindingAndCommandConnector()
        {
            ////-----------------------------------------------------------------------------------------------------
            this.relationship.Add("((0%))", "SetSpeedZero");
            this.relationship.Add("((10%))", "ForwardKey");
            this.relationship.Add("((20%))", "ForwardKey");
            this.relationship.Add("((25%))", "SetSpeed25");
            this.relationship.Add("((30%))", "ForwardKey");
            this.relationship.Add("((40%))", "BackwardKey");
            this.relationship.Add("((50%))", "SetSpeed50");
            this.relationship.Add("((60%))", "ForwardKey");
            this.relationship.Add("((70%))", "ForwardKey");
            this.relationship.Add("((75%))", "SetSpeed75");
            this.relationship.Add("((80%))", "BackwardKey");
            this.relationship.Add("((90%))", "BackwardKey");
            this.relationship.Add("((100%))", "SetSpeed100");           
            this.relationship.Add("((25% Reverse))", "SetSpeedMinus25");
            this.relationship.Add("((50% Reverse))", "SetSpeedMinus50");
            this.relationship.Add("((75% Reverse))", "SetSpeedMinus75");
            this.relationship.Add("((100% Reverse))", "SetSpeedMinus100");
            ////-----------------------------------------------------------------------------------------------------
            this.relationship.Add("((Supercruise))", "Supercruise");
            this.relationship.Add("((Jump Drive))", "HyperSuperCombination");
            this.relationship.Add("((Departure 30m))", "VerticalThrustersButton");
            this.relationship.Add("((Planet departure))", "VerticalThrustersButton");
            this.relationship.Add("((Take Off))", "VerticalThrustersButton");
            ////-----------------------------------------------------------------------------------------------------
            this.relationship.Add("((Afterburners))", "UseBoostJuice");
            this.relationship.Add("((Chaff))", "FireChaffLauncher");
            this.relationship.Add("((Cargo Scoop))", "ToggleCargoScoop");
            this.relationship.Add("((Hardpoints))", "DeployHardpointToggle");
            this.relationship.Add("((Heatsink))", "DeployHeatSink");
            this.relationship.Add("((Landing Gear))", "LandingGearToggle");
            this.relationship.Add("((Lights))", "ShipSpotLightToggle");
            this.relationship.Add("((Shield Cell))", "UseShieldCell");
            this.relationship.Add("((Silent Running))", "ToggleButtonUpInput");
            ////-----------------------------------------------------------------------------------------------------
            this.relationship.Add("((Comms Panel))", "FocusCommsPanel");
            this.relationship.Add("((Left Panel))", "FocusLeftPanel");
            this.relationship.Add("((Right Panel))", "FocusRightPanel");
            this.relationship.Add("((Role Panel))", "FocusRadarPanel");
            ////-----------------------------------------------------------------------------------------------------
            this.relationship.Add("((UI Cancel))", "UI_Back");
            this.relationship.Add("((UI Down))", "UI_Down");
            this.relationship.Add("((UI Left))", "UI_Left");
            this.relationship.Add("((UI Previous))", "UI_Left");
            this.relationship.Add("((UI Next))", "UI_Right");
            this.relationship.Add("((UI Right))", "UI_Right");
            this.relationship.Add("((UI Accept))", "UI_Select");
            this.relationship.Add("((UI Up))", "UI_Up");
            this.relationship.Add("((UI Okay))", "UIFocus");
            ////-----------------------------------------------------------------------------------------------------
            this.relationship.Add("((Balance Power))", "ResetPowerDistribution");
            this.relationship.Add("((Power To Engines))", "IncreaseEnginesPower");
            this.relationship.Add("((Power To Systems))", "IncreaseSystemsPower");
            this.relationship.Add("((Power To Weapons))", "IncreaseWeaponsPower");
            ////-----------------------------------------------------------------------------------------------------
            this.relationship.Add("((Next Fire Group))", "CycleFireGroupNext");
            this.relationship.Add("((Previous Fire Group))", "CycleFireGroupPrevious");
            ////-----------------------------------------------------------------------------------------------------
            this.relationship.Add("((Cycle Next Hostile Ship))", "CycleNextHostileTarget");
            this.relationship.Add("((Cycle Next Ship))", "CycleNextTarget");
            this.relationship.Add("((Cycle Next Subsystem))", "CycleNextSubsystem");
            this.relationship.Add("((Cycle Previous Hostile Ship))", "CyclePreviousHostileTarget");
            this.relationship.Add("((Cycle Previous Ship))", "CyclePreviousTarget");
            this.relationship.Add("((Cycle Previous Subsystem))", "CyclePreviousSubsystem");
            ////-----------------------------------------------------------------------------------------------------
            this.relationship.Add("((Radar))", "FocusRadarPanel");
            this.relationship.Add("((Decrease Sensor Range))", "RadarDecreaseRange");
            this.relationship.Add("((Decrease Sensor Range Max))", "RadarDecreaseRange");
            this.relationship.Add("((Increase Sensor Range))", "RadarIncreaseRange");
            this.relationship.Add("((Increase Sensor Range Max))", "RadarIncreaseRange");
            //// ((Discovery Scanner)) (MouseAction)
            ////-----------------------------------------------------------------------------------------------------
            this.relationship.Add("((Galaxy Map))", "GalaxyMapOpen");
            this.relationship.Add("((System Map))", "SystemMapOpen");
            this.relationship.Add("((Next Route System))", "TargetNextRouteSystem");
            ////-----------------------------------------------------------------------------------------------------
            this.relationship.Add("((Select Highest Threat))", "SelectHighestThreat");
            this.relationship.Add("((Select Target Ahead))", "SelectTarget");
            ////-----------------------------------------------------------------------------------------------------
            this.relationship.Add("((Wingman 1))", "TargetWingman0");
            this.relationship.Add("((Wingman 2))", "TargetWingman1");
            this.relationship.Add("((Wingman 3))", "TargetWingman2");
            this.relationship.Add("((Wingman NavLock))", "WingNavLock");
            this.relationship.Add("((Wingmans Target))", "SelectTargetsTarget");
            ////-----------------------------------------------------------------------------------------------------
            this.relationship.Add("((Fighter Attack Posture))", "OrderAggressiveBehaviour");
            this.relationship.Add("((Fighter Defence Posture))", "OrderDefensiveBehaviour");
            this.relationship.Add("((Fighter Focus Target))", "OrderFocusTarget");
            this.relationship.Add("((Fighter Follow Me))", "OrderFollow");
            this.relationship.Add("((Fighter Hold Fire))", "OrderHoldFire");
            this.relationship.Add("((Fighter Hold Position))", "OrderHoldPosition");
            this.relationship.Add("((Fighter Recall))", "OrderRequestDock");
            this.relationship.Add("((Fighter Recovery))", "OrderRequestDock");
            ////-----------------------------------------------------------------------------------------------------
            this.relationship.Add("((Flight Assist))", "ToggleFlightAssist");
            this.relationship.Add("((Look Ahead))", "HeadLookReset");
            this.relationship.Add("((Oculus Reset))", "HMDReset");
            this.relationship.Add("((External Camera))", "PhotoCameraToggle");
            ////-----------------------------------------------------------------------------------------------------
            this.relationship.Add("((SRV Panel))", "UIFocus_Buggy");
            this.relationship.Add("((Handbrake Off))", "AutoBreakBuggyButton");
            this.relationship.Add("((Handbrake On))", "AutoBreakBuggyButton");
            this.relationship.Add("((SRV Dismiss/Recall))", "RecallDismissShip");
            ////-----------------------------------------------------------------------------------------------------
        }

        /// <summary>
        /// Get ED Action
        /// </summary>
        /// {Dictionary Value}
        /// <param name="command"></param>
        /// <returns></returns>
        public string GetEliteDangerousBinding(string command)
        {
            try
            {
                return this.relationship[command];
            }
            catch
            {
                return UnknownBinding;
            }
        }

        /// <summary>
        /// Get VA Command
        /// </summary>
        /// {Dictionary Key}
        /// <param name="binding"></param>
        /// <returns></returns>
        public string GetVoiceAttackCommand(string binding)
        {
            // Initialise ..
            string keyValue = string.Empty;

            // Look to derive value from key ..
            if (keyValue == null)
            {
                keyValue = this.relationship.FirstOrDefault(x => x.Value == binding).Key;
            }
            
            // return with key value or default if null
            return keyValue != null ? keyValue : UnknownCommand;
        }

        /// <summary>
        /// Export internal dictionary as key-value XML
        /// </summary>
        /// <param name="xmlFilepath"></param>
        public void Export(string xmlFilepath)
        {
            // Convert dictionary to XML (key value (ED Action) first as key (VA Command) uses illegal characters (((,)))..
            XElement el = new XElement("root", this.relationship.Select(kv => new XElement(kv.Value, kv.Key)));

            // Save ..
            el.Save(xmlFilepath);
        }

        /// <summary>
        /// Import key-value XML as internal dictionary
        /// </summary>
        /// <param name="xmlFilepath"></param>
        public void Import(string xmlFilepath)
        {
            // Convert to XML to key-value dictionary<string, string> ..
            XElement root = XElement.Load(xmlFilepath);

            // Clear existing dictionary created in class constructor ..
            this.relationship.Clear();

            // Traverse XML, adding each element to dictionary (Value is actually Dictionary key) ..
            foreach (var el in root.Elements())
            {
                this.relationship.Add(el.Value, el.Name.LocalName);
            }
        }
    }
}