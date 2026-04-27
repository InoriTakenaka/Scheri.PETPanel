using Scheri.PETPanel.Network.Contract;
using System.Text.Json.Serialization;

namespace Scheri.PETPanel.Utils
{
    [JsonSourceGenerationOptions(
        WriteIndented = true, 
        PropertyNameCaseInsensitive=true)]
    [JsonSerializable(typeof(StatusInfo))]
    [JsonSerializable(typeof(WorkStationConfiguration))]
    [JsonSerializable(typeof(PlcConfiguration))]
    [JsonSerializable(typeof(AppInfoConfiguration))]
    [JsonSerializable(typeof(AppSettings))]
    public partial class AppJsonContext : JsonSerializerContext { }
}
