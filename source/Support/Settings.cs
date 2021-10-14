using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;
using KSP.Localization;

// This will add a tab to the Stock Settings in the Difficulty settings
// To use, reference the setting using the following:
//
// HighLogic.CurrentGame.Parameters.CustomParams<KaboomSettings>()
//
// As it is set up, the option is disabled, so in order to enable it, the player would have
// to deliberately go in and change it
//
namespace Kaboom
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings

    public class KaboomSettings : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "#BOOM-settings-titl"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "#BOOM-settings-sect"; } }
        public override string DisplaySection { get { return "#BOOM-settings-disp"; } }
        public override int SectionOrder { get { return 1; } }


        [GameParameters.CustomParameterUI("#BOOM-settings-softExplode",
            toolTip = "#BOOM-settings-softExplode-tt",
            unlockedDuringMission = true)]
        public bool softExplode = false;

        [GameParameters.CustomParameterUI("#BOOM-settings-coloredPaw",
            toolTip = "#BOOM-settings-coloredPaw-tt",
            unlockedDuringMission = true)]
        public bool coloredPAW = true;

        [GameParameters.CustomParameterUI("#BOOM-settings-xDebug",
            toolTip = "#BOOM-settings-xDebug-tt",
            unlockedDuringMission = true)]
        public bool xDebug = false;

        [GameParameters.CustomParameterUI("#BOOM-settings-xLogging",
            toolTip = "#BOOM-settings-xLogging-tt",
            unlockedDuringMission = true)]
        public bool xLogging = false;

        // If you want to have some of the game settings default to enabled,  change 
        // the "if false" to "if true" and set the values as you like


#if false
        public override bool HasPresets { get { return true; } }
        public override void SetDifficultyPreset(GameParameters.Preset preset)
        {
            Debug.Log("Setting difficulty preset");
            switch (preset)
            {
                case GameParameters.Preset.Easy:
                    needsECtoStart = false;
                    autoSwitch = true;
                    break;

                case GameParameters.Preset.Normal:
                    needsECtoStart = false;
                    autoSwitch = true;
                    break;

                case GameParameters.Preset.Moderate:
                    needsECtoStart = true;
                    autoSwitch = true;
                    break;

                case GameParameters.Preset.Hard:
                    needsECtoStart = true;
                    autoSwitch = false;
                    break;
            }
        }

#else
        public override bool HasPresets { get { return false; } }
        public override void SetDifficultyPreset(GameParameters.Preset preset) { }
#endif

        public override bool Enabled(MemberInfo member, GameParameters parameters) { return true; }
        public override bool Interactible(MemberInfo member, GameParameters parameters) { return true; }
        public override IList ValidValues(MemberInfo member) { return null; }
    }
}


