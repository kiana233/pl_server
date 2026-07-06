using System.Globalization;
using System.Text.Json;

namespace PlServer.Replay;

public sealed class ReplayJsonLinesImporter
{
    public ReplayImportResult ImportFile(string filePath)
    {
        using var reader = File.OpenText(filePath);
        return Import(reader);
    }

    public ReplayImportResult ImportString(string jsonLines)
    {
        using var reader = new StringReader(jsonLines);
        return Import(reader);
    }

    public ReplayImportResult Import(TextReader reader)
    {
        ArgumentNullException.ThrowIfNull(reader);

        var steps = new List<ReplayStep>();
        var errors = new List<ReplayImportError>();
        var lineNumber = 0;

        while (reader.ReadLine() is { } line)
        {
            lineNumber++;

            if (string.IsNullOrWhiteSpace(line))
            {
                continue;
            }

            ImportLine(line, lineNumber, steps, errors);
        }

        return new ReplayImportResult(new ReplayCase(steps), errors);
    }

    public static byte[] ParseHex(string? hex)
    {
        if (string.IsNullOrWhiteSpace(hex))
        {
            return Array.Empty<byte>();
        }

        var tokens = hex.Split(' ', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        var bytes = new byte[tokens.Length];

        for (var i = 0; i < tokens.Length; i++)
        {
            bytes[i] = byte.Parse(tokens[i], NumberStyles.HexNumber, CultureInfo.InvariantCulture);
        }

        return bytes;
    }

    private static void ImportLine(
        string line,
        int lineNumber,
        List<ReplayStep> steps,
        List<ReplayImportError> errors)
    {
        using var document = ParseDocument(line, lineNumber, errors);
        if (document is null)
        {
            return;
        }

        var root = document.RootElement;
        var rawHex = ReadString(root, "rawHex") ?? string.Empty;
        var decodedHex = ReadString(root, "decodedHex") ?? string.Empty;

        if (string.IsNullOrWhiteSpace(rawHex) && string.IsNullOrWhiteSpace(decodedHex))
        {
            errors.Add(new ReplayImportError(lineNumber, "rawHex and decodedHex cannot both be empty."));
            return;
        }

        if (!TryReadDirection(root, lineNumber, errors, out var direction))
        {
            return;
        }

        byte[] rawBytes;
        byte[] decodedBytes;
        try
        {
            rawBytes = ParseHex(rawHex);
            decodedBytes = ParseHex(decodedHex);
        }
        catch (FormatException exception)
        {
            errors.Add(new ReplayImportError(lineNumber, $"Invalid hex value: {exception.Message}"));
            return;
        }

        var step = new ReplayStep
        {
            StepIndex = steps.Count,
            Direction = direction,
            RawHex = rawHex,
            DecodedHex = decodedHex,
            RawBytes = rawBytes,
            DecodedBytes = decodedBytes,
            ExpectedAc = ReadByte(root, "ac"),
            ExpectedSubAc = ReadByte(root, "subAc"),
            ProtocolName = ReadString(root, "protocolName"),
            Behavior = ReadString(root, "behavior"),
            SourceLabel = ReadString(root, "sourceLabel"),
            Status = ReadString(root, "status")
        };

        steps.Add(step);
    }

    private static JsonDocument? ParseDocument(
        string line,
        int lineNumber,
        List<ReplayImportError> errors)
    {
        try
        {
            return JsonDocument.Parse(line);
        }
        catch (JsonException exception)
        {
            errors.Add(new ReplayImportError(lineNumber, $"Invalid JSON: {exception.Message}"));
            return null;
        }
    }

    private static bool TryReadDirection(
        JsonElement root,
        int lineNumber,
        List<ReplayImportError> errors,
        out ReplayDirection direction)
    {
        var directionText = ReadString(root, "direction");

        if (string.IsNullOrWhiteSpace(directionText))
        {
            errors.Add(new ReplayImportError(lineNumber, "direction is required."));
            direction = ReplayDirection.Internal;
            return false;
        }

        if (!Enum.TryParse(directionText, ignoreCase: true, out direction))
        {
            errors.Add(new ReplayImportError(lineNumber, $"Unsupported direction: {directionText}."));
            direction = ReplayDirection.Internal;
            return false;
        }

        return true;
    }

    private static string? ReadString(JsonElement root, string propertyName)
    {
        if (!root.TryGetProperty(propertyName, out var property) || property.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        return property.ValueKind == JsonValueKind.String
            ? property.GetString()
            : property.ToString();
    }

    private static byte? ReadByte(JsonElement root, string propertyName)
    {
        if (!root.TryGetProperty(propertyName, out var property) || property.ValueKind == JsonValueKind.Null)
        {
            return null;
        }

        if (property.ValueKind == JsonValueKind.Number && property.TryGetByte(out var numericValue))
        {
            return numericValue;
        }

        if (property.ValueKind == JsonValueKind.String
            && byte.TryParse(property.GetString(), NumberStyles.Integer, CultureInfo.InvariantCulture, out var textValue))
        {
            return textValue;
        }

        return null;
    }
}
