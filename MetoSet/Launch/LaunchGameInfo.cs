using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace MTMCL.Launch
{
    public class LaunchGameInfo
    {
        public string MCPath { get; private set; }
        public Login.IAuth Auth { get; private set; }
        public Versions.VersionJson Version { get; private set; }
        public string JavaPath { get; private set; }
        public int JavaXmx { get; private set; }
        public WindowSize Size { get; private set; }
        public ServerInfo Server { get; private set; }
        private LaunchGameInfo(string MCPath, Login.IAuth auth, Versions.VersionJson ver, string java, int xmx, WindowSize size, ServerInfo info) {
            Auth = auth;
            Version = ver;
            JavaPath = java;
            JavaXmx = xmx;
            Size = size;
            Server = info;
            this.MCPath = MCPath;
        }
        public static LaunchGameInfo CreateInfo(string path, Login.IAuth auth, Versions.VersionJson ver, string java, int xmx)
        {
            return CreateInfo(path, auth, ver, java, xmx, new WindowSize() { Width = 854, Height = 480 }, null);
        }
        public static LaunchGameInfo CreateInfo(string path,Login.IAuth auth, Versions.VersionJson ver, string java, int xmx, WindowSize size)
        {
            return CreateInfo(path, auth, ver, java, xmx, size, null);
        }
        public static LaunchGameInfo CreateInfo(string path, Login.IAuth auth, Versions.VersionJson ver, string java, int xmx, ServerInfo server)
        {
            return CreateInfo(path, auth, ver, java, xmx, new WindowSize() { Width = 854, Height = 480 }, server);
        }
        public static LaunchGameInfo CreateInfo(string path,Login.IAuth auth, Versions.VersionJson ver, string java, int xmx, WindowSize size, ServerInfo server)
        {
            return new LaunchGameInfo(path,auth, ver, java, xmx, size, server);
        }
    }
    public class WindowSize
    {
        public int Width { get; set; }
        public int Height { get; set; }
    }
    public class ServerInfo
    {
        public string Ip { get; set; }
        public ushort Port { get; set; }
    }
}
