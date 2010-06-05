/*
 * Copyright (C) 2010 VistaDB Software, Inc.
 * CLR Proc Sample for VistaDB 4
 * http://www.vistadb.net/tutorials
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using VistaDB.DDA;
using VistaDB;
using System.Linq;


namespace CLRProcSampleRunner
{
    class RegistrationRoutines
    {
        #region Register Helper Methods

        public static void RegisterAssembly()
        {
            using (IVistaDBDatabase db = SampleRunner.DDAObj.OpenDatabase(SampleRunner.DatabaseFilename, VistaDBDatabaseOpenMode.NonexclusiveReadWrite, null))
            {
                IVistaDBAssemblyCollection assemblies = db.GetAssemblies();
                
                // Does the assembly already exist in the database?
                if (assemblies[SampleRunner.AssemblyName] != null )
                {
                    // Call update instead of add
                    db.UpdateAssembly(SampleRunner.AssemblyName, SampleRunner.AssemblyFileName, "Updated Sample CLR Assembly");
                    return;
                }

                db.AddAssembly(SampleRunner.AssemblyName, SampleRunner.AssemblyFileName, "My great sample CLR Proc asssembly");
            }
        }

        public static void RegisterAllMethodsDDA()
        {
            using (IVistaDBDatabase db = SampleRunner.DDAObj.OpenDatabase(SampleRunner.DatabaseFilename, VistaDBDatabaseOpenMode.NonexclusiveReadWrite, null))
            {
                // NOTE:  There is a difference in how you register the CLR Proc in DDA than through SQL
                // The ClrHostedMethod must just be the Namespace.Class.Method.  The assembly is added for you from the assemblyName parameter (3rd param)
                // In SQL during an add function or add procedure we don't know which assembly name the method belongs to, so it must be specified in the 
                // full naming, but through DDA we have the assembly through the assemblyName param.

                // procedureName = the name we want to use to call the procedure or function (the attribute on the method tells us which type it is)
                // ClrHostedMethod = Namespace.Class.Method of the method to call 
                // assemblyName = The friendly assembly name used when registering the assembly into the database
                // description = The text description for this method
                db.RegisterClrProcedure("ExportSchemaAndData", "MyClrProcExportNamespace.CLRProcedures.ExportSchemaAndData", SampleRunner.AssemblyName, null);

                db.RegisterClrProcedure("GetVersionFunction", "MyClrProcExportNamespace.CLRProcedures.GetDatabaseVersionFunction", SampleRunner.AssemblyName, null);

                db.RegisterClrProcedure("GetVersionProcedure", "MyClrProcExportNamespace.CLRProcedures.GetDatabaseVersionProcedure", SampleRunner.AssemblyName, null);
            }
        }

        public static void ShowRegisteredCLRMethods()
        {
            using (IVistaDBDatabase db = SampleRunner.DDAObj.OpenDatabase(SampleRunner.DatabaseFilename, VistaDBDatabaseOpenMode.NonexclusiveReadWrite, null))
            {
                IVistaDBClrProcedureCollection registeredCLRProcs = db.GetClrProcedures();

                foreach (IVistaDBClrProcedureInformation info in registeredCLRProcs)
                {
                    Console.WriteLine(string.Format("Method: {0} - {1}", info.Name, info.FullHostedName));
                }
            }
        }

        #endregion

        #region Unregister Help Methods
        
        public static void UnregisterAllMethodsDDA()
        {
            using (IVistaDBDatabase db = SampleRunner.DDAObj.OpenDatabase(SampleRunner.DatabaseFilename, VistaDBDatabaseOpenMode.NonexclusiveReadWrite, null))
            {
                IVistaDBClrProcedureCollection registeredCLRProcs = db.GetClrProcedures();
                foreach (IVistaDBClrProcedureInformation info in registeredCLRProcs)
                {
                    db.UnregisterClrProcedure(info.Name);
                }
            }
        }

        public static void UnregisterAssemblyDDA()
        {
            using (IVistaDBDatabase db = SampleRunner.DDAObj.OpenDatabase(SampleRunner.DatabaseFilename, VistaDBDatabaseOpenMode.NonexclusiveReadWrite, null))
            {
                db.DropAssembly(SampleRunner.AssemblyName, true);
            }
        }


        #endregion
    }
}
