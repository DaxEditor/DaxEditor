using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace DaxEditor
{
    [Serializable]
    public class DaxException : Exception
    {
        public DaxException() { }
        public DaxException(string message) : base(message) { }
        public DaxException(string message, Exception inner) : base(message, inner) { }
        protected DaxException(
          System.Runtime.Serialization.SerializationInfo info,
          System.Runtime.Serialization.StreamingContext context)
            : base(info, context) { }
    }
}
