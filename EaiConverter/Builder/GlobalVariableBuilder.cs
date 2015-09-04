namespace EaiConverter.Builder
{
    using System;
    using System.CodeDom;
    using System.Collections.Generic;
    using System.Reflection;

    using EaiConverter.Model;
    using EaiConverter.Utils;

    public class GlobalVariableBuilder
	{
        private readonly Dictionary<string,string> globalVaraibleTypeDictionnary = new Dictionary <string,string>
        {
            { "String", "System.String" },
            { "Integer", "System.Int32" }
        };

        public CodeNamespace Build(GlobalVariablesRepository globalVariablesRepository)
        {
            var globalVariableNameSpace = new CodeNamespace(globalVariablesRepository.Package);

            // Generate the Service
            globalVariableNameSpace.Imports.AddRange(this.GenerateImports());
            var globalVariableClass = this.GenerateClass(globalVariablesRepository);
            globalVariableNameSpace.Types.Add(globalVariableClass);

            return globalVariableNameSpace;
        }

        public CodeNamespaceImport[] GenerateImports()
        {
            return new CodeNamespaceImport[1]
            {
                new CodeNamespaceImport("System")
            };
        }


        public CodeTypeDeclaration GenerateClass(GlobalVariablesRepository globalVariablesRepository)
        {
            var globalPropertyClass = new CodeTypeDeclaration(globalVariablesRepository.Name);
            globalPropertyClass.IsClass = true;
            globalPropertyClass.TypeAttributes = TypeAttributes.Public;

            var properties = this.GenerateProperties(globalVariablesRepository);
            if (properties != null)
            {
                globalPropertyClass.Members.AddRange(properties);
            }

            globalPropertyClass.Members.Add(this.GenerateConstructor(globalVariablesRepository));

            return globalPropertyClass;
        }

        public CodeTypeMemberCollection GenerateProperties(GlobalVariablesRepository globalVariablesRepository)
        {
            var result = new CodeTypeMemberCollection();
            if(globalVariablesRepository.GlobalVariables == null)
            {
                return null;
            }

            foreach (var variable in globalVariablesRepository.GlobalVariables)
            {
                result.Add(CodeDomUtils.GenerateStaticProperty(variable.Name, globalVaraibleTypeDictionnary[variable.Type.ToString()]));
            }

            return result;
        }

        CodeConstructor GenerateConstructor(GlobalVariablesRepository globalVariablesRepository)
        {
            var constructor = new CodeConstructor();
            constructor.Attributes = MemberAttributes.Public;
            foreach (var variable in globalVariablesRepository.GlobalVariables)
            {
                //var propertyReference = new CodeFieldReferenceExpression(new CodeThisReferenceExpression(), variable.Name);
                var propertyReference = new CodeVariableReferenceExpression(variable.Name);
                
                constructor.Statements.Add(new CodeAssignStatement(propertyReference,
                    new CodePrimitiveExpression(ConvertToPrimitiveType(globalVaraibleTypeDictionnary[variable.Type.ToString()], variable.Value))));
            }

            return constructor;
        }

        public static object ConvertToPrimitiveType (string type, string value) 
        {
            if (type == typeof(System.Int32).ToString())
            {
                return Int32.Parse(value);
            }
            return value;
        }
	}

}

