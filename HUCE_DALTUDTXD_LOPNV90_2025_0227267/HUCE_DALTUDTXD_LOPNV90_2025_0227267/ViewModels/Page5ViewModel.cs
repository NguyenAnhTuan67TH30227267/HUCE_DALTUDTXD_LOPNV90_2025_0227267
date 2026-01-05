using HUCE_DALTUDTXD_LOPNV90_2025_0227267.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Windows;
using System.Windows.Media;

namespace HUCE_DALTUDTXD_LOPNV90_2025_0227267.ViewModels
{
    public class Page5ViewModel : ViewModelBase
    {
        public class Page3ViewModel : ViewModelBase, INotifyPropertyChanged
        {
            private readonly MainViewModel _mainViewModel;

            private ForceInputEntry _selectedForceInput;
            private double _terzaghiP;
            private double _terzaghiP120;
            private double _chieuRongMong;
            private double _chieuSauChonMong;

            private double _chenhLechKinhTe;
            private Brush _dieuKien1Brush = Brushes.Black;
            private Brush _dieuKien2Brush = Brushes.Black;
            private Brush _kinhTeBrush = Brushes.Black;
            private string _ketLuanDieuKien1;
            private string _ketLuanDieuKien2;
            private string _ketLuanKinhTe;
            private string _ketLuanTongThe;

            private double _calculatedSteelArea = 3.85; // Giả định diện tích thép yêu cầu (cm2)
            private int _selectedSteelDiameter;
            private int _calculatedSteelBars;
            private double _actualSteelArea;
            private int _steelSpacing;

            private Geometry _foundationOutline;
            private Geometry _geometryGroup_LongRebar;

            public Page3ViewModel(MainViewModel mainViewModel)
            {
                _mainViewModel = mainViewModel;

                if (_mainViewModel?.Page5ViewModel?.CalculationResults != null)
                {
                    _mainViewModel.Page5ViewModel.CalculationResults.CollectionChanged += (s, e) =>
                    {
                        UpdateCalculationDisplay();
                    };
                }
            }

            #region Danh sách và Lựa chọn (ComboBox Binding)

            public ObservableCollection<ForceInputEntry> ForceInputList => _mainViewModel.Page4ViewModel.ForceInputList;

            public ForceInputEntry SelectedForceInput
            {
                get => _selectedForceInput;
                set
                {
                    if (_selectedForceInput != value)
                    {
                        _selectedForceInput = value;
                        OnPropertyChanged();

                        if (value?.Mong != null)
                        {
                            // Tự động điền kích thước từ móng đã chọn
                            ChieuRongMong = value.Mong.ChieuRongMong;
                            ChieuSauChonMong = value.Mong.ChieuSauChonMong;

                            // Cập nhật đường kính thép mặc định nếu chưa chọn
                            if (SelectedSteelDiameter == 0) SelectedSteelDiameter = 12;
                        }
                        else
                        {
                            ClearCalculatedValues();
                        }

                        UpdateCalculationDisplay();
                    }
                }
            }

            #endregion

            #region Thuộc tính hiển thị kết quả từ Page 5

            public double PtbDisplayed
            {
                get
                {
                    var result = _mainViewModel?.Page5ViewModel?.CalculationResults?
                        .FirstOrDefault(r => r.TenMong == SelectedForceInput?.Mong?.TenMong);
                    return result?.Ptb ?? 0;
                }
            }

            public double PmaxDisplayed
            {
                get
                {
                    var result = _mainViewModel?.Page5ViewModel?.CalculationResults?
                        .FirstOrDefault(r => r.TenMong == SelectedForceInput?.Mong?.TenMong);
                    return result?.Pmax ?? 0;
                }
            }

            #endregion

            #region Thuộc tính Binding UI

            public double ChieuRongMong
            {
                get => _chieuRongMong;
                set { _chieuRongMong = value; OnPropertyChanged(); RecalculateAll(); }
            }

            public double ChieuSauChonMong
            {
                get => _chieuSauChonMong;
                set { _chieuSauChonMong = value; OnPropertyChanged(); RecalculateAll(); }
            }

            public double TerzaghiP { get => _terzaghiP; set { _terzaghiP = value; OnPropertyChanged(); } }
            public double TerzaghiP120 { get => _terzaghiP120; set { _terzaghiP120 = value; OnPropertyChanged(); } }
            public double ChenhLechKinhTe { get => _chenhLechKinhTe; set { _chenhLechKinhTe = value; OnPropertyChanged(); } }

