using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;
using Bingosoft.TrioFramework;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle("TrioFramework.K2Client")]
[assembly: AssemblyDescription("K2工作流实现")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Bingosoft")]
[assembly: AssemblyProduct("TrioFramework")]
[assembly: AssemblyCopyright("Copyright ©Hades 2014")]
[assembly: AssemblyTrademark("")]
[assembly: AssemblyCulture("")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid("1f00dd7a-0eb8-4358-a262-ef0fba3904c8")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion"1.4.2"")]
[assembly: AssemblyVersion(TrioVer.K2Workflow)]
[assembly: AssemblyFileVersion(TrioVer.K2Workflow)]
[assembly: InternalsVisibleTo("Bingosoft.TrioFramework.Workflow.Core")]
[assembly: InternalsVisibleTo("Bingosoft.TrioFramework.WindowsServices")]
[assembly: InternalsVisibleTo("WorkflowK2ClientTest")]
