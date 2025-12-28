using HUCE_DALTUDTXD_LOPNV90_2025_0227267.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;

namespace HUCE_DALTUDTXD_LOPNV90_2025_0227267.ViewModels
{
    public class Page3ViewModel : ViewModelBase, INotifyPropertyChanged
    {
        private readonly MainViewModel _mainViewModel;
        private ForceInputEntry _selectedForceInput;
        private double _terzaghiP;
        private double _chieuRongMong;
        private double _chieuSauChonMong;
        private double _terzaghiP120;
        private double _chenhLechKinhTe;
        private Brush _dieuKien1Brush = Brushes.Black;
        private Brush _dieuKien2Brush = Brushes.Black;
        private Brush _kinhTeBrush = Brushes.Black;
        private string _ketLuanDieuKien1;
        private string _ketLuanDieuKien2;
        private string _ketLuanKinhTe;
        private string _ketLuanTongThe;
        private double _calculatedSteelArea = 3.85; // Diện tích thép yêu cầu từ tính toán
        private int _selectedSteelDiameter;
        private int _calculatedSteelBars;
        private double _actualSteelArea;
        private int _steelSpacing;

        public string KetLuanTongThe
        {
            get => _ketLuanTongThe;
            set
            {
                _ketLuanTongThe = value;
                OnPropertyChanged(nameof(KetLuanTongThe));
            }
        }


