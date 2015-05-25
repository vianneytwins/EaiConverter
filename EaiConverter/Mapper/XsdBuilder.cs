using System;
using System.IO;
using System.Collections.Generic;
using System.Reflection;
using System.Text;
using System.Xml;
using System.Xml.Serialization;
using System.Xml.Schema;
using System.CodeDom;
using System.CodeDom.Compiler;
using System.Collections;

using Microsoft.CSharp;
using System.Xml.Linq;


namespace EaiConverter.Mapper
{

	public class XsdBuilder
	{
		const string httpwwwworgXMLSchema = "http://www.w3.org/2001/XMLSchema";

		const string schema = "schema";

		const string xsd = "xsd";

		public CodeNamespace Build(IEnumerable<XNode> inputNodes, string nameSpace)
		{
			MemoryStream stream = new MemoryStream ();

			XNamespace xsdPrefix = httpwwwworgXMLSchema;
			var rootElement = new XElement (xsdPrefix + schema , new XAttribute (XNamespace.Xmlns + xsd, xsdPrefix), inputNodes);

			rootElement.Save (stream);
            var xsdCodeNamespace = GeneratedClassFromStream (stream, nameSpace);

			return xsdCodeNamespace;
		}

        public CodeNamespace Build(string fileName)
        {
            FileStream stream = new FileStream(fileName, FileMode.Open, FileAccess.Read);
  
            var xsdCodeNamespace = GeneratedClassFromStream (stream, TibcoProcessClassesBuilder.ConvertXsdImportToNameSpace(fileName));

            return xsdCodeNamespace;
        }

		private CodeNamespace GeneratedClassFromStream (Stream stream, string nameSpace)
		{
			XmlSchema xsd;
			stream.Seek(0, SeekOrigin.Begin);
			using (stream) {

				xsd = XmlSchema.Read (stream, null);
			}
				
			XmlSchemas xsds = new XmlSchemas ();

            xsds.Add (xsd);
			xsds.Compile (null, true);
			XmlSchemaImporter schemaImporter = new XmlSchemaImporter (xsds);

			// create the codedom
			CodeNamespace codeNamespace = new CodeNamespace (nameSpace);
			XmlCodeExporter codeExporter = new XmlCodeExporter (codeNamespace);
			List<XmlTypeMapping> maps = new List<XmlTypeMapping> ();
			foreach (XmlSchemaType schemaType in xsd.SchemaTypes.Values) {
				maps.Add (schemaImporter.ImportSchemaType (schemaType.QualifiedName));
			}
			foreach (XmlSchemaElement schemaElement in xsd.Elements.Values) {
				maps.Add (schemaImporter.ImportTypeMapping (schemaElement.QualifiedName));
			}
			foreach (XmlTypeMapping map in maps) {
				codeExporter.ExportTypeMapping (map);
			}

			this.RemoveUnusedStuff (codeNamespace);

			return codeNamespace;	
		}
			
		private void RemoveUnusedStuff(CodeNamespace codeNamespace)
		{
			foreach(CodeTypeDeclaration codeType in codeNamespace.Types)
			{
			
				codeType.Comments.Clear ();
				codeType.CustomAttributes.Clear();
				codeType.IsPartial = false;
				foreach (CodeTypeMember codeTypeMember in codeType.Members){
					codeTypeMember.CustomAttributes = null;
					codeTypeMember.Comments.Clear ();
				}
			
			}
		}

	}
}

