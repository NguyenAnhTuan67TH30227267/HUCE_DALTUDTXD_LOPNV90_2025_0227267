using HUCE_DALTUDTXD_LOPNV90_2025_0227267.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Media;
using System.Runtime.CompilerServices;
using System.Windows.Input;

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

        private void ClearCalculatedValues()
        {
            TerzaghiP = 0;
            TerzaghiP120 = 0;
            ChenhLechKinhTe = 0;
            KetLuanDieuKien1 = "";
            KetLuanDieuKien2 = "";
            KetLuanKinhTe = "";
            KetLuanTongThe = "";
            DieuKien1Brush = Brushes.Black;
            DieuKien2Brush = Brushes.Black;
            KinhTeBrush = Brushes.Black;
        }

        public ObservableCollection<ForceInputEntry> ForceInputList => _mainViewModel.Page4ViewModel.ForceInputList;

        private TerzaghiCoefficients GetTerzaghiCoefficients(double phi)
        {
            // Tạo bảng Terzaghi đầy đủ
            var table = new List<TerzaghiCoefficients>
            {
                new TerzaghiCoefficients { Phi = 0, Ny = 0, Nq = 1, Nc = 5.14 },
                new TerzaghiCoefficients { Phi = 5, Ny = 1, Nq = 1.56, Nc = 6.47 },
                new TerzaghiCoefficients { Phi = 10, Ny = 1, Nq = 2.49, Nc = 8.45 },
                new TerzaghiCoefficients { Phi = 11, Ny = 1.2, Nq = 2.71, Nc = 8.8 },
                new TerzaghiCoefficients { Phi = 12, Ny = 1.43, Nq = 2.97, Nc = 9.29 },
                new TerzaghiCoefficients { Phi = 13, Ny = 1.69, Nq = 3.26, Nc = 9.8 },
                new TerzaghiCoefficients { Phi = 14, Ny = 1.99, Nq = 3.59, Nc = 10.4 },
                new TerzaghiCoefficients { Phi = 15, Ny = 2.32, Nq = 3.94, Nc = 11 },
                new TerzaghiCoefficients { Phi = 16, Ny = 2.72, Nq = 4.33, Nc = 11.6 },
                new TerzaghiCoefficients { Phi = 17, Ny = 3.14, Nq = 4.77, Nc = 12.3 },
                new TerzaghiCoefficients { Phi = 18, Ny = 3.69, Nq = 5.25, Nc = 13.1 },
                new TerzaghiCoefficients { Phi = 19, Ny = 4.29, Nq = 5.8, Nc = 13.9 },
                new TerzaghiCoefficients { Phi = 20, Ny = 4.97, Nq = 6.4, Nc = 14.8 },
                new TerzaghiCoefficients { Phi = 21, Ny = 5.76, Nq = 7.07, Nc = 15.8 },
                new TerzaghiCoefficients { Phi = 22, Ny = 6.68, Nq = 8.83, Nc = 16.9 },
                new TerzaghiCoefficients { Phi = 23, Ny = 7.73, Nq = 8.66, Nc = 18.1 },
                new TerzaghiCoefficients { Phi = 24, Ny = 8.97, Nq = 9.6, Nc = 19.3 },
                new TerzaghiCoefficients { Phi = 25, Ny = 10.4, Nq = 10.7, Nc = 20.7 },
                new TerzaghiCoefficients { Phi = 26, Ny = 12, Nq = 11.8, Nc = 22.2 },
                new TerzaghiCoefficients { Phi = 27, Ny = 13.9, Nq = 13.2, Nc = 24 },
                new TerzaghiCoefficients { Phi = 28, Ny = 16.1, Nq = 14.7, Nc = 25.8 },
                new TerzaghiCoefficients { Phi = 29, Ny = 18.8, Nq = 16.4, Nc = 27.9 },
                new TerzaghiCoefficients { Phi = 30, Ny = 21.8, Nq = 18.4, Nc = 30.4 },
                new TerzaghiCoefficients { Phi = 31, Ny = 25.5, Nq = 20.6, Nc = 32.7 },
                new TerzaghiCoefficients { Phi = 32, Ny = 29.8, Nq = 23.2, Nc = 35.5 },
                new TerzaghiCoefficients { Phi = 33, Ny = 34.8, Nq = 26.1, Nc = 38.7 },
                new TerzaghiCoefficients { Phi = 34, Ny = 40.9, Nq = 29.4, Nc = 42.2 },
                new TerzaghiCoefficients { Phi = 35, Ny = 48, Nq = 33.3, Nc = 46.1 },
                new TerzaghiCoefficients { Phi = 36, Ny = 56.6, Nq = 37.8, Nc = 50.6 },
                new TerzaghiCoefficients { Phi = 37, Ny = 67, Nq = 42.9, Nc = 55.7 },
                new TerzaghiCoefficients { Phi = 38, Ny = 79.5, Nq = 48.9, Nc = 61.4 },
                new TerzaghiCoefficients { Phi = 39, Ny = 94.7, Nq = 56, Nc = 67.9 },
                new TerzaghiCoefficients { Phi = 40, Ny = 113, Nq = 64.2, Nc = 75.4 },
                new TerzaghiCoefficients { Phi = 41, Ny = 133, Nq = 73.9, Nc = 83.9 },
                new TerzaghiCoefficients { Phi = 42, Ny = 164, Nq = 85.4, Nc = 93.7 },
                new TerzaghiCoefficients { Phi = 43, Ny = 199, Nq = 99, Nc = 105 },
                new TerzaghiCoefficients { Phi = 44, Ny = 244, Nq = 111.5, Nc = 118 },
                new TerzaghiCoefficients { Phi = 45, Ny = 297, Nq = 135, Nc = 135 }
            };

            // Sắp xếp bảng theo góc φ
            var ordered = table.OrderBy(d => d.Phi).ToList();

            // Tìm giá trị chính xác
            var exact = ordered.FirstOrDefault(d => Math.Abs(d.Phi - phi) < 0.001);
            if (exact != null) return exact;

            // Tìm 2 điểm gần nhất
            var lower = ordered.LastOrDefault(d => d.Phi <= phi);
            var upper = ordered.FirstOrDefault(d => d.Phi >= phi);

            if (lower == null || upper == null) return null;

            // Tính nội suy tuyến tính
            double ratio = (phi - lower.Phi) / (upper.Phi - lower.Phi);

            return new TerzaghiCoefficients
            {
                Phi = phi,
                Ny = lower.Ny + ratio * (upper.Ny - lower.Ny),
                Nq = lower.Nq + ratio * (upper.Nq - lower.Nq),
                Nc = lower.Nc + ratio * (upper.Nc - lower.Nc)
            };
        }

        private void CalculateReinforcementDetails()
        {
            if (SelectedSteelDiameter <= 0) return;

            // Tính diện tích một thanh thép (cm2)
            double singleBarArea = Math.PI * Math.Pow(SelectedSteelDiameter / 10.0, 2) / 4;

            // Tính số thanh thép cần thiết
            int numberOfBars = (int)Math.Ceiling(_calculatedSteelArea / singleBarArea);

            // Đảm bảo số thanh là số lẻ để đối xứng
            if (numberOfBars % 2 == 0) numberOfBars++;

            // Tính diện tích thép thực tế
            double actualArea = numberOfBars * singleBarArea;

            // Tính khoảng cách giữa các thanh (mm)
            int spacing = 200; // Mặc định 200mm hoặc tính toán dựa trên kích thước móng

            CalculatedSteelBars = numberOfBars;
            CalculatedSteelArea = actualArea.ToString();
            CalculatedSteelSpacing = spacing;
        }

        public new event PropertyChangedEventHandler PropertyChanged;

        protected new virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
    }

    public class TerzaghiCoefficients
    {
        public double Phi { get; set; }
        public double Ny { get; set; }
        public double Nq { get; set; }
        public double Nc { get; set; }
    }
}