using NLog.Time;
using System;
using System.Collections.Generic;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.Json.Serialization.Metadata;

namespace Scheri.PETPanel.Network.Contract;
/** Packet Structure:
 * [0] Start Marker (0x0F)
 * [1-4] Command (4 bytes, UTF-8)
 * [5..N-2] Payload (variable length)
 * [N-1] End Marker (0xFF)
 */
public readonly ref struct DevicePacket
{
    public static readonly byte StartMarker = 0x0F;
    public static readonly byte EndMarker = 0xFF;
    public static readonly int MinPacketLength = 6; // Start(1) + Command(4) + Payload(0) + End(1)
    
    public ReadOnlySpan<byte> Command { get; }
    public ReadOnlySpan<byte> Payload { get; }

    /// <summary>
    /// Initializes a new instance of the DevicePacket class with the specified command and optional payload.
    /// </summary>
    /// <param name="command">A read-only span of bytes representing the command to be sent to the device. Cannot be empty.</param>
    /// <param name="payload">An optional read-only span of bytes containing the payload data associated with the command. If not specified,
    /// the payload is empty.</param>
    public DevicePacket(ReadOnlySpan<byte> command, ReadOnlySpan<byte> payload = default)
    {
        Command = command;
        Payload = payload;
    }

    public static int TryParse(ReadOnlySpan<byte> buffer, out DevicePacket packet)
    {
        packet = default;
        if (buffer.Length < MinPacketLength) return 0; // Not enough data for a valid packet

        int headIndex = buffer.IndexOf(StartMarker);
        if(headIndex==-1) return buffer.Length;

        var slice = buffer.Slice(headIndex);
        int tail = slice.IndexOf(EndMarker);
        if (tail == -1) return headIndex;

        int packetLength = tail + 1; // Include the end marker
        if(packetLength< MinPacketLength) return headIndex + packetLength; // Not a valid packet, skip it

        packet = new DevicePacket(slice.Slice(1, 4), slice.Slice(5, tail - 5));
        return headIndex +packetLength; // Return the index after the end of the packet
    }

    public byte[] ToBytes()
    {
        int totalLength = 1 + 4 + Payload.Length + 1;
        byte[] packet = new byte[totalLength];

        packet[0] = StartMarker; // 0x0F
        Command.CopyTo(packet.AsSpan(1, 4));
        Payload.CopyTo(packet.AsSpan(5, Payload.Length));
        packet[totalLength - 1] = EndMarker; // 0xFF

        return packet;
    }
}


[JsonSerializable(typeof(StatusInfo))]
internal partial class AppJsonSerializerContext : JsonSerializerContext
{
}
