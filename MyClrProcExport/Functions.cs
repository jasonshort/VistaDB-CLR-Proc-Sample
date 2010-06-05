/*
 * Copyright (C) 2010 VistaDB Software, Inc.
 * CLR Proc Sample for VistaDB 4
 * http://www.vistadb.net/tutorials
 * 
 */

using System;
using System.Collections.Generic;
using System.Text;
using VistaDB.Compatibility.SqlServer;
using VistaDB.Provider;
using VistaDB.VistaDBTypes;
using VistaDB.DDA;
using System.IO;
using VistaDB;

namespace MyClrProcExportNamespace
{
    //
    //This class contains CLR methods that can be loaded into VistaDB through DataBuilder using the included scripts
    //
    public class CLRProcedures
    {
        /// <summary>
        /// This proc uses DDA to get the current context and perform actions against the database.
        /// In this case we are exporting the data and schema to an XML file
        /// NOTE: This is a SqlFunction because we are returning the string rather than using an OUTPUT parameter. (See help for more info)
        /// </summary>
        /// <param name="fileName">Filename to write out for the XML Data and Schema</param>
        /// <returns></returns>
        [SqlFunction]
        public static bool ExportSchemaAndData(string fileName)
        {
            try
            {
//#if DEBUG
//                // If we are not debugging force the debugger to attach
//                if (!System.Diagnostics.Debugger.IsAttached)
//                    System.Diagnostics.Debugger.Launch();
//                else
//                    // Otherwise just force a break in the debugger
//                    System.Diagnostics.Debugger.Break();
//#endif

                //To use DDA within a CLR proc you must create a new IVistaDBDatabase from the current VistaDBContext like this...
                // NOTE: DO NOT Dispose this object!  It is an internal engine object that you are getting a copy of, you didn't
                // allocate it here, so don't dispose of it.
                IVistaDBDatabase db = VistaDB.VistaDBContext.DDAChannel.CurrentDatabase;
                {
                    foreach (string s in db.GetTableNames())
                    {
                        db.AddToXmlTransferList(s);
                    }

                    db.ExportXml(string.Format(@"{0}\{1}.xml", Directory.GetCurrentDirectory(), fileName), VistaDBXmlWriteMode.All);
                }
            }
            catch( Exception e )
            {
                throw new ApplicationException("CLR Function failed exporting data and schema", e );
            }
            
            return true;
        }

        /// <summary>
        /// This proc uses the ADO.NET SQL interface to execute a command against the database.
        /// The command gets the current database version string.
        /// NOTE: This is a SqlFunction because we are returning the string rather than using an OUTPUT parameter. (See help for more info)
        /// </summary>
        /// <returns></returns>
        [SqlFunction]
        public static string GetDatabaseVersionFunction()
        {
            try
            {
                //To open a SQL connection to VistaDB from within a CLR Proc you must set the connection string to Context connection=true like this....
                //NOTE: We DO want to dispose of this object because we are the ones allocating it
                using (VistaDBConnection conn = new VistaDBConnection("Context Connection=true"))
                {
                    conn.Open();
                    using (VistaDBCommand command = new VistaDBCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "SELECT @@Version";
                        return Convert.ToString(command.ExecuteScalar());
                    }
                }
            }
            catch (Exception e)
            {
                return e.Message;
            }
        }

        /// <summary>
        /// This proc uses the ADO.NET SQL interface to execute a command against the database.
        /// The command gets the current database version string.
        /// </summary>
        /// <returns></returns>
        [SqlProcedure]
        public static int GetDatabaseVersionProcedure(out string versionString )
        {
            try
            {
                //To open a SQL connection to VistaDB from within a CLR Proc you must set the connection string to Context connection=true like this....
                //NOTE: We DO want to dispose of this object because we are the ones allocating it
                using (VistaDBConnection conn = new VistaDBConnection("Context Connection=true"))
                {
                    conn.Open();
                    using (VistaDBCommand command = new VistaDBCommand())
                    {
                        command.Connection = conn;
                        command.CommandText = "SELECT @@Version";
                        versionString = Convert.ToString(command.ExecuteScalar());
                        return 0;
                    }
                }
            }
            catch (Exception e)
            {
                throw new ApplicationException("Unable to get the database version due to Application Error", e);
            }
        }

        /// <summary>
        /// This is the basic signature for a CLR Trigger.  This will be covered in a separate tutorial further.
        /// </summary>
        [SqlTrigger(Name = "MyTrigger", Target = "TargetTable", Event = "FOR UPDATE")]
        public static void MyTrigger()
        {
            // Replace with your own code
            SqlContext.Pipe.Send("Trigger FIRED");
        }

    }
}
