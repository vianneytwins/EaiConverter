using System;
using System.Xml.Linq;
using System.Linq;
using System.Collections.Generic;
using EaiConverter.Model;
using EaiConverter.Parser.Utils;
using EaiConverter.Processor;

namespace EaiConverter.Parser
{
	public class GlobalVariableParser
	{
        public const string Defaultsubstvar = "defaultVars.substvar";
        public const string DefaultsubstvarDirectory = "/defaultVars";

        public GlobalVariablesRepository ParseVariable(string filePath)
        {
            var globalVariablesRepository = new GlobalVariablesRepository();
            globalVariablesRepository.Name = this.ParseFileName(filePath);

            globalVariablesRepository.Package = this.ParsePackageName(filePath);

            XElement allFileElement = XElement.Load(filePath);
            globalVariablesRepository.GlobalVariables =  this.ParseVariable (allFileElement);

            return globalVariablesRepository;
        }

        public string ParsePackageName(string fileName)
        {
            var initialProjectPath = ConfigurationApp.GetProperty(MainClass.ProjectDirectory) + DefaultsubstvarDirectory;

            var path = fileName.Replace(Defaultsubstvar,String.Empty);
            path = path.Remove(path.Length - 1, 1);
            path = path.Replace("/", ".");
            path = path.Replace("\\", ".");
            if (initialProjectPath != null)
            {
                initialProjectPath = initialProjectPath.Replace("/", ".");
                initialProjectPath = initialProjectPath.Replace("\\", ".");
                path = path.Replace(initialProjectPath, string.Empty);
            }

            if (path.LastIndexOf(".") != -1)
            {
                path = path.Remove(path.LastIndexOf("."));
            }

            if (path.StartsWith("."))
            {
                path = path.Remove(0, 1);
            }

            return path;
        }

        public string ParseFileName(string fileName)
        {
            var path = fileName.Replace(Defaultsubstvar,String.Empty);
            path = path.Remove(path.Length - 1, 1);
            path = path.Replace("/", ".");
            path = path.Replace("\\", ".");
            path = path.Remove(0, path.LastIndexOf(".") + 1);
            return path;
        }

        public List<GlobalVariable> ParseVariable(XElement allFileElement)
        {
            //var repoElement = allFileElement.Element(XmlnsConstant.globalVariableNameSpace + "repository");
            var globalVariablesElement = allFileElement.Element(XmlnsConstant.globalVariableNameSpace + "globalVariables");

            IEnumerable<XElement> xElement = from element in globalVariablesElement.Elements (XmlnsConstant.globalVariableNameSpace + "globalVariable")
                select element;
            if (xElement == null) {
                return null;
            }

            var globalVariables = new List<GlobalVariable>();
            foreach (var element in xElement)
            {
                var globalVariable = new GlobalVariable
                    {
                        Name = element.Element(XmlnsConstant.globalVariableNameSpace + "name").Value,
                        Value = element.Element(XmlnsConstant.globalVariableNameSpace + "value").Value,
                        Type = (GlobalVariableType) Enum.Parse(typeof(GlobalVariableType),element.Element(XmlnsConstant.globalVariableNameSpace + "type").Value)
                    };
                globalVariables.Add(globalVariable);
            }
            return globalVariables;

        }
            
	}

}