            public string KetLuanDieuKien1 { get => _ketLuanDieuKien1; set { _ketLuanDieuKien1 = value; OnPropertyChanged(); } }
            public string KetLuanDieuKien2 { get => _ketLuanDieuKien2; set { _ketLuanDieuKien2 = value; OnPropertyChanged(); } }
            public string KetLuanKinhTe { get => _ketLuanKinhTe; set { _ketLuanKinhTe = value; OnPropertyChanged(); } }
            public string KetLuanTongThe { get => _ketLuanTongThe; set { _ketLuanTongThe = value; OnPropertyChanged(); } }

            public Brush DieuKien1Brush { get => _dieuKien1Brush; set { _dieuKien1Brush = value; OnPropertyChanged(); } }
            public Brush DieuKien2Brush { get => _dieuKien2Brush; set { _dieuKien2Brush = value; OnPropertyChanged(); } }
            public Brush KinhTeBrush { get => _kinhTeBrush; set { _kinhTeBrush = value; OnPropertyChanged(); } }

            #endregion

            #region Tính toán Reinforcement (Cốt thép)

            public ObservableCollection<int> SteelDiameters { get; } = new ObservableCollection<int> { 6, 8, 10, 12, 14, 16, 18, 20, 22, 25, 28, 30, 32, 36, 40 };

            public int SelectedSteelDiameter
            {
                get => _selectedSteelDiameter;
                set { if (_selectedSteelDiameter != value) { _selectedSteelDiameter = value; OnPropertyChanged(); CalculateReinforcement(); } }
            }

            public int CalculatedSteelBars { get => _calculatedSteelBars; set { _calculatedSteelBars = value; OnPropertyChanged(); } }
            public string CalculatedSteelArea { get => _actualSteelArea.ToString("F3"); }
            public int CalculatedSteelSpacing { get => _steelSpacing; set { _steelSpacing = value; OnPropertyChanged(); } }

            #endregion

            #region Logic Tính toán và Kiểm tra

            private void RecalculateAll()
            {
                CalculateTerzaghiValues();
                UpdateCalculationDisplay();
            }

            private void UpdateCalculationDisplay()
            {
                OnPropertyChanged(nameof(PtbDisplayed));
                OnPropertyChanged(nameof(PmaxDisplayed));
                CheckConditions();
                CalculateReinforcement();
            }

            private void CalculateTerzaghiValues()
            {
                if (SelectedForceInput?.Mong?.SoilLayer == null || ChieuRongMong <= 0) return;

                var soil = SelectedForceInput.Mong.SoilLayer;
                double phi = soil.Gocmasattrong;
                double gamma = soil.Khoiluongtunhien;
                double c = soil.Lucdinhket;
                double b = ChieuRongMong;
                double hm = ChieuSauChonMong;

                var coeffs = GetTerzaghiCoefficients(phi);
                if (coeffs != null)
                {
                    // Công thức Terzaghi (Fs = 2.5)
                    double result = (0.5 * coeffs.Ny * gamma * b + coeffs.Nq * (gamma * hm) + coeffs.Nc * c) / 2.5;
                    TerzaghiP = Math.Round(result, 2);
                    TerzaghiP120 = Math.Round(1.2 * TerzaghiP, 2);
                }
            }

            private void CheckConditions()
            {
                if (SelectedForceInput == null) return;

                double ptb = PtbDisplayed;
                double pmax = PmaxDisplayed;

                // Điều kiện 1: Ptb < P_Terzaghi
                if (ptb < TerzaghiP && TerzaghiP > 0)
                {
                    KetLuanDieuKien1 = $"THỎA MÃN (Ptb={ptb} < P={TerzaghiP})";
                    DieuKien1Brush = Brushes.Green;
                }
                else
                {
                    KetLuanDieuKien1 = $"KHÔNG ĐẠT (Ptb={ptb} ≥ P={TerzaghiP})";
                    DieuKien1Brush = Brushes.Red;
                }

                // Điều kiện 2: Pmax < 1.2 * P_Terzaghi
                if (pmax < TerzaghiP120 && TerzaghiP120 > 0)
                {
                    KetLuanDieuKien2 = $"THỎA MÃN (Pmax={pmax} < 1.2P={TerzaghiP120})";
                    DieuKien2Brush = Brushes.Green;
                }
                else
                {
                    KetLuanDieuKien2 = $"KHÔNG ĐẠT (Pmax={pmax} ≥ 1.2P={TerzaghiP120})";
                    DieuKien2Brush = Brushes.Red;
                }

                // Tính kinh tế
                if (TerzaghiP120 > 0)
                {
                    ChenhLechKinhTe = Math.Round(((TerzaghiP120 - pmax) / TerzaghiP120) * 100, 2);
                    if (ChenhLechKinhTe >= 0 && ChenhLechKinhTe <= 15) { KetLuanKinhTe = $"TỐI ƯU ({ChenhLechKinhTe}%)"; KinhTeBrush = Brushes.Green; }
                    else if (ChenhLechKinhTe > 15 && ChenhLechKinhTe <= 40) { KetLuanKinhTe = $"CHƯA TỐI ƯU ({ChenhLechKinhTe}%)"; KinhTeBrush = Brushes.Orange; }
                    else { KetLuanKinhTe = $"LÃNG PHÍ ({ChenhLechKinhTe}%)"; KinhTeBrush = Brushes.Red; }
                }

                KetLuanTongThe = (DieuKien1Brush == Brushes.Green && DieuKien2Brush == Brushes.Green)
                    ? "KẾT LUẬN: MÓNG ĐẢM BẢO CHỊU LỰC."
                    : "KẾT LUẬN: KÍCH THƯỚC MÓNG CHƯA HỢP LÝ.";
            }

