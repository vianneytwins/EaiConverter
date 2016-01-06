namespace EaiConverter.Builder
{
    using EaiConverter.Model;
    using EaiConverter.Utils;

    public class NullActivityBuilder : AbstractActivityBuilder
    {
        public override string GetReturnType(Activity activity)
        {
            return CSharpTypeConstant.SystemVoid;
        }
    }
}

