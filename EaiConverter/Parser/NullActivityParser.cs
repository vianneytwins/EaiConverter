using System;

namespace EaiConverter.Parser
{
    public class NullActivityParser : IActivityParser
    {
        public NullActivityParser()
        {
        }

        #region IActivityParser implementation

        public EaiConverter.Model.Activity Parse(System.Xml.Linq.XElement inputElement)
        {
            throw new NotImplementedException();
        }

        #endregion
    }
}

