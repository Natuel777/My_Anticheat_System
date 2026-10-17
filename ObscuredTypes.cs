using System;

public struct ObscuredInt
{
    private int _encryptedValue;
    private int _key;

    public ObscuredInt(int value)
    {
        // Si unity no acepta Random.Shared.Next() es porque usa una versión vieja de .NET, reemplazar con Random.Range(0, int.MaxValue)
        _key = Random.Shared.Next();
        
        // ^ representa la operación XOR (Exclusive Or), \[(A \oplus B) \oplus B = A\] 
        // La operación consiste en analizar los binarios de 2 números
        // "XOR devuelve 1 cuando los dos bits son diferentes y 0 cuando son iguales."
            // 13 = 1101
            // 9 = 1001
            // --------
        // XOR 0100 = 4 El resultado el número convertido a int resultando de la diferencia de binarios aplicando la regla XOR
        _encryptedValue = value ^ _key; 
    }

    // Para asignar un valor a un ObscuredInt luego de haberlo inicializado, se usa ObscuredInt.Value = (int)
    public int Value
    {
        get => _encryptedValue ^ _key;
        set => _encryptedValue = value ^ _key;
    }

    // Esto define una conversión implícita desde ObscuredInt hacia int. En otras palabras, permite usar ObscuredInt como un int
    // MyObscuredInt coins = 100;
    // int total = coins;
    public static implicit operator int(ObscuredInt value) => value.Value;

    // Esta permite convertir automáticamente un int en ObscuredInt. MyObscuredInt coins = 100;
    public static implicit operator ObscuredInt(int value) => new ObscuredInt(value);
}

public struct ObscuredFloat
{
    private int _encryptedBits;
    private int _key;

    public MyObscuredFloat(float value)
    {
        _key = GenerateKey();

        //BitConverter.SingleToInt32Bits() convierte de float a int
        int bits = BitConverter.SingleToInt32Bits(value);
        _encryptedBits = bits ^ _key;
    }

    //BitConverter.Int32BitsToSingle() convierte de int a float
    public float Value
    {
        get 
        {
            int bits = encryptedBits ^ _key;
            return BitConverter.Int32BitsToSingle(bits);
        }

        set
        {
            if(_key == 0) _key = GenerateKey();

            int bits = BitConverter.SingleToInt32Bits(value);
            _encryptedBits = bits ^ _key;
        }
    }

    public static implicit operator float(ObscuredFloat value) => value.Value;

    public static implicit operator ObscuredFloat(float value) => new ObscuredFloat(value);

    private static int GenerateKey() => Random.Shared.Next();
}

public struct ObscuredBool
{
    private int _encryptedValue;
    private int _key;

    private static int GenerateKey() => Random.Shared.Next();

    public ObscuredBool(bool value)
    {
        _key = GenerateKey();
        _encryptedValue = (value ? 1 : 0) ^ _key;
    }

    public bool Value
    {
        get => (_encryptedValue ^ _key) != 0;

        set
        {
            if(_key == 0) _key = GenerateKey();
            
            _encryptedValue ? (value ? 1 : 0) ^ _key;
        }
    }

    public static implicit operator bool(ObscuredBool value) => value.Value;

    public static implicit operator ObscuredBool(bool value) => new ObscuredBool(value);
}