/*
 * Copyright (C) 2010 VistaDB Software, Inc.
 * CLR Proc Sample for VistaDB 4
 * http://www.vistadb.net/tutorials
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using VistaDB.Provider;
using System.IO;
using VistaDB.DDA;
using VistaDB;

namespace CLRProcSampleRunner
{
	class SampleRunner
	{
        public static IVistaDBDDA DDAObj = VistaDBEngine.Connections.OpenDDA();
		public static string ConnectionString { get; set; }
        public static string DatabaseFilename { get; set; }
        public static string AssemblyName { get; set; }
        public static string AssemblyFileName { get; set; }

		static void Main(string[] args)
		{
            ConnectionString = "Data Source = |DataDirectory|\\SampleDB.vdb4; Open Mode=NonExclusiveReadWrite";
            DatabaseFilename = "SampleDB.vdb4";
            
            AssemblyFileName = "MyClrProcExportAssembly.dll";       // The name of the CLR Assembly in disk
            AssemblyName = "MyClrProcExport";                       // The assembly name we want to reference it by (does not have to be the same as the actual assembly name)

            RegistrationRoutines.RegisterAssembly();
            RegistrationRoutines.RegisterAllMethodsDDA();
            RegistrationRoutines.ShowRegisteredCLRMethods();

            CallingRoutines.CallAllSQL();

            RegistrationRoutines.UnregisterAllMethodsDDA();
            RegistrationRoutines.UnregisterAssemblyDDA();
            
            Console.WriteLine("Done..Press any key to finish");
			Console.ReadLine();

            Shutdown();
		}

        static void Shutdown()
        {
            // Cleanup when we go to shutdown
            if (DDAObj != null)
                DDAObj.Dispose();
        }


	}
}
