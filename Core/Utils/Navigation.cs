using Player.Core.Utils.MVVM;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Controls;

namespace Player.Core.Utils
{
    public class Navigation : Notifier
    {
        private readonly List<Page> Journal = new();

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

        public Navigation(Page startPage)
        {
            CurrentPage = startPage;
            Journal.Add(startPage);
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
            return Journal.Count > 1;
        }
    }
}
