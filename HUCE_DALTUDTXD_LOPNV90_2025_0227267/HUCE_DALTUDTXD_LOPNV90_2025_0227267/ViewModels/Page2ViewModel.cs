using HUCE_DALTUDTXD_LOPNV90_2025_0227267.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Data;
using System.Windows.Input;

namespace HUCE_DALTUDTXD_LOPNV90_2025_0227267.ViewModels
{
    public class Page2ViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;

        // Cố định Offset 0.3m theo yêu cầu
        private const double WALL_OFFSET_M = 0.3;

        // --- Dữ liệu công trình ---
        private ConstructionEntry _newConstructionEntry = new ConstructionEntry();
        public ConstructionEntry NewConstructionEntry
        {
            get => _newConstructionEntry;
            set
            {
                _newConstructionEntry = value;

                OnPropertyChanged(nameof(NewConstructionEntry));
                OnPropertyChanged(nameof(MongWidthPx));
                OnPropertyChanged(nameof(MongHeightPx));

                // Cập nhật các thuộc tính Canvas MÓNG
                OnPropertyChanged(nameof(DaiHeightPx));
                OnPropertyChanged(nameof(FoundationBottomPx));
                OnPropertyChanged(nameof(DaiTopPx));
                OnPropertyChanged(nameof(GroundLevelLineTopPx));

                // Cập nhật các thuộc tính Canvas TƯỜNG 
                OnPropertyChanged(nameof(WallWidthPx));
                OnPropertyChanged(nameof(WallHeightPx));
                OnPropertyChanged(nameof(WallTopPx));

                // Cập nhật Proxy
                OnPropertyChanged(nameof(ChieuRongMongProxy));
                OnPropertyChanged(nameof(ChieuSauChonMongProxy));
                OnPropertyChanged(nameof(ChieuCaoDaiProxy));
                OnPropertyChanged(nameof(BeDayTuongProxy));
            }
        }

        // Proxy cho TextBox để cập nhật Canvas
        public double ChieuRongMongProxy
        {
            get => NewConstructionEntry.ChieuRongMong;
            set
            {
                NewConstructionEntry.ChieuRongMong = value;
                OnPropertyChanged(nameof(ChieuRongMongProxy));
                OnPropertyChanged(nameof(MongWidthPx));  // cập nhật Canvas Width
            }
        }

        public double ChieuSauChonMongProxy
        {
            get => NewConstructionEntry.ChieuSauChonMong;
            set
            {
                NewConstructionEntry.ChieuSauChonMong = value;
                OnPropertyChanged(nameof(ChieuSauChonMongProxy));
                OnPropertyChanged(nameof(MongHeightPx));

                // Thay đổi chiều sâu chôn móng -> Đáy móng thay đổi -> Đỉnh móng và Chiều cao tường thay đổi
                OnPropertyChanged(nameof(FoundationBottomPx));
                OnPropertyChanged(nameof(DaiTopPx));
                OnPropertyChanged(nameof(WallHeightPx));
            }
        }

        public double ChieuCaoDaiProxy
        {
            get => NewConstructionEntry.ChieuCaoDai;
            set
            {
                NewConstructionEntry.ChieuCaoDai = value;
                OnPropertyChanged(nameof(ChieuCaoDaiProxy));

                // Thay đổi chiều cao đài -> Chiều cao hình thay đổi -> Đỉnh móng và Chiều cao tường thay đổi
                OnPropertyChanged(nameof(DaiHeightPx));
                OnPropertyChanged(nameof(DaiTopPx));
                OnPropertyChanged(nameof(WallHeightPx));
            }
        }

        public double BeDayTuongProxy // Proxy cho Bề dày tường
        {
            get => NewConstructionEntry.BeDayTuong;
            set
            {
                NewConstructionEntry.BeDayTuong = value;
                OnPropertyChanged(nameof(BeDayTuongProxy));
                OnPropertyChanged(nameof(WallWidthPx)); // Cập nhật chiều rộng tường
            }
        }