            private TerzaghiCoefficients GetTerzaghiCoefficients(double phi)
            {
                var table = new List<TerzaghiCoefficients>
            {
                new TerzaghiCoefficients { Phi = 0, Ny = 0, Nq = 1, Nc = 5.14 },
                new TerzaghiCoefficients { Phi = 5, Ny = 0.5, Nq = 1.6, Nc = 6.5 },
                new TerzaghiCoefficients { Phi = 10, Ny = 1.2, Nq = 2.5, Nc = 8.4 },
                new TerzaghiCoefficients { Phi = 15, Ny = 2.5, Nq = 4.0, Nc = 11.0 },
                new TerzaghiCoefficients { Phi = 20, Ny = 5.0, Nq = 6.4, Nc = 14.8 },
                new TerzaghiCoefficients { Phi = 25, Ny = 9.7, Nq = 10.7, Nc = 20.7 },
                new TerzaghiCoefficients { Phi = 30, Ny = 19.7, Nq = 18.4, Nc = 30.1 },
                new TerzaghiCoefficients { Phi = 35, Ny = 42.4, Nq = 33.3, Nc = 46.1 },
                new TerzaghiCoefficients { Phi = 40, Ny = 100.4, Nq = 64.2, Nc = 75.3 }
            };

                var lower = table.OrderByDescending(t => t.Phi).FirstOrDefault(t => t.Phi <= phi);
                var upper = table.OrderBy(t => t.Phi).FirstOrDefault(t => t.Phi >= phi);

                if (lower == null || upper == null || lower.Phi == upper.Phi) return lower ?? upper;

                double ratio = (phi - lower.Phi) / (upper.Phi - lower.Phi);
                return new TerzaghiCoefficients
                {
                    Phi = phi,
                    Ny = lower.Ny + ratio * (upper.Ny - lower.Ny),
                    Nq = lower.Nq + ratio * (upper.Nq - lower.Nq),
                    Nc = lower.Nc + ratio * (upper.Nc - lower.Nc)
                };
            }

            private void CalculateReinforcement()
            {
                if (SelectedSteelDiameter <= 0 || ChieuRongMong <= 0) return;

                double areaPerBar = Math.PI * Math.Pow(SelectedSteelDiameter / 10.0, 2) / 4.0;
                int n = (int)Math.Ceiling(_calculatedSteelArea / areaPerBar);
                if (n < 2) n = 2;
                if (n % 2 == 0) n++;

                CalculatedSteelBars = n;
                _actualSteelArea = n * areaPerBar;
                OnPropertyChanged(nameof(CalculatedSteelArea));

                CalculatedSteelSpacing = (int)((ChieuRongMong * 1000 - 100) / (n - 1));

                DrawFoundationDrawing();
            }

            private void ClearCalculatedValues()
            {
                TerzaghiP = TerzaghiP120 = ChenhLechKinhTe = 0;
                KetLuanDieuKien1 = KetLuanDieuKien2 = KetLuanKinhTe = KetLuanTongThe = "";
            }

            #endregion

            #region Đồ họa (Drawing Logic)

            public Geometry FoundationOutline { get => _foundationOutline; set { _foundationOutline = value; OnPropertyChanged(); } }
            public Geometry GeometryGroup_LongRebar { get => _geometryGroup_LongRebar; set { _geometryGroup_LongRebar = value; OnPropertyChanged(); } }

