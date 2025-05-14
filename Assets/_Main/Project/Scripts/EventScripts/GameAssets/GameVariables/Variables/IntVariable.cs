using EventScripts.GameAssets.GameVariables.References;
using EventScripts.GameAssets.GameVariables.Variables.Base;
using UnityEngine;

namespace EventScripts.GameAssets.GameVariables.Variables
{
    ///<Summary> Bu class oyun içerisinde haberleşmeye sağlamaktadır.
    ///Burada bir adet int değişkeni bu classa yazılır ve onu realtime okuyan bitün classlar bu değeri set etmektedir.
    ///Bu haberleşmeyi variablelar ile yapmaktadır.</Summary>
    ///<see cref="GameVariable"/>
    [CreateAssetMenu(fileName = "NewIntVariable", menuName = "GameAssets/GameVariables/IntVariable")]
    public class IntVariable : GameVariable
    {
        public int Value { get => savedValue; }

        [Tooltip("Oyun her başladığında değer kaçta kalmış olursa olsun variableın başlayacağı değerdir. Bir nevi resetable variable yapılmaktadır. Sadece oyun başında setlenir.")]
        [SerializeField] private int initialValue;

        private int savedValue;

        private void OnEnable() 
        {
            if(!useInitialValue) return;
            savedValue = initialValue;
        }

        ///<Summary> Int ile değer ataması yapılmaktadır.</Summary>
        public void SetValue(int amount) => savedValue = amount;

        ///<Summary> IntVariable ile değer ataması yapılmaktadır.</Summary>
        public void SetValue(IntVariable amount) => savedValue = amount.Value;

        ///<Summary> IntReference ile değer ataması yapılmaktadır.</Summary>
        public void SetValue(IntReference amount) => savedValue = amount.Value;

        ///<Summary> Int ile toplama yapılmaktadır.</Summary>
        public void Increase(int amount) => savedValue += amount;

        ///<Summary> IntVariable ile toplama yapılmaktadır.</Summary>
        public void Increase(IntVariable amount) => savedValue += amount.Value;

        ///<Summary> IntReference ile toplama yapılmaktadır.</Summary>
        public void Increase(IntReference amount) => savedValue += amount.Value;

        ///<Summary> Int ile çıkarma yapılmaktadır.</Summary>
        public void Decrease(int amount) => savedValue -= amount;

        ///<Summary> IntVariable ile çıkarma yapılmaktadır.</Summary>
        public void Decrease(IntVariable amount) => savedValue -= amount.Value;

        ///<Summary> IntReference ile çıkarma yapılmaktadır.</Summary>
        public void Decrease(IntReference amount) => savedValue -= amount.Value;

        ///<Summary> Int ile çarpma yapılmaktadır.</Summary>
        public void Multiply(int multiplier) => savedValue *= multiplier;

        ///<Summary> IntVariable ile çarpma yapılmaktadır.</Summary>
        public void Multiply(IntVariable multiplier) => savedValue *= multiplier.Value;

        ///<Summary> IntReference ile çarpma yapılmaktadır.</Summary>
        public void Multiply(IntReference multiplier) => savedValue *= multiplier.Value;

        ///<Summary> Int ile bölme yapılmaktadır.
        /// Eğer bölümün sonucu ondalıklı ise en yakın olan tam sayıya tamamlanır.</Summary>
        public void Devide(int divider) => savedValue = Mathf.RoundToInt(savedValue / divider);

        ///<Summary> IntVariable ile bölme yapılmaktadır.
        /// Eğer bölümün sonucu ondalıklı ise en yakın olan tam sayıya tamamlanır.</Summary>
        public void Devide(IntVariable divider) => savedValue = Mathf.RoundToInt(savedValue / divider.Value);

        ///<Summary> IntReference ile bölme yapılmaktadır.
        /// Eğer bölümün sonucu ondalıklı ise en yakın olan tam sayıya tamamlanır.</Summary>
        public void Devide(IntReference divider) => savedValue = Mathf.RoundToInt(savedValue / divider.Value);
    }
}