        public Page3ViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            // Subscribe to Ptb/Pmax changes from Page5ViewModel
            if (_mainViewModel?.Page5ViewModel != null)
            {
                _mainViewModel.Page5ViewModel.PropertyChanged += Page5ViewModel_PropertyChanged;
            }
        }
        private void Page5ViewModel_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            // Re-check conditions if Ptb or Pmax from Page5ViewModel changes
            if (e.PropertyName == nameof(Page5ViewModel.Ptb) || e.PropertyName == nameof(Page5ViewModel.Pmax))
            {
                CheckConditions();
            }
        }

        public double ChieuRongMong
        {
            get => _chieuRongMong;
            set
            {
                if (_chieuRongMong != value)
                {
                    _chieuRongMong = value;
                    OnPropertyChanged(nameof(ChieuRongMong));
                    CalculateTerzaghiValues(); // Recalculate TerzaghiP
                    CheckConditions(); // Re-check conditions
                }
            }
        }

        public double ChieuSauChonMong
        {
            get => _chieuSauChonMong;
            set
            {
                if (_chieuSauChonMong != value)
                {
                    _chieuSauChonMong = value;
                    OnPropertyChanged(nameof(ChieuSauChonMong));
                    CalculateTerzaghiValues(); // Recalculate TerzaghiP
                    CheckConditions(); // Re-check conditions
                }
            }
        }

        public double TerzaghiP
        {
            get => _terzaghiP;
            set
            {
                _terzaghiP = value;
                OnPropertyChanged(nameof(TerzaghiP));
            }
        }
        public double PtbDisplayed
        {
            get => _mainViewModel?.Page5ViewModel?.Ptb ?? 0;
        }

        public double PmaxDisplayed
        {
            get => _mainViewModel?.Page5ViewModel?.Pmax ?? 0;
        }
        public ForceInputEntry SelectedForceInput
        {
            get => _selectedForceInput;
            set
            {
                if (_selectedForceInput != value)
                {
                    _selectedForceInput = value;
                    OnPropertyChanged(nameof(SelectedForceInput));
                    if (value?.Mong != null)
                    {
                        // Update ChieuRongMong and ChieuSauChonMong based on selected foundation
                        ChieuRongMong = value.Mong.ChieuRongMong;
                        ChieuSauChonMong = value.Mong.ChieuSauChonMong;
                        // Note: Setting ChieuRongMong/ChieuSauChonMong will trigger CalculateTerzaghiValues() and CheckConditions()
                    }
                    else
                    {
                        // Clear values if no force input is selected
                        ClearCalculatedValues();
                    }
                }
            }
        }

        public double TerzaghiP120
        {
            get => _terzaghiP120;
            set
            {
                _terzaghiP120 = value;
                OnPropertyChanged(nameof(TerzaghiP120));
            }
        }

        public double ChenhLechKinhTe
        {
            get => _chenhLechKinhTe;
            set
            {
                _chenhLechKinhTe = value;
                OnPropertyChanged(nameof(ChenhLechKinhTe));
            }
        }

        public Brush DieuKien1Brush
        {
            get => _dieuKien1Brush;
            set
            {
                _dieuKien1Brush = value;
                OnPropertyChanged(nameof(DieuKien1Brush));
            }
        }

        public Brush DieuKien2Brush
        {
            get => _dieuKien2Brush;
            set
            {
                _dieuKien2Brush = value;
                OnPropertyChanged(nameof(DieuKien2Brush));
            }
        }

        public Brush KinhTeBrush
        {
            get => _kinhTeBrush;
            set
            {
                _kinhTeBrush = value;
                OnPropertyChanged(nameof(KinhTeBrush));
            }
        }

        public string KetLuanDieuKien1
        {
            get => _ketLuanDieuKien1;
            set
            {
                _ketLuanDieuKien1 = value;
                OnPropertyChanged(nameof(KetLuanDieuKien1));
            }
        }

        public string KetLuanDieuKien2
        {
            get => _ketLuanDieuKien2;
            set
            {
                _ketLuanDieuKien2 = value;
                OnPropertyChanged(nameof(KetLuanDieuKien2));
            }
        }

        public string KetLuanKinhTe
        {
            get => _ketLuanKinhTe;
            set
            {
                _ketLuanKinhTe = value;
                OnPropertyChanged(nameof(KetLuanKinhTe));
            }
        }

        public ObservableCollection<int> SteelDiameters { get; } = new ObservableCollection<int>
        {
            6, 8, 10, 12, 14, 16, 18, 20, 22, 25, 28, 30, 32, 36, 40
        };

        public int SelectedSteelDiameter
        {
            get => _selectedSteelDiameter;
            set
            {
                if (_selectedSteelDiameter != value)
                {
                    _selectedSteelDiameter = value;
                    OnPropertyChanged();
                    CalculateReinforcementDetails();
                }
            }
        }

        public int CalculatedSteelBars
        {
            get => _calculatedSteelBars;
            private set
            {
                if (_calculatedSteelBars != value)
                {
                    _calculatedSteelBars = value;
                    OnPropertyChanged();
                }
            }
        }

        public string CalculatedSteelArea
        {
            get => _actualSteelArea.ToString("F3");
            private set
            {
                if (double.TryParse(value, out double newValue) && _actualSteelArea != newValue)
                {
                    _actualSteelArea = newValue;
                    OnPropertyChanged();
                }
            }
        }

        public int CalculatedSteelSpacing
        {
            get => _steelSpacing;
            private set
            {
                if (_steelSpacing != value)
                {
                    _steelSpacing = value;
                    OnPropertyChanged();
                }
            }
        }

        private void CalculateTerzaghiValues()
        {
            try
            {
                if (SelectedForceInput?.Mong?.SoilLayer == null ||
                    ChieuRongMong <= 0 ||
                    ChieuSauChonMong <= 0)
                {
                    TerzaghiP = 0;
                    return;
                }

                // Lấy dữ liệu từ lớp đất của móng
                var soilLayer = SelectedForceInput.Mong.SoilLayer;
                double phi = soilLayer.Gocmasattrong;
                double gamma = soilLayer.Khoiluongtunhien;
                double c = soilLayer.Lucdinhket;
                double hm = ChieuSauChonMong;
                double b = ChieuRongMong;

                // Tính toán các hệ số alpha
                double alpha1 = 1 - 0.2 * b;
                double alpha2 = 1;
                double alpha3 = 1 + 0.2 * b;

                // Nội suy giá trị Nγ, Nq, Nc
                var terzaghiCoeffs = GetTerzaghiCoefficients(phi);
                if (terzaghiCoeffs != null)
                {
                    double ny = terzaghiCoeffs.Ny;
                    double nq = terzaghiCoeffs.Nq;
                    double nc = terzaghiCoeffs.Nc;

                    // Tính toán giá trị q = γ * hm
                    double qLoad = gamma * hm;

                    // Tính sức chịu tải theo Terzaghi
                    TerzaghiP = (0.5 * ny * alpha1 * gamma * b +
                                nq * alpha2 * qLoad +
                                nc * alpha3 * c) / 2.5;

                    // Làm tròn kết quả
                    TerzaghiP = Math.Round(TerzaghiP, 2);
                }
                else
                {
                    TerzaghiP = 0;
                }
            }
            catch (Exception ex)
            {
                TerzaghiP = 0;
                MessageBox.Show("Lỗi khi tính toán sức chịu tải Terzaghi: " + ex.Message, "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void CheckConditions()
        {
            // Reset màu và kết luận
            DieuKien1Brush = Brushes.Black;
            DieuKien2Brush = Brushes.Black;
            KinhTeBrush = Brushes.Black;
            KetLuanDieuKien1 = "";
            KetLuanDieuKien2 = "";
            KetLuanKinhTe = "";
            KetLuanTongThe = "";

            if (SelectedForceInput == null || _mainViewModel?.Page5ViewModel == null)
            {
                return;
            }

            // Tính giá trị 1.2 * TerzaghiP
            TerzaghiP120 = Math.Round(1.2 * TerzaghiP, 2);

            // Lấy giá trị Ptb và Pmax từ Page5ViewModel
            double Ptb = _mainViewModel.Page5ViewModel.Ptb;
            double Pmax = _mainViewModel.Page5ViewModel.Pmax;

            bool dieuKien1ThoaMan = false;
            bool dieuKien2ThoaMan = false;
            bool kinhTeThoaMan = false;

            // Kiểm tra điều kiện 1: Ptb < TerzaghiP
            if (Ptb < TerzaghiP)
            {
                KetLuanDieuKien1 = $"THỎA MÃN ĐIỀU KIỆN (Ptb = {Ptb:F2} < P = {TerzaghiP:F2})";
                DieuKien1Brush = Brushes.Green;
                dieuKien1ThoaMan = true;
            }
            else
            {
                KetLuanDieuKien1 = $"KHÔNG ĐẠT - CHỌN LẠI KÍCH THƯỚC MÓNG (Ptb = {Ptb:F2} ≥ P = {TerzaghiP:F2})";
                DieuKien1Brush = Brushes.Red;
            }

            // Kiểm tra điều kiện 2: Pmax < 1.2 * TerzaghiP
            if (Pmax < TerzaghiP120)
            {
                KetLuanDieuKien2 = $"THỎA MÃN ĐIỀU KIỆN (Pmax = {Pmax:F2} < 1.2P = {TerzaghiP120:F2})";
                DieuKien2Brush = Brushes.Green;
                dieuKien2ThoaMan = true;
            }
            else
            {
                KetLuanDieuKien2 = $"KHÔNG ĐẠT - CHỌN LẠI KÍCH THƯỚC MÓNG (Pmax = {Pmax:F2} ≥ 1.2P = {TerzaghiP120:F2})";
                DieuKien2Brush = Brushes.Red;
            }

            // Kiểm tra điều kiện kinh tế
            if (TerzaghiP120 != 0)
            {
                ChenhLechKinhTe = Math.Round(((TerzaghiP120 - Pmax) / TerzaghiP120) * 100, 2);
            }
            else
            {
                ChenhLechKinhTe = 0;
            }

            if (ChenhLechKinhTe <= 15 && ChenhLechKinhTe <= 40)
            {
                KetLuanKinhTe = $"THỎA MÃN ĐIỀU KIỆN KINH TẾ (Chênh lệch = {ChenhLechKinhTe:F2}%)";
                KinhTeBrush = Brushes.Green;
                kinhTeThoaMan = true;
            }
            else if (ChenhLechKinhTe > 15)
            {
                KetLuanKinhTe = $"KHÔNG ĐẠT - CHƯA TỐI ƯU (Chênh lệch = {ChenhLechKinhTe:F2}% < 15%)";
                KinhTeBrush = Brushes.Red;
            }
            else // ChenhLechKinhTe > 40
            {
                KetLuanKinhTe = $"KHÔNG ĐẠT - LÃNG PHÍ (Chênh lệch = {ChenhLechKinhTe:F2}% > 40%)";
                KinhTeBrush = Brushes.Red;
            }


            // Tổng kết
            if (dieuKien1ThoaMan && dieuKien2ThoaMan && kinhTeThoaMan)
            {
                KetLuanTongThe = "ĐẠT TẤT CẢ CÁC ĐIỀU KIỆN VÀ TỐI ƯU KINH TẾ. CÓ THỂ TIẾN HÀNH THIẾT KẾ THÉP.";
            }
            else
            {
                KetLuanTongThe = "KHÔNG ĐẠT CÁC ĐIỀU KIỆN. VUI LÒNG CHỌN LẠI KÍCH THƯỚC MÓNG HOẶC ĐIỀU CHỈNH THÔNG SỐ ĐẤT/TẢI TRỌNG.";
            }
        }
    }
}