            public void DrawFoundationDrawing()
            {
                double B_mm = ChieuRongMong * 1000;
                if (B_mm <= 0) return;

                double Bc_mm = 300; // Bề rộng cổ cột giả định
                double H1 = 300, H2 = 200, H3 = 400; // Các cao độ giả định
                double canvasW = 380, canvasH = 220;
                double scale = Math.Min((canvasW - 40) / B_mm, (canvasH - 40) / (H1 + H2 + H3));
                double oX = canvasW / 2, oY = canvasH - 20;

                // Vẽ bao móng
                StreamGeometry geo = new StreamGeometry();
                using (StreamGeometryContext ctx = geo.Open())
                {
                    ctx.BeginFigure(new Point(oX - (B_mm / 2) * scale, oY), true, true);
                    ctx.LineTo(new Point(oX + (B_mm / 2) * scale, oY), true, false);
                    ctx.LineTo(new Point(oX + (B_mm / 2) * scale, oY - H1 * scale), true, false);
                    ctx.LineTo(new Point(oX + (Bc_mm / 2) * scale, oY - (H1 + H2) * scale), true, false);
                    ctx.LineTo(new Point(oX + (Bc_mm / 2) * scale, oY - (H1 + H2 + H3) * scale), true, false);
                    ctx.LineTo(new Point(oX - (Bc_mm / 2) * scale, oY - (H1 + H2 + H3) * scale), true, false);
                    ctx.LineTo(new Point(oX - (Bc_mm / 2) * scale, oY - (H1 + H2) * scale), true, false);
                    ctx.LineTo(new Point(oX - (B_mm / 2) * scale, oY - H1 * scale), true, false);
                }
                FoundationOutline = geo;

                // Vẽ thép
                GeometryGroup rebar = new GeometryGroup();
                double cover = 50 * scale;
                rebar.Children.Add(new LineGeometry(
                    new Point(oX - (B_mm / 2) * scale + cover, oY - cover),
                    new Point(oX + (B_mm / 2) * scale - cover, oY - cover)));

                int n = CalculatedSteelBars;
                if (n > 1)
                {
                    double startX = oX - (B_mm / 2) * scale + cover + 5;
                    double endX = oX + (B_mm / 2) * scale - cover - 5;
                    double step = (endX - startX) / (n - 1);
                    for (int i = 0; i < n; i++)
                        rebar.Children.Add(new EllipseGeometry(new Point(startX + i * step, oY - cover - 5), 2, 2));
                }
                GeometryGroup_LongRebar = rebar;
            }

            #endregion

            public new event PropertyChangedEventHandler PropertyChanged;
            protected new virtual void OnPropertyChanged([CallerMemberName] string propertyName = null)
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private readonly MainViewModel _mainViewModel;
        private ObservableCollection<FootingResult> _calculationResults;

        public ObservableCollection<FootingResult> CalculationResults
        {
            get => _calculationResults;
            set { _calculationResults = value; OnPropertyChanged(nameof(CalculationResults)); }
        }

        // Lấy danh sách đầu vào từ Page 4
        public ObservableCollection<ForceInputEntry> ForceInputList => _mainViewModel.Page4ViewModel.ForceInputList;

        public Page5ViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            CalculationResults = new ObservableCollection<FootingResult>();

            // Mỗi khi danh sách móng ở Page 4 thay đổi, tự động tính lại bảng này
            ForceInputList.CollectionChanged += (s, e) => CalculateAll();
            CalculateAll();
        }

