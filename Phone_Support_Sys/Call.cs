using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Phone_Support_Sys
{
    public class Call
    {
        Random m_Rand;
        public string levelOfDifficulty { get; private set; }

        public Call()
        {
            m_Rand = new Random();
            setLevelOfDifficulty();
        }

        private void setLevelOfDifficulty()
        {
            int randomNumber = m_Rand.Next(1, 4);

            if (randomNumber == 1)
            {
                levelOfDifficulty = "Tier1";
            }
            else if (randomNumber == 2)
            {
                levelOfDifficulty = "Tier2";
            }
            else
            {
                levelOfDifficulty = "Manager";
            }
        }
    }
}
