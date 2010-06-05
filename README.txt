NOTE: This is a work in progress and is not complete
Release: June 3, 2010

This tutorial is intended to show SQL CLR Procedures and Functions being used by a simple application.

There is a LOT of information in the VSQL4 script files.  They show the bulk of the information for loading and executing SQL CLR Procs and Functions.  READ THEM

The MyClrProcExport project is a single assembly that includes several SqlFunction and SqlProcedure methods that are loaded and executed using Data Builder and the CLR Proc Sample runner application.

STEPS
*************************************************************
Compile the MyClrProcExport project.  

Load the SampleDB.VDB4 from the debug folder of the CLRProcSampleRunner.  (this project does not do anything yet)

Load the AddAssemblyAndProcs.vsql4 file.  Run to load the assembly and assign the functions.  Also demonstrates how to update an asssembly using only SQL code.

Load the CallCLRProcs.vsql4 to see different ways of calling the functions and procedures.

Load the RemoveAssemblyAndProcs.vsql4 to remove all of the methods and then drop the assembly.

