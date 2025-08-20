#if UNITY_EDITOR

using UnityEditor;
using UnityEditorInternal;
using FIMSpace.FEditor;

#endif

using FIMSpace.FGenerating;
using UnityEngine;
using UnityEngine.Events;
using System;

namespace FIMSpace.FProceduralAnimation
{
    public class RAF_AutoGetUp : RagdollAnimatorFeatureUpdate
    {
        public override bool UseFixedUpdate => true;

        private FUniversalVariable getupDelay;
        private FUniversalVariable maxAvgTranslation;
        private FUniversalVariable maxAvgTorq;
        private FUniversalVariable groundMask;
        private FUniversalVariable coreGrounded;
        private FUniversalVariable minimumStable;
        private FUniversalVariable ragdollStandupBlendDuration;
        private FUniversalVariable crossfadesDelay;
        private FUniversalVariable quickBlendFade;
        private FUniversalVariable freezeHipsDuration;
        //private FUniversalVariable isAnimal;
        private FUniversalVariable standingRestore;
        private FUniversalVariable standingRestoreMinTime;
        private FUniversalVariable restoreAngle;

        private FUniversalVariable raycastRangeMul;
        private FUniversalVariable raycastingMode;
        private FUniversalVariable raycastScale;

        [field: NonSerialized] RagdollBonesChain coreChain = null;

        enum ERaycastMode
        {
            Line, Sphere, Box
        }

        public override bool OnInit()
        {
            getupDelay = InitializedWith.RequestVariable("Minimum Delay:", 0.4f);
            maxAvgTranslation = InitializedWith.RequestVariable("Max avg. Translation:", 0.075f);
            maxAvgTorq = InitializedWith.RequestVariable("Max avg. Torque:", 1f);
            groundMask = InitializedWith.RequestVariable("Ground Mask:", 0 >> 0);
            coreGrounded = InitializedWith.RequestVariable("Needs Core Grounded:", 0f);
            minimumStable = InitializedWith.RequestVariable("Minimum Stability:", 0.15f);
            crossfadesDelay = InitializedWith.RequestVariable("Animator Fade Delay:", 0f);
            ragdollStandupBlendDuration = InitializedWith.RequestVariable("To Standing Transition Duration:", 1f);
            quickBlendFade = InitializedWith.RequestVariable("Quick Blend Fade:", 0.3f);

            CheckBackCompatibility(InitializedWith);
            freezeHipsDuration = InitializedWith.RequestVariable("Freeze Source Animator Hips:", 0f);
            standingRestore = InitializedWith.RequestVariable("Allow Standing Restore:", false);
            standingRestoreMinTime = InitializedWith.RequestVariable("Restore After:", 0.3f);
            restoreAngle = InitializedWith.RequestVariable("Max Body Angle To Restore:", 35f);

            raycastRangeMul = InitializedWith.RequestVariable("Raycast Range Multiplier:", 1f);
            raycastingMode = InitializedWith.RequestVariable("Raycast Mode:", 0);
            raycastScale = InitializedWith.RequestVariable("Raycast Scale:", 0.2f);

            //isAnimal = InitializedWith.RequestVariable( "Is Animal:", false );

            fallingDuration = 0f;
            stableTime = 0f;
            getUpType = ERagdollGetUpType.None;
            groundHit = new RaycastHit();
            coreChain = ParentRagdollHandler.GetChain(ERagdollChainType.Core);

            return base.OnInit();
        }

        void CheckBackCompatibility(RagdollAnimatorFeatureHelper helper)
        {
            // Required by variable name change (there was typo)
            if (helper.HasVariable("Feeze Source Animator Hips:"))
            {
                var oldVar = helper.RequestVariable("Feeze Source Animator Hips:", 0f);
                if (oldVar.GetFloat() != 0f)
                {
                    var newVar = helper.RequestVariable("Freeze Source Animator Hips:", 0f);
                    newVar.SetValue(oldVar.GetFloat());
                    oldVar.SetValue(0f);
                }
            }
        }

        private float fallingDuration = 0f;
        private float stableTime = 0f;

