using HUCE_DALTUDTXD_LOPNV90_2025_0227267.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace HUCE_DALTUDTXD_LOPNV90_2025_0227267.ViewModels
{
    public class Page2ViewModel : ViewModelBase
    {
        // --- Du lieu công trình ---
        private ConstructionEntry _newConstructionEntry = new ConstructionEntry();

        // Danh sách c?p ?? bê tông
        public List<string> DanhSachCapDoBeTong { get; } = new List<string>
        {
            "B12.5",
            "B15",
            "B20",
            "B25",
            "B30",
            "B35",
            "B40"
        };

        // Danh sách lo?i thép
        public List<string> DanhSachLoaiThep { get; } = new List<string>
        {
            "CB240-T",
            "CB300-V",
            "CB400-V",
            "CB500-V"
        };

        public ICommand AddConstructionCommand { get; }
        public ICommand DeleteConstructionCommand { get; }

        public Page2ViewModel(MainViewModel mainViewModel)
        {

        }
    }
}
