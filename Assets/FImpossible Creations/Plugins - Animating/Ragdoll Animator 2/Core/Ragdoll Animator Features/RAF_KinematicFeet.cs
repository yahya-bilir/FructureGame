namespace FIMSpace.FProceduralAnimation
{
    public class RAF_KinematicFeet : RagdollAnimatorFeatureBase
    {
        public override bool OnInit()
        {
            foreach( var chain in ParentRagdollHandler.Chains )
            {
                if (chain.BoneSetups.Count == 0) continue;
                if( chain.ChainType.IsLeg() == false ) continue;

                if( chain.ChainType.IsLeg() )
                {
                    var bone = chain.BoneSetups[chain.BoneSetups.Count - 1];
                    bone.ForceKinematicOnStanding = true;
                    bone.RefreshDynamicPhysicalParameters( chain, chain.ParentHandler.IsFallingOrSleep, ParentRagdollHandler.InstantConnectedMassChange);
                }
            }

            return base.OnInit();
        }

        public override void OnDestroyFeature()
        {
            foreach( var chain in ParentRagdollHandler.Chains )
            {
                if (chain.BoneSetups.Count == 0) continue;
                if( chain.ChainType.IsLeg() == false ) continue;

                chain.BoneSetups[chain.BoneSetups.Count - 1].ForceKinematicOnStanding = false;
                chain.BoneSetups[chain.BoneSetups.Count - 1].RefreshDynamicPhysicalParameters( chain, ParentRagdollHandler.IsInFallingMode, ParentRagdollHandler.InstantConnectedMassChange);
            }
        }

#if UNITY_EDITOR

        public override bool Editor_DisplayEnableSwitch => false;

        public override void Editor_OnRemoveFeatureInEditorGUI( RagdollHandler ragdollHandler, RagdollAnimatorFeatureHelper helper )
        {
            // Restore values
            foreach( var chain in ragdollHandler.Chains )
            {
                if (chain.BoneSetups.Count == 0) continue;
                if( chain.ChainType.IsLeg() == false ) continue;

                chain.BoneSetups[chain.BoneSetups.Count - 1].ForceKinematicOnStanding = false;
                if( ragdollHandler.WasInitialized ) chain.BoneSetups[chain.BoneSetups.Count - 1].RefreshDynamicPhysicalParameters( chain, ragdollHandler.IsInFallingMode, ParentRagdollHandler.InstantConnectedMassChange);
            }
        }

        public override string Editor_FeatureDescription => "Switching kinematic state for feet bones during standing mode for better leg animation match. (3 bones needed)";

#endif
    }
}