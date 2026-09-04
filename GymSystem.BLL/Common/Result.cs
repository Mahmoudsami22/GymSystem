using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.Collections.Generic;
using System.Text;

namespace GymSystem.BLL.Common
{
    public record Result(bool Success, string? Error = null, ResultKind kind = ResultKind.Ok)
    {
        public static Result Ok() => new Result(true);
        public static Result Fail(string errorMessage, ResultKind kind = ResultKind.Conflict) => new Result(false, errorMessage, kind);

        public static Result NotFound(string errorMessage = "Not Found") => new Result(false, errorMessage, ResultKind.NotFound);

        public static Result Validation(string errorMessage) => new Result(false, errorMessage, ResultKind.ValidationFailed);

    }
    public record Result<T>(bool Success, T? Value, string? Error = null, ResultKind kind = ResultKind.Ok)
    {

        public static Result<T> Ok(T Value) => new(true, Value);
        public static Result<T> Fail(string errorMessage, ResultKind kind = ResultKind.Conflict) => new(false, default, errorMessage, kind);
        public static Result<T> NotFound(string errorMessage = "Not Found") => new(false, default, errorMessage, ResultKind.NotFound);
    }

    public enum ResultKind
    {
        Ok,
        NotFound,
        Conflict,
        ValidationFailed,
        Forbidden
    }

 }