        public ERagdollGetUpType getUpType { get; private set; }
        public RaycastHit groundHit { get; private set; }
        float legsStandElapsed = 0f;
        float coreLiesOnGroundElapsed = 0f;

        public override void FixedUpdate()
        {
            if (InitializedWith.Enabled == false) return;

            var handler = ParentRagdollHandler;

            if (handler.AnimatingMode == RagdollHandler.EAnimatingMode.Standing)
            {
                fallingDuration = 0f;
                stableTime = 0f;
                coreLiesOnGroundElapsed = 0f;
            }
            else if (handler.AnimatingMode == RagdollHandler.EAnimatingMode.Falling) // Don't call get up when sleep mode / anchor 0
            {
                fallingDuration += Time.fixedDeltaTime;
                // Falling for too short duration
                if (fallingDuration < getupDelay.GetFloat()) { legsStandElapsed = 0f; return; };

                // The velocity of core bones are in move, so not ready for getup
                float averageTranslation = handler.User_GetChainBonesAverageTranslation(ERagdollChainType.Core).magnitude;

                if (averageTranslation > maxAvgTranslation.GetFloat()) { stableTime = 0f; legsStandElapsed = 0f; return; }

                // The velocity of core bones are in move, so not ready for getup
                if (handler.User_GetChainAngularVelocity(ERagdollChainType.Core).magnitude > maxAvgTorq.GetFloat() * handler.User_CoreLowTranslationFactor(averageTranslation))
                { stableTime = 0f; legsStandElapsed = 0f; return; }

                stableTime += Time.deltaTime;
                if (stableTime < minimumStable.GetFloat()) { return; } // Let's be in static pose for a small amount of time

                bool groundBelow = true;

                if (groundMask.GetInt() != 0)
                {
                    float probeDist = handler.GetAnchorBoneController.MainBoneCollider.bounds.size.magnitude + 0.01f;
                    probeDist *= raycastRangeMul.GetFloat();

                    groundHit = ProbeGround(ParentRagdollHandler.GetAnchorBoneController, probeDist);
                    //groundHit = handler.ProbeGroundBelowHips(groundMask.GetInt(), probeDist);

                    if (groundHit.transform == null) groundBelow = false; // No Ground below

                    if (coreGrounded.GetFloat() > 0f) // Checking if character lies on the ground with the whole core chain for a while
                    {
                        bool coreChainOnGround = true;
                        int maxI = Mathf.Min(4, coreChain.BoneSetups.Count); // Dont check too many bones, its not needed

                        for (int i = 0; i < maxI; i++)
                        {
                            var hit = ProbeGround(coreChain.BoneSetups[i], probeDist);
                            //var hit = handler.ProbeGroundBelow(coreChain.BoneSetups[i], groundMask.GetInt(), probeDist);
                            if (hit.transform == null) { coreChainOnGround = false; break; }
                        }

                        if (coreChainOnGround)
                        {
                            coreLiesOnGroundElapsed += Time.fixedDeltaTime;
                            if (coreLiesOnGroundElapsed < coreGrounded.GetFloat()) groundBelow = false;
                        }
                        else
                        {
                            coreLiesOnGroundElapsed = 0f;
                            groundBelow = false;
                        }
                    }
                }

                if (groundBelow == false) // Not detected ground to make character get up on its surface
                {
                    #region Standing Restore - standing on legs check

                    if (standingRestore.GetBool())
                    {
                        // Check if anchor is in somewhat standing-correct rotation

                        Vector3 anchorUp = handler.User_BoneWorldUp(handler.GetAnchorBoneController);
                        float groundAngle = Vector3.Angle(anchorUp, Vector3.up);

                        if (groundAngle > restoreAngle.GetFloat()) return; // Not enough aligned with ground

                        var lLeg = handler.GetChain(ERagdollChainType.LeftLeg);
                        if (lLeg == null) return;
                        var rLeg = handler.GetChain(ERagdollChainType.RightLeg);
                        if (rLeg == null) return;

                        var lHit = handler.ProbeGroundBelow(lLeg.GetBone(100), groundMask.GetInt());
                        if (lHit.transform == null) return;

                        var rHit = handler.ProbeGroundBelow(rLeg.GetBone(100), groundMask.GetInt());
                        if (rHit.transform == null) return;

                        // Detected legs standing
                        legsStandElapsed += Time.fixedDeltaTime;
                        if (legsStandElapsed > standingRestoreMinTime.GetFloat())
                        {
                            handler.User_TransitionToStandingMode(ragdollStandupBlendDuration.GetFloat(), quickBlendFade.GetFloat(), crossfadesDelay.GetFloat() > 0f ? 0.1f : 0f, 0f, 0f, true);
                            Helper.customEventsList[0].Invoke(); // On Get Up - Like Mover Disable
                        }

                        return;
                    }

                    #endregion Standing Restore - standing on legs check


                    return;
                }

                getUpType = handler.User_CanGetUpByRotation(false, null, false, 0.5f);

                // Checking how hips are rotated in current pose to define target getup
                //if( getUpType == ERagdollGetUpType.None ) return;

                handler.User_TransitionToStandingMode(ragdollStandupBlendDuration.GetFloat(), quickBlendFade.GetFloat(), crossfadesDelay.GetFloat() > 0f ? 0.1f : 0f, freezeHipsDuration.GetFloat(), 0f);

                Helper.customEventsList[0].Invoke(); // On Get Up - Like Mover Disable
            }
        }

