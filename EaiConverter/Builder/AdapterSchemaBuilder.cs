namespace EaiConverter.Builder
{
    using System.CodeDom;

    using EaiConverter.Model;

    public class AdapterSchemaBuilder 
    {
        public CodeNamespace Build(AdapterSchemaModel adapterSchemaModel)
        {
            return new CodeNamespace();
        }
    }
}
