namespace EaiConverter.CodeGenerator
{
    using System;
    using System.CodeDom;
    using System.CodeDom.Compiler;
    using System.IO;

    using EaiConverter.Processor;

    public class CsharpSourceCodeGeneratorService : ISourceCodeGeneratorService
    {
        //TODO VC : Put the output directory as a parameter";
        public const string SolutionDestinationPath = "./GeneratedSolution";

        public const string ProjectDestinationPath = "./GeneratedSolution/GeneratedSolution";

        private const string LineToReplace = "    <!-- insert file Generated here -->";

        public void Generate(CodeCompileUnit targetUnit)
        {
            CodeDomProvider provider = CodeDomProvider.CreateProvider("CSharp");
            CodeGeneratorOptions options = new CodeGeneratorOptions();
            options.BracingStyle = "C";
            options.IndentString = "    ";
            options.BlankLinesBetweenMembers = true;

            this.CreateSolutionDirectory();

            // Build the output file name.
            foreach (CodeNamespace namespaceUnit in targetUnit.Namespaces)
            {
                var namespaceName = namespaceUnit.Name;

                if (namespaceUnit.Types.Count > 0)
                {
                    string sourceFile;
                    string relativeSourceFile;

                    if (provider.FileExtension[0] == '.')
                    {
                        sourceFile = this.PathFromNamespace(ProjectDestinationPath, namespaceName) + "/" + namespaceUnit.Types[0].Name + provider.FileExtension;
                        relativeSourceFile = this.ConvertNamespaceToPath(namespaceName) + "/" + namespaceUnit.Types[0].Name + provider.FileExtension;
                    }
                    else
                    {
                        sourceFile = this.PathFromNamespace(ProjectDestinationPath, namespaceName) + "/" + namespaceUnit.Types[0].Name + "." + provider.FileExtension;
                        relativeSourceFile = this.ConvertNamespaceToPath(namespaceName) + "/" + namespaceUnit.Types[0].Name + "." + provider.FileExtension;
                    }

                    if (ConfigurationApp.GetProperty(sourceFile) != "true")
                    {
                        ConfigurationApp.SaveProperty(sourceFile, "true");
                        using (var sw = new StreamWriter(sourceFile, false))
                        {
                            var tw = new IndentedTextWriter(sw, "    ");
                            provider.GenerateCodeFromNamespace(namespaceUnit, tw, options);
                            tw.Close();
                        }

                       // using (var file = new StreamWriter(ProjectDestinationPath + "\\GeneratedSolution.csproj", true))
                       // {
                       //     file.WriteLine("    <Compile Include=\"" + relativeSourceFile + "\"/>\n");
                       // }

                        if (File.Exists(ProjectDestinationPath + "/GeneratedSolution.csproj"))
                        {
                            string projectFile = File.ReadAllText(ProjectDestinationPath + "/GeneratedSolution.csproj");
                            projectFile = projectFile.Replace(
                                LineToReplace,
                                LineToReplace + "\n" + "    <Compile Include=\"" + relativeSourceFile + "\"/>");
                            File.WriteAllText(ProjectDestinationPath + "/GeneratedSolution.csproj", projectFile);
                        }

                        Console.WriteLine(sourceFile + " has been generated");
                    }
                    else
                    {
                        Console.WriteLine("############## Warning" + sourceFile + " has already been generated");
                    }

                }
                else
                {
                    Console.WriteLine("################### Warning" + namespaceName + " is empty");
                }
            }
        }

        public void GenerateSolutionAndProjectFiles()
        {
            this.CreateSolutionDirectory();
            using (var file = new StreamWriter(SolutionDestinationPath + "/GeneratedSolution.sln"))
            {
                file.Write(GeneratedSolution_sln);
            }

            using (var file = new StreamWriter(ProjectDestinationPath + "/GeneratedSolution.csproj"))
            {
                file.Write(GeneratedSolution_csproj);
            }

            using (var file = new StreamWriter(ProjectDestinationPath + "/Properties/AssemblyInfo.cs"))
            {
                file.Write(AssemblyInfo_cs);
            }
        }

        private void CreateSolutionDirectory()
        {
            if (Directory.Exists(SolutionDestinationPath)
                && ConfigurationApp.GetProperty("IsGeneratedSolutionDirectoryPurged") != "true")
            {
                Directory.Delete(SolutionDestinationPath, true);
                ConfigurationApp.SaveProperty("IsGeneratedSolutionDirectoryPurged", "true");
            }

            Directory.CreateDirectory(SolutionDestinationPath);
            Directory.CreateDirectory(ProjectDestinationPath);
            Directory.CreateDirectory(ProjectDestinationPath + "/Properties");
        }

        // TODO refactor because not really SRP
        private string PathFromNamespace(string outputPath, string ns)
        {
            var path = String.Format("{0}/{1}", outputPath, this.ConvertNamespaceToPath(ns));

            Directory.CreateDirectory(path);
            return path;
        }

        private string ConvertNamespaceToPath(string ns)
        {
            var path = ns.Replace('.', '/');
            return path;
        }


