using EaiConverter.Model;

namespace EaiConverter.Builder
{
    public interface IActivityBuilder
    {
        ActivityCodeDom Build (Activity activity);
    }
}

