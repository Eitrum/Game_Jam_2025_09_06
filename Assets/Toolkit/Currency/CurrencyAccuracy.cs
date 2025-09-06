using UnityEngine;

namespace Toolkit.Currency
{
    public enum CurrencyAccuracy
    {
        [InspectorName("Low - 32 bit")]
        Low,
        [InspectorName("Mid - 64 bit")]
        Mid,
        [InspectorName("High - 128 bit")]
        High
    }
}
