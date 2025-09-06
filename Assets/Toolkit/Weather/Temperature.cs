using UnityEngine;

namespace Toolkit.Weather
{
    public enum TemperatureType
    {
        Kelvin,
        Celcius,
        Fahrenheit
    }

    [System.Serializable]
    public struct Temperature
    {
        #region Variable

        [SerializeField] private float value;

        #endregion

        #region Properties

        public float Celcius {
            get => ConvertKelvinToCelcius(value);
            set => this.value = Mathf.Max(0f, ConvertCelciusToKelvin(value));
        }

        public float Kelvin {
            get => value;
            set => this.value = Mathf.Max(0f, value);
        }

        public float Fahrenheit {
            get => ConvertKelvinToFahrenheit(value);
            set => this.value = Mathf.Max(0f, ConvertFahrenheitToKelvin(value));
        }

        #endregion

        #region Constructor

        public Temperature(float value) {
            this.value = Mathf.Max(0f, value);
        }

        public Temperature(float value, TemperatureType type) {
            switch(type) {
                case TemperatureType.Kelvin:
                    this.value = value;
                    break;
                case TemperatureType.Fahrenheit:
                    this.value = ConvertFahrenheitToKelvin(value);
                    break;
                case TemperatureType.Celcius:
                    this.value = ConvertCelciusToKelvin(value);
                    break;
                default:
                    this.value = value;
                    break;
            }
        }

        public static Temperature CreateFromCelcius(float value) {
            return new Temperature(value - 273.15f);
        }

        public static Temperature CreateFromKelvin(float value) {
            return new Temperature(value);
        }

        public static Temperature CreateFromFahrenheit(float value) {
            return new Temperature(ConvertFahrenheitToKelvin(value));
        }

        #endregion

        #region Convert between types

        public static float ConvertFahrenheitToKelvin(float fahrenheit) {
            return (fahrenheit - 32f) * 0.555555555f + 273.15f;
        }

        public static float ConvertKelvinToFahrenheit(float kelvin) {
            return (kelvin - 273.15f) / 0.555555555f + 32f;
        }

        public static float ConvertKelvinToCelcius(float kelvin) {
            return kelvin - 273.15f;
        }

        public static float ConvertCelciusToKelvin(float celcius) {
            return celcius + 273.15f;
        }

        public static float ConvertCelciusToFahrenheit(float celcius) {
            return celcius / 0.555555555f + 32f;
        }

        public static float ConvertFahrenheitToCelcius(float fahrenheit) {
            return (fahrenheit - 32f) * 0.555555555f;
        }

        #endregion
    }
}