        RaycastHit ProbeGround(RagdollChainBone bone, float probeDist)
        {
            ERaycastMode rayMode = (ERaycastMode)raycastingMode.GetInt();
        
            if ( rayMode == ERaycastMode.Line)
            {
                return ParentRagdollHandler.ProbeGroundBelow(bone, groundMask.GetInt(), probeDist);
            }
            else if (rayMode == ERaycastMode.Sphere)
            {
                return ParentRagdollHandler.ProbeGroundBelowSpherecast(bone, groundMask.GetInt(), raycastScale.GetFloat(), probeDist);
            }
            else if ( rayMode == ERaycastMode.Box)
            {
                Vector3 boxScale = new Vector3(raycastScale.GetFloat(), 0f, 0f);
                boxScale.y = boxScale.x;
                boxScale.z = boxScale.x;
                return ParentRagdollHandler.ProbeGroundBelowBoxcast(bone, groundMask.GetInt(), boxScale, Quaternion.identity, probeDist);
            }

            return new RaycastHit();
        }

#if UNITY_EDITOR
        public override bool Editor_DisplayEnableSwitch => true;
        public override string Editor_FeatureDescription => "Switching from fall mode to standing mode after body calms down on the ground. Use in combination with <Fall-Get Up Animate> feature";

        public override void Editor_OnSceneGUI(RagdollHandler ragdollHandler, RagdollAnimatorFeatureHelper helper)
        {
            base.Editor_OnSceneGUI(ragdollHandler, helper);

            var raycastingModeV = helper.RequestVariable("Raycast Mode:", 0);
            var raycastScaleV = helper.RequestVariable("Raycast Scale:", 0.2f);
            ERaycastMode rayMode = (ERaycastMode)raycastingModeV.GetInt();

            if (rayMode == ERaycastMode.Sphere)
            {
                Handles.color = Color.green * 0.5f;
                Handles.SphereHandleCap(0, ragdollHandler.GetBaseTransform().position, Quaternion.identity, raycastScaleV.GetFloat(), EventType.Repaint);
            }
            else if (rayMode == ERaycastMode.Box)
            {
                Handles.color = Color.green * 0.5f;
                Handles.CubeHandleCap(0, ragdollHandler.GetBaseTransform().position, Quaternion.identity, raycastScaleV.GetFloat(), EventType.Repaint);
            }

            Handles.color = Color.white;
        }

#endif

        #region Prepare Events

        private bool RefreshHelperEvents(RagdollAnimatorFeatureHelper helper)
        {
            bool changed = false;
            if (helper.customEventsList == null)
            {
                helper.customEventsList = new System.Collections.Generic.List<UnityEvent>();
                changed = true;
            }

            while (helper.customEventsList.Count < 1) { helper.customEventsList.Add(new UnityEvent()); changed = true; }

            return changed;
        }

