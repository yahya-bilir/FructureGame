using UnityEngine;

namespace EventScripts.GameAssets.Base
{
    /// <Summary> Bu class oyundaki yardımcı olan bütün ScriptableObject olan assetleri atasıdır.
    /// İçerisinde bu assetin infosunu tutmaktadır. </Summary>
    public abstract class GameAsset : ScriptableObject
    {
        [HideInInspector] public string eventInfo;
    }
}
