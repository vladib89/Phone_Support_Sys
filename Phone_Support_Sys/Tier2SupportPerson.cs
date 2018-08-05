using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phone_Support_Sys
{
    public class Tier2SupportPerson : ISupportPerson
    {
        private string m_MyLevelOfExpertise = "Tier2";

        public bool TryToHandleIt(Call i_Caller)
        {
            bool isHandled = false;
            if (m_MyLevelOfExpertise == i_Caller.levelOfDifficulty)
            {
                isHandled = true;
            }

            return isHandled;
        }
    }
}
