using EventScripts.GameAssets.GameVariables.References;
using EventScripts.GameAssets.GameVariables.Variables.Base;
using UnityEngine;

namespace EventScripts.GameAssets.GameVariables.Variables
{
    ///<Summary> Bu class oyun içerisinde haberleşmeye sağlamaktadır.
    ///Burada bir adet float değişkeni bu classa yazılır ve onu realtime okuyan bitün classlar bu değeri set etmektedir.
    ///Bu haberleşmeyi variablelar ile yapmaktadır.</Summary>
    ///<see cref="GameVariable"/>
    [CreateAssetMenu(fileName = "NewFloatVariable", menuName = "GameAssets/GameVariables/FloatVariable")]
    public class FloatVariable : GameVariable
    {
        public float Value { get => savedValue; }

        [Tooltip("Oyun her başladığında değer kaçta kalmış olursa olsun variableın başlayacağı değerdir. Bir nevi resetable variable yapılmaktadır. Sadece oyun başında setlenir.")]
        [SerializeField] private float initialValue;

        private float savedValue;

        private void OnEnable() 
        {
            if(!useInitialValue) return;
            savedValue = initialValue;
        }

        ///<Summary> Float ile değer ataması yapılmaktadır.</Summary>
        public void SetValue(float amount) => savedValue = amount;

        ///<Summary> FloatVariable ile değer ataması yapılmaktadır.</Summary>
        public void SetValue(FloatVariable amount) => savedValue = amount.Value;

        ///<Summary> FloatReference ile değer ataması yapılmaktadır.</Summary>
        public void SetValue(FloatReference amount) => savedValue = amount.Value;

        ///<Summary> Float ile toplama yapılmaktadır.</Summary>
        public void Increase(float amount) => savedValue += amount;

        ///<Summary> FloatVariable ile toplama yapılmaktadır.</Summary>
        public void Increase(FloatVariable amount) => savedValue += amount.Value;

        ///<Summary> FloatReference ile toplama yapılmaktadır.</Summary>
        public void Increase(FloatReference amount) => savedValue += amount.Value;

        ///<Summary> Float ile çıkarma yapılmaktadır.</Summary>
        public void Decrease(float amount) => savedValue -= amount;

        ///<Summary> FloatVariable ile çıkarma yapılmaktadır.</Summary>
        public void Decrease(FloatVariable amount) => savedValue -= amount.Value;

        ///<Summary> FloatReference ile çıkarma yapılmaktadır.</Summary>
        public void Decrease(FloatReference amount) => savedValue -= amount.Value;

        ///<Summary> Float ile çarpma yapılmaktadır.</Summary>
        public void Multiply(float multiplier) => savedValue *= multiplier;

        ///<Summary> FloatVariable ile çarpma yapılmaktadır.</Summary>
        public void Multiply(FloatVariable multiplier) => savedValue *= multiplier.Value;

        ///<Summary> FloatReference ile çarpma yapılmaktadır.</Summary>
        public void Multiply(FloatReference multiplier) => savedValue *= multiplier.Value;

        ///<Summary> Float ile bölme yapılmaktadır.</Summary>
        public void Devide(float divider) => savedValue /= divider;

        ///<Summary> FloatVariable ile bölme yapılmaktadır.</Summary>
        public void Devide(FloatVariable divider) => savedValue /= divider.Value;
    
        ///<Summary> FloatReference ile bölme yapılmaktadır.</Summary>
        public void Devide(FloatReference divider) => savedValue /= divider.Value;
    }
}