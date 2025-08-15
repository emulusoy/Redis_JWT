using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
namespace Redis_JWT.Application.Common.Results
{
    public readonly record struct Error(string Code, string Description);
    public class Result
    {
        //burayi kurma sebebim handler bize bir result doner bu resulttan daha once var mi diye bakariz ayrica error gibi hazir olan seyleri dondeririz burada error verdiriyotum

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error? Error { get; }
        protected Result(bool success, Error? error) { IsSuccess = success; Error = error; }
        public static Result Success() => new(true, null);
        public static Result Failure(Error error) => new(false, error);
    }
    public class Result<T>
    {
        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;
        public Error? Error { get; }
        public T? Value { get; }
        private Result(bool success, T? value, Error? error) { IsSuccess = success; Value = value; Error = error; }
        public static Result<T> Success(T value) => new(true, value, null);
        public static Result<T> Failure(Error error) => new(false, default, error);
    }
}
