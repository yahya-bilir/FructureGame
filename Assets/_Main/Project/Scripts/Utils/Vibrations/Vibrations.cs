using Lofelt.NiceVibrations;

namespace Utilities.Vibrations
{
    public static class Vibrations
    {
        public static void Failure()
        {
        
            HapticManager.Haptic(HapticPatterns.PresetType.Failure);
        }

        public static void Heavy()
        {
        
            HapticManager.Haptic(HapticPatterns.PresetType.HeavyImpact);
        }

        public static void Light()
        {
            HapticManager.Haptic(HapticPatterns.PresetType.LightImpact);
        }    

        public static void Warning()
        {
        
            HapticManager.Haptic(HapticPatterns.PresetType.Warning);
        }

        public static void Medium()
        {
        
            HapticManager.Haptic(HapticPatterns.PresetType.MediumImpact);
        }

        public static void Soft()
        {
        
            HapticManager.Haptic(HapticPatterns.PresetType.SoftImpact);
        }

        public static void Rigid()
        {
        
            HapticManager.Haptic(HapticPatterns.PresetType.RigidImpact);
        }

        public static void Succes()
        {
        
            HapticManager.Haptic(HapticPatterns.PresetType.Success);
        }

        public static void Selection()
        {
        
            HapticManager.Haptic(HapticPatterns.PresetType.Selection);
        }

        // public static void Stack()
        // {
        //     
        //     HapticManager.Haptic(HapticPatterns.PresetType.Stack);
        // }
        //
        // public static void Buy()
        // {
        //     
        //     HapticManager.Haptic(HapticPatterns.PresetType.Buy);
        // }
        //
        // public static void GetMoney()
        // {
        //     
        //     HapticManager.Haptic(HapticPatterns.PresetType.GetMoney);
        // }
        //
        // public static void SpendMoney()
        // {
        //     
        //     HapticManager.Haptic(HapticPatterns.PresetType.SpendMoney);
        // }
    }
}