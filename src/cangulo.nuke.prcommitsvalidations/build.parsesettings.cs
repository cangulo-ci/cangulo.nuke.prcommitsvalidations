using cangulo.nuke.prcommitsvalidations.Constants;
using cangulo.nuke.prcommitsvalidations.Models;
using Nuke.Common;
using System;
using System.IO;
using System.Text.Json;

internal partial class Build : NukeBuild
{
    private void ParseSettings()
    {
        if (File.Exists(ValidationSettingsPath))
        {
            var fileContent = File.ReadAllText(ValidationSettingsPath);
            ValidationSettings = JsonSerializer.Deserialize<ValidationSettings>(fileContent, SerializerContants.DESERIALIZER_OPTIONS);

            Logger.Trace($"Request Mapped {JsonSerializer.Serialize(ValidationSettings, SerializerContants.SERIALIZER_OPTIONS)}");
        }
        else
            throw new Exception($"validation settings file not provided in path {ValidationSettingsPath}");
    }
}