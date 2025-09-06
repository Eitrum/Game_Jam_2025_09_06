using UnityEngine;

namespace Toolkit.Currency
{
    public enum CurrencySize
    {
        [InspectorName("Short - 16 bit")]
        Short,
        [InspectorName("Integer - 32 bit")]
        Integer,
        [InspectorName("Long - 64 bit")]
        Long,
        [InspectorName("Big Integer - a lot of bits")]
        BigInteger
    }
}
