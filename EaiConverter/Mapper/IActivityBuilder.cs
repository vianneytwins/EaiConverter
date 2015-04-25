using System;
using EaiConverter.Model;

namespace EaiConverter.Mapper
{
    public interface IActivityBuilder
    {
        ActivityCodeDom Build (Activity activity);
    }
}

