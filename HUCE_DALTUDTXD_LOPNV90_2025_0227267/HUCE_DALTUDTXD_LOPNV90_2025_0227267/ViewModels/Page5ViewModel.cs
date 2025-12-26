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
    }
}