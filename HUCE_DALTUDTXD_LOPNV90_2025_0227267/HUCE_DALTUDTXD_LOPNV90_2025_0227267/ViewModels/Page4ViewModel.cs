using HUCE_DALTUDTXD_LOPNV90_2025_0227267.Models;
using HUCE_DALTUDTXD_LOPNV90_2025_0227267.Views;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HUCE_DALTUDTXD_LOPNV90_2025_0227267.ViewModels
{
    public class Page4ViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;
        // Đối tượng này chỉ giữ giá trị nội lực đang được nhập (input area)
        private ForceInputEntry _forceInput;
        private string _selectedElementName;

        public string SelectedElementName
        {
            get => _selectedElementName;
            set
            {
                _selectedElementName = value;
                OnPropertyChanged(nameof(SelectedElementName));
            }
        }

        public ForceInputEntry ForceInput
        {
            get => _forceInput;
            set
            {
                _forceInput = value;
                OnPropertyChanged(nameof(ForceInput));
            }
        }

        public ConstructionEntry SelectedMong
        {
            get => ForceInput.Mong;
            set
            {
                // Gán móng được chọn vào ForceInput (đối tượng nhập)
                ForceInput.Mong = value;
                OnPropertyChanged(nameof(SelectedMong));

                // 1. Tìm ForceInputEntry tương ứng trong danh sách DataGrid
                var existingEntry = ForceInputList.FirstOrDefault(e => e.Mong.TenMong == value?.TenMong);

                // 2. Cập nhật các ô nhập nội lực (liên kết với ForceInput)
                if (existingEntry != null)
                {
                    // Lấy nội lực đã lưu
                    UpdateForceInputValues(existingEntry);
                }
                else
                {
                    // Reset nội lực về 0 nếu không tìm thấy (trường hợp móng mới chưa có trong list)
                    UpdateForceInputValues(new ForceInputEntry());
                }
            }
        }

        // Phương thức cập nhật các thuộc tính nội lực và kích hoạt OnPropertyChanged :))
        private void UpdateForceInputValues(ForceInputEntry sourceEntry)
        {
            // Cập nhật giá trị nội lực của đối tượng ForceInput đang được binding
            _forceInput.Moment = sourceEntry.Moment;
            _forceInput.AxialForce = sourceEntry.AxialForce;
            _forceInput.ShearForce = sourceEntry.ShearForce;

            // Kích hoạt cập nhật UI cho các TextBox
            OnPropertyChanged(nameof(ForceInput));
        }

        // Danh sách móng từ Page2 (ConstructionList của MainViewModel)
        public ObservableCollection<ConstructionEntry> DanhSachMong => _mainViewModel.ConstructionList;

        // Danh sách Nội lực (dữ liệu cho DataGrid)
        public ObservableCollection<ForceInputEntry> ForceInputList { get; set; } = new ObservableCollection<ForceInputEntry>();

        public ICommand SaveCommand { get; }
        public ICommand ShowCalculatorViewCommand { get; }
        public ICommand ImportEtabsDataCommand { get; }

        public Page4ViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            _forceInput = new ForceInputEntry();
            //SaveCommand = new RelayCommand(SaveData);
            ShowCalculatorViewCommand = new RelayCommand(ExecuteShowCalculatorView);

            // Khởi tạo danh sách nội lực
            InitializeForceInputList();

            // Đồng bộ hóa khi danh sách móng ở Page 2 thay đổi (thêm/xóa)
            _mainViewModel.ConstructionList.CollectionChanged += ConstructionList_CollectionChanged;

            // Thiết lập móng đầu tiên làm mặc định sau khi đã khởi tạo danh sách
            if (DanhSachMong.Any())
            {
                SelectedMong = DanhSachMong.First();
            }
        }

        private void ConstructionList_CollectionChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            // Đồng bộ lại danh sách khi có thay đổi từ View 2
            InitializeForceInputList();

            // Nếu móng đang được chọn bị xóa, chọn lại móng đầu tiên
            if (e.Action == NotifyCollectionChangedAction.Remove && SelectedMong != null)
            {
                if (!DanhSachMong.Contains(SelectedMong))
                {
                    SelectedMong = DanhSachMong.FirstOrDefault();
                }
            }
        }

        private void ExecuteShowCalculatorView()
        {
            _mainViewModel.ExecuteShowCal5View();
        }

        private void InitializeForceInputList()
        {
            // Lưu trữ nội lực hiện tại (trước khi clear)
            var currentForceInputs = ForceInputList.ToList();
            ForceInputList.Clear();

            foreach (var mong in DanhSachMong)
            {
                // Tìm xem móng này đã có nội lực được lưu chưa (dựa vào TenMong)
                var existingEntry = currentForceInputs.FirstOrDefault(e => e.Mong.TenMong == mong.TenMong);

                if (existingEntry != null)
                {
                    // Giữ lại nội lực đã nhập
                    ForceInputList.Add(existingEntry);
                }
                else
                {
                    // Tạo entry mới với nội lực mặc định là 0
                    ForceInputList.Add(new ForceInputEntry
                    {
                        Mong = mong,
                        Moment = 0,
                        AxialForce = 0,
                        ShearForce = 0
                    });
                }
            }

            // Nếu danh sách rỗng, reset SelectedMong
            if (!ForceInputList.Any() && ForceInput.Mong != null)
            {
                SelectedMong = null;
                UpdateForceInputValues(new ForceInputEntry());
            }
        }
    }
}