        #endregion Prepare Events

#if UNITY_EDITOR
        bool _compatibilityCheck = false;

        public override void Editor_InspectorGUI(SerializedProperty handlerProp, RagdollHandler ragdollHandler, RagdollAnimatorFeatureHelper helper)
        {

            #region Define Unity Events

            int featureIdx = -1;
            for (int i = 0; i < ragdollHandler.ExtraFeatures.Count; i++) if (ragdollHandler.ExtraFeatures[i] == helper) { featureIdx = i; break; }

            if (featureIdx == -1)
            {
                EditorGUILayout.HelpBox("Something went wrong with identifying feature in ragdoll handler", UnityEditor.MessageType.None);
                return;
            }

            if (RefreshHelperEvents(helper))
            {
                EditorUtility.SetDirty(handlerProp.serializedObject.targetObject);
                handlerProp.serializedObject.ApplyModifiedProperties();
                handlerProp.serializedObject.Update();
            }

            var sp = handlerProp.FindPropertyRelative("ExtraFeatures").GetArrayElementAtIndex(featureIdx);
            sp = sp.FindPropertyRelative("customEventsList");

            #endregion Define Unity Events

            RAF_FallGetUpAnimate getUpH = ragdollHandler.GetExtraFeature<RAF_FallGetUpAnimate>();
            if (getUpH == null)
            {
                EditorGUILayout.BeginHorizontal();
                EditorGUILayout.HelpBox("Consider using 'Fall Get Up Animate' ragdoll feature in combination with this feature.", MessageType.Info);
                //if( GUILayout.Button( new GUIContent( "Add", "Adding 'Fall Get Up Animate' ragdoll feature" ) ) )
                //{
                //    ragdollHandler.AddRagdollFeature<RAF_FallGetUpAnimate>();
                //    UnityEditor.EditorUtility.SetDirty( handlerProp.serializedObject.targetObject );
                //}
                EditorGUILayout.EndHorizontal();
            }

            GUILayout.Space(4);

            //FUniversalVariable isAnimal = helper.RequestVariable( "Is Animal:", false );
            //isAnimal.AssignTooltip( "Enable if you're using Ragdoll Animator for animal/creature which moves on more than 2 legs for proper get up rotation detection." );
            //isAnimal.Editor_DisplayVariableGUI();

            EditorGUIUtility.labelWidth = 0;
            EditorGUILayout.LabelField(new GUIContent(" Get Up Trigger Conditions:", EditorGUIUtility.IconContent("RaycastCollider Icon").image), EditorStyles.boldLabel);
            GUILayout.Space(2);

            FGenerating.FUniversalVariable getupDelay = helper.RequestVariable("Minimum Delay:", 0.4f);
            getupDelay.AssignTooltip("Minimum delay in seconds after falling to trigger get up action");
            getupDelay.Editor_DisplayVariableGUI();
            if (getupDelay.GetFloat() < 0f) getupDelay.SetValue(0f);

            FGenerating.FUniversalVariable minimumStable = helper.RequestVariable("Minimum Stability:", 0.25f);
            minimumStable.AssignTooltip("Minimum stable body duration to allow get up");
            minimumStable.Editor_DisplayVariableGUI();
            if (minimumStable.GetFloat() < 0f) minimumStable.SetValue(0f);

            FGenerating.FUniversalVariable maxAvgTranslation = helper.RequestVariable("Max avg. Translation:", 0.075f);
            maxAvgTranslation.AssignTooltip("If average translation of core chain will surpass this value, get up will not be triggered.\nHigher Value = Get up more frequently\nLower Value = Get up only when ragdoll calm down");
            maxAvgTranslation.Editor_DisplayVariableGUI();
            if (maxAvgTranslation.GetFloat() < 0.01f) maxAvgTranslation.SetValue(0.01f);

            FGenerating.FUniversalVariable maxAvgTorq = helper.RequestVariable("Max avg. Torque:", 1f);
            maxAvgTorq.AssignTooltip("If average angular velocity/torque of core chain will surpass this value, get up will not be triggered.\nHigher Value = Get up more frequently\nLower Value = Get up only when ragdoll calm down");
            maxAvgTorq.Editor_DisplayVariableGUI();
            if (maxAvgTorq.GetFloat() < 0.5f) maxAvgTorq.SetValue(0.5f);

            GUILayout.Space(3);
            var coreGrounded = helper.RequestVariable("Needs Core Grounded:", 0f);
            if (coreGrounded.GetFloat() > 1f) coreGrounded.SetValue(1f);
            else if (coreGrounded.GetFloat() < 0f) coreGrounded.SetValue(0f);
            coreGrounded.AssignTooltip("If set to zero, then this feature is not used.\nRaycasting for ground contact using core chain bones. If all bones lies on the ground for this duration, character will be allowed to get up.\n(Not used for standing restore checks)");
            if (coreGrounded.GetFloat() <= 0f) GUI.color = new Color(1f, 1f, 1f, 0.7f);
            coreGrounded.Editor_DisplayVariableGUI();
            GUI.color = Color.white;

            GUILayout.Space(3);
            var groundMask = helper.RequestVariable("Ground Mask:", 0 << 0);
            int layer = groundMask.GetInt();
            layer = EditorGUILayout.MaskField(new GUIContent("Ground Mask:", "Ground below check layer mask"), InternalEditorUtility.LayerMaskToConcatenatedLayersMask(layer), InternalEditorUtility.layers);
            layer = InternalEditorUtility.ConcatenatedLayersMaskToLayerMask(layer);
            groundMask.SetValue(layer);

            EditorGUIUtility.labelWidth = 0;
            GUILayout.Space(6);
            EditorGUILayout.LabelField(new GUIContent(" Transition To Standing Parameters:", EditorGUIUtility.IconContent("Animator Icon").image), EditorStyles.boldLabel);
            GUILayout.Space(2);

            EditorGUIUtility.labelWidth = 188;
            var ragdollStandupBlendDuration = helper.RequestVariable("To Standing Transition Duration:", 1f);
            ragdollStandupBlendDuration.SetMinMaxSlider(0f, 2f);
            EditorGUIUtility.fieldWidth = 32;
            ragdollStandupBlendDuration.AssignTooltip("Duration of transition process from falling to standing ragdoll");
            ragdollStandupBlendDuration.Editor_DisplayVariableGUI();
            EditorGUIUtility.fieldWidth = 0;

            FGenerating.FUniversalVariable quickBlendFade = helper.RequestVariable("Quick Blend Fade:", 0.7f);
            quickBlendFade.AssignTooltip("Fade applied to the 'Ragdoll Blend' parameter to make character be animated without any physical effect for Get Up animation.");
            quickBlendFade.Editor_DisplayVariableGUI();
            quickBlendFade.SetMinMaxSlider(0f, 1f);
            if (quickBlendFade.GetFloat() > ragdollStandupBlendDuration.GetFloat()) quickBlendFade.SetValue(ragdollStandupBlendDuration.GetFloat() * 0.9f);
            if (quickBlendFade.GetFloat() < 0f) quickBlendFade.SetValue(0f);

            EditorGUIUtility.labelWidth = 188;
            if (!_compatibilityCheck) { CheckBackCompatibility(helper); _compatibilityCheck = true; }
            var freezeHipsDuration = helper.RequestVariable("Freeze Source Animator Hips:", 0f);
            freezeHipsDuration.AssignTooltip("Freezing position/rotation of source animation anchor bone, to make crossfade to get up animation seamless (no hovering - caused by falling to get up animation crossfade)");
            freezeHipsDuration.SetMinMaxSlider(0f, 0.5f);
            freezeHipsDuration.Editor_DisplayVariableGUI();

            EditorGUIUtility.labelWidth = 130;
            GUILayout.Space(2);
            var crossfadesDelay = helper.RequestVariable("Animator Fade Delay:", 0f);
            crossfadesDelay.AssignTooltip("Adding delay of animator fade, in case of crossfading to get up animation. If you transition immedietely to get up animation, set this value as zero");
            crossfadesDelay.Editor_DisplayVariableGUI();

            GUILayout.Space(5);

            var standingRestoreV = helper.RequestVariable("Allow Standing Restore:", false);
            standingRestoreV.AssignTooltip("If character ragdoll falls, but is standing on both legs during ragdolled state for a moment, you can trigger different 'Get Up', restoring character to the standing animating mode without playing 'get up from floor' animations.");

            EditorGUILayout.BeginHorizontal();

            EditorGUILayout.LabelField(new GUIContent(FGUI_Resources.FindIcon("Ragdoll Animator/SPR_RagdollState")), GUILayout.Width(20));

            EditorGUIUtility.labelWidth = 142;
            standingRestoreV.Editor_DisplayVariableGUI();

            if (standingRestoreV.GetBool())
            {
                EditorGUIUtility.labelWidth = 86;
                var standingRestoreMinTimeV = helper.RequestVariable("Restore After:", 0.3f);
                standingRestoreMinTimeV.AssignTooltip("Time the character needs to stand on legs in order to trigger standing restore.");
                standingRestoreMinTimeV.Editor_DisplayVariableGUI();
                if (standingRestoreMinTimeV.GetFloat() < 0f) standingRestoreMinTimeV.SetValue(0f);
                EditorGUILayout.EndHorizontal();

                EditorGUI.indentLevel++;
                EditorGUIUtility.labelWidth = 0;
                var restoreAngle = helper.RequestVariable("Max Body Angle To Restore:", 35f);
                restoreAngle.AssignTooltip("Max anchor-body angle in comparison to ground (Up) in which standing restore can be triggered.\nLower value = character needs to stand vertically on both legs to be able for standing restore.");
                if (restoreAngle.GetFloat() < 5f) restoreAngle.SetValue(5f);
                if (restoreAngle.GetFloat() > 90f) restoreAngle.SetValue(90f);
                restoreAngle.SetMinMaxSlider(0f, 0f); // remove slider if was used
                EditorGUIUtility.labelWidth = 190;
                restoreAngle.Editor_DisplayVariableGUI();
                EditorGUIUtility.labelWidth = 0;
                EditorGUI.indentLevel--;
            }
            else
            {
                EditorGUILayout.EndHorizontal();
            }

            EditorGUIUtility.labelWidth = 0;

            GUILayout.Space(6);


            #region Use Get Up Event

            EditorGUILayout.BeginHorizontal();
            EditorGUILayout.LabelField(new GUIContent("  On Get Up Event:", EditorGUIUtility.IconContent("EventSystem Icon").image), EditorStyles.boldLabel);
            if (getUpH == null) EditorGUILayout.HelpBox(" Apply character repositioning and play get up animation in the event.", UnityEditor.MessageType.None);
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.PropertyField(sp.GetArrayElementAtIndex(0));

            #endregion Use Get Up Event


            GUILayout.Space(4);

            FGUI_Inspector.DrawUILineCommon();

            GUILayout.Space(4);
            EditorGUILayout.LabelField("Extra: ", EditorStyles.boldLabel);
            GUILayout.Space(2);

            var raycastingMulV = helper.RequestVariable("Raycast Range Multiplier:", 1f);
            raycastingMulV.Editor_DisplayVariableGUI();

            var raycastingModeV = helper.RequestVariable("Raycast Mode:", 0);

            ERaycastMode rayMode = (ERaycastMode)raycastingModeV.GetInt();
            rayMode = (ERaycastMode)EditorGUILayout.EnumPopup("Raycast Mode:", rayMode);
            raycastingModeV.SetValue((int)rayMode);

            if (rayMode != ERaycastMode.Line)
            {
                var raycastScaleV = helper.RequestVariable("Raycast Scale:", 0.2f);
                raycastScaleV.Editor_DisplayVariableGUI();
                if (raycastScaleV.GetFloat() < 0f) raycastScaleV.SetValue(0f);
            }

            GUILayout.Space(4);
        }

#endif
    }
}