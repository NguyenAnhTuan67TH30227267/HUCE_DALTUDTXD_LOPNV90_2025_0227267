using HUCE_DALTUDTXD_LOPNV90_2025_0227267.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace HUCE_DALTUDTXD_LOPNV90_2025_0227267.ViewModels
{
    public class Page1ViewModel : ViewModelBase
    {
        private FoundationEntry _newEntry = new FoundationEntry();
        public FoundationEntry NewEntry
        {
            get => _newEntry;
            set { _newEntry = value; OnPropertyChanged(nameof(NewEntry)); }
        }

        public ObservableCollection<FoundationEntry> FoundationList { get; set; } = new ObservableCollection<FoundationEntry>();
        public ObservableCollection<GeologicalAxis> GeologicalAxes { get; set; } = new ObservableCollection<GeologicalAxis>();

        private int _currentAxisNumber = 1;

        public ICommand AddFoundationCommand { get; }
        public ICommand SaveGeologicalAxisCommand { get; }
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
        public Page1ViewModel()
        {
            AddFoundationCommand = new RelayCommand(AddFoundationEntry);
            SaveGeologicalAxisCommand = new RelayCommand(SaveGeologicalAxis);
        }

        private void AddFoundationEntry()
        {
            var newEntry = new FoundationEntry
            {
                Sothutulopdat = NewEntry.Sothutulopdat,
                Sohieudiachat = NewEntry.Sohieudiachat,
                Tenlopdat = NewEntry.Tenlopdat,
                Chieudaylopdat = NewEntry.Chieudaylopdat,
                Khoiluongtunhien = NewEntry.Khoiluongtunhien,
                Gocmasattrong = NewEntry.Gocmasattrong,
                Lucdinhket = NewEntry.Lucdinhket,
                Modunbiendang = NewEntry.Modunbiendang,
                Vitrimong = NewEntry.Vitrimong
            };

            FoundationList.Add(newEntry);
            NewEntry = new FoundationEntry();
        }

        private void SaveGeologicalAxis()
        {
            if (FoundationList.Count == 0)
            {
                MessageBox.Show("Vui lòng nhập dữ liệu trước khi lưu trục địa chất!", "Thông báo", MessageBoxButton.OK, MessageBoxImage.Warning);
                return;
            }

            var newAxis = new GeologicalAxis
            {
                Name = $"Trục địa chất {_currentAxisNumber}"
            };

            foreach (var entry in FoundationList)
            {
                newAxis.Entries.Add(new FoundationEntry
                {
                    Sothutulopdat = entry.Sothutulopdat,
                    Sohieudiachat = entry.Sohieudiachat,
                    Tenlopdat = entry.Tenlopdat,
                    Chieudaylopdat = entry.Chieudaylopdat,
                    Khoiluongtunhien = entry.Khoiluongtunhien,
                    Gocmasattrong = entry.Gocmasattrong,
                    Lucdinhket = entry.Lucdinhket,
                    Modunbiendang = entry.Modunbiendang,
                    Vitrimong = entry.Vitrimong
                });
            }

            GeologicalAxes.Add(newAxis);
            _currentAxisNumber++;

            // Xóa dữ liệu trong bảng hiện tại
            FoundationList.Clear();
            MessageBox.Show("Đã lưu trục địa chất thành công!");
        }
    }
}

