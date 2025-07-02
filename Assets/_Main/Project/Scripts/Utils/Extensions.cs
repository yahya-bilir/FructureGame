using System.Collections.Generic;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.SceneManagement;
using Random = System.Random;
using RandomRange = UnityEngine.Random;

namespace Utilities
{
    public static class Extensions
    {
        public static void Shuffle<T>(this Random rng, T[] array)
        {
            int n = array.Length;
            while (n > 1)
            {
                int k = rng.Next(n--);
                (array[n], array[k]) = (array[k], array[n]);
            }
        }        
        
        public static void Shuffle<T>(this Random rng, List<T> array)
        {
            int n = array.Count;
            while (n > 1)
            {
                int k = rng.Next(n--);
                (array[n], array[k]) = (array[k], array[n]);
            }
        }

        public static Vector3 RandomColliderPositionGetter(Collider collider)
        {
            var bounds = collider.bounds;

            Vector3 randomPosition = new Vector3(
                UnityEngine.Random.Range(bounds.min.x, bounds.max.x),
                UnityEngine.Random.Range(bounds.min.y, bounds.max.y),
                UnityEngine.Random.Range(bounds.min.z, bounds.max.z)
            );
            return randomPosition;
        }

        public static Vector3 GetRandomVector3(float min, float max) => new(RandomRange.Range(min, max), RandomRange.Range(min, max), RandomRange.Range(min, max));

        public static string ToKFormat(int value)
        {
            if (value >= 1000000)
                return (value / 1000000D).ToString("0.#M");
            else if (value >= 1000)
                return (value / 1000D).ToString("0.#K");
            else
                return value.ToString();
        }

        public static string ColoredText(string v, Color color)
        {
            string colorCode = ColorUtility.ToHtmlStringRGB(color);
            return ($"<color=#{colorCode}>{v}</color>");
        }

        public static int GetMoneyAmountToSpawn(int scoreToCalculate)
        {
            var division = 1;
            if (scoreToCalculate is >= 2 and < 5) division = 2;
            else if (scoreToCalculate is >= 5 and < 20) division = 3;
            else if (scoreToCalculate is >= 20 and < 50) division = 5;
            else if (scoreToCalculate is >= 50 and < 100) division = 7;
            else if (scoreToCalculate is >= 100 and < 250) division = 11;
            else if (scoreToCalculate >= 250)
            {
                division = scoreToCalculate / 20;
            }

            return scoreToCalculate / division;
        }

        public static string GenerateHashedString(string str)
        {
            using (SHA256 sha256 = SHA256.Create())
            {
                byte[] bytes = sha256.ComputeHash(Encoding.UTF8.GetBytes(str));
                StringBuilder builder = new StringBuilder();
                foreach (byte b in bytes)
                {
                    builder.Append(b.ToString("x2"));
                }

                return builder.ToString();
            }
        }

        public static string GenerateUID(Transform transform)
        {
            var sceneName = SceneManager.GetActiveScene().name;
            return GenerateHashedString($"{sceneName}|{transform.position}|{transform.gameObject.name}");
        }

        public static string GenerateUID(string path)
        {
            return GenerateHashedString(path);
        }
    }
}