using EaiConverter.Builder.Utils;
using EaiConverter.Model;
using EaiConverter.Utils;

namespace EaiConverter.Builder
{
    using System.CodeDom;
    using System.Collections.Generic;
    using System.IO;
    using System.Xml.Linq;
    using System.Xml.Schema;
    using System.Xml.Serialization;

    public class XsdBuilder
    {
        const string httpwwwworgXMLSchema = "http://www.w3.org/2001/XMLSchema";

        const string schema = "schema";

        const string xsd = "xsd";

        public CodeNamespace Build(IEnumerable<XNode> inputNodes, string nameSpace)
        {
            var stream = new MemoryStream();

            XNamespace xsdPrefix = httpwwwworgXMLSchema;
            var rootElement = new XElement(xsdPrefix + schema, new XAttribute(XNamespace.Xmlns + xsd, xsdPrefix), inputNodes);

            rootElement.Save(stream);
            var xsdCodeNamespace = this.GeneratedClassFromStream(stream, nameSpace);

            return xsdCodeNamespace;
        }


        public CodeNamespace Build(List<ClassParameter> parameters, string nameSpace)
        {
  
            var xsdCodeNamespace = new CodeNamespace();
            xsdCodeNamespace.Name = nameSpace;
            foreach (var parameter in parameters)
            {
                if (!CodeDomUtils.IsBasicType(parameter.Type))
                {
                    
                }
            }
            throw new System.NotImplementedException();
           
        }

        public CodeNamespace Build(string fileName)
        {
            var stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);

			var xsdCodeNamespace = this.GeneratedClassFromStream(stream, TargetAppNameSpaceService.ConvertXsdImportToNameSpace(fileName));

            return xsdCodeNamespace;
        }

        private CodeNamespace GeneratedClassFromStream(Stream stream, string nameSpace)
        {
            XmlSchema xsd;
            stream.Seek(0, SeekOrigin.Begin);
            using (stream)
            {

                xsd = XmlSchema.Read(stream, null);
            }

            XmlSchemas xsds = new XmlSchemas();

            xsds.Add(xsd);
            xsds.Compile(null, true);
            XmlSchemaImporter schemaImporter = new XmlSchemaImporter(xsds);

            // create the codedom
            CodeNamespace codeNamespace = new CodeNamespace(nameSpace);
            XmlCodeExporter codeExporter = new XmlCodeExporter(codeNamespace);
            List<XmlTypeMapping> maps = new List<XmlTypeMapping>();
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

            //this.RemoveUnusedStuff(codeNamespace);

            return codeNamespace;
        }

        private void RemoveUnusedStuff(CodeNamespace codeNamespace)
        {
            foreach (CodeTypeDeclaration codeType in codeNamespace.Types)
            {

                codeType.Comments.Clear();
                codeType.CustomAttributes.Clear();
                codeType.IsPartial = false;
                foreach (CodeTypeMember codeTypeMember in codeType.Members)
                {
                    codeTypeMember.CustomAttributes = null;
                    codeTypeMember.Comments.Clear();
                }
            }
        }
    }
}

