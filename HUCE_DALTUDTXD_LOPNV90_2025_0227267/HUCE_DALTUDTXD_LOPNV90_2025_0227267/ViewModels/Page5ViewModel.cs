using HUCE_DALTUDTXD_LOPNV90_2025_0227267.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;

namespace HUCE_DALTUDTXD_LOPNV90_2025_0227267.ViewModels
{
    public class Page5ViewModel : ViewModelBase
    {
        private readonly MainViewModel _mainViewModel;
        private double _chieuRongMong;
        private double _chieuSauChonMong;
        private double _ptb;
        public double _pmax;
        private double _pmin;
        private double _p0;
        private double _p0max;
        private double _p0min;
        private double _pDamThung;
        private double _pChongDamThung;
        private ForceInputEntry _selectedForceInput;
        public double ChieuRongMong
        {
            get => _chieuRongMong;
            set
            {
                if (_chieuRongMong != value)
                {
                    _chieuRongMong = value;
                    OnPropertyChanged(nameof(ChieuRongMong));
                    CalculateValues();
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
                    CalculateValues();
                }
            }
        }

        public double Ptb
        {
            get => _ptb;
            set
            {
                if (_ptb != value)
                {
                    _ptb = value;
                    OnPropertyChanged(nameof(Ptb));
                }
            }
        }

        public double Pmax
        {
            get => _pmax;
            set
            {
                if (_pmax != value)
                {
                    _pmax = value;
                    OnPropertyChanged(nameof(Pmax));
                }
            }
        }

        public double Pmin
        {
            get => _pmin;
            set
            {
                if (_pmin != value)
                {
                    _pmin = value;
                    OnPropertyChanged(nameof(Pmin));
                }
            }
        }

        public double P0
        {
            get => _p0;
            set
            {
                if (_p0 != value)
                {
                    _p0 = value;
                    OnPropertyChanged(nameof(P0));
                }
            }
        }

        public double P0max
        {
            get => _p0max;
            set
            {
                if (_p0max != value)
                {
                    _p0max = value;
                    OnPropertyChanged(nameof(P0max));
                }
            }
        }

        public double P0min
        {
            get => _p0min;
            set
            {
                if (_p0min != value)
                {
                    _p0min = value;
                    OnPropertyChanged(nameof(P0min));
                }
            }
        }

        public double PDamThung
        {
            get => _pDamThung;
            set
            {
                if (_pDamThung != value)
                {
                    _pDamThung = value;
                    OnPropertyChanged(nameof(PDamThung));
                }
            }
        }

        public double PChongDamThung
        {
            get => _pChongDamThung;
            set
            {
                if (_pChongDamThung != value)
                {
                    _pChongDamThung = value;
                    OnPropertyChanged(nameof(PChongDamThung));
                }
            }
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
                        ChieuRongMong = value.Mong.ChieuRongMong;
                        ChieuSauChonMong = value.Mong.ChieuSauChonMong;
                    }
                    CalculateValues();
                }
            }
        }

        public ObservableCollection<ForceInputEntry> ForceInputList => _mainViewModel.Page4ViewModel.ForceInputList;

        public Page5ViewModel(MainViewModel mainViewModel)
        {
            _mainViewModel = mainViewModel;
            // Set default values
            ChieuRongMong = 1.0;
            ChieuSauChonMong = 1.0;
        }

        private void CalculateValues()
        {
            try
            {
                if (SelectedForceInput?.Mong == null || ChieuRongMong <= 0 || ChieuSauChonMong <= 0)
                {
                    ClearValues();
                    return;
                }

                // Các tính toán (giữ nguyên phần còn lại)
                double N = SelectedForceInput.AxialForce;
                double M = SelectedForceInput.Moment;
                double b = ChieuRongMong;
                double h = ChieuSauChonMong;
                double bt = SelectedForceInput.Mong.BeDayTuong;
                double hd = SelectedForceInput.Mong.ChieuCaoDai;
                double bv = SelectedForceInput.Mong.ChieuDayLopBaoVe / 1000.0;

                double rbt = ConcreteProperties.GetRbtInTM2(SelectedForceInput.Mong.CapDoBeTong);

                Ptb = N / (1.15 * b) + 2 * h;
                Pmax = N / (1.15 * b) + 2 * h + 6 * M / (1.15 * b * b);
                Pmin = N / (1.15 * b) + 2 * h - 6 * M / (1.15 * b * b);

                P0 = N / (b);
                P0max = N / (b) + 6 * M / (b * b);
                P0min = N / (b) - 6 * M / (b * b);

                double bHieuDung = b - bt - 2 * (hd - bv);
                PDamThung = (P0min + (P0max - P0min) * (b - 0.5 * bHieuDung) / b + P0max) / 2 * (0.5 * bHieuDung);
                PChongDamThung = (hd - bv) * rbt;

                Ptb = Math.Round(Ptb, 2);
                Pmax = Math.Round(Pmax, 2);
                Pmin = Math.Round(Pmin, 2);
                P0 = Math.Round(P0, 2);
                P0max = Math.Round(P0max, 2);
                P0min = Math.Round(P0min, 2);
                PDamThung = Math.Round(PDamThung, 2);
                PChongDamThung = Math.Round(PChongDamThung, 2);

                if (Pmin < 0)
                {
                    MessageBox.Show("Cảnh báo: Pmin < 0, có thể xảy ra hiện tượng nhổ móng!",
                        "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }

                if (PDamThung > PChongDamThung)
                {
                    MessageBox.Show("Cảnh báo: PDamThung > PChongDamThung, có thể xảy ra hiện tượng đâm thủng!",
                        "Cảnh báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                }
            }
            catch (Exception ex)
            {
                ClearValues();
                MessageBox.Show($"Lỗi khi tính toán: {ex.Message}",
                    "Lỗi", MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }

        private void ClearValues()
        {
            Ptb = 0;
            Pmax = 0;
            Pmin = 0;
            P0 = 0;
            P0max = 0;
            P0min = 0;
            PDamThung = 0;
            PChongDamThung = 0;
        }
    }
}