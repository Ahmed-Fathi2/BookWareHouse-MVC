using System;
using System.Collections.Generic;
using System.Text;

namespace BookWarehouse.Application.Comman
{
    public class Result
    {

        public bool IsSuccess { get; }
        public bool IsFailure => !IsSuccess;

        public Error Error { get; } 


        public Result(bool isSuccess, Error error)
        {

            if ((isSuccess && error != Error.None ) || (!isSuccess && error ==Error.None ))
                throw new InvalidOperationException("Invalid result state");

            IsSuccess = isSuccess;
            Error = error!;

        }

        public static Result Success() => new Result(true, Error.None);

        public static Result Failure(Error error) => new Result(false, error);

        public static Result<TValue> Success<TValue>(TValue value) => new Result<TValue>(true, Error.None, value);

        public static Result<TValue> Failure<TValue>(Error error) => new Result<TValue>(false, error, default);
    }


    public class Result<TValue> : Result
    {
        private readonly TValue? _value;

        public Result(bool isSuccess, Error error, TValue? value) : base(isSuccess, error)
        {
            _value = value;
        }

        public TValue Value => IsSuccess
           ? _value!
           : throw new InvalidOperationException("Failure results cannot have value");



    }
}
