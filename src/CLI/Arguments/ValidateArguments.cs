namespace CLI.ValidateArguments;

using ErrorHandler;
using Flags;

public enum Result
{
    Success,
    Error
}

public class Validate
{
    public static Result Arguments(Dictionary<FlagType, string> options)
    {
        var validators = new Func<Dictionary<FlagType, string>, Result>[]
        {
            ValidateVersion,
            ValidateContains,
            ValidateAllRemote,
            ValidateSortValue,
            ValidatePrintTopValue,
        };

        foreach (var validator in validators)
        {
            Result result = validator(options);

            if (result == Result.Error) return Result.Error;
        }

        return Result.Success;
    }

    private static Result ValidateVersion(Dictionary<FlagType, string> options)
    {
        if (options.ContainsKey(FlagType.Version) && options.Count > 1)
        {
            Error.Register("You cannot use --version with any other option");
            return Result.Error;
        }

        return Result.Success;
    }

    private static Result ValidateContains(Dictionary<FlagType, string> options)
    {
        bool contains = (
            options.ContainsKey(FlagType.Contains) && options.ContainsKey(FlagType.Nocontains)
        );

        if (contains)
        {
            Error.Register("You cannot use both --contains and --no-contains");
            return Result.Error;
        }

        return Result.Success;
    }

    private static Result ValidateAllRemote(Dictionary<FlagType, string> options)
    {
        if (options.ContainsKey(FlagType.All) && options.ContainsKey(FlagType.Remote))
        {
            Error.Register("You cannot use both --all and --remote");
            return Result.Error;
        }

        return Result.Success;
    }

    private static Result ValidateSortValue(Dictionary<FlagType, string> options)
    {
        if (options.TryGetValue(FlagType.Sort, out string? value))
        {
            if (value == "date" || value == "name" || value == "ahead" || value == "behind")
            {
                return Result.Success;
            }

            Error.Register(
                "Value for --sort is missing. Valid values are: date, name, ahead, behind"
            );

            return Result.Error;
        }

        return Result.Success;
    }

    private static Result ValidatePrintTopValue(Dictionary<FlagType, string> options)
    {
        if (options.TryGetValue(FlagType.Printtop, out string? value))
        {
            if (!int.TryParse(value, out int numberValue)) return Result.Error;

            if (numberValue < 1)
            {
                Error.Register("Value for --print-top must be greater than 0");
                return Result.Error;
            }
        }

        return Result.Success;
    }
}