        // --- CLick để sửa ---
        private ConstructionEntry _selectedConstructionEntry;
        public ConstructionEntry SelectedConstructionEntry
        {
            get => _selectedConstructionEntry;
            set
            {
                _selectedConstructionEntry = value;
                OnPropertyChanged(nameof(SelectedConstructionEntry));

                if (_selectedConstructionEntry != null)
                {
                    // ĐỔ DỮ LIỆU LÊN FORM
                    NewConstructionEntry.TenMong = _selectedConstructionEntry.TenMong;
                    NewConstructionEntry.ChieuSauChonMong = _selectedConstructionEntry.ChieuSauChonMong;
                    NewConstructionEntry.ChieuRongMong = _selectedConstructionEntry.ChieuRongMong;
                    NewConstructionEntry.CapDoBeTong = _selectedConstructionEntry.CapDoBeTong;
                    NewConstructionEntry.LoaiThep = _selectedConstructionEntry.LoaiThep;
                    NewConstructionEntry.ChieuDayLopBaoVe = _selectedConstructionEntry.ChieuDayLopBaoVe;
                    NewConstructionEntry.BeDayTuong = _selectedConstructionEntry.BeDayTuong;
                    NewConstructionEntry.ChieuCaoDai = _selectedConstructionEntry.ChieuCaoDai;

                    // BẮT BUỘC cập nhật UI + Canvas
                    OnPropertyChanged(nameof(NewConstructionEntry));
                    OnPropertyChanged(nameof(ChieuRongMongProxy));
                    OnPropertyChanged(nameof(ChieuSauChonMongProxy));
                    OnPropertyChanged(nameof(ChieuCaoDaiProxy));
                    OnPropertyChanged(nameof(BeDayTuongProxy));
                }

                // cập nhật trạng thái Command
                CommandManager.InvalidateRequerySuggested();
            }
        }

        // --- Select để xóa ---
        private FoundationEntry _selectedSoilLayer;
        public FoundationEntry SelectedSoilLayer
        {
            get => _selectedSoilLayer;
            set
            {
                _selectedSoilLayer = value;
                OnPropertyChanged(nameof(SelectedSoilLayer));
            }
        }



        // Cập nhật khi chọn tên móng
        public string SelectedFoundationName
        {
            get => NewConstructionEntry.TenMong;
            set
            {
                NewConstructionEntry.TenMong = value;
                OnPropertyChanged(nameof(SelectedFoundationName));

                SelectedSoilLayer = FindSoilLayerByName(value);
            }
        }

        private FoundationEntry FindSoilLayerByName(string name)
        {
            foreach (var axis in _mainViewModel.Page1ViewModel.GeologicalAxes)
            {
                var layer = axis.Entries.FirstOrDefault(e => e.Vitrimong == name);
                if (layer != null) return layer;
            }
            return null;
        }

        // --- Danh sách tên móng từ Page1 ---
        public ObservableCollection<string> FoundationNames
        {
            get
            {
                var names = new ObservableCollection<string>();
                foreach (var axis in _mainViewModel.Page1ViewModel.GeologicalAxes)
                {
                    foreach (var entry in axis.Entries)
                    {
                        if (!string.IsNullOrEmpty(entry.Vitrimong) && !names.Contains(entry.Vitrimong))
                        {
                            names.Add(entry.Vitrimong);
                        }
                    }
                }
                return names;
            }
        }

        // Danh sách cấp độ bê tông
        public List<string> DanhSachCapDoBeTong { get; } = new List<string>
        {
            "B12.5", "B15", "B20", "B25", "B30", "B35", "B40"
        };

        // Danh sách loại thép
        public List<string> DanhSachLoaiThep { get; } = new List<string>
        {
            "CB240-T", "CB300-V", "CB400-V", "CB500-V"
        };

        // SCALE cho Canvas
        private const double SCALE = 50;
        private const double GROUND_LEVEL_Y = 125; // Mặt đất tự nhiên ở 1/2 H Canvas (250/2)

        // --- Thuộc tính MÓNG ---
        public double MongWidthPx => NewConstructionEntry.ChieuRongMong * SCALE;
        public double MongHeightPx => NewConstructionEntry.ChieuSauChonMong * SCALE;
        public double DaiHeightPx => NewConstructionEntry.ChieuCaoDai * SCALE;

        // Vị trí Line Mặt đất Tự nhiên
        public double GroundLevelLineTopPx => GROUND_LEVEL_Y;

        // Vị trí ĐÁY ĐÀI MÓNG (Điểm cố định)
        public double FoundationBottomPx
        {
            get => GROUND_LEVEL_Y + NewConstructionEntry.ChieuSauChonMong * SCALE;
        }

