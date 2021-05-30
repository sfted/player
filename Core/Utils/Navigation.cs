using Player.Core.Utils.MVVM;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Player.Core.Utils
{
    public class Navigation : Notifier
    {
        private readonly List<Page> Journal = new();

        public bool IsNavigationAllowed { get; set; } = true;

        private Page currentPage;
        public Page CurrentPage
        {
            get => currentPage;
            private set
            {
                currentPage = value;
                NotifyPropertyChanged(nameof(CurrentPage));
            }
        }

        public void NavigateTo(Page page)
        {
            if (CurrentPage != page)
            {
                Journal.Add(CurrentPage);
                CurrentPage = page;
            }
        }

        public void NavigateBack()
        {
            if (CanNavigateBack())
            {
                CurrentPage = Journal.Last();
                Journal.Remove(CurrentPage);
            }
        }

        public bool CanNavigateBack()
        {
            return Journal.Count > 1 && IsNavigationAllowed;
        }

        public void ClearJournal()
        {
            Journal.Clear();
        }
    }
}
