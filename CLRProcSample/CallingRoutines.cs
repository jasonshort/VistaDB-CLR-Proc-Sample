using System;
using System.Collections.Generic;
using System.Text;
using VistaDB.Provider;
using System.IO;

namespace CLRProcSampleRunner
{
    class CallingRoutines
    {
        /// <summary>
        /// Call all of the CLR routines using SQL
        /// </summary>
        public static void CallAllSQL()
        {
            CallGetDatabaseVersionProcedureSQL();
            CallExportSchemaAndDataSQL("SampleDBSchema");
            CallGetDatabaseVersionFunctionSQL();
        }

        /// <summary>
        /// Call the export schema and data sql function to write out the xml file
        /// </summary>
        /// <param name="outputFilename">Name of the file to write to disk</param>
        public static void CallExportSchemaAndDataSQL(string outputFilename)
        {
            Console.WriteLine("Attempting to execute CLR Proc ExportSchemaAndData");
            using (VistaDBConnection connection = new VistaDBConnection())
            {
                connection.ConnectionString = SampleRunner.ConnectionString;
                connection.Open();

                try
                {
                    using (VistaDBCommand command = new VistaDBCommand())
                    {
                        // Straight forward way to call a function is just using SELECT
                        // You cannot EXEC a SqlFunction, and you cannot set the command here to be a stored proc
                        // Setting this command to a stored proc is a common error, the two are not the same
                        // SqlFunction = SELECT to call
                        // SqlProcdure = EXEC or direct call using StoredProcedure command type
                        command.Connection = connection;
                        command.CommandText = string.Format("SELECT ExportSchemaAndData('{0}');", outputFilename);
                        // This command does not return anything in the rowset, so just execute non query
                        command.ExecuteNonQuery();
                    }
                    Console.WriteLine(string.Format("Schema and Data export to {0}\\{1}.xml", Directory.GetCurrentDirectory(), outputFilename));
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to execute CLR-Proc ExportSchemaAndData, Reason: " + e.Message);
                }
            }
        }

        /// <summary>
        /// Call the Sql Function version to get the database version
        /// </summary>
        public static void CallGetDatabaseVersionFunctionSQL()
        {
            Console.WriteLine("Attempting to execute CLR Function GetDatabaseVersionFunction");
            using (VistaDBConnection connection = new VistaDBConnection(SampleRunner.ConnectionString))
            {
                connection.Open();

                try
                {
                    // Straight forward way to call a function is just using SELECT
                    // You cannot EXEC a SqlFunction, and you cannot set the command here to be a stored proc
                    // Setting this command to a stored proc is a common error, the two are not the same
                    // SqlFunction = SELECT to call
                    // SqlProcdure = EXEC or direct call using StoredProcedure command type
                    using (VistaDBCommand command = new VistaDBCommand())
                    {
                        command.Connection = connection;
                        command.CommandText = "SELECT GetVersionFunction();";
                        // The results are returned as a part of the standard rowset, so we only need to get back the first entry
                        Console.WriteLine(Convert.ToString(command.ExecuteScalar()));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to execute CLR Function GetVersionFunction, Reason: " + e.Message);
                }
            }
        }

        /// <summary>
        /// Call the Stored Proc version to get the database version
        /// </summary>
        public static void CallGetDatabaseVersionProcedureSQL()
        {
            Console.WriteLine("Attempting to execute CLR Procedure GetVersionProcedure");

            using (VistaDBConnection connection = new VistaDBConnection(SampleRunner.ConnectionString))
            {
                connection.Open();

                try
                {

                    // Setup a command against the database like any other command, but then you have to change the command type
                    // to tell it you are calling a stored proc directly
                    using (VistaDBCommand command = new VistaDBCommand())
                    {
                        // Use our connection from above
                        command.Connection = connection;

                        // Put the name of the stored proc, you don't need to EXEC.  This command will be called directly
                        // Be sure to include all the parameters
                        command.CommandText = "GetVersionProcedure(@versionout);";
                        command.CommandType = System.Data.CommandType.StoredProcedure;  // Normally this is just text that is being executed

                        // Build up the parameter to the clr proc
                        VistaDBParameter outparam = new VistaDBParameter();
                        // This name has to match the entry in the commandtext
                        outparam.ParameterName = "@versionout";
                        // Telling it that this is an OUTPUT parameter
                        // This is how you should always get values back from a stored proc.  The return value in a stored proc is really only
                        // meant to tell you the number of rows affected, not values.
                        outparam.Direction = System.Data.ParameterDirection.Output;

                        // Add it to the command
                        command.Parameters.Add(outparam);

                        // We are not expecting any return values, and the output parameters will still be filled out
                        // using ExecuteNonQuery.  This saves object setup and teardown of a reader when we don't need it.
                        command.ExecuteNonQuery();

                        // Make sure the outparam is not null
                        if (outparam.Value != null)
                        {
                            // Print it to the console
                            Console.WriteLine(Convert.ToString(outparam.Value));
                        }

                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine("Failed to execute CLR Function GetVersionProcedure, Reason: " + e.Message);
                }
            }
        }


    }
}