        // Vị trí ĐỈNH ĐÀI MÓNG (Canvas.Top của hình chữ nhật đài móng)
        public double DaiTopPx
        {
            get => FoundationBottomPx - DaiHeightPx;
        }

        // --- Thuộc tính TƯỜNG ---

        // Chiều rộng tường theo pixel (Bề dày tường)
        public double WallWidthPx => NewConstructionEntry.BeDayTuong * SCALE;

        // 🔥 LOGIC ĐÃ SỬA: Vị trí TOP của tường: Cách mặt đất 0.3m LÊN PHÍA TRÊN (TRỪ)
        public double WallTopPx
        {
            get => GROUND_LEVEL_Y - WALL_OFFSET_M * SCALE;
        }

        // Chiều cao tường: Khoảng cách từ đỉnh đài móng (DaiTopPx) đến WallTopPx
        public double WallHeightPx
        {
            // Chiều cao = Y_bottom (DaiTopPx) - Y_top (WallTopPx)
            get => DaiTopPx - WallTopPx;
        }

        public ICommand AddConstructionCommand { get; }
        public ICommand DeleteConstructionCommand { get; }

        public Page2ViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;

            AddConstructionCommand = new RelayCommand(AddConstructionEntry);
            DeleteConstructionCommand = new RelayCommand(DeleteSelectedConstructionEntry);

            // Thiết lập giá trị mặc định ban đầu
            NewConstructionEntry.CapDoBeTong = DanhSachCapDoBeTong[2];
            NewConstructionEntry.LoaiThep = DanhSachLoaiThep[2];
            NewConstructionEntry.ChieuDayLopBaoVe = 25;
            NewConstructionEntry.BeDayTuong = 0.22;
            NewConstructionEntry.ChieuCaoDai = 0.6;
            NewConstructionEntry.ChieuRongMong = 1.0;
            NewConstructionEntry.ChieuSauChonMong = 1.0; // Đặt mặc định là 1.0m

