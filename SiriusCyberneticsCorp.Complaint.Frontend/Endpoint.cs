﻿namespace SiriusCyberneticsCorp.Complaint.Frontend
{
    using System;
    using System.Runtime.InteropServices;

    using NServiceBus;

    /// <summary>
    /// When using the generic host you have to have at least an implementor
    /// of IConfigureThisEndpoint
    /// 
    /// IWantCustomInitialization allows to specialize the configuration of the bus
    /// </summary>
    public class Endpoint : IConfigureThisEndpoint, AsA_Server, IWantCustomInitialization, ICanBeMean
    {
        const int SwpNosize = 0x0001;

        [DllImport("kernel32.dll", ExactSpelling = true)]
        private static extern IntPtr GetConsoleWindow();

        private static readonly IntPtr ConsolePtr = GetConsoleWindow();

        [DllImport("user32.dll", EntryPoint = "SetWindowPos")]
        public static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cx, int cy, int wFlags);

        public void Init()
        {
            Console.SetWindowSize(70, 30);
            SetWindowPos(ConsolePtr, 0, 10, 420, 0, 0, SwpNosize);

            Configure.With(AllAssemblies.Except("Raven.Backup.exe"))
                .DefaultBuilder()
                .DefiningCommandsAs(t => t.Namespace != null && t.Namespace.StartsWith("SiriusCyberneticsCorp.InternalMessages.Complaint"))
                .DefiningEventsAs(t => t.Namespace != null && t.Namespace.StartsWith("SiriusCyberneticsCorp.Contract.Facility"))
                .JsonSerializer();
        }

        public void BeMean()
        {
            Configure.Instance.Configurer.ConfigureProperty<ComplainAboutSender>(s => s.MeanMode, true);
        }
    }

    public interface ICanBeMean
    {
        void BeMean();
    }
}