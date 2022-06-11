﻿using Dalamud.Utility;
using System.Timers;
using XIVSlothComboPlugin;
using XIVSlothComboPlugin.Attributes;
using XIVSlothComboPlugin.Combos;

namespace XIVSlothCombo.CustomCombo
{
    /// <summary>
    /// Base class for each combo.
    /// </summary>
    internal abstract partial class CustomCombo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="CustomCombo"/> class.
        /// </summary>
        protected CustomCombo()
        {
            var presetInfo = Preset.GetAttribute<CustomComboInfoAttribute>();
            JobID = presetInfo.JobID;
            ClassID = JobID switch
            {
                ADV.JobID => ADV.ClassID,
                BLM.JobID => BLM.ClassID,
                BRD.JobID => BRD.ClassID,
                DRG.JobID => DRG.ClassID,
                MNK.JobID => MNK.ClassID,
                NIN.JobID => NIN.ClassID,
                PLD.JobID => PLD.ClassID,
                SCH.JobID => SCH.ClassID,
                SMN.JobID => SMN.ClassID,
                WAR.JobID => WAR.ClassID,
                WHM.JobID => WHM.ClassID,
                _ => 0xFF,
            };

            combatTimer = new Timer(1000); // in miliseconds
            combatTimer.Elapsed += UpdateCombatTimer;
            combatTimer.Start();

        }

        /// <summary>
        /// Gets the preset associated with this combo.
        /// </summary>
        protected internal abstract CustomComboPreset Preset { get; }

        /// <summary>
        /// Gets the class ID associated with this combo.
        /// </summary>
        protected byte ClassID { get; }

        /// <summary>
        /// Gets the job ID associated with this combo.
        /// </summary>
        protected byte JobID { get; }

        /// <summary>
        /// Performs various checks then attempts to invoke the combo.
        /// </summary>
        /// <param name="actionID">Starting action ID.</param>
        /// <param name="level">Player level.</param>
        /// <param name="lastComboMove">Last combo action ID.</param>
        /// <param name="comboTime">Combo timer.</param>
        /// <param name="newActionID">Replacement action ID.</param>
        /// <returns>True if the action has changed, otherwise false.</returns>

        public bool TryInvoke(uint actionID, byte level, uint lastComboMove, float comboTime, out uint newActionID)
        {
            newActionID = 0;
            // Movement

            CheckMovement();

            if (!IsEnabled(Preset))
                return false;

            var classJobID = LocalPlayer!.ClassJob.Id;

            if (classJobID >= 8 && classJobID <= 15)
                classJobID = DOH.JobID;

            if (classJobID >= 16 && classJobID <= 18)
                classJobID = DoL.JobID;

            if (JobID != ADV.JobID && ClassID != ADV.ClassID &&
                JobID != classJobID && ClassID != classJobID)
                return false;

            var resultingActionID = Invoke(actionID, lastComboMove, comboTime, level);
            //Dalamud.Logging.PluginLog.Debug(resultingActionID.ToString());

            if (resultingActionID == 0 || actionID == resultingActionID)
                return false;

            newActionID = resultingActionID;

            return true;
        }

        /// <summary>
        /// Invokes the combo.
        /// </summary>
        /// <param name="actionID">Starting action ID.</param>
        /// <param name="lastComboActionID">Last combo action.</param>
        /// <param name="comboTime">Current combo time.</param>
        /// <param name="level">Current player level.</param>
        /// <returns>The replacement action ID.</returns>
        protected abstract uint Invoke(uint actionID, uint lastComboActionID, float comboTime, byte level);
    }
}
