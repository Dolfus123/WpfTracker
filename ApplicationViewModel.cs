using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using WpfTracker.Interfaces;

namespace WpfTracker
{
    public class ApplicationViewModel : INotifyPropertyChanged
    {
        private User selectedUser;

        IFileService fileService;
        IDialogService dialogService;

        private string pathTestData = new DirectoryInfo(Directory.GetCurrentDirectory()).Parent.Parent.Parent.FullName + "\\TestData";

        public ObservableCollection<User> Users { get; set; }

        // сохранение
        private RelayCommand saveCommand;
        public RelayCommand SaveCommand
        {
            get
            {
                return saveCommand ??
                  (saveCommand = new RelayCommand(obj =>
                  {
                      try
                      {
                          if (dialogService.SaveFileDialog() == true)
                          {
                              fileService.Save(dialogService.FilePaths[0], selectedUser);
                              dialogService.ShowMessage("Файл сохранен");
                          }
                      }
                      catch (Exception ex)
                      {
                          dialogService.ShowMessage(ex.Message);
                      }
                  }));
            }
        }

        // открытие
        private RelayCommand openCommand;
        public RelayCommand OpenCommand
        {
            get
            {
                return openCommand ??
                  (openCommand = new RelayCommand(obj =>
                  {
                      try
                      {
                          if (dialogService.OpenFileDialog() == true)
                          {
                              foreach (var Path in dialogService.FilePaths)
                              {
                                  var newUsers = fileService.Open(Path, Users.ToList());
                                  Users.Clear();
                                  foreach (var user in newUsers)
                                      Users.Add(user);
                              }
                              dialogService.ShowMessage("Файлы открыты");
                          }
                      }
                      catch (Exception ex)
                      {
                          dialogService.ShowMessage(ex.Message);
                      }
                  }));
            }
        }
        public User SelectedUser
        {
            get { return selectedUser; }
            set
            {
                selectedUser = value;
                OnPropertyChanged("SelectedUser");
            }
        }

        public ApplicationViewModel(IDialogService dialogService, IFileService fileService)
        {
            this.dialogService = dialogService;
            this.fileService = fileService;

            // данные по умлолчанию

            Users = new ObservableCollection<User>();
            string[] filePaths = Directory.GetFiles(pathTestData);
            Array.Sort(filePaths, (x, y) => int.Parse(x.Split("day").Last().Replace(".json", "")).CompareTo(int.Parse(y.Split("day").Last().Replace(".json", ""))));
            foreach (var filePath in filePaths)
            {
                var newUsers = fileService.Open(filePath, Users.ToList());
                Users.Clear();
                foreach (var user in newUsers)
                    Users.Add(user);
            }
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged([CallerMemberName] string prop = "")
        {
            if (PropertyChanged != null)
                PropertyChanged(this, new PropertyChangedEventArgs(prop));
        }
    }
}
