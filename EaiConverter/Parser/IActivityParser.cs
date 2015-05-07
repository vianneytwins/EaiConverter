using System;
using EaiConverter.Model;
using System.Xml.Linq;

namespace EaiConverter.Parser
{
    public interface IActivityParser
    {
        Activity Parse(XElement inputElement);
    }
}

