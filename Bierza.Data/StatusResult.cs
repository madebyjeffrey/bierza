using System.Security.Cryptography;

namespace Bierza.Data;


public record Status<TE>(TE Code) where TE : Enum
{
    public static implicit operator Status<TE>(TE code) => new Status<TE>(code);
}

public record Status<TE, T>(TE Code, T? Data) where TE : Enum
{
    public static implicit operator Status<TE, T>(TE code) => new(code, default);
    public static implicit operator Status<TE, T>((TE code, T? data) datum) => new(datum.code, datum.data);
}

public class Status
{
    public static Status<TE, T> From<TE, T>(TE code, T data) where TE : Enum => new Status<TE, T>(code, data);
    public static Status<TE> From<TE>(TE code) where TE : Enum => new Status<TE>(code);
}