            // Kích hoạt cập nhật Canvas lần đầu
            OnPropertyChanged(nameof(MongWidthPx));
            OnPropertyChanged(nameof(DaiHeightPx));
            OnPropertyChanged(nameof(FoundationBottomPx));
            OnPropertyChanged(nameof(DaiTopPx));
            OnPropertyChanged(nameof(GroundLevelLineTopPx));
            OnPropertyChanged(nameof(WallWidthPx));
            OnPropertyChanged(nameof(WallHeightPx));
            OnPropertyChanged(nameof(WallTopPx));
        }

        private void AddConstructionEntry()
        {
            if (ValidateEntry())
            {
                var soilLayers = FindSoilLayersByName(NewConstructionEntry.TenMong);
                var soilLayerAtDepth = FindSoilLayerAtDepth(soilLayers, NewConstructionEntry.ChieuSauChonMong);

                ConstructionList.Add(new ConstructionEntry
                {
                    TenMong = NewConstructionEntry.TenMong,
                    ChieuSauChonMong = NewConstructionEntry.ChieuSauChonMong,
                    ChieuRongMong = NewConstructionEntry.ChieuRongMong,
                    CapDoBeTong = NewConstructionEntry.CapDoBeTong,
                    LoaiThep = NewConstructionEntry.LoaiThep,
                    ChieuDayLopBaoVe = NewConstructionEntry.ChieuDayLopBaoVe,
                    BeDayTuong = NewConstructionEntry.BeDayTuong,
                    ChieuCaoDai = NewConstructionEntry.ChieuCaoDai,
                    SoilLayer = soilLayerAtDepth
                });

                // Lấy các giá trị thiết lập không phải kích thước để gán lại cho Entry mới
                var capDoBeTong = NewConstructionEntry.CapDoBeTong;
                var loaiThep = NewConstructionEntry.LoaiThep;
                var chieuDayBaoVe = NewConstructionEntry.ChieuDayLopBaoVe;
                var beDayTuong = NewConstructionEntry.BeDayTuong;
                var chieuCaoDai = NewConstructionEntry.ChieuCaoDai;

                // TẠO MỘT INSTANCE MỚI VÀ GÁN LẠI CÁC GIÁ TRỊ MẶC ĐỊNH
                NewConstructionEntry = new ConstructionEntry
                {
                    CapDoBeTong = capDoBeTong,
                    LoaiThep = loaiThep,
                    ChieuDayLopBaoVe = chieuDayBaoVe,
                    BeDayTuong = beDayTuong,

                    // Gán lại mặc định cho kích thước để hình ảnh vẫn hiển thị
                    ChieuRongMong = 1.0,
                    ChieuSauChonMong = 1.0,
                    ChieuCaoDai = 0.6
                };
            }
        }

        private List<FoundationEntry> FindSoilLayersByName(string name)
        {
            var layers = new List<FoundationEntry>();
            foreach (var axis in _mainViewModel.Page1ViewModel.GeologicalAxes)
            {
                foreach (var entry in axis.Entries)
                {
                    if (entry.Vitrimong == name)
                        layers.Add(entry);
                }
            }
            return layers.OrderBy(l => l.Sothutulopdat).ToList();
        }

        private FoundationEntry FindSoilLayerAtDepth(List<FoundationEntry> layers, double depth)
        {
            double cumulativeDepth = 0;
            foreach (var layer in layers)
            {
                cumulativeDepth += layer.Chieudaylopdat;
                if (depth <= cumulativeDepth)
                    return layer;
            }
            return layers.LastOrDefault();
        }

        private bool ValidateEntry()
        {
            if (string.IsNullOrWhiteSpace(NewConstructionEntry.TenMong))
            {
                System.Windows.MessageBox.Show("Vui lòng chọn tên móng!");
                return false;
            }
            // ... (Phần ValidateEntry giữ nguyên) ...
            if (NewConstructionEntry.ChieuSauChonMong <= 0)
            {
                System.Windows.MessageBox.Show("Chiều sâu chôn móng phải lớn hơn 0!");
                return false;
            }

            if (NewConstructionEntry.ChieuRongMong <= 0)
            {
                System.Windows.MessageBox.Show("Chiều rộng móng phải lớn hơn 0!");
                return false;
            }

            if (NewConstructionEntry.ChieuDayLopBaoVe <= 0)
            {
                System.Windows.MessageBox.Show("Chiều dày lớp bảo vệ phải lớn hơn 0!");
                return false;
            }

            if (NewConstructionEntry.BeDayTuong <= 0)
            {
                System.Windows.MessageBox.Show("Bề dày tường phải lớn hơn 0!");
                return false;
            }

            if (NewConstructionEntry.ChieuCaoDai <= 0)
            {
                System.Windows.MessageBox.Show("Chiều cao đài phải lớn hơn 0!");
                return false;
            }

            return true;
        }

        private void DeleteSelectedConstructionEntry()
        {
            if (SelectedConstructionEntry == null) return;

            var result = MessageBox.Show(
                "Bạn có chắc chắn muốn xóa móng đang chọn không?",
                "Xác nhận xóa",
                MessageBoxButton.YesNo,
                MessageBoxImage.Warning);

            if (result == MessageBoxResult.Yes)
            {
                ConstructionList.Remove(SelectedConstructionEntry);
                SelectedConstructionEntry = null;

                //  THÊM NGAY TẠI ĐÂY
                CommandManager.InvalidateRequerySuggested();

                //  RESET FORM NHẬP
                NewConstructionEntry = new ConstructionEntry
                {
                    CapDoBeTong = DanhSachCapDoBeTong[2],
                    LoaiThep = DanhSachLoaiThep[2],
                    ChieuDayLopBaoVe = 25,
                    BeDayTuong = 0.22,
                    ChieuCaoDai = 0.6,
                    ChieuRongMong = 1.0,
                    ChieuSauChonMong = 1.0
                };
            }
        }


        public ObservableCollection<ConstructionEntry> ConstructionList => _mainViewModel.ConstructionList;
    }

    // Các lớp Converter phải nằm ở cấp độ Namespace
    public class CenterBottomThirdConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0;

            if (!double.TryParse(value.ToString(), out double h))
                return 0;
            double canvasHeight = 210;
            double targetCenterY = canvasHeight - canvasHeight / 3;

            return targetCenterY - h / 2;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }

    public class CenterConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0;

            if (!double.TryParse(value.ToString(), out double w))
                return 0;

            double canvasWidth = 300; // Chiều rộng Canvas trong XAML
            return (canvasWidth - w) / 2; // Vị trí Left để căn giữa
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }

    public class TextPositionConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return 0;
            if (!double.TryParse(value.ToString(), out double top)) return 0;

            // Dịch xuống 5 pixel so với đường line để text không bị đè lên
            return top + 5;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
            => Binding.DoNothing;
    }
   
   

          

}