        public static string GeneratedSolution_sln = @"
Microsoft Visual Studio Solution File, Format Version 12.00
# Visual Studio 2012
Project(""{FAE04EC0-301F-11D3-BF4B-00C04F79EFBC}"") = ""GeneratedSolution"", ""GeneratedSolution\GeneratedSolution.csproj"", ""{35F53A37-C2B2-41CE-BB32-F6AD0C862E0F}""
EndProject
Global
	GlobalSection(SolutionConfigurationPlatforms) = preSolution
		Debug|Any CPU = Debug|Any CPU
		Release|Any CPU = Release|Any CPU
	EndGlobalSection
	GlobalSection(ProjectConfigurationPlatforms) = postSolution
		{35F53A37-C2B2-41CE-BB32-F6AD0C862E0F}.Debug|Any CPU.ActiveCfg = Debug|Any CPU
		{35F53A37-C2B2-41CE-BB32-F6AD0C862E0F}.Debug|Any CPU.Build.0 = Debug|Any CPU
		{35F53A37-C2B2-41CE-BB32-F6AD0C862E0F}.Release|Any CPU.ActiveCfg = Release|Any CPU
		{35F53A37-C2B2-41CE-BB32-F6AD0C862E0F}.Release|Any CPU.Build.0 = Release|Any CPU
	EndGlobalSection
	GlobalSection(SolutionProperties) = preSolution
		HideSolutionNode = FALSE
	EndGlobalSection
EndGlobal

";


        public static string GeneratedSolution_csproj = @"<?xml version=""1.0"" encoding=""utf-8""?>
<Project ToolsVersion=""4.0"" DefaultTargets=""Build"" xmlns=""http://schemas.microsoft.com/developer/msbuild/2003"">
  <Import Project=""$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props"" Condition=""Exists('$(MSBuildExtensionsPath)\$(MSBuildToolsVersion)\Microsoft.Common.props')"" />
  <PropertyGroup>
    <Configuration Condition="" '$(Configuration)' == '' "">Debug</Configuration>
    <Platform Condition="" '$(Platform)' == '' "">AnyCPU</Platform>
    <ProjectGuid>{35F53A37-C2B2-41CE-BB32-F6AD0C862E0F}</ProjectGuid>
    <OutputType>Library</OutputType>
    <AppDesignerFolder>Properties</AppDesignerFolder>
    <RootNamespace>GeneratedSolution</RootNamespace>
    <AssemblyName>GeneratedSolution</AssemblyName>
    <TargetFrameworkVersion>v4.5</TargetFrameworkVersion>
    <FileAlignment>512</FileAlignment>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Debug|AnyCPU' "">
    <DebugSymbols>true</DebugSymbols>
    <DebugType>full</DebugType>
    <Optimize>false</Optimize>
    <OutputPath>bin\Debug\</OutputPath>
    <DefineConstants>DEBUG;TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <PropertyGroup Condition="" '$(Configuration)|$(Platform)' == 'Release|AnyCPU' "">
    <DebugType>pdbonly</DebugType>
    <Optimize>true</Optimize>
    <OutputPath>bin\Release\</OutputPath>
    <DefineConstants>TRACE</DefineConstants>
    <ErrorReport>prompt</ErrorReport>
    <WarningLevel>4</WarningLevel>
  </PropertyGroup>
  <ItemGroup>
    <Reference Include=""System"" />
    <Reference Include=""System.Core"" />
    <Reference Include=""System.Xml.Linq"" />
    <Reference Include=""System.Data.DataSetExtensions"" />
    <Reference Include=""Microsoft.CSharp"" />
    <Reference Include=""System.Data"" />
    <Reference Include=""System.Xml"" />
  </ItemGroup>
  <ItemGroup>
    <Compile Include=""Properties\AssemblyInfo.cs"" />
    <!-- insert file Generated here -->
  </ItemGroup>
  <Import Project=""$(MSBuildToolsPath)\Microsoft.CSharp.targets"" />
  <!-- To modify your build process, add your task inside one of the targets below and uncomment it. 
       Other similar extension points exist, see Microsoft.Common.targets.
  <Target Name=""BeforeBuild"">
  </Target>
  <Target Name=""AfterBuild"">
  </Target>
  -->
</Project>
";

        private static string AssemblyInfo_cs = @"using System.Reflection;
using System.Runtime.CompilerServices;
using System.Runtime.InteropServices;

// General Information about an assembly is controlled through the following 
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
[assembly: AssemblyTitle(""GeneratedSolution"")]
[assembly: AssemblyDescription("""")]
[assembly: AssemblyConfiguration("""")]
[assembly: AssemblyCompany(""Microsoft"")]
[assembly: AssemblyProduct(""GeneratedSolution"")]
[assembly: AssemblyCopyright(""Copyright © Microsoft 2015"")]
[assembly: AssemblyTrademark("""")]
[assembly: AssemblyCulture("""")]

// Setting ComVisible to false makes the types in this assembly not visible 
// to COM components.  If you need to access a type in this assembly from 
// COM, set the ComVisible attribute to true on that type.
[assembly: ComVisible(false)]

// The following GUID is for the ID of the typelib if this project is exposed to COM
[assembly: Guid(""6e10df1e-7154-44be-9dc1-c95a47c19670"")]

// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version 
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Build and Revision Numbers 
// by using the '*' as shown below:
// [assembly: AssemblyVersion(""1.0.*"")]
[assembly: AssemblyVersion(""1.0.0.0"")]
[assembly: AssemblyFileVersion(""1.0.0.0"")]
";
    }
}
