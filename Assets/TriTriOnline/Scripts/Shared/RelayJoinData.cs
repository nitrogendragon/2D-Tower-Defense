using System;

public struct RelayJoinData
{
    public string JoinCode;
    public string IPV4Address;
    public ushort Port;
    public Guid AllocationID;
    public byte[] AllocationIDBytes;
    public byte[] ConnectionData;
    public byte[] HostconnectionData;
    public byte[] key;
}
