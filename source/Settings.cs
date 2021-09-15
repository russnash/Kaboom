using System;
using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System.Reflection;

// This will add a tab to the Stock Settings in the Difficulty settings
// To use, reference the setting using the following:
//
//  HighLogic.CurrentGame.Parameters.CustomParams<Options>()
//
// As it is set up, the option is disabled, so in order to enable it, the player would have
// to deliberately go in and change it
//
namespace Kaboom
{
    // http://forum.kerbalspaceprogram.com/index.php?/topic/147576-modders-notes-for-ksp-12/#comment-2754813
    // search for "Mod integration into Stock Settings

    public class Options : GameParameters.CustomParameterNode
    {
        public override string Title { get { return "Kaboom!"; } }
        public override GameParameters.GameMode GameMode { get { return GameParameters.GameMode.ANY; } }
        public override string Section { get { return "Kaboom!"; } }
        public override string DisplaySection { get { return "Kaboom!"; } }
        public override int SectionOrder { get { return 1; } }


        [GameParameters.CustomParameterUI("Soft Explode",
            toolTip = "Kaboom Explodes makes less fire",
            unlockedDuringMission = true
            )]
        public bool softExplode = false;

        [GameParameters.CustomParameterUI("PAW Safety Cover is Red",
            toolTip = "Red color coding of Kaboom Safety Cover in the Part Action Window.\nUpdates after scene change.",
            unlockedDuringMission = true)]
        public bool coloredPAW = true;

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

   