        public void CalculateAll()
        {
            CalculationResults.Clear();
            var soilList = _mainViewModel.Page1ViewModel.FoundationList;

            foreach (var input in ForceInputList)
            {
                if (input.Mong == null || input.Mong.SoilLayer == null) continue;

                // --- 1. Thông số cơ bản ---
                double N = input.AxialForce;
                double M = input.Moment;
                double b = input.Mong.ChieuRongMong;
                double hm = input.Mong.ChieuSauChonMong;
                double bt = input.Mong.BeDayTuong;
                double hd = input.Mong.ChieuCaoDai;
                double bv = input.Mong.ChieuDayLopBaoVe / 1000.0;
                double rbt = ConcreteProperties.GetRbtInTM2(input.Mong.CapDoBeTong);
                double gamma = input.Mong.SoilLayer.Khoiluongtunhien;
                double E0 = input.Mong.SoilLayer.Modunbiendang;
                double h_lop_hien_tai = input.Mong.SoilLayer.Chieudaylopdat;

                // --- 2. Tính toán các loại áp lực ---
                double ptb = Math.Round(N / (1.15 * b) + 2 * hm, 2);
                double pmax = Math.Round(N / (1.15 * b) + 2 * hm + 6 * M / (1.15 * b * b), 2);
                double pmin = Math.Round(N / (1.15 * b) + 2 * hm - 6 * M / (1.15 * b * b), 2);

                // Áp lực ròng (P0)
                double p0max = Math.Round(N / b + 6 * M / (b * b), 2);
                double p0min = Math.Round(N / b - 6 * M / (b * b), 2);
                double Pgl = ptb - (gamma * hm);

                // --- 3. Logic chiều dày đất (Xử lý lớp 1 + lớp 2 nếu móng sâu) ---
                double h_duoi_day = h_lop_hien_tai - hm;
                if (h_duoi_day < 0)
                {
                    int currentIndex = soilList.IndexOf(input.Mong.SoilLayer);
                    if (currentIndex >= 0 && currentIndex < soilList.Count - 1)
                    {
                        double h_lop_ke_tiep = soilList[currentIndex + 1].Chieudaylopdat;
                        h_duoi_day = (h_lop_hien_tai + h_lop_ke_tiep) - hm;
                    }
                }
                double h_final = Math.Max(0, h_duoi_day);

                // --- 4. Tính độ lún S (Nội suy w) ---
                double tiSo = (b >= 1) ? b : (1 / b);
                double w = GetWInterpolated(tiSo);
                double Bqu = b + 2 * h_final * Math.Tan(30 * Math.PI / 180);

                double doLunS = 0;
                if (E0 > 0)
                {
                    double quy = 0.25;
                    double s_met = (Pgl * Bqu * w * (1 - Math.Pow(quy, 2))) / E0;
                    doLunS = Math.Round(s_met, 2); // cm
                }

                // --- 5. Kiểm tra chọc thủng ---
                double bHieuDung = b - bt - 2 * (hd - bv);
                double pDamThung = Math.Round((p0min + (p0max - p0min) * (b - 0.5 * bHieuDung) / b + p0max) / 2 * (0.5 * bHieuDung), 2);
                double pChongDamThung = Math.Round((hd - bv) * rbt, 2);

                // --- 6. Ghi chú trạng thái ---
                bool isOk = pmin >= 0 && pDamThung <= pChongDamThung && doLunS <= 8.0;
                string status = isOk ? "Thỏa mãn" : (doLunS > 8.0 ? "Lún lớn!" : (pmin < 0 ? "Pmin < 0" : "Check!"));

                // --- 7. Xuất kết quả ra bảng (Đầy đủ P0min) ---
                CalculationResults.Add(new FootingResult
                {
                    TenMong = input.Mong.TenMong,
                    Ptb = ptb,
                    Pmax = pmax,
                    Pmin = pmin,
                    P0max = p0max,
                    P0min = p0min, // Gán giá trị hiển thị ra bảng ở đây
                    PDamThung = pDamThung,
                    PChongDamThung = pChongDamThung,
                    DoLun = doLunS,
                    GhiChu = status
                });
            }
        }

        /// <summary>
        /// Hàm nội suy hệ số w dựa trên tỷ số r
        /// </summary>
        private double GetWInterpolated(double r)
        {
            // Bảng dữ liệu bạn cung cấp
            double[] rValues = { 1, 1.5, 2, 3, 4, 5, 6, 7, 10 };
            double[] wValues = { 0.88, 1.08, 1.22, 1.44, 1.61, 1.72, 1.83, 1.91, 2.12 };

            // Nếu nhỏ hơn giá trị đầu tiên
            if (r <= rValues[0]) return wValues[0];
            // Nếu lớn hơn giá trị cuối cùng
            if (r >= rValues[rValues.Length - 1]) return wValues[wValues.Length - 1];

            // Tìm khoảng để nội suy
            for (int i = 0; i < rValues.Length - 1; i++)
            {
                if (r >= rValues[i] && r <= rValues[i + 1])
                {
                    // Công thức nội suy tuyến tính: y = y0 + (r - r0) * (y1 - y0) / (r1 - r0)
                    return wValues[i] + (r - rValues[i]) * (wValues[i + 1] - wValues[i]) / (rValues[i + 1] - rValues[i]);
                }
            }

            return 0.88; // Mặc định phòng ngừa
        }
    }
}