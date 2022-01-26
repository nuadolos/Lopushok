using Lopushok.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace Lopushok.Utilities
{
    internal class Transition
    {
        public static Frame MainFrame { get; set; }

        private static LopushokModel context;
        public static LopushokModel Context
        {
            get
            {
                if (context == null)
                    context = new LopushokModel();

                return context;
            }
        }
    }
}
