using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Phone_Support_Sys
{
    public interface ISupportPerson
    {
        bool TryToHandleIt(Call i_Caller);
    }
}
