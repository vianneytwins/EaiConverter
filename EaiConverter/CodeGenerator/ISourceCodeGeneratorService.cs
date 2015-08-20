namespace EaiConverter.CodeGenerator
{
    using System.CodeDom;

    public interface ISourceCodeGeneratorService
    {
        void Generate(CodeCompileUnit targetUnit);

        void GenerateSolutionAndProjectFiles();
    }
}
