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

public struct ObscuredUShort
{
    private ushort _encryptedValue;
    private ushort _key;

    public ObscuredInt(ushort value)
    {
        _key = (ushort)Random.Shared.Next(1, 65536);
        _encryptedValue = value ^ _key; 
    }

    public ushort Value
    {
        get => _encryptedValue ^ _key;
        set => _encryptedValue = value ^ _key;
    }

    public static implicit operator ushort(ObscuredUShort value) => value.Value;

    public static implicit operator ObscuredUShort(ushort value) => new ObscuredUShort(value);
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

public struct ObscuredString
{
    private char[] _encryptedChars;
    private ushort _key;
    private bool _isNull;

    private static ushort GenerateKey() => (ushort)Random.Shared.Next(1, 65536);

    public ObscuredString(string value)
    {
        key = GenerateKey();
        _isNull = value == null;
        encryptedChars = Array.Empty<char>();

        if(_isNull) return;

        encryptedChars = new char[value.Length];

        for(int i = 0; i < value.Length; i++)
            encryptedChars[i] = (char)(value[i] ^ key);
    }

    public string Value
    {
        get
        {
            if(_isNull) return null;

            if(_encryptedChars == null || _encryptedChars.Length == 0) return string.Empty;

            char[] decodedChars = new char[_encryptedChars.Length];

            for(int i = 0; i < _encryptedChars.Length; i++)
                decodedChars[i] = (char)_encryptedChars[i] ^ _key;

            return new string(decodedChars);
        }

        set
        {
            key = GenerateKey();
            _isNull = value == null;
            encryptedChars = Array.Empty<char>();
    
            if(_isNull) return;
    
            encryptedChars = new char[value.Length];
    
            for(int i = 0; i < value.Length; i++)
                encryptedChars[i] = (char)(value[i] ^ key);
        }
    }

    public static implicit operator string(ObscuredString value) => value.Value;

    public static implicit operator ObscuredString(string value) => new ObscuredString(value);
}

using UnityEngine;

public struct ObscuredVector3
{
    private int _encryptedX, _encryptedY, _encryptedZ;
    private int _key;

    public MyObscuredFloat(Vector3 value)
    {
        _key = GenerateKey();

        _encryptedX = (BitConverter.SingleToInt32Bits(value.x) ^ _key)
        _encryptedY = (BitConverter.SingleToInt32Bits(value.y) ^ _key)
        _encryptedZ = (BitConverter.SingleToInt32Bits(value.z) ^ _key)
    }

    public float Value
    {
        get 
        {
            int bitsX = encryptedBits ^ _key;
            float x = BitConverter.Int32BitsToSingle(bitsX);
            int bitsY = encryptedBits ^ _key;
            float y = BitConverter.Int32BitsToSingle(bitsY);
            int bitsZ = encryptedBits ^ _key;
            float z = BitConverter.Int32BitsToSingle(bitsZ);
            return new Vector3(x, y, z);
        }

        set
        {
            if(_key == 0) _key = GenerateKey();

            int bitsX = BitConverter.SingleToInt32Bits(value.x);
            _encryptedX = bitsX ^ _key;
            int bitsY = BitConverter.SingleToInt32Bits(value.y);
            _encryptedY = bitsY ^ _key;
            int bitsZ = BitConverter.SingleToInt32Bits(value.z);
            _encryptedZ = bitsZ ^ _key;
        }
    }

    public static implicit operator Vector3(ObscuredVector3 value) => value.Value;

    public static implicit operator ObscuredVector3(Vector3 value) => new ObscuredVector3(value);

    private static int GenerateKey() => Random.Shared.Next();
}

public struct ObscuredVector2
{
    private int _encryptedX, _encryptedY;
    private int _key;

    public MyObscuredFloat(Vector2 value)
    {
        _key = GenerateKey();

        _encryptedX = (BitConverter.SingleToInt32Bits(value.x) ^ _key)
        _encryptedY = (BitConverter.SingleToInt32Bits(value.y) ^ _key)
    }

    public float Value
    {
        get 
        {
            int bitsX = encryptedBits ^ _key;
            float x = BitConverter.Int32BitsToSingle(bitsX);
            int bitsY = encryptedBits ^ _key;
            float y = BitConverter.Int32BitsToSingle(bitsY);
            return new Vector2(x, y);
        }

        set
        {
            if(_key == 0) _key = GenerateKey();

            int bitsX = BitConverter.SingleToInt32Bits(value.x);
            _encryptedX = bitsX ^ _key;
            int bitsY = BitConverter.SingleToInt32Bits(value.y);
            _encryptedY = bitsY ^ _key;
        }
    }

    public static implicit operator Vector2(ObscuredVector2 value) => value.Value;

    public static implicit operator ObscuredVector2(Vector2 value) => new ObscuredVector2(value);

    private static int GenerateKey() => Random.Shared.Next();
}

public struct ObscuredVector2Int
{
    private int _encryptedX, _encryptedY;
    private int _key;

    public MyObscuredFloat(Vector2Int value)
    {
        _key = GenerateKey();

        _encryptedX = (BitConverter.SingleToInt32Bits(value.x) ^ _key)
        _encryptedY = (BitConverter.SingleToInt32Bits(value.y) ^ _key)
    }

    public float Value
    {
        get 
        {
            int x = encryptedBits ^ _key;
            int y = encryptedBits ^ _key;
            return new Vector2Int(x, y);
        }

        set
        {
            if(_key == 0) _key = GenerateKey();

            _encryptedX = value.x ^ _key;
            _encryptedY = value.y ^ _key;
        }
    }

    public static implicit operator Vector2Int(ObscuredVector2 value) => value.Value;

    public static implicit operator ObscuredVector2(Vector2Int value) => new ObscuredVector2(value);

    private static int GenerateKey() => Random.Shared.Next();
}