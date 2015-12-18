namespace EaiConverter.Builder
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.IO;
    using System.Reflection;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    using EaiConverter.Builder.Utils;
    using EaiConverter.Model;
    using EaiConverter.Parser;
    using EaiConverter.Utils;

    using log4net;

    public class XsdBuilder
    {
        private const string HttpwwwworgXmlSchema = "http://www.w3.org/2001/XMLSchema";

        private const string Schema = "schema";

        private const string Xsd = "xsd";

        private static readonly ILog Log = LogManager.GetLogger(typeof(XsdBuilder));

        private readonly XsdParser xsdParser;

        public XsdBuilder()
        {
            this.xsdParser = new XsdParser();
        }

        public CodeNamespace Build(IEnumerable<XNode> inputNodes, string nameSpace)
        {
            var stream = new MemoryStream();

            XNamespace xsdPrefix = HttpwwwworgXmlSchema;
            var rootElement = new XElement(xsdPrefix + Schema, new XAttribute(XNamespace.Xmlns + Xsd, xsdPrefix), inputNodes);

            rootElement.Save(stream);
            var xsdCodeNamespace = this.GeneratedClassFromStream(stream, nameSpace);

            return xsdCodeNamespace;
        }


        public CodeNamespace Build(List<ClassParameter> parameters, string nameSpace)
        {
            var xsdCodeNamespace = new CodeNamespace();
            xsdCodeNamespace.Name = nameSpace;
            xsdCodeNamespace.Types.AddRange(GenerateClassForParameters(parameters));
            return xsdCodeNamespace;
           
        }

        private CodeTypeDeclarationCollection GenerateClassForParameters(List<ClassParameter> parameters)
        {
            var classes = new CodeTypeDeclarationCollection();
            foreach (var parameter in parameters)
            {
                if (!CodeDomUtils.IsBasicType(parameter.Type) && parameter.ChildProperties != null)
                {
                    classes.Add(this.CreateParameterClass(parameter));
                    classes.AddRange(this.GenerateClassForParameters(parameter.ChildProperties));
                }
            }

            return classes;
        }

        private CodeTypeDeclaration CreateParameterClass(ClassParameter parameter)
        {
            var parameterClass = new CodeTypeDeclaration();
            parameterClass.IsClass = true;
            parameterClass.TypeAttributes = TypeAttributes.Public;

            parameterClass.Name = parameter.Name;

            parameterClass.Members.AddRange(this.GenererateProperties(parameter.ChildProperties));

            return parameterClass;
        }

        private CodeTypeMember[] GenererateProperties(List<ClassParameter> childProperties)
        {
            var properties = new List<CodeTypeMember>();
            foreach (var parameter in childProperties)
            {
                properties.Add(CodeDomUtils.GenerateProperty(parameter.Name, parameter.Type));
            }

            return properties.ToArray();
        }
   
        // TODO : refacto to split the 2 generation method in 2 sub-service
        public CodeNamespace Build(string fileName)
        {
            var xsdCodeNamespace = new CodeNamespace();
            var convertXsdImportToNameSpace = TargetAppNameSpaceService.myAppName() + "." + TargetAppNameSpaceService.ConvertXsdImportToNameSpace(fileName);
            try
            {
                var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

                xsdCodeNamespace = this.GeneratedClassFromStream(stream, convertXsdImportToNameSpace);
            }
            catch (Exception e)
            {
                Log.Warn("Switching mode to generate class from XSD file because it contains custom element:" + fileName);
                XElement allFileElement = XElement.Load(fileName);
                xsdCodeNamespace = this.Build(this.Parse(allFileElement, convertXsdImportToNameSpace), convertXsdImportToNameSpace);
            }

            return xsdCodeNamespace;
        }

        private List<ClassParameter> Parse(XElement allFileElement, string targetNamespace)
        {
            return this.xsdParser.Parse(allFileElement.Nodes(), targetNamespace);
        }

        private CodeNamespace GeneratedClassFromStream(Stream stream, string nameSpace)
        {
            XmlSchema xsd;
            stream.Seek(0, SeekOrigin.Begin);
            using (stream)
            {
                xsd = XmlSchema.Read(stream, null);
            }

            var xsds = new XmlSchemas();

            xsds.Add(xsd);
            xsds.Compile(null, true);
            var schemaImporter = new XmlSchemaImporter(xsds);

            // create the codedom
            var codeNamespace = new CodeNamespace(nameSpace);
            var codeExporter = new XmlCodeExporter(codeNamespace);
            var maps = new List<XmlTypeMapping>();
            foreach (XmlSchemaType schemaType in xsd.SchemaTypes.Values)
            {
                maps.Add(schemaImporter.ImportSchemaType(schemaType.QualifiedName));
            }

            foreach (XmlSchemaElement schemaElement in xsd.Elements.Values)
            {
                maps.Add(schemaImporter.ImportTypeMapping(schemaElement.QualifiedName));
            }

            foreach (XmlTypeMapping map in maps)
            {
                codeExporter.ExportTypeMapping(map);
            }

            return codeNamespace;
        }

    }
}

