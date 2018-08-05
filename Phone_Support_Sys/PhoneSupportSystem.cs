using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Phone_Support_Sys
{
    public class PhoneSupportSystem
    {
        Manager m_ManagerInstance;
        Queue<Call> m_WaitingCallQueue = new Queue<Call>();

        public PhoneSupportSystem()
        {
            m_ManagerInstance = Manager.GetManager();
            m_ManagerInstance.Initialize();
        }

        public void HandleNewCall(Call i_NewCall)
        {
            Thread thread = new Thread(() => m_ManagerInstance.ManageCall(i_NewCall, m_WaitingCallQueue));
            thread.Start();
        }
    }
}
