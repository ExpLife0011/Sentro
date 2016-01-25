﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Net.NetworkInformation;
using System.Threading.Tasks;
using PcapDotNet.Core;
using PcapDotNet.Core.Extensions;

namespace Sentro.Utilities
{
    /*
        Responsipility : Provide general network functions
    */
    public class NetworkUtilites
    { 
        
        public static void InsertStaticMac(NetworkInterface nic,string ipAddress,string macAddress)
        {            
            /*netish interface ip add neightbors "Network type" Ip mac*/
            macAddress = macAddress.Replace(':', '-');
            var networkName = nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                ? "\"Local Area Connection\""
                : "\"Wireless Network Connection\"";
            Process pProcess = new Process
            {
                StartInfo =
                {
                    FileName = "netsh",
                    Arguments = "interface ip add neighbors "+ networkName + " " + ipAddress + " " + macAddress,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            pProcess.Start();            
        }

        public static void DeleteStaticMac(NetworkInterface nic,string ipAddress)
        {
            var networkName = nic.NetworkInterfaceType == NetworkInterfaceType.Ethernet
                ? "\"Local Area Connection\""
                : "\"Wireless Network Connection\"";
            Process pProcess = new Process
            {
                StartInfo =
                {
                    FileName = "netsh",
                    Arguments = "interface ip delete neighbors "+ networkName + " " + ipAddress,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            pProcess.Start();
        }

        /*http://stackoverflow.com/questions/12802888/get-a-machines-mac-address-on-the-local-network-from-its-ip-in-c-sharp*/
        public static string GetMacAddress(string ipAddress)
        {
            Ping ping = new Ping();
            ping.Send(ipAddress);
            Process pProcess = new Process
            {
                StartInfo =
                {
                    FileName = "arp",
                    Arguments = "-a " + ipAddress,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            pProcess.Start();
            var strOutput = pProcess.StandardOutput.ReadToEnd();
            var substrings = strOutput.Split('-');
            if (substrings.Length < 8) return "not found";
            var macAddress = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", substrings[3].Substring(Math.Max(0, substrings[3].Length - 2)), substrings[4], substrings[5], substrings[6], substrings[7], substrings[8].Substring(0, 2));
            return macAddress;
        }
        
        /*
        public static Dictionary<string,string> GetMacAddress(HashSet<string> ipAddresses)
        {
            Ping ping = new Ping();            
            Task[] pingTasks = new Task[ipAddresses.Count];
            int i = 0;
            foreach (var ip in ipAddresses)            
                pingTasks[i++] = ping.SendPingAsync(ip);
            Task.WaitAll(pingTasks);
            //TODO: test out the format of the ouput and capture its output to get mac addresses
            Process pProcess = new Process
            {
                StartInfo =
                {
                    FileName = "arp",
                    Arguments = "-a " + ipAddress,
                    UseShellExecute = false,
                    RedirectStandardOutput = true,
                    CreateNoWindow = true
                }
            };
            pProcess.Start();
            var strOutput = pProcess.StandardOutput.ReadToEnd();
            var substrings = strOutput.Split('-');
            if (substrings.Length < 8) return "not found";
            var macAddress = string.Format("{0}:{1}:{2}:{3}:{4}:{5}", substrings[3].Substring(Math.Max(0, substrings[3].Length - 2)), substrings[4], substrings[5], substrings[6], substrings[7], substrings[8].Substring(0, 2));
            return macAddress;
        }
        */

        public static string GetGatewayIp(LivePacketDevice nic)
        {
            var gatewayIpAddressInformation = nic.GetNetworkInterface().GetIPProperties().GatewayAddresses.FirstOrDefault();
            return gatewayIpAddressInformation?.Address.MapToIPv4().ToString() ?? "not found";
        }

        public static LivePacketDevice GetLivePacketDevice(string ip)
        {
            var devices = LivePacketDevice.AllLocalMachine;
            foreach (var device in devices)
            {
                foreach (var address in device.Addresses)
                {
                    if (address.Address.Family == SocketAddressFamily.Internet)
                        if (address.Address.ToString().Replace("Internet ", "").Equals(ip))
                            return device;
                }
            }
            return null;
        }

    }
}
 