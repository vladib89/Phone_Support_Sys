using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;

namespace Phone_Support_Sys
{
    /*The calss was implemented using singleton pattern so that only 1 manager could be loaded
     *implements ISupportPerson interface.
     *Notice that the manager distributes the incoming calls and manages the throught their processed life unless 
     *forwarded to the waiting queue.
     */
    public class Manager : ISupportPerson
    {
        private readonly int Tier1SupportStaffSize = 5;
        private readonly int Tier2SupportStaffSize = 2;
        private readonly object sr_Tier1EmployeeLock = new object();
        private readonly object sr_Tier2EmployeeLock = new object();
        private readonly object sr_CallsLock = new object();
        List<Tier1SupportPerson> m_BusyTier1Employees = new List<Tier1SupportPerson>(5);
        List<Tier2SupportPerson> m_BusyTier2Employees = new List<Tier2SupportPerson>(2);
        Stack<Tier1SupportPerson> m_FreeTier1Employees = new Stack<Tier1SupportPerson>(5);
        Stack<Tier2SupportPerson> m_FreeTier2Employees = new Stack<Tier2SupportPerson>(2);

        private static readonly Manager m_Instance = new Manager();

        private Manager()
        {

        }

        //initializing the employees stacks
        public void Initialize()
        {
            for (int i = 0; i < Tier1SupportStaffSize; i++)
            {
                m_FreeTier1Employees.Push(new Tier1SupportPerson());
            }

            for (int i = 0; i < Tier2SupportStaffSize; i++)
            {
                m_FreeTier2Employees.Push(new Tier2SupportPerson());
            }

        }

        public static Manager GetManager()
        {
            return m_Instance;
        }

        public void ManageCall(Call i_NewCall, Queue<Call> i_WaitingQueue)
        {
            //aquiring an employee
            ISupportPerson vacantEmployee = AllocateCallsToEmployees();

            if (vacantEmployee == null)
            {
                lock (sr_CallsLock)
                {
                    i_WaitingQueue.Enqueue(i_NewCall);
                }
            }
            
            else if (!vacantEmployee.TryToHandleIt(i_NewCall))
            {
                /*the employee failed to solve the problem.
                 *if it was tier 1 employee then we'll release it back to the free tier1 employee stack and
                 *and wait for a tier2 employee to be freed so he could try and solve the problem.
                 */
                bool didTier2SolvedIt = false;

                if (vacantEmployee is Tier1SupportPerson)
                {
                    //releasing the tier1 employee
                    lock (sr_Tier1EmployeeLock)
                    {
                        m_FreeTier1Employees.Push(vacantEmployee as Tier1SupportPerson);
                        m_BusyTier1Employees.Remove(vacantEmployee as Tier1SupportPerson);
                    }
                    //acquiring a tier2 employee
                    vacantEmployee = ReallocateCallToEmployeeTier2();
                    didTier2SolvedIt = vacantEmployee.TryToHandleIt(i_NewCall);
                }
                /*in any case we'll reach this point holding a reference to a tier2 employee
                 */
                lock (sr_Tier2EmployeeLock)
                {
                    m_FreeTier2Employees.Push(vacantEmployee as Tier2SupportPerson);
                    m_BusyTier2Employees.Remove(vacantEmployee as Tier2SupportPerson);
                }

                if (!didTier2SolvedIt)
                {
                    //in the scenario which no one of the support personnal was able to sole the problem
                    //the managers steps in and solves it
                }
            }
        }

        /*allocating the incoming call by employee vacancy if tier1 isn't available then the call
         * will be directed to a tier2 employee if none is available then the call will be placed into the waiting queue.
         */
        public ISupportPerson AllocateCallsToEmployees()
        {
            ISupportPerson employee = null;

            lock (sr_Tier1EmployeeLock)
            {
                if (m_FreeTier1Employees.Count != 0)
                {
                    employee = m_FreeTier1Employees.Pop();
                    m_BusyTier1Employees.Add(employee as Tier1SupportPerson);
                }
            }

            if (employee == null)
            {
                lock (sr_Tier2EmployeeLock)
                {
                    if (m_FreeTier2Employees.Count != 0)
                    {
                        employee = m_FreeTier2Employees.Pop();
                        m_BusyTier2Employees.Add(employee as Tier2SupportPerson);
                    }
                }
            }

            return employee;
        }
        
        /*in case tier1 wasnt able to solve the problem it'll be allocated to tier2 employee
         *if none is available it'll we prompt the stack of free tier2 employees until one is vacant
         */
        public Tier2SupportPerson ReallocateCallToEmployeeTier2()
        {
            Tier2SupportPerson employee = null;
            while (employee == null)
            {
                lock (sr_Tier2EmployeeLock)
                {
                    if (m_FreeTier2Employees.Count != 0)
                    {
                        employee = m_FreeTier2Employees.Pop();
                        m_BusyTier2Employees.Add(employee);
                    }
                }
                Thread.Sleep(100);
            }

            return employee;
        }

        public bool TryToHandleIt(Call i_Caller)
        {
            return true;
        }
    }
}