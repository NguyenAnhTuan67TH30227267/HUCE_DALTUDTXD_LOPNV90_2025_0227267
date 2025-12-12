using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HUCE_DALTUDTXD_LOPNV90_2025_0227267.Models
{
    public class GeologicalAxis
    {
        public string Name { get; set; }
        public ObservableCollection<FoundationEntry> Entries { get; set; }

        public GeologicalAxis()
        {
            Entries = new ObservableCollection<FoundationEntry>();
        }
    }
}